using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class ReadOnlyMemoryBytesSwitcher : BytesSwitcher<ReadOnlyMemory<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(ReadOnlyMemory<byte> value)
            => value.ToArray();

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] SerializeWithLZ4(ReadOnlyMemory<byte> value, out int serializedLength)
        {
            serializedLength = value.Length;
            return LZ4ArrayCodec.EncodeToArray(value.Span);
        }


        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlyMemory<byte> Deserialize(byte[] bytes, int offset, int count)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            return new ReadOnlyMemory<byte>(bytes, offset, count);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlyMemory<byte> DeserializeInUTC(byte[] bytes, int offset, int count)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            return new ReadOnlyMemory<byte>(bytes, offset, count);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlyMemory<byte> DeserializeWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
        {
            decodedLength = count;
            byte[] result = LZ4ArrayCodec.DecodeToRentArray(new Span<byte>(bytes, offset, count), ref decodedLength);
            return new ReadOnlyMemory<byte>(result, 0, decodedLength);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlyMemory<byte> DeserializeInUTCWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
        {
            decodedLength = count;
            byte[] result = LZ4ArrayCodec.DecodeToRentArray(new Span<byte>(bytes, offset, count), ref decodedLength);
            return new ReadOnlyMemory<byte>(result, 0, decodedLength);
        }

    }
}
