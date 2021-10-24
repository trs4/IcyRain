namespace IcyRain.Benchmarks
{
    public interface IZeroFormatterBenchmark
    {
        void ZeroFormatter_Ser();
        void ZeroFormatter_DeepClone();
        void ZeroFormatterLZ4_Ser();
        void ZeroFormatterLZ4_DeepClone();
    }
}
