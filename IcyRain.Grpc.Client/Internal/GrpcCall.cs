using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Client.Balancer.Internal;
using IcyRain.Grpc.Client.Internal.Http;
using IcyRain.Grpc.Service.Internal;

namespace IcyRain.Grpc.Client.Internal;

#pragma warning disable CA2000 // Dispose objects before losing scope
internal sealed partial class GrpcCall<TRequest, TResponse> : GrpcCall, IGrpcCall<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    internal const string ErrorStartingCallMessage = "Error starting gRPC call.";

#pragma warning disable CA2213 // Disposable fields should be disposed
    private readonly CancellationTokenSource _callCts;
#pragma warning restore CA2213 // Disposable fields should be disposed
    private readonly TaskCompletionSource<HttpResponseMessage> _httpResponseTcs;
    private readonly TaskCompletionSource<Status> _callTcs;
    private readonly GrpcMethodInfo _grpcMethodInfo;
    private readonly int _attemptCount;

    internal Task<HttpResponseMessage> HttpResponseTask => _httpResponseTcs.Task;
    private Task<Metadata>? _responseHeadersTask;
    private Timer? _deadlineTimer;
    private DateTime _deadline;
    private CancellationTokenRegistration? _ctsRegistration;

    public bool Disposed { get; private set; }
    public Method<TRequest, TResponse> Method { get; }

    // These are set depending on the type of gRPC call
    private TaskCompletionSource<TResponse>? _responseTcs;
    private TRequest? _request;

    public int MessagesWritten { get; private set; }

    public int MessagesRead { get; private set; }

    public HttpContentClientStreamWriter<TRequest, TResponse>? ClientStreamWriter { get; private set; }

    public HttpContentClientStreamReader<TRequest, TResponse>? ClientStreamReader { get; private set; }

    public GrpcCall(Method<TRequest, TResponse> method, GrpcMethodInfo grpcMethodInfo, CallOptions options, GrpcChannel channel,
        int attemptCount, bool forceAsyncHttpResponse)
        : base(options, channel)
    {
        // Validate deadline before creating any objects that require cleanup
        GrpcCall<TRequest, TResponse>.ValidateDeadline(options.Deadline);

        _callCts = new CancellationTokenSource();
        // Retries and hedging can run multiple calls at the same time and use locking for thread-safety.
        // Running HTTP response continuation asynchronously is required for locking to work correctly.
        _httpResponseTcs = new TaskCompletionSource<HttpResponseMessage>(
            forceAsyncHttpResponse ? TaskCreationOptions.RunContinuationsAsynchronously : TaskCreationOptions.None);
        // Run the callTcs continuation immediately to keep the same context. Required for Activity.
        _callTcs = new TaskCompletionSource<Status>();
        Method = method;
        _grpcMethodInfo = grpcMethodInfo;
        _deadline = options.Deadline ?? DateTime.MaxValue;
        _attemptCount = attemptCount;

        Channel.RegisterActiveCall(this);
    }

    public MethodConfigInfo? MethodConfig => _grpcMethodInfo.MethodConfig;

    private static void ValidateDeadline(DateTime? deadline)
    {
        if (deadline is not null && deadline != DateTime.MaxValue && deadline != DateTime.MinValue && deadline.Value.Kind != DateTimeKind.Utc)
            throw new InvalidOperationException("Deadline must have a kind DateTimeKind.Utc or be equal to DateTime.MaxValue or DateTime.MinValue");
    }

    public override Task<Status> CallTask => _callTcs.Task;

    public override CancellationToken CancellationToken => _callCts.Token;

    public override Type RequestType => typeof(TRequest);

    public override Type ResponseType => typeof(TResponse);

    IClientStreamWriter<TRequest>? IGrpcCall<TRequest, TResponse>.ClientStreamWriter => ClientStreamWriter;

    IAsyncStreamReader<TResponse>? IGrpcCall<TRequest, TResponse>.ClientStreamReader => ClientStreamReader;

    public object? CallWrapper { get; set; }

    public void StartUnary(TRequest request)
    {
        _request = request;
        StartUnaryCore(CreatePushUnaryContent(request));
    }

    public void StartClientStreaming()
    {
        var clientStreamWriter = new HttpContentClientStreamWriter<TRequest, TResponse>(this);
        var content = new PushStreamContent<TRequest, TResponse>(clientStreamWriter);

        StartClientStreamingCore(clientStreamWriter, content);
    }

    public void StartServerStreaming(TRequest request) => StartServerStreamingCore(CreatePushUnaryContent(request));

    private HttpContent CreatePushUnaryContent(TRequest request)
    {
        // WinHttp currently doesn't support streaming request data so a length needs to be specified.
        // This may change in a future version of Windows. When that happens an OS build version check
        // can be added here to avoid WinHttpUnaryContent.
        return Channel.HttpHandlerType != HttpHandlerType.WinHttpHandler
            ? new PushUnaryContent<TRequest, TResponse>(request, WriteAsync)
            : new WinHttpUnaryContent<TRequest, TResponse>(request, WriteAsync, this);

        Task WriteAsync(TRequest request, Stream stream)
            => WriteMessageAsync(stream, request, Options);
    }

    public void StartDuplexStreaming()
    {
        var clientStreamWriter = new HttpContentClientStreamWriter<TRequest, TResponse>(this);
        var content = new PushStreamContent<TRequest, TResponse>(clientStreamWriter);

        StartDuplexStreamingCore(clientStreamWriter, content);
    }

    internal void StartUnaryCore(HttpContent content)
    {
        // Not created with RunContinuationsAsynchronously to avoid unnecessary dispatch to the thread pool.
        // The TCS is set from RunCall but it is the last operation before the method exits so there shouldn't
        // be an impact from running the response continutation synchronously.
        _responseTcs = new TaskCompletionSource<TResponse>();

        var timeout = GetTimeout();
        var message = CreateHttpRequestMessage(timeout);
        SetMessageContent(content, message);
        _ = RunCall(message, timeout);
    }

    internal void StartServerStreamingCore(HttpContent content)
    {
        var timeout = GetTimeout();
        var message = CreateHttpRequestMessage(timeout);
        SetMessageContent(content, message);
        ClientStreamReader = new HttpContentClientStreamReader<TRequest, TResponse>(this);
        _ = RunCall(message, timeout);
    }

    internal void StartClientStreamingCore(HttpContentClientStreamWriter<TRequest, TResponse> clientStreamWriter, HttpContent content)
    {
        // Not created with RunContinuationsAsynchronously to avoid unnecessary dispatch to the thread pool.
        // The TCS is set from RunCall but it is the last operation before the method exits so there shouldn't
        // be an impact from running the response continutation synchronously.
        _responseTcs = new TaskCompletionSource<TResponse>();

        var timeout = GetTimeout();
        var message = CreateHttpRequestMessage(timeout);
        SetWriter(message, clientStreamWriter, content);
        _ = RunCall(message, timeout);
    }

    public void StartDuplexStreamingCore(HttpContentClientStreamWriter<TRequest, TResponse> clientStreamWriter, HttpContent content)
    {
        var timeout = GetTimeout();
        var message = CreateHttpRequestMessage(timeout);
        SetWriter(message, clientStreamWriter, content);
        ClientStreamReader = new HttpContentClientStreamReader<TRequest, TResponse>(this);
        _ = RunCall(message, timeout);
    }

    public void Dispose()
    {
        Disposed = true;

        Cleanup(GrpcProtocolConstants.CreateDisposeCanceledStatus(exception: null));

        // If the call was disposed then observe any potential response exception.
        // Observe the task's exception to prevent TaskScheduler.UnobservedTaskException from firing.
        _responseTcs?.Task.ObserveException();
    }

    /// <summary>
    /// Clean up can be called by:
    /// 1. The user. AsyncUnaryCall.Dispose et al will call this on Dispose
    /// 2. <see cref="GrpcCall.ValidateHeaders"/> will call dispose if errors fail validation
    /// 3. <see cref="FinishResponseAndCleanUp"/> will call dispose
    /// </summary>
    private void Cleanup(Status status)
    {
        if (!ResponseFinished)
        {
            // If the response is not finished then cancel any pending actions:
            // 1. Call HttpClient.SendAsync
            // 2. Response Stream.ReadAsync
            // 3. Client stream
            //    - Getting the Stream from the Request.HttpContent
            //    - Holding the Request.HttpContent.SerializeToStream open
            //    - Writing to the client stream
            CancelCall(status);
        }
        else
        {
            _callTcs.TrySetResult(status);

            ClientStreamWriter?.WriteStreamTcs.TrySetCanceled();
            ClientStreamWriter?.CompleteTcs.TrySetCanceled();
            ClientStreamReader?.HttpResponseTcs.TrySetCanceled();
        }

        Channel.FinishActiveCall(this);

        _ctsRegistration?.Dispose();

        if (_deadlineTimer is not null)
        {
            lock (this)
            {
                // Timer callback can call Timer.Change so dispose deadline timer in a lock
                // and set to null to indicate to the callback that it has been disposed.
                _deadlineTimer?.Dispose();
                _deadlineTimer = null;
            }
        }

        HttpResponse?.Dispose();

        // To avoid racing with Dispose, skip disposing the call CTS.
        // This avoid Dispose potentially calling cancel on a disposed CTS.
        // The call CTS is not exposed externally and all dependent registrations
        // are cleaned up.
    }

    public void EnsureNotDisposed()
        => ObjectDisposedException.ThrowIf(Disposed, typeof(GrpcCall<TRequest, TResponse>));

    private void FinishResponseAndCleanUp(Status status)
    {
        ResponseFinished = true;

        // Clean up call resources once this call is finished
        // Call may not be explicitly disposed when used with unary methods
        // e.g. var reply = await client.SayHelloAsync(new HelloRequest());
        Cleanup(status);
    }

    /// <summary>
    /// Used by response stream reader to report it is finished.
    /// </summary>
    /// <param name="status">The completed response status code.</param>
    /// <param name="finishedGracefully">true when the end of the response stream was read, otherwise false.</param>
    public void ResponseStreamEnded(Status status, bool finishedGracefully)
    {
        if (finishedGracefully)
        {
            // Set response finished immediately rather than set it in logic resumed
            // from the callTcs to avoid race condition.
            // e.g. response stream finished and then immediately call GetTrailers().
            ResponseFinished = true;
        }

        _callTcs.TrySetResult(status);
    }

    public Task<Metadata> GetResponseHeadersAsync()
    {
        if (_responseHeadersTask is null)
        {
            // Allocate metadata and task only when requested.
            _responseHeadersTask = GetResponseHeadersCoreAsync();

            // ResponseHeadersAsync could be called inside a client interceptor when a call is wrapped.
            // Most people won't use the headers result. Observed exception to avoid unobserved exception event.
            _responseHeadersTask.ObserveException();
        }

        return _responseHeadersTask;
    }

    private async Task<Metadata> GetResponseHeadersCoreAsync()
    {
        try
        {
            var httpResponse = await HttpResponseTask.ConfigureAwait(false);

            // Check if the headers have a status. If they do then wait for the overall call task
            // to complete before returning headers. This means that if the call failed with a
            // a status then it is possible to await response headers and then call GetStatus().
            var grpcStatus = HttpRequestHelpers.GetHeaderValue(httpResponse.Headers, GrpcProtocolConstants.StatusTrailer);

            if (grpcStatus is not null)
                await CallTask.ConfigureAwait(false);

            var metadata = GrpcProtocolHelpers.BuildMetadata(httpResponse.Headers);

            if (_attemptCount > 1)
                metadata.Add(GrpcProtocolConstants.RetryPreviousAttemptsHeader, (_attemptCount - 1).ToString(CultureInfo.InvariantCulture));

            return metadata;
        }
        catch (Exception ex)
        {
            // If there was an error fetching response headers then it's likely the same error is reported
            // by response TCS. The user is unlikely to observe both errors.
            // Observe the task's exception to prevent TaskScheduler.UnobservedTaskException from firing
            _responseTcs?.Task.ObserveException();

            if (ResolveException(ErrorStartingCallMessage, ex, out _, out var resolvedException))
                throw resolvedException;
            else
                throw;
        }
    }

    public Status GetStatus()
    {
        if (CallTask.IsCompletedSuccessfully)
            return CallTask.Result;

        throw new InvalidOperationException("Unable to get the status because the call is not complete.");
    }

    public Task<TResponse> GetResponseAsync()
    {
        Debug.Assert(_responseTcs is not null);
        return _responseTcs.Task;
    }

    public Metadata GetTrailers()
    {
        if (!TryGetTrailers(out var trailers))
        {
            // Throw InvalidOperationException here because documentation on GetTrailers says that
            // InvalidOperationException is thrown if the call is not complete
            throw new InvalidOperationException("Can't get the call trailers because the call has not completed successfully");
        }

        return trailers;
    }

    private void SetMessageContent(HttpContent content, HttpRequestMessage message)
    {
        RequestGrpcEncoding = GrpcProtocolHelpers.GetRequestEncoding(message.Headers);
        message.Content = content;
    }

    public bool TryRegisterCancellation(
        CancellationToken token,
        [NotNullWhen(true)] out CancellationTokenRegistration? cancellationTokenRegistration)
    {
        // Only register if the token:
        // 1. Can be canceled.
        // 2. The token isn't the same one used in CallOptions. Already listening for its cancellation.
        if (token.CanBeCanceled && token != Options.CancellationToken)
        {
            cancellationTokenRegistration = RegisterCancellation(token);
            return true;
        }

        cancellationTokenRegistration = null;
        return false;
    }

    private CancellationTokenRegistration RegisterCancellation(CancellationToken token)
        => token.Register(static (state, ct) =>
        {
            var call = (GrpcCall<TRequest, TResponse>)state!;
            call.CancelCallFromCancellationToken(ct);
        }, this);

    private void CancelCallFromCancellationToken(CancellationToken token)
        => CancelCall(GrpcProtocolConstants.CreateClientCanceledStatus(new OperationCanceledException(token)));

    private void CancelCall(Status status)
    {
        // Set overall call status first. Status can be used in throw RpcException from cancellation.
        // If response has successfully finished then the status will come from the trailers.
        // If it didn't finish then complete with a status.
        _callTcs.TrySetResult(status);

        // Checking if cancellation has already happened isn't threadsafe
        // but there is no adverse effect other than an extra log message
        if (!_callCts.IsCancellationRequested)
        {
            // Cancel in-progress HttpClient.SendAsync and Stream.ReadAsync tasks.
            // Cancel will send RST_STREAM if HttpClient.SendAsync isn't complete.
            // Cancellation will also cause reader/writer to throw if used afterwards.
            _callCts.Cancel();

            // Cancellation token won't send RST_STREAM if HttpClient.SendAsync is complete.
            // Dispose HttpResponseMessage to send RST_STREAM to server for in-progress calls.
            HttpResponse?.Dispose();

            // Canceling call will cancel pending writes to the stream
            ClientStreamWriter?.WriteStreamTcs.TrySetCanceled();
            ClientStreamWriter?.CompleteTcs.TrySetCanceled();
            ClientStreamReader?.HttpResponseTcs.TrySetCanceled();
        }
    }

    private async Task RunCall(HttpRequestMessage request, TimeSpan? timeout)
    {
        InitializeCall(request, timeout);
        Status? status; // Unset variable to check that FinishCall is called in every code path

        try
        {
            // User supplied call credentials could error and so must be run
            // inside try/catch to handle errors.
            if (Options.Credentials != null || Channel.CallCredentials?.Count > 0)
                await ReadCredentials(request).ConfigureAwait(false);

            // Fail early if deadline has already been exceeded
            _callCts.Token.ThrowIfCancellationRequested();

            // If a HttpClient has been specified then we need to call it with ResponseHeadersRead
            // so that the response message is available for streaming
            var httpResponseTask = (Channel.HttpInvoker is HttpClient httpClient)
                ? httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _callCts.Token)
                : Channel.HttpInvoker.SendAsync(request, _callCts.Token);

            HttpResponse = await httpResponseTask.ConfigureAwait(false);
            _httpResponseTcs.TrySetResult(HttpResponse);

            status = ValidateHeaders(HttpResponse, out var trailers);

            if (trailers is not null)
                Trailers = trailers;

            // A status means either the call has failed or grpc-status was returned in the response header
            if (status != null)
            {
                if (_responseTcs != null)
                {
                    // gRPC status in the header
                    if (status.Value.StatusCode != StatusCode.OK)
                        FinishCall(request, status.Value);
                    else
                    {
                        // Change the status code to a more accurate status.
                        // This is consistent with Grpc.Core client behavior.
                        status = new Status(StatusCode.Internal,
                            "Failed to deserialize response message. The response header contains a gRPC status of OK, which means any message returned to the client for this call should be ignored. " +
                            "A unary or client streaming gRPC call must have a response message, which makes this response invalid.");

                        FinishCall(request, status.Value);
                    }

                    FinishResponseAndCleanUp(status.Value);

                    // Set failed result makes the response task thrown an error. Must be called after
                    // the response is finished. Reasons:
                    // - Finishing the response sets the status. Required for GetStatus to be successful.
                    // - We want GetStatus to always work when called after the response task is done.
                    SetFailedResult(status.Value);
                }
                else
                {
                    FinishCall(request, status.Value);
                    FinishResponseAndCleanUp(status.Value);
                }
            }
            else if (_responseTcs is not null)
            {
                // Read entire response body immediately and read status from trailers
                // Trailers are only available once the response body had been read
                var responseStream = await HttpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

                var message = await ReadMessageAsync(
                    responseStream,
                    GrpcProtocolHelpers.GetGrpcEncoding(HttpResponse),
                    singleMessage: true,
                    _callCts.Token).ConfigureAwait(false);

                status = GrpcProtocolHelpers.GetResponseStatus(
                    HttpResponse, Channel.OperatingSystem.IsBrowser, Channel.HttpHandlerType == HttpHandlerType.WinHttpHandler);

                if (message is null)
                {
                    if (status.Value.StatusCode == StatusCode.OK)
                    {
                        // Change the status code if OK is returned to a more accurate status.
                        // This is consistent with Grpc.Core client behavior.
                        status = new Status(StatusCode.Internal, "Failed to deserialize response message");
                    }

                    FinishResponseAndCleanUp(status.Value);
                    FinishCall(request, status.Value);

                    // Set failed result makes the response task thrown an error. Must be called after
                    // the response is finished. Reasons:
                    // - Finishing the response sets the status. Required for GetStatus to be successful.
                    // - We want GetStatus to always work when called after the response task is done.
                    SetFailedResult(status.Value);
                }
                else
                {
                    FinishResponseAndCleanUp(status.Value);
                    FinishCall(request, status.Value);

                    if (status.Value.StatusCode == StatusCode.OK)
                        _responseTcs.TrySetResult(message);
                    else
                    {
                        // Set failed result makes the response task thrown an error. Must be called after
                        // the response is finished. Reasons:
                        // - Finishing the response sets the status. Required for GetStatus to be successful.
                        // - We want GetStatus to always work when called after the response task is done.
                        SetFailedResult(status.Value);
                    }
                }
            }
            else
            {
                // Duplex or server streaming call
                Debug.Assert(ClientStreamReader is not null);
                ClientStreamReader.HttpResponseTcs.TrySetResult((HttpResponse, status));

                // Wait until the response has been read and status read from trailers.
                // TCS will also be set in Dispose.
                status = await CallTask.ConfigureAwait(false);

                FinishCall(request, status.Value);
                Cleanup(status.Value);
            }
        }
        catch (Exception ex)
        {
            ResolveException(ErrorStartingCallMessage, ex, out status, out var resolvedException);

            FinishCall(request, status.Value);

            // Update HTTP response TCS before clean up. Needs to happen first because cleanup will
            // cancel the TCS for anyone still listening.
            _httpResponseTcs.TrySetException(resolvedException);
            _httpResponseTcs.Task.ObserveException();

            Cleanup(status.Value);

            // Update response TCS after overall call status is resolved. This is required so that
            // the call is completed before an error is thrown from ResponseAsync. If it happens
            // afterwards then there is a chance GetStatus() will error because the call isn't complete.
            if (_responseTcs is not null)
            {
                _responseTcs.TrySetException(resolvedException);

                // Always observe cancellation-like exceptions
                if (IsCancellationOrDeadlineException(ex))
                    _responseTcs.Task.ObserveException();
            }
        }

        // Should be completed before exiting.
        Debug.Assert(_httpResponseTcs.Task.IsCompleted);
        Debug.Assert(_responseTcs is null || _responseTcs.Task.IsCompleted);
    }

    private bool IsCancellationOrDeadlineException(Exception ex)
    {
        // Don't log OperationCanceledException if deadline has exceeded
        // or the call has been canceled.
        if (ex is OperationCanceledException
            && _callTcs.Task.IsCompletedSuccessfully
            && (_callTcs.Task.Result.StatusCode == StatusCode.DeadlineExceeded || _callTcs.Task.Result.StatusCode == StatusCode.Cancelled))
        {
            return true;
        }

        // Exception may specify RST_STREAM or abort code that resolves to cancellation.
        // If protocol error is cancellation and deadline has been exceeded then that
        // means the server canceled and the local deadline timer hasn't triggered.
        if (GrpcProtocolHelpers.ResolveRpcExceptionStatusCode(ex) == StatusCode.Cancelled)
            return true;

        return false;
    }

    /// <summary>
    /// Resolve the specified exception to an end-user exception that will be thrown from the client.
    /// The resolved exception is normally a RpcException. Returns true when the resolved exception is changed.
    /// </summary>
    internal bool ResolveException(string summary, Exception ex, [NotNull] out Status? status, out Exception resolvedException)
    {
        if (ex is OperationCanceledException)
        {
            ex = EnsureUserCancellationTokenReported(ex, CancellationToken.None);
            status = CallTask.IsCompletedSuccessfully ? CallTask.Result : new Status(StatusCode.Cancelled, string.Empty, ex);

            if (!Channel.ThrowOperationCanceledOnCancellation)
            {
                resolvedException = CreateRpcException(status.Value);
                return true;
            }
        }
        else if (ex is RpcException rpcException)
        {
            status = rpcException.Status;

            // If trailers have been set, and the RpcException isn't using them, then
            // create new RpcException with trailers. Want to try and avoid this as
            // the exact stack location will be lost.
            //
            // Trailers could be set in another thread so copy to local variable.
            var trailers = Trailers;

            if (trailers is not null && rpcException.Trailers != trailers)
            {
                resolvedException = CreateRpcException(status.Value);
                return true;
            }
        }
        else
        {
            var s = GrpcProtocolHelpers.CreateStatusFromException(summary, ex);

            // The server could exceed the deadline and return a CANCELLED status before the
            // client's deadline timer is triggered. When CANCELLED is received check the
            // deadline against the clock and change status to DEADLINE_EXCEEDED if required.
            if (s.StatusCode == StatusCode.Cancelled)
            {
                lock (this)
                {
                    if (IsDeadlineExceededUnsynchronized())
                        s = new Status(StatusCode.DeadlineExceeded, s.Detail, s.DebugException);
                }
            }

            status = s;
            resolvedException = CreateRpcException(s);
            return true;
        }

        resolvedException = ex;
        return false;
    }

    // Report correct CancellationToken. This method creates a new OperationCanceledException with a different CancellationToken.
    // It attempts to preserve the original stack trace using ExceptionDispatchInfo where available.
    [StackTraceHidden]
    public Exception EnsureUserCancellationTokenReported(Exception ex, CancellationToken token)
    {
        var resultToken = GetCanceledToken(token);

        if (resultToken != CancellationToken)
            return ExceptionDispatchInfo.SetCurrentStackTrace(new OperationCanceledException(ex.Message, ex, resultToken));

        return ex;
    }

    private void SetFailedResult(Status status)
    {
        Debug.Assert(_responseTcs != null);

        if (Channel.ThrowOperationCanceledOnCancellation && status.StatusCode == StatusCode.DeadlineExceeded)
        {
            // Convert status response of DeadlineExceeded to OperationCanceledException when
            // ThrowOperationCanceledOnCancellation is true.
            // This avoids a race between the client-side timer and the server status throwing different
            // errors on deadline exceeded.
            _responseTcs.TrySetCanceled();
        }
        else
            _responseTcs.TrySetException(CreateRpcException(status));
    }

    private void InitializeCall(HttpRequestMessage request, TimeSpan? timeout)
    {
        // Deadline will cancel the call CTS
        // Only exceed deadline/start timer after reader/writer have been created, otherwise deadline will cancel
        // the call CTS before they are created and leave them in a non-canceled state
        if (timeout != null)
        {
            if (timeout.Value <= TimeSpan.Zero)
            {
                lock (this) // Call was started with a deadline in the past so immediately trigger deadline exceeded
                    DeadlineExceeded();
            }
            else
            {
                var dueTime = CommonGrpcProtocolHelpers.GetTimerDueTime(timeout.Value, Channel.MaxTimerDueTime);
                _deadlineTimer = NonCapturingTimer.Create(DeadlineExceededCallback, state: null, TimeSpan.FromMilliseconds(dueTime), Timeout.InfiniteTimeSpan);
            }
        }

        if (Options.CancellationToken.CanBeCanceled)
        {
            // The cancellation token will cancel the call CTS
            // This must be registered after the client writer has been created
            // so that cancellation will always complete the writer
            _ctsRegistration = RegisterCancellation(Options.CancellationToken);
        }
    }

    private void FinishCall(HttpRequestMessage request, Status status)
    {
        if (status.StatusCode == StatusCode.DeadlineExceeded)
        {
            // Usually a deadline will be triggered via the deadline timer. However,
            // if the client and server are on the same machine it is possible for the
            // client to get the response before the timer triggers. In that situation
            // treat a returned DEADLINE_EXCEEDED status as the client exceeding deadline.
            // To ensure that the deadline counter isn't incremented twice in a race
            // between the timer and status, lock and use _deadline to check whether
            // the client has processed that it has exceeded or not.
            lock (this)
            {
                if (IsDeadlineExceededUnsynchronized())
                    _deadline = DateTime.MaxValue;
            }
        }
    }

    private bool IsDeadlineExceededUnsynchronized()
    {
        Debug.Assert(Monitor.IsEntered(this), "Check deadline in a lock. Updating a DateTime isn't guaranteed to be atomic. Avoid struct tearing.");
        return _deadline <= DateTime.UtcNow;
    }

    private async Task ReadCredentials(HttpRequestMessage request)
    {
        // In C-Core the call credential auth metadata is only applied if the channel is secure
        // The equivalent in grpc-dotnet is only applying metadata if HttpClient is using TLS
        // HttpClient scheme will be HTTP if it is using H2C (HTTP2 without TLS)
        if (Channel.IsSecure || Channel.UnsafeUseInsecureChannelCallCredentials)
        {
            var configurator = new DefaultCallCredentialsConfigurator();

            if (Options.Credentials != null)
                await GrpcProtocolHelpers.ReadCredentialMetadata(configurator, Channel, request, Method, Options.Credentials, _callCts.Token).ConfigureAwait(false);

            if (Channel.CallCredentials?.Count > 0)
            {
                foreach (var credentials in Channel.CallCredentials)
                    await GrpcProtocolHelpers.ReadCredentialMetadata(configurator, Channel, request, Method, credentials, _callCts.Token).ConfigureAwait(false);
            }
        }
    }

    private void SetWriter(HttpRequestMessage message, HttpContentClientStreamWriter<TRequest, TResponse> clientStreamWriter, HttpContent content)
    {
        RequestGrpcEncoding = GrpcProtocolHelpers.GetRequestEncoding(message.Headers);
        ClientStreamWriter = clientStreamWriter;
        message.Content = content;
    }

    private HttpRequestMessage CreateHttpRequestMessage(TimeSpan? timeout)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, _grpcMethodInfo.CallUri)
        {
            Version = Channel.HttpVersion,
            VersionPolicy = Channel.HttpVersionPolicy,
        };

        // Set raw headers on request using name/values. Typed headers allocate additional objects.
        var headers = message.Headers;

        // User agent is optional but recommended.
        headers.TryAddWithoutValidation(GrpcProtocolConstants.UserAgentHeader, GrpcProtocolConstants.UserAgentHeaderValue);
        // TE is required by some servers, e.g. C Core.
        // A missing TE header results in servers aborting the gRPC call.
        headers.TryAddWithoutValidation(GrpcProtocolConstants.TEHeader, GrpcProtocolConstants.TEHeaderValue);
        //headers.TryAddWithoutValidation(GrpcProtocolConstants.MessageAcceptEncodingHeader, Channel.MessageAcceptEncoding);

        // https://github.com/grpc/proposal/blob/master/A6-client-retries.md#exposed-retry-metadata
        if (_attemptCount > 1)
            headers.TryAddWithoutValidation(GrpcProtocolConstants.RetryPreviousAttemptsHeader, (_attemptCount - 1).ToString(CultureInfo.InvariantCulture));

        if (Options.Headers != null && Options.Headers.Count > 0)
        {
            foreach (var entry in Options.Headers)
            {
                if (entry.Key == GrpcProtocolConstants.TimeoutHeader)
                {
                    // grpc-timeout is set via CallOptions.Deadline
                    continue;
                }
                else if (entry.Key == GrpcProtocolConstants.CompressionRequestAlgorithmHeader)
                {
                    // grpc-internal-encoding-request is used in the client to set message compression.
                    // 'grpc-encoding' is sent even if WriteOptions.Flags = NoCompress. In that situation
                    // individual messages will not be written with compression.
                    headers.TryAddWithoutValidation(GrpcProtocolConstants.MessageEncodingHeader, entry.Value);
                }
                else
                    GrpcProtocolHelpers.AddHeader(headers, entry);
            }
        }

        if (timeout is not null)
            headers.TryAddWithoutValidation(GrpcProtocolConstants.TimeoutHeader, GrpcProtocolHelpers.EncodeTimeout(timeout.Value.Ticks / TimeSpan.TicksPerMillisecond));

        if (Options.IsWaitForReady)
            message.SetOption(BalancerHttpHandler.WaitForReadyKey, true);

        return message;
    }

    private TimeSpan? GetTimeout()
    {
        if (_deadline == DateTime.MaxValue)
            return null;

        var timeout = _deadline - DateTime.UtcNow;

        // Maxmimum deadline of 99999999s is consistent with Grpc.Core
        // https://github.com/grpc/grpc/blob/907a1313a87723774bf59d04ed432602428245c3/src/core/lib/transport/timeout_encoding.h#L32-L34
        const long MaxDeadlineTicks = 99999999 * TimeSpan.TicksPerSecond;

        if (timeout.Ticks > MaxDeadlineTicks)
            timeout = TimeSpan.FromTicks(MaxDeadlineTicks);

        return timeout;
    }

    private void DeadlineExceededCallback(object? state)
    {
        try
        {
            // Deadline is only exceeded if the timeout has passed and
            // the response has not been finished or canceled
            if (!_callCts.IsCancellationRequested && !ResponseFinished)
            {
                TimeSpan remaining;

                lock (this)
                {
                    // If _deadline is MaxValue then the DEADLINE_EXCEEDED status has
                    // already been received by the client and the timer can stop.
                    if (_deadline == DateTime.MaxValue)
                        return;

                    remaining = _deadline - DateTime.UtcNow;

                    if (remaining <= TimeSpan.Zero)
                    {
                        DeadlineExceeded();
                        return;
                    }

                    if (_deadlineTimer != null)
                    {
                        var dueTime = CommonGrpcProtocolHelpers.GetTimerDueTime(remaining, Channel.MaxTimerDueTime);
                        _deadlineTimer.Change(dueTime, Timeout.Infinite);
                    }
                }
            }
        }
        catch { }
    }

    private void DeadlineExceeded()
    {
        Debug.Assert(Monitor.IsEntered(this));

        // Set _deadline to DateTime.MaxValue to signal that deadline has been exceeded.
        // This prevents duplicate logging and cancellation.
        _deadline = DateTime.MaxValue;

        CancelCall(new Status(StatusCode.DeadlineExceeded, string.Empty));
    }

    internal Task WriteMessageAsync(Stream stream, ReadOnlyMemory<byte> message, CancellationToken token)
    {
        MessagesWritten++;
        return stream.WriteMessageAsync(this, message, token);
    }

    internal Task WriteMessageAsync(Stream stream, TRequest message, CallOptions callOptions)
    {
        MessagesWritten++;
        return stream.WriteMessageAsync(this, message, Method.RequestMarshaller.ContextualSerializer, callOptions);
    }

    internal async ValueTask<TResponse?> ReadMessageAsync(Stream responseStream, string grpcEncoding, bool singleMessage, CancellationToken token)
    {
        var message = await responseStream.ReadMessageAsync(
            this,
            Method.ResponseMarshaller.ContextualDeserializer,
            grpcEncoding,
            singleMessage,
            token).ConfigureAwait(false);

        if (message is null)
            return null;

        MessagesRead++;
        return message;
    }

    public Task WriteClientStreamAsync<TState>(Func<GrpcCall<TRequest, TResponse>, Stream, CallOptions, TState, Task> writeFunc, TState state,
        CancellationToken token)
        => ClientStreamWriter!.WriteAsync(writeFunc, state, token);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => GrpcProtocolConstants.GetDebugEnumerator(Channel, Method, _request);
}
#pragma warning restore CA2000 // Dispose objects before losing scope
