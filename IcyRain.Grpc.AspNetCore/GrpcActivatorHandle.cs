namespace IcyRain.Grpc.AspNetCore;

/// <summary>
/// Handle to the activator instance.
/// </summary>
/// <typeparam name="T">The instance type.</typeparam>
#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct GrpcActivatorHandle<T>
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
    /// <summary>
    /// Creates a new instance of <see cref="GrpcActivatorHandle{T}"/>.
    /// </summary>
    /// <param name="instance">The activated instance.</param>
    /// <param name="created">A value indicating whether the instance was created by the activator.</param>
    /// <param name="state">State related to the instance.</param>
    public GrpcActivatorHandle(T instance, bool created, object? state)
    {
        Instance = instance;
        Created = created;
        State = state;
    }

    /// <summary>
    /// Gets the activated instance.
    /// </summary>
    public T Instance { get; }

    /// <summary>
    /// Gets a value indicating whether the instanced was created by the activator.
    /// </summary>
    public bool Created { get; }

    /// <summary>
    /// Gets state related to the instance.
    /// </summary>
    public object? State { get; }
}
