using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeToolTip : MonoBehaviour{
    
    public static RecipeToolTip Instance;
    
    [HideInInspector]public Recipe currentRecipe;

    [SerializeField] private CanvasGroup cg;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform tooltipRT;

    [SerializeField] private Transform inputList;
    [SerializeField] private Transform outputList;

    private void Awake(){
        Instance = this;
        Close();
    }
    

    public void Update(){
        if (currentRecipe != null){
            cg.alpha = 1;
            nameText.text = currentRecipe.name;
            timeText.text = "Base Time:" + (currentRecipe.craftTime / 20f).ToString("F") + "s";
            icon.sprite = currentRecipe.icon;
        }
        else{
            cg.alpha = 0;
        }
    }

    public void Open(Recipe recipe, Vector2 pos){
        currentRecipe = recipe;
        
        foreach (Transform child in inputList){
            Destroy(child.gameObject);
        }
        foreach (Transform child in outputList){
            Destroy(child.gameObject);
        }
        foreach (ItemStack input in recipe.ingredients){
            Utils.Instance.CreateItemstackUI(inputList, input);
        }
        foreach (ItemStack output in recipe.results){
            Utils.Instance.CreateItemstackUI(outputList, output);
        }
        
        transform.position = pos;
        tooltipRT.anchoredPosition = new Vector2( (pos.x > Screen.width-200)? -1 *(120 ): 120, 0);
        Canvas.ForceUpdateCanvases();
    }

    public void Close(){
        currentRecipe = null;
    }
}