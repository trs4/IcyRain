using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class BytesBytesSwitcher : BytesSwitcher<byte[]>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(byte[] value)
            => value ?? throw new ArgumentNullException(nameof(value));

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] SerializeWithLZ4(byte[] value, out int serializedLength)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            serializedLength = value.Length;
            return LZ4ArrayCodec.EncodeToArray(value);
        }


        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Deserialize(byte[] bytes, int offset, int count)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            if (offset == 0 && (count < 0 || bytes.Length == count))
                return bytes;

            var result = new byte[count];
            result.WriteTo(bytes, offset, count);
            return result;
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] DeserializeInUTC(byte[] bytes, int offset, int count)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            if (offset == 0 && (count < 0 || bytes.Length == count))
                return bytes;

            var result = new byte[count];
            result.WriteTo(bytes, offset, count);
            return result;
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] DeserializeWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            decodedLength = count;

            if (offset == 0 && (count < 0 || bytes.Length == count))
                return LZ4ArrayCodec.DecodeToArray(bytes, ref decodedLength);

            var result = new byte[count];
            result.WriteTo(bytes, offset, count);
            return LZ4ArrayCodec.DecodeToArray(bytes, ref decodedLength);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] DeserializeInUTCWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            decodedLength = count;

            if (offset == 0 && (count < 0 || bytes.Length == count))
                return LZ4ArrayCodec.DecodeToArray(bytes, ref decodedLength);

            var result = new byte[count];
            result.WriteTo(bytes, offset, count);
            return LZ4ArrayCodec.DecodeToArray(bytes, ref decodedLength);
        }

    }
}
