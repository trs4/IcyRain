namespace IcyRain.Serializers;

internal sealed class UnionByteData<T>
    where T : class
{
    public readonly byte Index;
    public readonly GetCapacityMethod<T> GetCapacity;
    public readonly SerializeMethod<T> SerializeSpot;

    public UnionByteData(byte index, GetCapacityMethod<T> getCapacity, SerializeMethod<T> serializeSpot)
    {
        Index = index;
        GetCapacity = getCapacity;
        SerializeSpot = serializeSpot;
    }

}
