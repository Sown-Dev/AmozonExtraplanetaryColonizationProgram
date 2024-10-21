using System.Collections.Generic;
using Systems.Block.CustomBlocks;
using Systems.Items;
using UnityEngine;

namespace Systems.Block{
    public class RecipeBlock : ProgressMachineContainerBlock{
        public GameObject[] CraftingFX;
    


        public RecipeSelector recipeSelector;
        
        public Container input;
        public ContainerProperties inputProperties;
       
        public override bool Insert(ref ItemStack s, bool simulate = false){
            return input.Insert(ref s, simulate);
        }
        
        
        public override bool BlockDestroy(bool dropItems){
            lootTable.AddRange(input.GetItems());
            return base.BlockDestroy();
        }

        protected override void Awake(){
            base.Awake();
            
            input = new Container(inputProperties);
    
            
            recipeSelector.Priority = 10;

            input.Priority = 20;
            progressBar.Priority = 21;
            output.Priority = 22;
            
            

        

            recipeSelector.onRecipeChanged += SetRecipe;
        }

        private void Start(){
            recipeSelector.SelectRecipe(recipeSelector.recipes[0]);
        }

        bool isCrafting = false;
        
        // we dont use base tick bc we want to only reset progress if we cant craft, but keep progress if we can progress, mainly for burner items
        public override void Tick(){
            if (CanCraft()){
                if (CanProgress()){
                    Progress();
                }
            }
            else{
                progressBar.progress = 0;
            }

            foreach (GameObject fx in CraftingFX){
                fx.SetActive(isCrafting);
            }
        }

        public override bool CanProgress(){
            return base.CanProgress() && CanCraft();
            
        }

        public override void CompleteCycle(){
            base.CompleteCycle();
            Craft();
        }

        public virtual void Craft(){
            if (CanCraft()){
                foreach (ItemStack ingredient in recipeSelector.currentRecipe.ingredients){
                    input.RemoveItem(ingredient);
                }

                foreach (ItemStack result in recipeSelector.currentRecipe.results){
                    var itemStack = result.Clone();
                    output.Insert(ref itemStack);
                }
            }
        }

        public virtual bool CanCraft(){
            return input.Contains(recipeSelector.currentRecipe?.ingredients);
        }

        public virtual void SetRecipe(){
            foreach (Slot s in input.containerList){
                if(s.ItemStack != null)
                    s.ExtractToContainer(output);
            }

            if (recipeSelector.currentRecipe != null){
                List<Slot> newSlots = new List<Slot>();
                for (int i = 0; i < recipeSelector.currentRecipe.ingredients.Count; i++){
                    newSlots.Add(new Slot());
                    newSlots[i].filter = recipeSelector.currentRecipe.ingredients[i].item;
                }

                inputProperties.size = recipeSelector.currentRecipe.ingredients.Count;
                inputProperties.gridWidth = inputProperties.size;
                input = new Container(inputProperties, newSlots);
                input.blackList = true;
                input.Priority = 20;
            }
            //move all input items to output
        
            UpdateUI?.Invoke();
        }
    }
}