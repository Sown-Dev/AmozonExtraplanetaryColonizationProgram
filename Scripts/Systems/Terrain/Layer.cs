using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class Layer<T> where T : class
{
    private const int CHUNK_SIZE = 16;

    private class Chunk
    {
        public readonly T[,] data = new T[CHUNK_SIZE, CHUNK_SIZE];
    }

    private readonly Dictionary<Vector2Int, Chunk> chunks = new();

    private static Vector2Int ChunkPos(Vector2Int pos)
    {
        int cx = Mathf.FloorToInt((float)pos.x / CHUNK_SIZE);
        int cy = Mathf.FloorToInt((float)pos.y / CHUNK_SIZE);
        return new Vector2Int(cx, cy);
    }

    private static Vector2Int LocalPos(Vector2Int pos)
    {
        int lx = pos.x - Mathf.FloorToInt((float)pos.x / CHUNK_SIZE) * CHUNK_SIZE;
        int ly = pos.y - Mathf.FloorToInt((float)pos.y / CHUNK_SIZE) * CHUNK_SIZE;
        if (lx < 0) lx += CHUNK_SIZE;
        if (ly < 0) ly += CHUNK_SIZE;
        return new Vector2Int(lx, ly);
    }

    public Layer() {}

    [CanBeNull]
    public T Get(Vector2Int position)
    {
        var cp = ChunkPos(position);
        if (chunks.TryGetValue(cp, out var chunk))
        {
            var lp = LocalPos(position);
            return chunk.data[lp.x, lp.y];
        }
        return null;
    }

    public void Set(Vector2Int position, T obj)
    {
        var cp = ChunkPos(position);
        if (!chunks.TryGetValue(cp, out var chunk))
        {
            chunk = new Chunk();
            chunks[cp] = chunk;
        }
        var lp = LocalPos(position);
        chunk.data[lp.x, lp.y] = obj;
    }

    public void Remove(T obj)
    {
        foreach (var entry in GetAll())
        {
            if (EqualityComparer<T>.Default.Equals(entry.Value, obj))
            {
                Remove(entry.Key);
                return;
            }
        }
    }

    public void Remove(Vector2Int position)
    {
        var cp = ChunkPos(position);
        if (chunks.TryGetValue(cp, out var chunk))
        {
            var lp = LocalPos(position);
            chunk.data[lp.x, lp.y] = null;
        }
    }

    public IEnumerable<KeyValuePair<Vector2Int, T>> GetAll()
    {
        foreach (var kv in chunks)
        {
            int baseX = kv.Key.x * CHUNK_SIZE;
            int baseY = kv.Key.y * CHUNK_SIZE;
            var data = kv.Value.data;
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    var obj = data[x, y];
                    if (obj != null)
                        yield return new KeyValuePair<Vector2Int, T>(
                            new Vector2Int(baseX + x, baseY + y), obj);
                }
            }
        }
    }
}
