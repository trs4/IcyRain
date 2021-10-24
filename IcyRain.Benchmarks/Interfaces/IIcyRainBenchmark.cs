namespace IcyRain.Benchmarks
{
    public interface IIcyRainBenchmark
    {
        void IcyRain_Ser();
        void IcyRain_DeepClone();
        void IcyRainLZ4_Ser();
        void IcyRainLZ4_DeepClone();
        void IcyRainLZ4UTC_DeepClone();
    }
}
