using System;
using Systems.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class ItemInfoUI: MonoBehaviour{
        
        public static ItemInfoUI Instance;

        [SerializeField] private LayoutElement layoutElement;
        
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private TMP_Text sellPriceText;
        //[SerializeField] private TMP_Text tierText;
        [SerializeField] private Image icon;

        private ItemStack item =null;

        private void Awake(){
            Instance=this;
            
            item = null;
        }   

        public void Update(){
            
            if (!Selected){
                cg.alpha = 0;
                cg.blocksRaycasts = false;
                cg.interactable = false;
                layoutElement.ignoreLayout = true;
            }
            else{
                cg.alpha = 1;
                cg.blocksRaycasts = true;
                cg.interactable = true;
                nameText.text = item.item?.name;
                descText.text =item.item?.ToString();
                icon.sprite = item.item?.icon;
                sellPriceText.text = "$"+item.item?.value;
                
                //tierText.text = item.item?.tier.ToString();
                
                layoutElement.ignoreLayout = false;
            }
        }
        bool Selected = false;
        public void Select(ItemStack it){
            item = it;
            Selected = true;
        }
        public void Deselect(){
            item = null;
            Selected = false;
        }
    }
}