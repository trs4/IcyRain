using System;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Data.Objects;
using IcyRain.Streams;

namespace IcyRain.Data.Streams;

public sealed class GrpcStreamDataWriter : TransferStreamDataWriter<SealedData>
{
    private readonly IAsyncStreamWriter<StreamDataPart<SealedData>> _responseStream;

    public GrpcStreamDataWriter(IAsyncStreamWriter<StreamDataPart<SealedData>> responseStream)
        => _responseStream = responseStream ?? throw new ArgumentNullException(nameof(responseStream));

    public sealed override Task WriteAsync(StreamDataPart<SealedData> message)
        => _responseStream.WriteAsync(message);
}
