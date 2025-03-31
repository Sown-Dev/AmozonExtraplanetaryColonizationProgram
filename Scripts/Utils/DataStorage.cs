using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class DataStorage
{
    [JsonProperty]
    private Dictionary<string, JToken> data = new Dictionary<string, JToken>();

    public void SetFloat(string key, float value) => data[key] = JToken.FromObject(value);
    public void SetInt(string key, int value) => data[key] = JToken.FromObject(value);
    public void SetString(string key, string value) => data[key] = JToken.FromObject(value);
    public void SetBool(string key, bool value) => data[key] = JToken.FromObject(value);

    public bool HasKey(string key) => data.ContainsKey(key);

    public float GetFloat(string key, float defaultValue = 0f) => data.TryGetValue(key, out var value) && value.Type == JTokenType.Float ? value.Value<float>() : defaultValue;
    public int GetInt(string key, int defaultValue = 0) => data.TryGetValue(key, out var value) && value.Type == JTokenType.Integer ? value.Value<int>() : defaultValue;
    public string GetString(string key, string defaultValue = "") => data.TryGetValue(key, out var value) && value.Type == JTokenType.String ? value.Value<string>() : defaultValue;
    public bool GetBool(string key, bool defaultValue = false) => data.TryGetValue(key, out var value) && value.Type == JTokenType.Boolean ? value.Value<bool>() : defaultValue;

    public string Serialize() => JsonConvert.SerializeObject(this);

    public static DataStorage Deserialize(string json) => JsonConvert.DeserializeObject<DataStorage>(json);
}