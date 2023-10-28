using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Streams;

namespace IcyRain.Data.Streams;

public sealed class GrpcStreamReader : TransferStreamReader
{
    private readonly IAsyncStreamReader<StreamPart> _requestStream;
    private bool _moveNext = true;

    public GrpcStreamReader(IAsyncStreamReader<StreamPart> requestStream)
        => _requestStream = requestStream ?? throw new ArgumentNullException(nameof(requestStream));

    public sealed override bool IsCompleted => !_moveNext;

    public sealed override StreamPart Current => _requestStream.Current;

    public sealed override async Task<bool> MoveNext(CancellationToken cancellationToken)
        => _moveNext && (_moveNext = await _requestStream.MoveNext(cancellationToken).ConfigureAwait(false));
}
