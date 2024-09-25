using System;
using Systems.Block;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class BlockInfoUI:MonoBehaviour{
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private Image icon;

        [SerializeField] private LayoutElement layoutElement;

        
        [HideInInspector] public Block block;

        public void Update(){
            
            if (block != null){
                cg.alpha = 1;
                cg.blocksRaycasts = true;
                cg.interactable = true;
                nameText.text = block.properties.name;
                descText.text = block.GetDescription();
                icon.sprite = block.sr.sprite;
                icon.SetNativeSize();
                layoutElement.ignoreLayout = false;
            }
            else{
                cg.alpha = 0;
                cg.blocksRaycasts = false;
                cg.interactable = false;
                layoutElement.ignoreLayout = true;
            }
        }
    }
}