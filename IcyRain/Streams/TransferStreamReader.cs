using System.Threading;
using System.Threading.Tasks;

namespace IcyRain.Streams;

public abstract class TransferStreamReader
{
    public abstract bool IsCompleted { get; }

    public abstract StreamPart Current { get; }

    public abstract Task<bool> MoveNext(CancellationToken token);
}
