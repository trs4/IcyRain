using System.Buffers;
using System.Diagnostics;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal sealed class DefaultDeserializationContext : DeserializationContext
{
    private ReadOnlySequence<byte>? _payload;

    public void SetPayload(in ReadOnlySequence<byte>? payload)
        => _payload = payload;

    public override byte[] PayloadAsNewBuffer()
    {
        Debug.Assert(_payload is not null, "Payload must be set.");

        // The array returned by PayloadAsNewBuffer must be the exact message size.
        // There is no opportunity here to return a pooled array.
        return _payload.GetValueOrDefault().ToArray();
    }

    public override ReadOnlySequence<byte> PayloadAsReadOnlySequence()
    {
        Debug.Assert(_payload is not null, "Payload must be set.");
        return _payload.GetValueOrDefault();
    }

    public override int PayloadLength => _payload.HasValue ? (int)_payload.GetValueOrDefault().Length : 0;
}
