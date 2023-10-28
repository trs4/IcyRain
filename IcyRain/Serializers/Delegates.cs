using IcyRain.Internal;

namespace IcyRain.Serializers;

internal delegate int GetCapacityMethod<T>(T value);

internal delegate void SerializeMethod<T>(ref Writer writer, T value);

internal delegate T DeserializeMethod<T>(ref Reader reader);
