using System;
using IcyRain.Grpc.Client.Configuration;

namespace IcyRain.Grpc.Client.Internal.Configuration;

internal sealed class ConfigProperty<TValue, TInner>
    where TValue : IConfigValue
{
    private TValue? _value;
    private readonly Func<TInner?, TValue?> _valueFactory;
    private readonly string _key;

    public ConfigProperty(Func<TInner?, TValue?> valueFactory, string key)
    {
        _value = default;
        _valueFactory = valueFactory;
        _key = key;
    }

    public TValue? GetValue(ConfigObject inner)
    {
        if (_value is null)
        {
            // Multiple threads can get a property at the same time. We want this to be safe.
            // Because a value could be lazily initialized, lock to ensure multiple threads
            // don't try to update the underlying dictionary at the same time.
            lock (this)
            {
                // Double-check locking.
                if (_value is null)
                {
                    var innerValue = inner.GetValue<TInner>(_key);
                    _value = _valueFactory(innerValue);

                    if (_value is not null && innerValue is null)
                        SetValue(inner, _value); // Set newly created value
                }
            }
        }

        return _value;
    }

    public void SetValue(ConfigObject inner, TValue? value)
    {
        _value = value;
        inner.SetValue(_key, _value?.Inner);
    }

}
