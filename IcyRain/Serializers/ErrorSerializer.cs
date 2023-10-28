using System;
using System.Runtime.ExceptionServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class ErrorSerializer<TResolver, T> : Serializer<TResolver, T>, IErrorSerializer
    where TResolver : Resolver
{
    private readonly Exception _exception;

    public ErrorSerializer()
        => _exception = new InvalidOperationException(Naming.GetName(typeof(T), typeof(TResolver) == Types.UnionResolver));

    public ErrorSerializer(Exception exception)
        => _exception = exception;

    public sealed override int? GetSize()
    {
        Throw();
        return default;
    }

    public sealed override int GetCapacity(T value)
    {
        Throw();
        return default;
    }

    public sealed override void Serialize(ref Writer writer, T value) => Throw();

    public sealed override void SerializeSpot(ref Writer writer, T value) => Throw();

    public sealed override T Deserialize(ref Reader reader)
    {
        Throw();
        return default;
    }

    public sealed override T DeserializeInUTC(ref Reader reader)
    {
        Throw();
        return default;
    }

    public sealed override T DeserializeSpot(ref Reader reader)
    {
        Throw();
        return default;
    }

    public sealed override T DeserializeInUTCSpot(ref Reader reader)
    {
        Throw();
        return default;
    }

    public void Throw() => ExceptionDispatchInfo.Capture(_exception).Throw();
}
