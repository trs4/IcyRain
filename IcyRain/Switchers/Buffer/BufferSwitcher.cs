using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers;

public abstract class BufferSwitcher<T>
{
    public static BufferSwitcher<T> Instance
    {
        [MethodImpl(Flags.HotPath)]
        get;
    }

    static BufferSwitcher()
        => Instance = BufferBuilder.Get<T>();


    public abstract int Serialize(IBufferWriter<byte> buffer, T value);

    public abstract int SerializeWithLZ4(IBufferWriter<byte> buffer, T value, out int serializedLength);


    public abstract T Deserialize(in ReadOnlySequence<byte> sequence);

    public abstract T DeserializeInUTC(in ReadOnlySequence<byte> sequence);

    public abstract T DeserializeWithLZ4(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength);

    public abstract T DeserializeInUTCWithLZ4(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength);
}
