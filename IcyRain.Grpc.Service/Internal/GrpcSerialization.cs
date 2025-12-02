using System.Runtime.CompilerServices;
using Grpc.Core;
using IcyRain.Internal;
using IcyRain.Streams;

namespace IcyRain.Grpc.Service.Internal;

public static class GrpcSerialization
{
    [MethodImpl(Flags.HotPath)]
    public static void SerializeData<T>(T obj, SerializationContext context)
    {
        var buffer = context.GetBufferWriter();
        Serialization.Serialize(buffer, obj);
        context.Complete();
    }

    [MethodImpl(Flags.HotPath)]
    public static T DeserializeData<T>(DeserializationContext context)
        => Serialization.Deserialize<T>(context.PayloadAsReadOnlySequence());

    [MethodImpl(Flags.HotPath)]
    public static void SerializeStreamPart(StreamPart obj, SerializationContext context)
    {
        var buffer = context.GetBufferWriter();
        Serialization.Streams.Serialize(buffer, obj);
        context.Complete();
    }

    [MethodImpl(Flags.HotPath)]
    public static StreamPart DeserializeStreamPart(DeserializationContext context)
        => Serialization.Streams.Deserialize(context.PayloadAsReadOnlySequence());

    [MethodImpl(Flags.HotPath)]
    public static void SerializeStreamDataPart<T>(StreamDataPart<T> obj, SerializationContext context)
    {
        var buffer = context.GetBufferWriter();
        Serialization.Streams.Serialize(buffer, obj);
        context.Complete();
    }

    [MethodImpl(Flags.HotPath)]
    public static StreamDataPart<T> DeserializeStreamDataPart<T>(DeserializationContext context)
        => Serialization.Streams.DeserializeData<T>(context.PayloadAsReadOnlySequence());
}
