using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Systems.Block.CustomBlocks;
using Systems.Items;
using UnityEngine;

namespace Systems.Block{
    public class RecipeBlock : ProgressMachineContainerBlock{
        public GameObject[] CraftingFX;
    
    
        
        //need a new solution for keeping track of recipes but still saving them
        public RecipeSelector recipeSelector;

        
        //public RecipeBlockData data => (RecipeBlockData)base.data;
        public ContainerProperties inputProperties;
            public Container input;

       

        public override bool Insert(ref ItemStack s, bool simulate = false){
            return 
                input.Insert(ref s, simulate);
        }
        
        
        public override bool BlockDestroy(bool dropItems){
            data.lootTable.AddRange( 
                input.GetItems());
            return base.BlockDestroy();
        }

        protected override void Awake(){
            base.Awake();
            

        
           

        }

        public override void Init(Orientation orientation){
            base.Init(orientation); 
            input = new Container(inputProperties); 
            input.Priority = 20; 
            progressBar.Priority = 21; 
            output.Priority = 22;
    

        }

        private void Start(){
            recipeSelector.onRecipeChanged += SetRecipe;
            recipeSelector.Priority = 10;
            recipeSelector.SelectRecipe(0);
        }

        bool isCrafting = false;
        
        // we dont use base tick bc we want to only reset progress if we cant craft, but keep progress if we can progress, mainly for burner items
        public override void Tick(){
            isCrafting = false;

            if (CanCraft()){
                if (CanProgress()){
                    isCrafting = true;
                    Progress();
                }
            }
            else{
                progressBar.progress = 0;
            }

            foreach (GameObject fx in CraftingFX){
                fx.SetActive(isCrafting);
            }
            progressBar.maxProgress = recipeSelector.currentRecipe.craftTime;

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
        //TODO: somehow make recipeselector 
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
                    
                    newSlots[i].filter  = new Filter( recipeSelector.currentRecipe.ingredients[i].item);
                }

                inputProperties.size = recipeSelector.currentRecipe.ingredients.Count;
                inputProperties.gridWidth = inputProperties.size;
                input = new Container(inputProperties, newSlots);
                input.blackList = true;
                input.Priority = 20;
            }
            //move all data.input items to output
        
            UpdateUI?.Invoke();
        }
        
        public override BlockData Save(){
            BlockData d = base.Save();
            d.data.SetInt( "selectedRecipe", recipeSelector.currentRecipeIndex);
            d.data.SetString( "input", JsonConvert.SerializeObject( input, GameManager.JSONsettings ) );
return d;
        }

        public override void Load(BlockData d){
            base.Load(d);
            //recipeSelector.SelectRecipe( JsonConvert.DeserializeObject<Recipe>( d.data.GetString( "selectedRecipe" ), GameManager.JSONsettings ) );
            input = JsonConvert.DeserializeObject<Container>( d.data.GetString( "input" ), GameManager.JSONsettings );
            recipeSelector.SelectRecipe( d.data.GetInt( "selectedRecipe" ) );
            SetRecipe();
        }
    }
    [Serializable]
    public class RecipeBlockData : ProgressMachineContainerBlockData{
        
        public Recipe selectedRecipe;
        public Container input;

    }
}