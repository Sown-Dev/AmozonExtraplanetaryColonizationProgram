using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Systems.Items;
using UnityEngine;

[Serializable]
public class Stats
{
    public int blocksBroken = 0;
    public int itemsPickedUp = 0;
    public int moneyEarned = 0;
    public HashSet<KeyValuePair<Item, bool>> ItemsDiscovered; // Use HashSet of KeyValuePair

    public Stats()
    {
        blocksBroken = 0;
        itemsPickedUp = 0;
        moneyEarned = 0;
        ItemsDiscovered = new HashSet<KeyValuePair<Item, bool>>(); // Initialize HashSet
    }

    // Overloading the + operator using reflection
    public static Stats operator +(Stats a, Stats b)
    {
        Stats result = new Stats();

        // Get all fields of the Stats class
        FieldInfo[] fields = typeof(Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // If the field is addable (e.g., int, float), add the values from a and b
            if (IsAddableType(field.FieldType))
            {
                object valueA = field.GetValue(a);
                object valueB = field.GetValue(b);

                // Perform addition without dynamic
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

        // Merging ItemsDiscovered (non-addable field)
        foreach (var item in a.ItemsDiscovered)
        {
            result.AddOrUpdateItem(item.Key, item.Value);
        }

        foreach (var item in b.ItemsDiscovered)
        {
            result.AddOrUpdateItem(item.Key, item.Value);
        }

        return result;
    }

    // Check if the field is of an addable type (e.g., int, float, double)
    private static bool IsAddableType(Type type)
    {
        return type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(decimal);
    }

    // Helper method to add or update an item in ItemsDiscovered
    public void AddOrUpdateItem(Item item, bool discovered)
    {
        var existing = ItemsDiscovered.FirstOrDefault(pair => pair.Key.Equals(item));
        if (!existing.Equals(default(KeyValuePair<Item, bool>))) // Check if found
        {
            // Update discovered status
            var updatedPair = new KeyValuePair<Item, bool>(item, discovered || existing.Value);
            ItemsDiscovered.Remove(existing); // Remove old pair
            ItemsDiscovered.Add(updatedPair); // Add updated pair
        }
        else
        {
            ItemsDiscovered.Add(new KeyValuePair<Item, bool>(item, discovered)); // Add new pair
        }
    }
}
