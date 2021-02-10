using System;
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

        [MethodImpl(Flags.HotPath)]
        public override ReadOnlySequence<byte> Deserialize(byte[] bytes, int offset, int count, DeserializeOptions options)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            return new ReadOnlySequence<byte>(bytes, offset, count);
        }

    }
}
