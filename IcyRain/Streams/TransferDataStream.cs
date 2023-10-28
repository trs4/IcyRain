using System;
using System.Threading;
using System.Threading.Tasks;

namespace IcyRain.Streams;

public abstract class TransferDataStream<T> : TransferStream
{
    protected TransferDataStream(T data, Action onDispose)
        : base(onDispose)
        => Data = data;

    public T Data { get; }

    public static async ValueTask<TransferDataStream<T>> CreateWithDataAsync(
        TransferStreamDataReader<T> reader, Action onDispose = null, CancellationToken cancellationToken = default)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));

        await reader.MoveNext(cancellationToken).ConfigureAwait(false);
        return new TransferDataReaderStream<T>(reader, onDispose);
    }

}
