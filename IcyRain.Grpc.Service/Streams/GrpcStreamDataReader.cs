using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Streams;

namespace IcyRain.Grpc.Service.Streams;

public sealed class GrpcStreamDataReader<T> : TransferStreamDataReader<T>
{
    private readonly IAsyncStreamReader<StreamDataPart<T>> _requestStream;
    private bool _moveNext = true;

    public GrpcStreamDataReader(IAsyncStreamReader<StreamDataPart<T>> requestStream)
        => _requestStream = requestStream ?? throw new ArgumentNullException(nameof(requestStream));

    public sealed override bool IsCompleted => !_moveNext;

    public sealed override T Data => _requestStream.Current.Deserialize();

    public sealed override StreamPart Current => _requestStream.Current;

    public sealed override async Task<bool> MoveNext(CancellationToken token)
        => _moveNext && (_moveNext = await _requestStream.MoveNext(token).ConfigureAwait(false));
}
