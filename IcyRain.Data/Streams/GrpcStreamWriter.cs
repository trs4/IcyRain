using System;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Streams;

namespace IcyRain.Data.Streams;

public sealed class GrpcStreamWriter : TransferStreamWriter
{
    private readonly IAsyncStreamWriter<StreamPart> _responseStream;

    public GrpcStreamWriter(IAsyncStreamWriter<StreamPart> responseStream)
        => _responseStream = responseStream ?? throw new ArgumentNullException(nameof(responseStream));

    public sealed override Task WriteAsync(StreamPart message)
        => _responseStream.WriteAsync(message);
}
