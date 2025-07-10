using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
public class DataStorage
{
    [JsonInclude]
    private Dictionary<string, JsonElement> data = new Dictionary<string, JsonElement>();

    public void SetFloat(string key, float value) => data[key] = JsonSerializer.SerializeToElement(value);
    public void SetInt(string key, int value) => data[key] = JsonSerializer.SerializeToElement(value);
    public void SetString(string key, string value) => data[key] = JsonSerializer.SerializeToElement(value);
    public void SetBool(string key, bool value) => data[key] = JsonSerializer.SerializeToElement(value);

    public bool HasKey(string key) => data.ContainsKey(key);

    public float GetFloat(string key, float defaultValue = 0f) =>
        data.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetSingle() : defaultValue;
    public int GetInt(string key, int defaultValue = 0) =>
        data.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetInt32() : defaultValue;
    public string GetString(string key, string defaultValue = "") =>
        data.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : defaultValue;
    public bool GetBool(string key, bool defaultValue = false) =>
        data.TryGetValue(key, out var value) &&
        (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
            ? value.GetBoolean()
            : defaultValue;

    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        IncludeFields = true
    };

    public string Serialize() => JsonSerializer.Serialize(this, Options);

    public static DataStorage Deserialize(string json) => JsonSerializer.Deserialize<DataStorage>(json, Options);
}