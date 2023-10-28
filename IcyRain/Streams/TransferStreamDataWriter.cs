using System.Threading.Tasks;

namespace IcyRain.Streams;

public abstract class TransferStreamDataWriter<T>
{
    public abstract Task WriteAsync(StreamDataPart<T> message);
}
