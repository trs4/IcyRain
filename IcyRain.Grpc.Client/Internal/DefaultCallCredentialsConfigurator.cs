using System.Collections.Generic;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal sealed class DefaultCallCredentialsConfigurator : CallCredentialsConfiguratorBase
{
    public AsyncAuthInterceptor? Interceptor { get; private set; }

    public IReadOnlyList<CallCredentials>? CompositeCredentials { get; private set; }

    // A place to cache the context to avoid creating a new instance for each auth interceptor call.
    // It's ok not to reset this state because the context is only used for the lifetime of the call.
    public AuthInterceptorContext? CachedContext { get; set; }

    public void ResetPerCallCredentialState()
    {
        Interceptor = null;
        CompositeCredentials = null;
    }

    public override void SetAsyncAuthInterceptorCredentials(object? state, AsyncAuthInterceptor interceptor)
        => Interceptor = interceptor;

    public override void SetCompositeCredentials(object? state, IReadOnlyList<CallCredentials> credentials)
        => CompositeCredentials = credentials;
}
