using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    public abstract class BufferSwitcher<T>
    {
        public static BufferSwitcher<T> Instance
        {
            [MethodImpl(Flags.HotPath)]
            get;
        }

        static BufferSwitcher()
            => Instance = BufferBuilder.Get<T>();

        public abstract void Serialize(IBufferWriter<byte> buffer, T value);

        public abstract T Deserialize(in ReadOnlySequence<byte> sequence, DeserializeOptions options);
    }
}
