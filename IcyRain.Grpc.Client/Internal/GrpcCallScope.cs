using System;
using System.Collections;
using System.Collections.Generic;
using Grpc.Core;

namespace Grpc.Net.Client.Internal;

internal sealed class GrpcCallScope : IReadOnlyList<KeyValuePair<string, object>>
{
    private const string GrpcMethodTypeKey = "GrpcMethodType";
    private const string GrpcUriKey = "GrpcUri";

    private readonly MethodType _methodType;
    private readonly Uri _uri;
    private string? _cachedToString;

    public GrpcCallScope(MethodType methodType, Uri uri)
    {
        _methodType = methodType;
        _uri = uri;
    }

    public KeyValuePair<string, object> this[int index] => index switch
    {
        0 => new KeyValuePair<string, object>(GrpcMethodTypeKey, _methodType),
        1 => new KeyValuePair<string, object>(GrpcUriKey, _uri),
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public int Count => 2;

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        for (var i = 0; i < Count; ++i)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
        => _cachedToString ??= FormattableString.Invariant($"{GrpcMethodTypeKey}:{_methodType} {GrpcUriKey}:{_uri}");
}
