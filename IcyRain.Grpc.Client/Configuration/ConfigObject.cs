using System.Collections.Generic;
using IcyRain.Grpc.Client.Internal.Configuration;

namespace IcyRain.Grpc.Client.Configuration;

/// <summary>Represents a configuration object. Implementations provide strongly typed wrappers over collections of untyped values</summary>
public abstract class ConfigObject : IConfigValue
{
    internal ConfigObject() : this(new Dictionary<string, object>()) { }

    internal ConfigObject(IDictionary<string, object> inner)
        => Inner = inner;

    /// <summary>Gets the underlying configuration values</summary>
    public IDictionary<string, object> Inner { get; }

    object IConfigValue.Inner => Inner;

    internal T? GetValue<T>(string key) => Inner.TryGetValue(key, out var value) ? (T?)value : default;

    internal void SetValue<T>(string key, T? value)
    {
        if (value is null)
            Inner.Remove(key);
        else
            Inner[key] = value;
    }

}
