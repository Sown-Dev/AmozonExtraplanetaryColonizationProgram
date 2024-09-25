using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BlockUI{
    public class RecipeSelectWindow: UIWindow{
        public RecipeSelector RecipeSelector;
    
        public RecipeUI RecipeUIPrefab;
        
        public GridLayoutGroup list;

        private void Start(){
            BlockUIManager.Instance.currentBlockUI.OnClose += Close;
            Populate();
        }

        private void OnDestroy(){
            BlockUIManager.Instance.currentBlockUI.OnClose -= Close;
        }

        //not that we'll ever need to outside of start
        public void Populate(){
            list.constraintCount = Mathf.Min(RecipeSelector.recipes.Count, 6);
            
            foreach(Transform child in list.transform){
                Destroy(child.gameObject);
            }
            foreach (var recipe in RecipeSelector.recipes){
                RecipeUI ui = Instantiate(RecipeUIPrefab, list.transform);
                ui.SetRecipe(recipe);
                ui.button.onClick.AddListener(() => {
                    Close();
                    RecipeToolTip.Instance.Close();
                    RecipeSelector.SelectRecipe(recipe);
                });
            }
        }

        private void Update(){
            if (Input.GetKeyDown(KeyCode.Escape)){
                Close();
            }
        }
    }
}