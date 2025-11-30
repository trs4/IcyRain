using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal interface IGrpcCall<TRequest, TResponse> : IDisposable, IEnumerable<KeyValuePair<string, object>>
    where TRequest : class
    where TResponse : class
{
    object? CallWrapper { get; set; }

    bool Disposed { get; }

    bool ResponseFinished { get; }

    int MessagesRead { get; }

    Task<TResponse> GetResponseAsync();

    Task<Metadata> GetResponseHeadersAsync();

    Status GetStatus();

    Metadata GetTrailers();

    IClientStreamWriter<TRequest>? ClientStreamWriter { get; }

    IAsyncStreamReader<TResponse>? ClientStreamReader { get; }

    void StartUnary(TRequest request);

    void StartClientStreaming();

    void StartServerStreaming(TRequest request);

    void StartDuplexStreaming();

    Task WriteClientStreamAsync<TState>(
        Func<GrpcCall<TRequest, TResponse>, Stream, CallOptions, TState, Task> writeFunc,
        TState state,
        CancellationToken token);

    Exception CreateFailureStatusException(Status status);

    bool TryRegisterCancellation(
        CancellationToken token,
        [NotNullWhen(true)] out CancellationTokenRegistration? cancellationTokenRegistration);
}
