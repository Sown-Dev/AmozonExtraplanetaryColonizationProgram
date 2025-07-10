using System;
using System.Collections.Generic;
using MemoryPack;

[MemoryPackable]
public partial class DataStorage
{
    public Dictionary<string, byte[]> data = new();

    public void Set<T>(string key, T value)
    {
        data[key] = MemoryPackSerializer.Serialize(value);
    }

    public T Get<T>(string key, T defaultValue = default)
    {
        if (data.TryGetValue(key, out var bytes))
        {
            return MemoryPackSerializer.Deserialize<T>(bytes);
        }
        return defaultValue;
    }

    public void SetFloat(string key, float value) => Set(key, value);
    public float GetFloat(string key, float defaultValue = 0f) => Get(key, defaultValue);
    public void SetInt(string key, int value) => Set(key, value);
    public int GetInt(string key, int defaultValue = 0) => Get(key, defaultValue);
    public void SetString(string key, string value) => Set(key, value);
    public string GetString(string key, string defaultValue = "") => Get(key, defaultValue);
    public void SetBool(string key, bool value) => Set(key, value);
    public bool GetBool(string key, bool defaultValue = false) => Get(key, defaultValue);

    public bool HasKey(string key) => data.ContainsKey(key);
}