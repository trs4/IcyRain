using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Service.Internal;
using Microsoft.AspNetCore.Http.Features;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class HttpContextStreamReader<TRequest> : IAsyncStreamReader<TRequest>
    where TRequest : class
{
    private readonly HttpContextServerCallContext _serverCallContext;
    private readonly Func<DeserializationContext, TRequest> _deserializer;
    private readonly PipeReader _bodyReader;
    private readonly IHttpRequestLifetimeFeature _requestLifetimeFeature;
    private bool _completed;
    private long _readCount;

    public HttpContextStreamReader(HttpContextServerCallContext serverCallContext, Func<DeserializationContext, TRequest> deserializer)
    {
        _serverCallContext = serverCallContext;
        _deserializer = deserializer;

        // Copy HttpContext values.
        // This is done to avoid a race condition when reading them from HttpContext later when running in a separate thread.
        _bodyReader = _serverCallContext.HttpContext.Request.BodyReader;
        // Copy lifetime feature because HttpContext.RequestAborted on .NET 6 doesn't return the real cancellation token.
        _requestLifetimeFeature = GrpcProtocolHelpers.GetRequestLifetimeFeature(_serverCallContext.HttpContext);
    }

    public TRequest Current { get; private set; } = default!;

    //public void Dispose() { }

    public Task<bool> MoveNext(CancellationToken token)
    {
        async Task<bool> MoveNextAsync(ValueTask<TRequest?> readStreamTask)
            => ProcessPayload(await readStreamTask);

        if (token.IsCancellationRequested)
            return Task.FromCanceled<bool>(token);

        if (_completed || _requestLifetimeFeature.RequestAborted.IsCancellationRequested)
            return Task.FromException<bool>(new InvalidOperationException("Can't read messages after the request is complete."));

        // Clear current before moving next. This prevents rooting the previous value while getting the next one.
        // In a long running stream this can allow the previous value to be GCed.
        Current = null!;
        var request = _bodyReader.ReadStreamMessageAsync(_serverCallContext, _deserializer, token);

        if (!request.IsCompletedSuccessfully)
            return MoveNextAsync(request);

        return ProcessPayload(request.Result) ? CommonGrpcProtocolHelpers.TrueTask : CommonGrpcProtocolHelpers.FalseTask;
    }

    private bool ProcessPayload(TRequest? request)
    {
        // Stream is complete
        if (request is null)
        {
            Current = null!;
            return false;
        }

        Current = request;
        Interlocked.Increment(ref _readCount);
        return true;
    }

    public void Complete()
        => _completed = true;
}
