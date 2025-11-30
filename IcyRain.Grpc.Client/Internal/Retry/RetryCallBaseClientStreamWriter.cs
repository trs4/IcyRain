using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal.Retry;

internal sealed class RetryCallBaseClientStreamWriter<TRequest, TResponse> : ClientStreamWriterBase<TRequest>
    where TRequest : class
    where TResponse : class
{
    private readonly RetryCallBase<TRequest, TResponse> _retryCallBase;

    public RetryCallBaseClientStreamWriter(RetryCallBase<TRequest, TResponse> retryCallBase)
        => _retryCallBase = retryCallBase;

    public override WriteOptions? WriteOptions
    {
        get => _retryCallBase.ClientStreamWriteOptions;
        set => _retryCallBase.ClientStreamWriteOptions = value;
    }

    public override Task CompleteAsync()
    {
        lock (WriteLock)
        {
            // Pending writes need to be awaited first
            if (IsWriteInProgressUnsynchronized)
            {
                var ex = new InvalidOperationException("Can't complete the client stream writer because the previous write is in progress.");
                return Task.FromException(ex);
            }

            return _retryCallBase.ClientStreamCompleteAsync();
        }
    }

    public override Task WriteCoreAsync(TRequest message, CancellationToken token)
    {
        lock (WriteLock)
        {
            // CompleteAsync has already been called
            // Use explicit flag here. This error takes precedence over others.
            if (_retryCallBase.ClientStreamComplete)
                return CreateErrorTask("Request stream has already been completed.");

            // Pending writes need to be awaited first
            if (IsWriteInProgressUnsynchronized)
                return CreateErrorTask("Can't write the message because the previous write is in progress.");

            // Save write task to track whether it is complete. Must be set inside lock.
            WriteTask = _retryCallBase.ClientStreamWriteAsync(message, token);
        }

        return WriteTask;
    }

}
