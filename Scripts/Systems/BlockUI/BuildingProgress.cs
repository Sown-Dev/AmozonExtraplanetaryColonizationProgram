// BuildingProgress.cs
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Systems.BlockUI;
using Systems.Items;
using UnityEngine;

[Serializable]
public class BuildingProgress : IBlockUI
{
    public ItemStack[] resourcesNeeded;
    public int[] progress;
    
    // Callback with container reference for potential partial completion handling
    [JsonIgnore]public Action OnBuildComplete;

    public int Priority { get; set; }
    public bool Hidden { get; set; }

    public bool Build(Container c)
    {
        bool allComplete = true;
        
        for (int i = 0; i < resourcesNeeded.Length; i++)
        {
            if (progress[i] >= resourcesNeeded[i].amount) 
                continue;

            Item targetItem = resourcesNeeded[i].item;
            int remainingNeed = resourcesNeeded[i].amount - progress[i];

            foreach (Slot slot in c.containerList)
            {
                if (slot.ItemStack == null || slot.ItemStack.item != targetItem)
                    continue;

                int available = slot.ItemStack.amount;
                int taken = Mathf.Min(available, remainingNeed);

                progress[i] += taken;
                slot.ItemStack.amount -= taken;
                remainingNeed -= taken;

                if (slot.ItemStack.amount <= 0)
                    slot.ItemStack = null;

                slot.OnChange?.Invoke();
                slot.dirty = true;

                if (remainingNeed <= 0) break;
            }

            if (progress[i] < resourcesNeeded[i].amount)
                allComplete = false;
        }

        // Invoke progress callback with container reference
        
        if (allComplete)
        {
            OnBuildComplete?.Invoke();
            ClearCallbacks(); // Prevent memory leaks
        }

        return allComplete;
    }

    // Memory leak prevention
    public void ClearCallbacks()
    {
        OnBuildComplete = null;
    }

    public BuildingProgress(ItemStack[] requirements)
    {
        resourcesNeeded = requirements;
        progress = new int[resourcesNeeded.Length];
    }
}