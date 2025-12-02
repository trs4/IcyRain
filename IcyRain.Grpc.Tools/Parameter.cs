using System;

namespace IcyRain.Grpc.Tools;

internal sealed class Parameter
{
    public Parameter(string type)
        => Type = string.IsNullOrEmpty(type) ? throw new ArgumentNullException(nameof(type)) : type;

    public Parameter(string type, string name)
        : this(type)
        => Name = name;

    public string Type { get; }

    public string Name { get; }
}
