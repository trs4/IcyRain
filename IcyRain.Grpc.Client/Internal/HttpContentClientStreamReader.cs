using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal sealed class HttpContentClientStreamReader<TRequest, TResponse> : IAsyncStreamReader<TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly GrpcCall<TRequest, TResponse> _call;
    private readonly object _moveNextLock;

    public TaskCompletionSource<(HttpResponseMessage, Status?)> HttpResponseTcs { get; }

    private HttpResponseMessage? _httpResponse;
    private string? _grpcEncoding;
    private Stream? _responseStream;
    private Task<bool>? _moveNextTask;

    public HttpContentClientStreamReader(GrpcCall<TRequest, TResponse> call)
    {
        _call = call;
        _moveNextLock = new object();
        HttpResponseTcs = new TaskCompletionSource<(HttpResponseMessage, Status?)>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public TResponse Current { get; private set; } = default!;

    public Task<bool> MoveNext(CancellationToken token)
    {
        _call.EnsureNotDisposed();

        // HTTP response has finished
        if (_call.CancellationToken.IsCancellationRequested)
        {
            if (!_call.Channel.ThrowOperationCanceledOnCancellation)
            {
                var ex = _call.CreateCanceledStatusException(new OperationCanceledException(_call.CancellationToken));
                return Task.FromException<bool>(ex);
            }
            else
            {
                if (_call.Options.CancellationToken.IsCancellationRequested)
                    return Task.FromCanceled<bool>(_call.Options.CancellationToken);

                return Task.FromCanceled<bool>(_call.CancellationToken);
            }
        }

        if (_call.CallTask.IsCompletedSuccessfully)
        {
            var status = _call.CallTask.Result;

            if (status.StatusCode == StatusCode.OK)
                return CommonGrpcProtocolHelpers.FalseTask; // Response is finished and it was successful so just return false
            else
                return Task.FromException<bool>(_call.CreateRpcException(status));
        }

        lock (_moveNextLock)
        {
            // Pending move next need to be awaited first
            if (IsMoveNextInProgressUnsynchronized)
            {
                var ex = new InvalidOperationException("Can't read the next message because the previous read is still in progress.");
                return Task.FromException<bool>(ex);
            }

            // Save move next task to track whether it is complete
            _moveNextTask = MoveNextCore(token);
        }

        return _moveNextTask;
    }

    private async Task<bool> MoveNextCore(CancellationToken token)
    {
        _call.TryRegisterCancellation(token, out var ctsRegistration);

        try
        {
            _call.CancellationToken.ThrowIfCancellationRequested();

            if (_httpResponse is null)
            {
                var (httpResponse, status) = await HttpResponseTcs.Task.ConfigureAwait(false);

                if (status != null && status.Value.StatusCode != StatusCode.OK)
                    throw _call.CreateFailureStatusException(status.Value);

                _httpResponse = httpResponse;
                _grpcEncoding = GrpcProtocolHelpers.GetGrpcEncoding(_httpResponse);
            }

            if (_responseStream is null)
            {
                try
                {
                    _responseStream = await _httpResponse.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
                }
                catch (ObjectDisposedException)
                {
                    // The response was disposed while waiting for the content stream to start.
                    // This will happen if there is no content stream (e.g. a streaming call finishes with no messages).
                    // Treat this like a cancellation.
                    throw new OperationCanceledException();
                }
            }

            Debug.Assert(_grpcEncoding is not null, "Encoding should have been calculated from response.");

            // Clear current before moving next. This prevents rooting the previous value while getting the next one.
            // In a long running stream this can allow the previous value to be GCed.
            Current = null!;

            var readMessage = await _call.ReadMessageAsync(
                _responseStream,
                _grpcEncoding,
                singleMessage: false,
                _call.CancellationToken).ConfigureAwait(false);

            if (readMessage is null)
            {
                // No more content in response so report status to call.
                // The call will handle finishing the response.
                var status = GrpcProtocolHelpers.GetResponseStatus(_httpResponse, _call.Channel.OperatingSystem.IsBrowser,
                    _call.Channel.HttpHandlerType == HttpHandlerType.WinHttpHandler);

                _call.ResponseStreamEnded(status, finishedGracefully: true);

                if (status.StatusCode != StatusCode.OK)
                    throw _call.CreateFailureStatusException(status);

                Current = null!;
                return false;
            }

            Current = readMessage!;
            return true;
        }
        catch (OperationCanceledException ex)
        {
            var resolvedCanceledException = _call.EnsureUserCancellationTokenReported(ex, token);

            if (_call.ResponseFinished)
            {
                // Call status will have been set before dispose.
                var status = await _call.CallTask.ConfigureAwait(false);

                if (status.StatusCode == StatusCode.OK)
                    return false; // Return false to indicate that the stream is complete without a message
                else if (status.StatusCode != StatusCode.DeadlineExceeded && status.StatusCode != StatusCode.Cancelled)
                    throw _call.CreateRpcException(status);
            }
            else
                _call.ResponseStreamEnded(new Status(StatusCode.Cancelled, ex.Message, resolvedCanceledException), finishedGracefully: false);

            if (!_call.Channel.ThrowOperationCanceledOnCancellation)
                throw _call.CreateCanceledStatusException(resolvedCanceledException);
            else
            {
                ExceptionDispatchInfo.Capture(resolvedCanceledException).Throw();
                return false; // Won't reach here.
            }
        }
        catch (Exception ex)
        {
            var newException = _call.ResolveException("Error reading next message.", ex, out var status, out var resolvedException);

            if (!_call.ResponseFinished)
                _call.ResponseStreamEnded(status.Value, finishedGracefully: false);

            if (newException)
                throw resolvedException; // Throw RpcException from MoveNext. Consistent with Grpc.Core
            else
                throw;
        }
        finally
        {
            ctsRegistration?.Dispose();
        }
    }

    /// <summary>
    /// A value indicating whether there is an async move next already in progress.
    /// Should only check this property when holding the move next lock.
    /// </summary>
    private bool IsMoveNextInProgressUnsynchronized
    {
        get
        {
            var moveNextTask = _moveNextTask;
            return moveNextTask is not null && !moveNextTask.IsCompleted;
        }
    }

}
