using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using UnityEngine;

public static class BsonPlayerPrefsUtility
{
    public static void Save<T>(string key, T obj, JsonSerializerSettings settings = null)
    {
        byte[] data = Serialize(obj, settings);
        string base64 = Convert.ToBase64String(data);
        PlayerPrefs.SetString(key, base64);
    }

    public static T Load<T>(string key, JsonSerializerSettings settings = null)
    {
        if (!PlayerPrefs.HasKey(key))
            return default;
        string base64 = PlayerPrefs.GetString(key);
        byte[] data = Convert.FromBase64String(base64);
        return Deserialize<T>(data, settings);
    }

    public static byte[] Serialize<T>(T obj, JsonSerializerSettings settings = null)
    {
        var serializer = JsonSerializer.Create(settings);
        using (var ms = new MemoryStream())
        using (var writer = new BsonDataWriter(ms))
        {
            serializer.Serialize(writer, obj);
            writer.Flush();
            return ms.ToArray();
        }
    }

    public static T Deserialize<T>(byte[] data, JsonSerializerSettings settings = null)
    {
        var serializer = JsonSerializer.Create(settings);
        using (var ms = new MemoryStream(data))
        using (var reader = new BsonDataReader(ms))
        {
            return serializer.Deserialize<T>(reader);
        }
    }
}
