using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
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

        public abstract T Deserialize(byte[] bytes, DeserializeOptions options);
    }
}
