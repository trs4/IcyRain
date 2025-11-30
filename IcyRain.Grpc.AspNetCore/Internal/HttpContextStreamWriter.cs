using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http.Features;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class HttpContextStreamWriter<TResponse> : IServerStreamWriter<TResponse>
    where TResponse : class
{
    private readonly HttpContextServerCallContext _context;
    private readonly Action<TResponse, SerializationContext> _serializer;
    private readonly PipeWriter _bodyWriter;
    private readonly IHttpRequestLifetimeFeature _requestLifetimeFeature;
#pragma warning disable IDE0330 // Use 'System.Threading.Lock'
    private readonly object _writeLock = new object();
#pragma warning restore IDE0330 // Use 'System.Threading.Lock'
    private Task? _writeTask;
    private bool _completed;
    private long _writeCount;

    public HttpContextStreamWriter(HttpContextServerCallContext context, Action<TResponse, SerializationContext> serializer)
    {
        _context = context;
        _serializer = serializer;

        // Copy HttpContext values.
        // This is done to avoid a race condition when reading them from HttpContext later when running in a separate thread.
        _bodyWriter = context.HttpContext.Response.BodyWriter;
        // Copy lifetime feature because HttpContext.RequestAborted on .NET 6 doesn't return the real cancellation token.
        _requestLifetimeFeature = GrpcProtocolHelpers.GetRequestLifetimeFeature(context.HttpContext);
    }

    public WriteOptions? WriteOptions
    {
        get => _context.WriteOptions;
        set => _context.WriteOptions = value;
    }

    public Task WriteAsync(TResponse message)
        => WriteCoreAsync(message, CancellationToken.None);

    // Explicit implementation because this WriteAsync has a default interface implementation
    Task IAsyncStreamWriter<TResponse>.WriteAsync(TResponse message, CancellationToken token)
        => WriteCoreAsync(message, token);

    private async Task WriteCoreAsync(TResponse message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Register cancellation token early to ensure request is canceled if cancellation is requested
        CancellationTokenRegistration? registration = null;

        if (token.CanBeCanceled)
        {
            registration = token.Register(
                static (state) => ((HttpContextServerCallContext)state!).CancelRequest(),
                _context);
        }

        try
        {
            token.ThrowIfCancellationRequested();

            if (_completed || _requestLifetimeFeature.RequestAborted.IsCancellationRequested)
                throw new InvalidOperationException("Can't write the message because the request is complete.");

            lock (_writeLock)
            {
                // Pending writes need to be awaited first
                if (IsWriteInProgressUnsynchronized)
                    throw new InvalidOperationException("Can't write the message because the previous write is in progress.");

                // Save write task to track whether it is complete. Must be set inside lock.
                _writeTask = _bodyWriter.WriteStreamedMessageAsync(message, _context, _serializer, token);
            }

            await _writeTask;
            Interlocked.Increment(ref _writeCount);
        }
        finally
        {
            registration?.Dispose();
        }
    }

    public void Complete()
        => _completed = true;

    /// <summary>
    /// A value indicating whether there is an async write already in progress.
    /// Should only check this property when holding the write lock.
    /// </summary>
    private bool IsWriteInProgressUnsynchronized
    {
        get
        {
            var writeTask = _writeTask;
            return writeTask != null && !writeTask.IsCompleted;
        }
    }

}
