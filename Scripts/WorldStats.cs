using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

[Serializable]
[JsonObject]
public class WorldStats
{
    public int blocksBroken;
    public int itemsPickedUp;
    public int moneyEarned;
    public List<string> itemsDiscovered;

    public WorldStats()
    {
        blocksBroken = 0;
        itemsPickedUp = 0;
        moneyEarned = 0;
        itemsDiscovered = new List<string>();
    }

    public static WorldStats operator +(WorldStats a, WorldStats b)
    {
        var result = new WorldStats();

        var fields = typeof(WorldStats).GetFields(BindingFlags.Public | BindingFlags.Instance);
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
}
