using System;
using System.Collections.Generic;
using Crafting;
using Systems.Items;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "ScriptableObjects/Recipe")]
[Serializable]
public class Recipe: ScriptableObject{
    
    public Sprite icon;
    
    public List<ItemStack> ingredients;
    public List<ItemWrapper> newIngredients;
    public List<ItemStack> results;
    
    [Header("Crafting time in ticks")]
    public int craftTime;

    private void OnValidate(){
        if (icon == null){
            icon = results[0].item.icon;
        }
        //newIngredients = new List<ItemWrapper>();
        foreach (ItemStack itemStack in ingredients){
            //newIngredients.Add(new ItemWrapper(itemStack.item));
        }
    }
}