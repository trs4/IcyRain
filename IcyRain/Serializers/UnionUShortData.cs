namespace IcyRain.Serializers;

internal sealed class UnionUShortData<T>
    where T : class
{
    public readonly ushort Index;
    public readonly GetCapacityMethod<T> GetCapacity;
    public readonly SerializeMethod<T> SerializeSpot;

    public UnionUShortData(ushort index, GetCapacityMethod<T> getCapacity, SerializeMethod<T> serializeSpot)
    {
        Index = index;
        GetCapacity = getCapacity;
        SerializeSpot = serializeSpot;
    }

}
