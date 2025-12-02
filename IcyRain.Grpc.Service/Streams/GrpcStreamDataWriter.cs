using System;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Streams;

namespace IcyRain.Grpc.Service.Streams;

public sealed class GrpcStreamDataWriter<T> : TransferStreamDataWriter<T>
{
    private readonly IAsyncStreamWriter<StreamDataPart<T>> _responseStream;

    public GrpcStreamDataWriter(IAsyncStreamWriter<StreamDataPart<T>> responseStream)
        => _responseStream = responseStream ?? throw new ArgumentNullException(nameof(responseStream));

    public sealed override Task WriteAsync(StreamDataPart<T> message)
        => _responseStream.WriteAsync(message);
}
