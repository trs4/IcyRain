namespace IcyRain.Benchmarks
{
    public interface IMessagePackBenchmark
    {
        void MessagePack_Ser();
        void MessagePack_DeepClone();
    }
}
