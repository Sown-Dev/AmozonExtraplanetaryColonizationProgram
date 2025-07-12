using System.Collections.Generic;
using UnityEngine;

public class NoiseMap
{
    private readonly int size;
    public readonly Dictionary<string, float[][]> Maps;

    public NoiseMap(int halfSize, IEnumerable<string> keys)
    {
        size = halfSize;
        Maps = new Dictionary<string, float[][]>();
        foreach (string key in keys)
        {
            float[][] arr = new float[halfSize * 2][];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new float[halfSize * 2];
            }
            Maps[key] = arr;
        }
    }

    public float Get(string key, int x, int y)
    {
        return Maps[key][x + size][y + size];
    }

    public void Set(string key, int x, int y, float value)
    {
        Maps[key][x + size][y + size] = value;
    }
}
