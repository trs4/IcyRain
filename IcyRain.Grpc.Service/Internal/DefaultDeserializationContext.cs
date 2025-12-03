using System.Buffers;
using Grpc.Core;

namespace IcyRain.Grpc.Service.Internal;

internal sealed class DefaultDeserializationContext : DeserializationContext
{
    private ReadOnlySequence<byte>? _payload;

    public void SetPayload(in ReadOnlySequence<byte>? payload)
        => _payload = payload;

    public override byte[] PayloadAsNewBuffer()
        => _payload.GetValueOrDefault().ToArray();

    public override ReadOnlySequence<byte> PayloadAsReadOnlySequence()
        => _payload.GetValueOrDefault();

    public override int PayloadLength
        => _payload.HasValue ? (int)_payload.GetValueOrDefault().Length : 0;
}
