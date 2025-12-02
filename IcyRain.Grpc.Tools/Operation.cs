using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IcyRain.Grpc.Tools;

internal sealed class Operation
{
    private readonly List<Parameter> _parameters;

    public Operation(OperationType type, string name, List<Parameter> parameters, string responseType)
    {
        Type = type;
        Name = string.IsNullOrEmpty(name) ? throw new ArgumentNullException(nameof(name)) : name;
        _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        ResponseType = responseType;
    }

    public OperationType Type { get; }

    public string Name { get; }

    public ReadOnlyCollection<Parameter> Parameters => _parameters.AsReadOnly();

    public string ResponseType { get; }
}
