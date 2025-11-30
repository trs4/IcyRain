using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal.Retry;

internal sealed class RetryCallBaseClientStreamReader<TRequest, TResponse> : IAsyncStreamReader<TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly RetryCallBase<TRequest, TResponse> _retryCallBase;

    public RetryCallBaseClientStreamReader(RetryCallBase<TRequest, TResponse> retryCallBase)
        => _retryCallBase = retryCallBase;

    public TResponse Current
        => _retryCallBase.CommitedCallTask.IsCompletedSuccessfully
        ? _retryCallBase.CommitedCallTask.Result.ClientStreamReader!.Current : default!;

    public async Task<bool> MoveNext(CancellationToken token)
    {
        var call = await _retryCallBase.CommitedCallTask.ConfigureAwait(false);
        return await call.ClientStreamReader!.MoveNext(token).ConfigureAwait(false);
    }

}
