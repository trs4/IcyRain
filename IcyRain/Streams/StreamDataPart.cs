using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Streams
{
    /// <summary>System class for transfer stream</summary>
    public sealed class StreamDataPart<T> : StreamPart
    {
        /// <summary>Serialize side</summary>
        [MethodImpl(Flags.HotPath)]
        internal StreamDataPart(T data, Stream stream, int bufferSize = Buffers.StreamPartSize)
            : base(stream, bufferSize)
            => Data = data;

        /// <summary>Deserialize size</summary>
        [MethodImpl(Flags.HotPath)]
        internal StreamDataPart(ReadOnlySequence<byte> sequence) : base(sequence) { }

        public T Data { get; }

        public bool HasData { get; set; } = true;

        [MethodImpl(Flags.HotPath)]
        public sealed override bool CanRead() => HasData || base.CanRead();

        [MethodImpl(Flags.HotPath)]
        internal sealed override void Serialize(IBufferWriter<byte> buffer)
        {
            if (HasData)
            {
                HasData = false;
                Serialization.Serialize(buffer, Data);
            }
            else
                base.Serialize(buffer);
        }

        [MethodImpl(Flags.HotPath)]
        internal T Deserialize() => Serialization.Deserialize<T>(Buffer, 0, BufferSize);
    }
}
