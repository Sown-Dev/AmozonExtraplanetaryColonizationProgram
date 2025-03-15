using System;
using System.Collections.Generic;
using Systems.Items;
using Systems.Round;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI{
    public class ShopUI : UIWindow{
        public static ShopUI Instance;
        public LayoutGroup layoutGroup;

        public GameObject shopItemPrefab;
        public GameObject shopTierPrefab;
        
        
        public CanvasGroup loanCG;
        public TMP_Text debtText;
        public Button loanButton;

        public override void Awake(){
            base.Awake();
            Instance = this;
            DragAllow = false;
            loanButton.onClick.AddListener(delegate{ RoundManager.Instance.TakeLoan(); Refresh(); });
        }

        void Start(){
            Hide();
            CGRefresh();
        }

        public override void Refresh(){
            base.Refresh();
            foreach (Transform child in layoutGroup.transform){
                Destroy(child.gameObject);
            }

            foreach (ShopTier t in RoundManager.Instance.shopTiers){
                GameObject tier = Instantiate(shopTierPrefab, layoutGroup.transform);
                tier.GetComponent<TierShopUI>().Init(t);
            }

            
            loanCG.alpha = RoundManager.Instance.loansUnlocked? 1 : 0;
            loanCG.interactable = RoundManager.Instance.loansUnlocked;
            loanCG.blocksRaycasts = RoundManager.Instance.loansUnlocked;
            
            loanButton.interactable = RoundManager.Instance.loansTaken <= 3;
        }
    }

   

    

   
}