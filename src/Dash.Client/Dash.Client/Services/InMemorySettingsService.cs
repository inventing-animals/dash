using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Ink.Platform.Settings;

namespace Dash.Client.Core;

public sealed class InMemorySettingsService : ISettingsService
{
    private readonly Dictionary<string, JsonElement> _store = [];

    public T? Get<T>(string key, JsonTypeInfo<T> typeInfo)
    {
        if (_store.TryGetValue(key, out var element))
        {
            return element.Deserialize(typeInfo);
        }

        return default;
    }

    public void Set<T>(string key, T value, JsonTypeInfo<T> typeInfo)
    {
        _store[key] = JsonSerializer.SerializeToElement(value, typeInfo);
    }

    public void Remove(string key)
    {
        _store.Remove(key);
    }

    public bool Contains(string key)
    {
        return _store.ContainsKey(key);
    }
}
