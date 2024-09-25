using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.BlockUI{
    public class RecipeSelectUI : MonoBehaviour{
        
        public RecipeSelector recipeSelector;
        
        
        public RecipeUI currentRecipeUI;

        public RecipeSelectWindow windowPrefab;

        private RecipeSelectWindow myWindow;


        private void Start(){
            recipeSelector.onRecipeChanged += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI(){
           currentRecipeUI.SetRecipe(recipeSelector.currentRecipe); 
        }

        public void Open(){
            if(myWindow != null){
                myWindow.Close();
            }
            
            RecipeSelectWindow win =WindowManager.Instance.CreateWindow(windowPrefab).GetComponent<RecipeSelectWindow>();
            win.RecipeSelector = recipeSelector;
            myWindow = win;
        }
    }
}