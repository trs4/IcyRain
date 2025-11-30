using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IcyRain.Grpc.Client.Internal;

internal sealed class ThreadSafeLookup<TKey, TValue>
    where TKey : notnull
{
    // Avoid allocating ConcurrentDictionary until the threshold is reached.
    // Looking up a key in an array is as fast as a dictionary for small collections and uses much less memory.
    internal const int Threshold = 10;

    private KeyValuePair<TKey, TValue>[] _array = [];
    private ConcurrentDictionary<TKey, TValue>? _dictionary;

    /// <summary>
    /// Gets the value for the key if it exists. If the key does not exist then the value is created using the valueFactory.
    /// The value is created outside of a lock and there is no guarentee which value will be stored or returned.
    /// </summary>
    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
        if (_dictionary is not null)
            return _dictionary.GetOrAdd(key, valueFactory);

        if (TryGetValue(_array, key, out var value))
            return value;

        var newValue = valueFactory(key);

        lock (this)
        {
            if (_dictionary is not null)
                _dictionary.TryAdd(key, newValue);
            else
            {
                // Double check inside lock if the key was added to the array by another thread.
                if (TryGetValue(_array, key, out value))
                    return value;

                if (_array.Length > Threshold - 1)
                {
                    // Array length exceeds threshold so switch to dictionary
                    var newDict = new ConcurrentDictionary<TKey, TValue>();

                    foreach (var kvp in _array)
                        newDict.TryAdd(kvp.Key, kvp.Value);
                    
                    newDict.TryAdd(key, newValue);

                    _dictionary = newDict;
                    _array = [];
                }
                else
                {
                    // Add new value by creating a new array with old plus new value.
                    var newArray = new KeyValuePair<TKey, TValue>[_array.Length + 1];
                    Array.Copy(_array, newArray, _array.Length);
                    newArray[^1] = new KeyValuePair<TKey, TValue>(key, newValue);

                    _array = newArray;
                }
            }
        }

        return newValue;
    }

    private static bool TryGetValue(KeyValuePair<TKey, TValue>[] array, TKey key, out TValue value)
    {
        foreach (var kvp in array)
        {
            if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
            {
                value = kvp.Value;
                return true;
            }
        }

        value = default!;
        return false;
    }

}
