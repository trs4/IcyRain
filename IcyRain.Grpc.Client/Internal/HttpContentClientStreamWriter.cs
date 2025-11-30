using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal sealed class HttpContentClientStreamWriter<TRequest, TResponse> : ClientStreamWriterBase<TRequest>
    where TRequest : class
    where TResponse : class
{
    private readonly GrpcCall<TRequest, TResponse> _call;
    private bool _completeCalled;

    public TaskCompletionSource<Stream> WriteStreamTcs { get; }

    public TaskCompletionSource<bool> CompleteTcs { get; }

    public HttpContentClientStreamWriter(GrpcCall<TRequest, TResponse> call)
    {
        _call = call;

        // CompleteTcs doesn't use RunContinuationsAsynchronously because we want the caller of CompleteAsync
        // to wait until the TCS's awaiter, PushStreamContent, finishes completing the request.
        // This is required to avoid a race condition between the HttpContent completing, and sending an
        // END_STREAM flag to the server, and app code disposing the call, which will trigger a RST_STREAM
        // if HttpContent has finished.
        // See https://github.com/grpc/grpc-dotnet/issues/1394 for an example.
        CompleteTcs = new TaskCompletionSource<bool>(TaskCreationOptions.None);

        WriteStreamTcs = new TaskCompletionSource<Stream>(TaskCreationOptions.RunContinuationsAsynchronously);
        WriteOptions = _call.Options.WriteOptions;
    }

    public override WriteOptions? WriteOptions { get; set; }

    public override Task CompleteAsync()
    {
        _call.EnsureNotDisposed();

        lock (WriteLock)
        {
            // Pending writes need to be awaited first
            if (IsWriteInProgressUnsynchronized)
            {
                var ex = new InvalidOperationException("Can't complete the client stream writer because the previous write is in progress.");
                return Task.FromException(ex);
            }

            // Notify that the client stream is complete
            CompleteTcs.TrySetResult(true);
            _completeCalled = true;
        }

        return Task.CompletedTask;
    }

    public override async Task WriteCoreAsync(TRequest message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);
        _call.TryRegisterCancellation(token, out var ctsRegistration);

        try
        {
            await WriteAsync(WriteMessageToStream, message, token).ConfigureAwait(false);
        }
        finally
        {
            ctsRegistration?.Dispose();
        }

        static Task WriteMessageToStream(GrpcCall<TRequest, TResponse> call, Stream writeStream, CallOptions callOptions, TRequest message)
            => call.WriteMessageAsync(writeStream, message, callOptions);
    }

    public Task WriteAsync<TState>(Func<GrpcCall<TRequest, TResponse>, Stream, CallOptions, TState, Task> writeFunc, TState state, CancellationToken token)
    {
        _call.EnsureNotDisposed();

        lock (WriteLock)
        {
            // CompleteAsync has already been called
            // Use explicit flag here. This error takes precedence over others.
            if (_completeCalled)
                return CreateErrorTask("Request stream has already been completed.");

            // Call has already completed
            if (_call.CallTask.IsCompletedSuccessfully)
            {
                var status = _call.CallTask.Result;

                if (_call.CancellationToken.IsCancellationRequested
                    && _call.Channel.ThrowOperationCanceledOnCancellation
                    && (status.StatusCode == StatusCode.Cancelled || status.StatusCode == StatusCode.DeadlineExceeded))
                {
                    return Task.FromCanceled(_call.GetCanceledToken(token));
                }

                return Task.FromException(_call.CreateCanceledStatusException());
            }

            // Pending writes need to be awaited first
            if (IsWriteInProgressUnsynchronized)
                return CreateErrorTask("Can't write the message because the previous write is in progress.");

            // Save write task to track whether it is complete. Must be set inside lock.
            WriteTask = WriteAsyncCore(writeFunc, state, token);
        }

        return WriteTask;
    }

    public GrpcCall<TRequest, TResponse> Call => _call;

    public async Task WriteAsyncCore<TState>(Func<GrpcCall<TRequest, TResponse>, Stream, CallOptions, TState, Task> writeFunc, TState state, CancellationToken token)
    {
        try
        {
            // Wait until the client stream has started
            var writeStream = await WriteStreamTcs.Task.ConfigureAwait(false);

            // WriteOptions set on the writer take precedence over the CallOptions.WriteOptions
            var callOptions = _call.Options;

            if (WriteOptions is not null)
                callOptions = callOptions.WithWriteOptions(WriteOptions); // Creates a copy of the struct
            
            await writeFunc(_call, writeStream, callOptions, state).ConfigureAwait(false);

            // Flush stream to ensure messages are sent immediately.
            await writeStream.FlushAsync(_call.CancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
            var resolvedCanceledException = _call.EnsureUserCancellationTokenReported(ex, token);

            if (!_call.Channel.ThrowOperationCanceledOnCancellation)
                throw _call.CreateCanceledStatusException(resolvedCanceledException);
            
            ExceptionDispatchInfo.Capture(resolvedCanceledException).Throw();
        }
    }

}
