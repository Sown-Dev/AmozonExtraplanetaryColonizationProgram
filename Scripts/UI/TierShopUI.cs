using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TierShopUI : MonoBehaviour{
    public TMP_Text shopText;
    
    public Transform LogisticsList;
    public Transform ProductionList;
    public Transform MiscList;
    public Transform RefineList;

    public Transform UpgradeList;


    public GameObject shopItemPrefab;
    public GameObject upgradeButtonPrefab;

    public void Init(ShopTier tier){
        
        shopText.text = "Tier " + tier.tier +" Shop";
        foreach (Transform child in LogisticsList){
            if (!(child.GetComponent<LayoutElement>()?.ignoreLayout == true))
                Destroy(child.gameObject);
        }

        foreach (Transform child in ProductionList){
            if (!(child.GetComponent<LayoutElement>()?.ignoreLayout == true))
                Destroy(child.gameObject);
        }

        foreach (Transform child in MiscList){
            if (!(child.GetComponent<LayoutElement>()?.ignoreLayout == true))
                Destroy(child.gameObject);
        }

        if (tier.logistics.Length == 0){
            Destroy(LogisticsList.gameObject);
        }
        else{
            foreach (ShopOffer offer in tier.logistics){
                ShopButton button = Instantiate(shopItemPrefab, LogisticsList).GetComponent<ShopButton>();
                button.Init(offer);
            }
        }

        if (tier.production.Length == 0){
            Destroy(ProductionList.gameObject);
        }
        else{
            foreach (ShopOffer offer in tier.production){
                ShopButton button = Instantiate(shopItemPrefab, ProductionList).GetComponent<ShopButton>();
                button.Init(offer);
            }
        }

        if (tier.refinement.Length == 0){
            Destroy(RefineList.gameObject);
        }
        else{
            foreach (ShopOffer offer in tier.refinement){
                ShopButton button = Instantiate(shopItemPrefab, RefineList).GetComponent<ShopButton>();
                button.Init(offer);
            }
        }

        if (tier.misc.Length == 0){
            Destroy(MiscList.gameObject);
        }
        else{
            foreach (ShopOffer offer in tier.misc){
                ShopButton button = Instantiate(shopItemPrefab, MiscList).GetComponent<ShopButton>();
                button.Init(offer);
            }
        }

        if (tier.upgradeOffer != null){
            var button = Instantiate(upgradeButtonPrefab, UpgradeList).GetComponent<UpgradeButton>();
            button.Init(tier.upgradeOffer);
        }
    }
}