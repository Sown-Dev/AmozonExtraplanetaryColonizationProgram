using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class Layer<ObjectType> where ObjectType : class
{
    private readonly Dictionary<long, ObjectType> layer;

    public Layer(int capacity = 0)
    {
        layer = capacity > 0 ? new Dictionary<long, ObjectType>(capacity) : new Dictionary<long, ObjectType>();
    }

    private static long Encode(Vector2Int pos)
    {
        return ((long)pos.x << 32) | (uint)pos.y;
    }

    private static Vector2Int Decode(long key)
    {
        return new Vector2Int((int)(key >> 32), (int)key);
    }

    [CanBeNull]
    public ObjectType Get(Vector2Int position)
    {
        layer.TryGetValue(Encode(position), out var obj);
        return obj;
    }

    public void Set(Vector2Int position, ObjectType obj)
    {
        layer[Encode(position)] = obj;
    }

    public void Remove(ObjectType obj)
    {
        long removeKey = -1;
        foreach (var pair in layer)
        {
            if (ReferenceEquals(pair.Value, obj))
            {
                removeKey = pair.Key;
                break;
            }
        }

        if (removeKey != -1)
            layer.Remove(removeKey);
    }

    public void Remove(Vector2Int position)
    {
        layer.Remove(Encode(position));
    }

    public IEnumerable<KeyValuePair<Vector2Int, ObjectType>> Pairs()
    {
        foreach (var pair in layer)
            yield return new KeyValuePair<Vector2Int, ObjectType>(Decode(pair.Key), pair.Value);
    }

    public IEnumerable<ObjectType> Values => layer.Values;

    // Legacy compatibility
    public Dictionary<Vector2Int, ObjectType> GetDictionary()
    {
        var dict = new Dictionary<Vector2Int, ObjectType>(layer.Count);
        foreach (var pair in layer)
            dict.Add(Decode(pair.Key), pair.Value);
        return dict;
    }
}