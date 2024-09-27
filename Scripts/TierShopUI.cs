using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class TierShopUI : MonoBehaviour{
    public Transform LogisticsList;
    public Transform ProductionList;
    public Transform StorageList;

    public GameObject shopItemPrefab;

    public void Init(ShopTier tier){
        foreach (Transform child in LogisticsList){
            if (!(child.GetComponent<LayoutElement>()?.ignoreLayout == true))
                Destroy(child.gameObject);
        }

        foreach (Transform child in ProductionList){
            if (!(child.GetComponent<LayoutElement>()?.ignoreLayout == true))
                Destroy(child.gameObject);
        }

        foreach (Transform child in StorageList){
            if (!(child.GetComponent<LayoutElement>()?.ignoreLayout == true))
                Destroy(child.gameObject);
        }

        foreach (ShopOffer offer in tier.logistics){
            ShopButton button = Instantiate(shopItemPrefab, LogisticsList).GetComponent<ShopButton>();
            button.Init(offer);
        }

        foreach (ShopOffer offer in tier.refinement){
            ShopButton button = Instantiate(shopItemPrefab, ProductionList).GetComponent<ShopButton>();
            button.Init(offer);
        }

        foreach (ShopOffer offer in tier.storage){
            ShopButton button = Instantiate(shopItemPrefab, StorageList).GetComponent<ShopButton>();
            button.Init(offer);
        }
    }
}