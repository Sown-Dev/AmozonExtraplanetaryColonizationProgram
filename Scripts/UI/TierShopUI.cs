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
    public Transform ElectricalList;

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
                ShopOfferUI offerUI = Instantiate(shopItemPrefab, LogisticsList).GetComponent<ShopOfferUI>();
                offerUI.Init(offer);
            }
        }
        
        

        if (tier.production.Length == 0){
            Destroy(ProductionList.gameObject);
        }
        else{
            foreach (ShopOffer offer in tier.production){
                ShopOfferUI offerUI = Instantiate(shopItemPrefab, ProductionList).GetComponent<ShopOfferUI>();
                offerUI.Init(offer);
            }
        }
        
        if (tier.electrical.Length == 0){
            Destroy(ElectricalList.gameObject);
        }
        else{
            foreach (ShopOffer offer in tier.electrical){
                ShopOfferUI offerUI = Instantiate(shopItemPrefab, ElectricalList).GetComponent<ShopOfferUI>();
                offerUI.Init(offer);
            }
        }

        if (tier.refinement.Length == 0){
            Destroy(RefineList.gameObject);
        }
        else{
            foreach (ShopOffer offer in tier.refinement){
                ShopOfferUI offerUI = Instantiate(shopItemPrefab, RefineList).GetComponent<ShopOfferUI>();
                offerUI.Init(offer);
            }
        }

        if (tier.misc.Length == 0){
            Destroy(MiscList.gameObject);
        }
        else{
            foreach (ShopOffer offer in tier.misc){
                ShopOfferUI offerUI = Instantiate(shopItemPrefab, MiscList).GetComponent<ShopOfferUI>();
                offerUI.Init(offer);
            }
        }

        if (tier.upgradeOffer != null){
            var button = Instantiate(upgradeButtonPrefab, UpgradeList).GetComponent<UpgradeButton>();
            button.Init(tier.upgradeOffer);
        }
    }
}