using Systems.Items;
using Systems.Round;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RoundInfoUI:MonoBehaviour{
    //Controls quotabar, upgrade list, and sellable and money info


    [Header("references")]
    public GameObject contractInfo;
    public GameObject time;
    public TMP_Text contractText;
    public TMP_Text moneyText;
    public TMP_Text quotaText;
    public TMP_Text timeText;
    public SlicedFilledImage quotaFill;
    public LayoutGroup contractList; //not used for instantiating, just to refresh
    public LayoutGroup sellList;
    public Button shopButton;

    [Header("prefabs")]

    public GameObject upgradeUIPrefab;
    public GameObject sellUIPrefab;

    public void Update(){
        moneyText.text = "$" + RoundManager.Instance.money;

        if (RoundManager.Instance.currentContract == null){
            //Debug.Log(RoundManager.Instance.cooldownTimer);
            if(RoundManager.Instance.roundTime > -1){
 
                timeText.text =$"{(int)(RoundManager.Instance.roundTime / 60)}:{(int)(RoundManager.Instance.roundTime % 60):00}";
            }
            else{

            }

        }
        else{
            timeText.text =$"{(int)(RoundManager.Instance.roundTime / 60)}:{(int)(RoundManager.Instance.roundTime % 60):00}";

        }

    }

    public void Refresh(){
        //Update the quota bar 
        if (RoundManager.Instance.currentContract == null){
            contractInfo.SetActive(false); 
            time.SetActive(false);
            if( RoundManager.Instance.roundTime> -1){
                time.SetActive(true);
            }
            contractText.text = "Awaiting Contract";
        }
        else{
            time.SetActive(true);
            contractInfo.SetActive(true);
            contractText.text = "Buying:";
            quotaFill.fillAmount = (float)RoundManager.Instance.currentContract.quota / RoundManager.Instance.currentContract.requiredQuota;
            quotaText.text = RoundManager.Instance.currentContract.quota + "/" + RoundManager.Instance.currentContract.requiredQuota;

            //Update the money text

            //Update the upgrade list


            foreach (Transform child in sellList.transform){
                GameObject.Destroy(child.gameObject);

            }

            foreach (Item i in RoundManager.Instance.currentContract.sellList){
                ItemStackUI ui = GameObject.Instantiate(sellUIPrefab, sellList.transform).GetComponent<ItemStackUI>();
                ui.Init(i);
            }
        }
        
        
        shopButton.interactable = RoundManager.Instance.roundNum > -1; //first round is 0

        /*
        foreach (Transform child in upgradeList.transform){
            //GameObject.Destroy(child.gameObject);
        }
        foreach (WorldUpgrade u in TerrainManager.Instance.upgrades){
            //UpgradeUI ui = GameObject.Instantiate(upgradeUIPrefab, upgradeList.transform);
           // ui.upgrade = u;
        }*/
        
        //ShopUI.Refresh(); bad bad bad
        
        //refresh canvas 
        //dumbass fucking engine
        gameObject.SetActive(false);
        gameObject.SetActive(true);

    }

    public ShopUI ShopUI;
    
    public void ToggleShop(){
        TutorialManager.Instance.StartTutorial("shop");
        ShopUI.Toggle();
        ShopUI.Refresh();

    }
}