using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

public abstract class Serializer<TResolver, T> : ISerializer
    where TResolver : Resolver
{
    public static Serializer<TResolver, T> Instance
    {
        [MethodImpl(Flags.HotPath)]
        get;
    }

    static Serializer()
    {
        Serializer<TResolver, T> serializer;

        try
        {
            serializer = (Serializer<TResolver, T>)Builder<TResolver>.Get<T>() ?? new ErrorSerializer<TResolver, T>();
        }
        catch (Exception e)
        {
            serializer = new ErrorSerializer<TResolver, T>(e);
        }

        Instance = serializer;
    }

    protected Serializer() { }

    public abstract int? GetSize();

    public abstract int GetCapacity(T value);

    public abstract void Serialize(ref Writer writer, T value);

    public abstract void SerializeSpot(ref Writer writer, T value);

    public abstract T Deserialize(ref Reader reader);

    public abstract T DeserializeInUTC(ref Reader reader);

    public abstract T DeserializeSpot(ref Reader reader);

    public abstract T DeserializeInUTCSpot(ref Reader reader);
}
