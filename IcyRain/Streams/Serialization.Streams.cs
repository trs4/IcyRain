using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Streams;

namespace IcyRain;

/// <summary>Buffer fast serialization for concrete types</summary>
public static partial class Serialization
{
    /// <summary>Experimental gRPC System.IO.Stream transfering</summary>
    public static class Streams
    {
        /// <summary>Serialize via buffer</summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="part">Stream part</param>
        [MethodImpl(Flags.HotPath)]
        public static void Serialize(IBufferWriter<byte> buffer, StreamPart part)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            if (part is null)
                throw new ArgumentNullException(nameof(part));

            part.Serialize(buffer);
        }

        /// <summary>Deserialize via buffer</summary>
        /// <param name="sequence">Buffer</param>
        /// <returns>Stream part</returns>
        [MethodImpl(Flags.HotPath)]
        public static StreamPart Deserialize(in ReadOnlySequence<byte> sequence)
            => new StreamPart(sequence);

        /// <summary>Deserialize via buffer</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="sequence">Buffer</param>
        /// <returns>Stream part</returns>
        [MethodImpl(Flags.HotPath)]
        public static StreamDataPart<T> DeserializeData<T>(in ReadOnlySequence<byte> sequence)
            => new StreamDataPart<T>(sequence);
    }
}
