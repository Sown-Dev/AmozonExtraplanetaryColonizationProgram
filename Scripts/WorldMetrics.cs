using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemoryPack;

[Serializable]
[MemoryPackable]
public partial class WorldMetrics
{
    public int blocksBroken;
    public int blocksPlaced;
    public int terrainDestroyed;
    public int itemsPickedUp;
    public int moneyEarned;
    public float distanceTraveled;
    public List<string> itemsDiscovered;

    public WorldMetrics()
    {
        blocksBroken = 0;
        blocksPlaced = 0;
        terrainDestroyed = 0;
        itemsPickedUp = 0;
        moneyEarned = 0;
        distanceTraveled = 0f;
        itemsDiscovered = new List<string>();
    }

    public static WorldMetrics operator +(WorldMetrics a, WorldMetrics b)
    {
        var result = new WorldMetrics();

        var fields = typeof(WorldMetrics).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (IsAddableType(field.FieldType))
            {
                var valueA = field.GetValue(a);
                var valueB = field.GetValue(b);
                if (field.FieldType == typeof(int))
                {
                    field.SetValue(result, (int)valueA + (int)valueB);
                }
                else if (field.FieldType == typeof(float))
                {
                    field.SetValue(result, (float)valueA + (float)valueB);
                }
                else if (field.FieldType == typeof(double))
                {
                    field.SetValue(result, (double)valueA + (double)valueB);
                }
                else if (field.FieldType == typeof(decimal))
                {
                    field.SetValue(result, (decimal)valueA + (decimal)valueB);
                }
            }
        }

        result.itemsDiscovered = a.itemsDiscovered
            .Concat(b.itemsDiscovered)
            .Distinct()
            .ToList();

        return result;
    }

    private static bool IsAddableType(Type type)
    {
        return type == typeof(int)
            || type == typeof(float)
            || type == typeof(double)
            || type == typeof(decimal);
    }

    public void AddDiscoveredItem(string itemId)
    {
        if (!itemsDiscovered.Contains(itemId))
        {
            itemsDiscovered.Add(itemId);
        }
    }
    
    public override string ToString()
    {
        return $"BlocksBroken: {blocksBroken}, BlocksPlaced: {blocksPlaced}";
    }
}
