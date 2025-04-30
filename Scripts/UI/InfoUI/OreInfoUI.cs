using System;
using Systems.Items;
using Systems.Terrain;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI{
    public class OreInfoUI: MonoBehaviour{
        
        public static OreInfoUI Instance;

        [SerializeField] private LayoutElement layoutElement;
        
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Image icon;
        [SerializeField] private ItemStackUI itemStackUI;

        private Ore myOre;

        private void Awake(){
            Instance=this;
            
            myOre = null;
        }   

        public void Update(){
            
            if (Selected && myOre != null && !EventSystem.current.IsPointerOverGameObject()){
                cg.alpha = 1;
                cg.blocksRaycasts = true;
                cg.interactable = true;
                nameText.text = myOre.myProperties?.name;
                amountText.text = "Remaining: " + myOre.amount;
                icon.sprite = myOre.myProperties.tile.m_DefaultSprite;
                layoutElement.ignoreLayout = false;
                itemStackUI.Init( myOre.myProperties.oreItem);
            }
            else{
                cg.alpha = 0;
                cg.blocksRaycasts = false;
                cg.interactable = false;
                layoutElement.ignoreLayout = true;
            }
        }
        bool Selected = false;
        public void Select(Ore o){
            myOre= o;
            Selected = true;
        }
        public void Deselect(){
            myOre = null;
            Selected = false;
        }
    }
}