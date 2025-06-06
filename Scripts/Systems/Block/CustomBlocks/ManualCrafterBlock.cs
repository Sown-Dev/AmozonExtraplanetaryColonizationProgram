﻿using UI.BlockUI;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ManualCrafterBlock : RecipeBlock{
        public BlockUIButton craftButton;
        public Sprite icon;
        public int TicksPerClick = 10;
        public bool passiveCraft = false;

        protected override void Awake(){
            base.Awake();
            craftButton = new BlockUIButton(AddProgress, icon, 32);
        }

        public override void Tick(){
            if (passiveCraft){
                base.Tick();
            }
            else{
                if (TerrainManager.Instance.totalTicksElapsed % 2 == 0){
                    actuatedThisTick = false;
                    if (properties.actuatable)
                        mat.SetColor("_AddColor", new Color(0, 0, 0, 0));

                    currentState.SetNextSprite();
                    UpdateSprite();
                }

                progressBar.maxProgress = recipeSelector.currentRecipe.craftTime;
            }
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