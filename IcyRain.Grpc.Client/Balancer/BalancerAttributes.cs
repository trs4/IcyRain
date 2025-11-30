using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Collection of load balancing metadata attributes
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class BalancerAttributes : IDictionary<string, object?>, IReadOnlyDictionary<string, object?>
{
    /// <summary>Gets a read-only collection of metadata attributes</summary>
    public static readonly BalancerAttributes Empty = new BalancerAttributes([], readOnly: true);

    private readonly Dictionary<string, object?> _attributes;
    private readonly bool _readOnly;

    /// <summary>Initializes a new instance of the <see cref="BalancerAttributes"/> class</summary>
    public BalancerAttributes() : this([], readOnly: false) { }

    private BalancerAttributes(Dictionary<string, object?> attributes, bool readOnly)
    {
        _attributes = attributes;
        _readOnly = readOnly;
    }

    object? IDictionary<string, object?>.this[string key]
    {
        get => _attributes[key];
        set
        {
            ValidateReadOnly();
            _attributes[key] = value;
        }
    }

    ICollection<string> IDictionary<string, object?>.Keys => _attributes.Keys;

    ICollection<object?> IDictionary<string, object?>.Values => _attributes.Values;

    int ICollection<KeyValuePair<string, object?>>.Count => _attributes.Count;

    bool ICollection<KeyValuePair<string, object?>>.IsReadOnly => _readOnly || ((ICollection<KeyValuePair<string, object?>>)_attributes).IsReadOnly;

    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => _attributes.Keys;

    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => _attributes.Values;

    int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => _attributes.Count;

    object? IReadOnlyDictionary<string, object?>.this[string key] => _attributes[key];

    void IDictionary<string, object?>.Add(string key, object? value)
    {
        ValidateReadOnly();
        _attributes.Add(key, value);
    }
    void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item)
    {
        ValidateReadOnly();
        ((ICollection<KeyValuePair<string, object?>>)_attributes).Add(item);
    }
    void ICollection<KeyValuePair<string, object?>>.Clear()
    {
        ValidateReadOnly();
        _attributes.Clear();
    }
    bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item) => _attributes.Contains(item);

    bool IDictionary<string, object?>.ContainsKey(string key) => _attributes.ContainsKey(key);

    void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<string, object?>>)_attributes).CopyTo(array, arrayIndex);

    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => _attributes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_attributes).GetEnumerator();

    bool IDictionary<string, object?>.Remove(string key)
    {
        ValidateReadOnly();
        return _attributes.Remove(key);
    }

    bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
    {
        ValidateReadOnly();
        return ((ICollection<KeyValuePair<string, object?>>)_attributes).Remove(item);
    }

    bool IDictionary<string, object?>.TryGetValue(string key, out object? value) => _attributes.TryGetValue(key, out value);

    bool IReadOnlyDictionary<string, object?>.ContainsKey(string key) => _attributes.ContainsKey(key);

    bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object? value) => _attributes.TryGetValue(key, out value);

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="key">The key of the <see cref="BalancerAttributesKey{TValue}"/> to get.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key, if the key is found
    /// and the value type matches the specified type. Otherwise, contains the default value for the type of
    /// the <c>value</c> parameter.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="BalancerAttributes"/> contains an element with the specified key and value type; otherwise <c>false</c>.
    /// </returns>
    public bool TryGetValue<TValue>(BalancerAttributesKey<TValue> key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_attributes.TryGetValue(key.Key, out object? o) && o is TValue v)
        {
            value = v;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Sets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="key">The key of the value to set.</param>
    /// <param name="value">The value.</param>
    public void Set<TValue>(BalancerAttributesKey<TValue> key, TValue value)
    {
        ValidateReadOnly();
        _attributes[key.Key] = value;
    }

    /// <summary>
    /// Removes the value associated with the specified key.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="key">The key of the value to set.</param>
    /// <returns>
    /// <c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
    /// This method also returns <c>false</c> if key was not found.
    /// </returns>
    public bool Remove<TValue>(BalancerAttributesKey<TValue> key)
    {
        ValidateReadOnly();
        return _attributes.Remove(key.Key);
    }

    private void ValidateReadOnly()
    {
        if (_readOnly)
            throw new NotSupportedException("Collection is read-only");
    }

    internal static bool DeepEquals(BalancerAttributes? x, BalancerAttributes? y)
    {
        var xValues = x?._attributes;
        var yValues = y?._attributes;

        if (ReferenceEquals(xValues, yValues))
            return true;

        if (xValues is null || yValues is null)
            return false;

        if (xValues.Count != yValues.Count)
            return false;

        foreach (var kvp in xValues)
        {
            if (!yValues.TryGetValue(kvp.Key, out var value))
                return false;

            if (!Equals(kvp.Value, value))
                return false;
        }

        return true;
    }

}
