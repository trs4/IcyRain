using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal.Retry;

internal sealed class StatusGrpcCall<TRequest, TResponse> : IGrpcCall<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly Status _status;
    private readonly GrpcChannel _channel;
    private readonly Method<TRequest, TResponse> _method;
    private readonly TRequest? _request;
    private IClientStreamWriter<TRequest>? _clientStreamWriter;
    private IAsyncStreamReader<TResponse>? _clientStreamReader;

    public IClientStreamWriter<TRequest>? ClientStreamWriter => _clientStreamWriter ??= new StatusClientStreamWriter(_status);

    public IAsyncStreamReader<TResponse>? ClientStreamReader => _clientStreamReader ??= new StatusStreamReader(_status);

    public bool Disposed => true;

    public bool ResponseFinished => true;

    public int MessagesRead { get; }

    public object? CallWrapper { get; set; }

    public StatusGrpcCall(Status status, GrpcChannel channel, Method<TRequest, TResponse> method, int messagesRead, TRequest? request)
    {
        _status = status;
        _channel = channel;
        _method = method;
        MessagesRead = messagesRead;
        _request = request;
    }

    public void Dispose() { }

    public Task<TResponse> GetResponseAsync() => CreateErrorTask<TResponse>();

    public Task<Metadata> GetResponseHeadersAsync() => CreateErrorTask<Metadata>();

    private Task<T> CreateErrorTask<T>()
    {
        if (_channel.ThrowOperationCanceledOnCancellation && _status.DebugException is OperationCanceledException ex)
            return Task.FromException<T>(ex);

        return Task.FromException<T>(new RpcException(_status));
    }

    public Status GetStatus() => _status;

    public Metadata GetTrailers() => throw new InvalidOperationException("Can't get the call trailers because the call has not completed successfully.");

    public void StartClientStreaming() => throw new NotSupportedException();

    public void StartDuplexStreaming() => throw new NotSupportedException();

    public void StartServerStreaming(TRequest request) => throw new NotSupportedException();

    public void StartUnary(TRequest request) => throw new NotSupportedException();

    public Task WriteClientStreamAsync<TState>(Func<GrpcCall<TRequest, TResponse>, Stream, CallOptions, TState, Task> writeFunc, TState state,
        CancellationToken token)
        => Task.FromException(new RpcException(_status));

    public bool TryRegisterCancellation(CancellationToken token, [NotNullWhen(true)] out CancellationTokenRegistration? cancellationTokenRegistration)
    {
        cancellationTokenRegistration = null;
        return false;
    }

    public Exception CreateFailureStatusException(Status status)
    {
        if (_channel.ThrowOperationCanceledOnCancellation
            && (status.StatusCode == StatusCode.DeadlineExceeded || status.StatusCode == StatusCode.Cancelled))
        {
            // Convert status response of DeadlineExceeded to OperationCanceledException when
            // ThrowOperationCanceledOnCancellation is true.
            // This avoids a race between the client-side timer and the server status throwing different
            // errors on deadline exceeded.
            return new OperationCanceledException();
        }
        else
            return new RpcException(status);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => GrpcProtocolConstants.GetDebugEnumerator(_channel, _method, _request);

    private sealed class StatusClientStreamWriter : IClientStreamWriter<TRequest>
    {
        private readonly Status _status;

        public WriteOptions? WriteOptions { get; set; }

        public StatusClientStreamWriter(Status status)
            => _status = status;

        public Task CompleteAsync() => Task.FromException(new RpcException(_status));

        public Task WriteAsync(TRequest message) => Task.FromException(new RpcException(_status));
    }

    private sealed class StatusStreamReader : IAsyncStreamReader<TResponse>
    {
        private readonly Status _status;

        public TResponse Current { get; set; } = default!;

        public StatusStreamReader(Status status)
            => _status = status;

        public Task<bool> MoveNext(CancellationToken token)
            => Task.FromException<bool>(new RpcException(_status));
    }

}
