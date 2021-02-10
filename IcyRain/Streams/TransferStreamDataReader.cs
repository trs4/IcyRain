namespace IcyRain.Streams
{
    public abstract class TransferStreamDataReader<T> : TransferStreamReader
    {
        public abstract T Data { get; }
    }
}
