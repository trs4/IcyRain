using System.Threading.Tasks;

namespace IcyRain.Streams
{
    public abstract class TransferStreamWriter
    {
        public abstract Task WriteAsync(StreamPart message);
    }
}
