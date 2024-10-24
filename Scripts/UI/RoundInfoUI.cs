using Systems.Items;
using Systems.Round;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RoundInfoUI:MonoBehaviour{
    //Controls quotabar, upgrade list, and sellable and money info
    
    public TMP_Text moneyText;
    public TMP_Text quotaText;
    public TMP_Text timeText;
    public SlicedFilledImage quotaFill;
    public LayoutGroup upgradeList;
    public LayoutGroup sellList;

    
    public GameObject upgradeUIPrefab;

    public GameObject sellUIPrefab;

    public void Update(){
        timeText.text =$"{(int)(RoundManager.Instance.roundTime / 60)}:{(int)(RoundManager.Instance.roundTime % 60):00}";
    }

    public void Refresh(){
        //Update the quota bar 
        quotaFill.fillAmount = (float)RoundManager.Instance.currentContract.quota/ RoundManager.Instance.currentContract.requiredQuota;
        quotaText.text = RoundManager.Instance.currentContract.quota + "/" + RoundManager.Instance.currentContract.requiredQuota;
        
        //Update the money text
        moneyText.text = "$" + RoundManager.Instance.money;
        
        //Update the upgrade list
        

        foreach (Transform child in sellList.transform){
            GameObject.Destroy(child.gameObject);
            
        }

        foreach (Item i in RoundManager.Instance.currentContract.sellList){
            ItemStackUI ui = GameObject.Instantiate(sellUIPrefab, sellList.transform).GetComponent<ItemStackUI>();
            ui.Init(i);
        }
        
        
        /*
        foreach (Transform child in upgradeList.transform){
            //GameObject.Destroy(child.gameObject);
        }
        foreach (WorldUpgrade u in TerrainManager.Instance.upgrades){
            //UpgradeUI ui = GameObject.Instantiate(upgradeUIPrefab, upgradeList.transform);
           // ui.upgrade = u;
        }*/
        
        //ShopUI.Refresh(); bad bad bad
    }

    public ShopUI ShopUI;
    
    public void ToggleShop(){
        ShopUI.Toggle();
        ShopUI.Refresh();

    }
}