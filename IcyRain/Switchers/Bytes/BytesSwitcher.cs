using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers;

public abstract class BytesSwitcher<T>
{
    public static BytesSwitcher<T> Instance
    {
        [MethodImpl(Flags.HotPath)]
        get;
    }

    static BytesSwitcher()
        => Instance = BytesBuilder.Get<T>();


    public abstract byte[] Serialize(T value);

    public abstract byte[] SerializeWithLZ4(T value, out int serializedLength);


    public abstract T Deserialize(byte[] bytes, int offset, int count);

    public abstract T DeserializeInUTC(byte[] bytes, int offset, int count);

    public abstract T DeserializeWithLZ4(byte[] bytes, int offset, int count, out int decodedLength);

    public abstract T DeserializeInUTCWithLZ4(byte[] bytes, int offset, int count, out int decodedLength);
}
