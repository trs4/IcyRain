using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Client.Internal;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>
/// An abstract base type for <see cref="Resolver"/> implementations that use asynchronous polling logic to resolve the <see cref="Uri"/>.
/// <para>
/// <see cref="PollingResolver"/> adds a virtual <see cref="ResolveAsync"/> method. The resolver runs one asynchronous
/// resolve task at a time. Calling <see cref="Refresh()"/> on the resolver when a resolve task is already running has
/// no effect.
/// </para>
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public abstract partial class PollingResolver : Resolver
{
    private Task _resolveTask = Task.CompletedTask;
    private Action<ResolverResult>? _listener;
    private bool _disposed;
    private bool _resolveSuccessful;

    private readonly object _lock = new object();
#pragma warning disable CA2213 // Disposable fields should be disposed
    private readonly CancellationTokenSource _cts = new();
#pragma warning restore CA2213 // Disposable fields should be disposed
    private readonly IBackoffPolicyFactory? _backoffPolicyFactory;

    /// <summary>Gets the listener</summary>
    protected Action<ResolverResult> Listener => _listener!;

    /// <summary>Initializes a new instance of the <see cref="PollingResolver"/></summary>
    /// <param name="loggerFactory">The logger factory</param>
    protected PollingResolver() : this(null) { }

    /// <summary>Initializes a new instance of the <see cref="PollingResolver"/></summary>
    /// <param name="backoffPolicyFactory">The backoff policy factory</param>
    protected PollingResolver(IBackoffPolicyFactory? backoffPolicyFactory)
        => _backoffPolicyFactory = backoffPolicyFactory;

    /// <summary>Starts listening to resolver for results with the specified callback. Can only be called once
    /// <para>
    /// The <see cref="ResolverResult"/> passed to the callback has addresses when successful,
    /// otherwise a <see cref="Status"/> details the resolution error.
    /// </para>
    /// </summary>
    /// <param name="listener">The callback used to receive updates on the target.</param>
    public sealed override void Start(Action<ResolverResult> listener)
    {
        ArgumentNullException.ThrowIfNull(listener);

        if (_listener is not null)
            throw new InvalidOperationException("Resolver has already been started");

        _listener = (result) =>
        {
            if (result.Status.StatusCode == StatusCode.OK)
                _resolveSuccessful = true;

            listener(result);
        };

        OnStarted();
    }

    /// <summary>Executes after the resolver starts</summary>
    protected virtual void OnStarted() { }

    /// <summary>Refresh resolution. Can only be called after <see cref="Start(Action{ResolverResult})"/>
    /// <para>
    /// The resolver runs one asynchronous resolve task at a time. Calling <see cref="Refresh()"/> on the resolver when a
    /// resolve task is already running has no effect.
    /// </para>
    /// </summary>
    public sealed override void Refresh()
    {
        ObjectDisposedException.ThrowIf(_disposed, GetType());

        if (_listener is null)
            throw new InvalidOperationException("Resolver hasn't been started");

        lock (_lock)
        {
            if (_resolveTask.IsCompleted)
            {
                // Don't capture the current ExecutionContext and its AsyncLocals onto the connect
                var restoreFlow = false;
                try
                {
                    if (!ExecutionContext.IsFlowSuppressed())
                    {
                        ExecutionContext.SuppressFlow();
                        restoreFlow = true;
                    }

                    // Run ResolveAsync in a background task.
                    // This is done to prevent synchronous block inside ResolveAsync from blocking future Refresh calls.
                    _resolveTask = Task.Run(() => ResolveNowAsync(_cts.Token));
                }
                finally
                {
                    // Restore the current ExecutionContext
                    if (restoreFlow)
                        ExecutionContext.RestoreFlow();
                }
            }
        }
    }

    private async Task ResolveNowAsync(CancellationToken token)
    {
        // Reset resolve success to false. Will be set to true when an OK result is sent to listener.
        _resolveSuccessful = false;

        try
        {
            var backoffPolicy = _backoffPolicyFactory?.Create();

            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    await ResolveAsync(token).ConfigureAwait(false);

                    // ResolveAsync may report a failure but not throw. Check to see whether an OK result
                    // has been reported. If not then start retry loop.
                    if (_resolveSuccessful)
                        return;
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    // Ignore cancellation.
                    break;
                }
                catch (Exception ex)
                {
                    var status = GrpcProtocolHelpers.CreateStatusFromException("Error refreshing resolver.", ex);
                    Listener(ResolverResult.ForFailure(status));
                }

                // No backoff policy specified. Exit immediately.
                if (backoffPolicy is null)
                    break;

                var backoffTicks = backoffPolicy.NextBackoff().Ticks;
                // Task.Delay supports up to Int32.MaxValue milliseconds.
                // Note that even if the maximum backoff is configured to this maximum, the jitter could push it over the limit.
                // Force an upper bound here to ensure an unsupported backoff is never used.
                backoffTicks = Math.Min(backoffTicks, TimeSpan.TicksPerMillisecond * int.MaxValue);

                var backkoff = TimeSpan.FromTicks(backoffTicks);
                await Task.Delay(backkoff, token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { } // Ignore cancellation
        catch { }
    }

    /// <summary>
    /// Resolve the target <see cref="Uri"/>. Updated results are passed to the callback
    /// registered by <see cref="Start(Action{ResolverResult})"/>. Can only be called
    /// after the resolver has started.
    /// </summary>
    /// <param name="token">A cancellation token</param>
    /// <returns>A task</returns>
    protected abstract Task ResolveAsync(CancellationToken token);

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="LoadBalancer"/> and optionally releases
    /// the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        _cts.Cancel();
        _disposed = true;
    }

}
