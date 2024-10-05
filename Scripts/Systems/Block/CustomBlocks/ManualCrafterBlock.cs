using UI.BlockUI;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ManualCrafterBlock: RecipeBlock{
        public BlockUIButton craftButton;
        public Sprite icon;
        public int TicksPerClick = 10;
        protected override void Awake(){
            base.Awake();
            craftButton = new BlockUIButton(AddProgress, icon, 32);
        }

        public override void Tick(){
            //base.Tick(); do nothing, we only craft on click
            
            progressBar.maxProgress = recipeSelector.currentRecipe.craftTime;

        }

        public void AddProgress(){
            Debug.Log("AddProgress");
            if (CanCraft()){
                progressBar.progress += TicksPerClick;
                if (progressBar.progress >= recipeSelector.currentRecipe.craftTime){
                    progressBar.progress = 0;
                    Craft();
                }
            }
        }
    
    }
}