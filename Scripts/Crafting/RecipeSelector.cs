using System;
using System.Collections.Generic;
using Systems.BlockUI;

[Serializable]
public class RecipeSelector: IBlockUI{
    public List<Recipe> recipes;
    
    public Recipe currentRecipe;
    
    public Action onRecipeChanged;
    
    public void SelectRecipe(Recipe recipe){
        if (recipes.Contains(recipe)){
            currentRecipe = recipe;
            onRecipeChanged?.Invoke();
        }
    }

    public int Priority{ get; set; }
    public bool Hidden{ get; set; }
}