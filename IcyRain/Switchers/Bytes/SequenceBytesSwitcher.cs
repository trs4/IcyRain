using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SequenceBytesSwitcher : BytesSwitcher<ReadOnlySequence<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(ReadOnlySequence<byte> value)
            => value.IsSingleSegment ? value.First.Span.ToArray() : value.TransferToArray();

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> Deserialize(byte[] bytes, DeserializeOptions options)
            => new ReadOnlySequence<byte>(bytes);
    }
}
