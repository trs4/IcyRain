using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal abstract class ClientStreamWriterBase<TRequest> : IClientStreamWriter<TRequest>
    where TRequest : class
{
    protected ClientStreamWriterBase() { }

    protected object WriteLock { get; } = new object();

    protected Task? WriteTask { get; set; }

    public abstract WriteOptions? WriteOptions { get; set; }

    /// <summary>
    /// A value indicating whether there is an async write already in progress.
    /// Should only check this property when holding the write lock.
    /// </summary>
    protected bool IsWriteInProgressUnsynchronized
    {
        get
        {
            var writeTask = WriteTask;
            return writeTask != null && !writeTask.IsCompleted;
        }
    }

    public abstract Task CompleteAsync();

    public Task WriteAsync(TRequest message) => WriteCoreAsync(message, CancellationToken.None);

    // Explicit implementation because this WriteAsync has a default interface implementation.
    Task IAsyncStreamWriter<TRequest>.WriteAsync(TRequest message, CancellationToken token)
        => WriteCoreAsync(message, token);

    public abstract Task WriteCoreAsync(TRequest message, CancellationToken token);

    protected static Task CreateErrorTask(string message)
    {
        var ex = new InvalidOperationException(message);
        return Task.FromException(ex);
    }

}
