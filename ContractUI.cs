using System.Collections;
using System.Collections.Generic;
using Systems.Items;
using Systems.Round;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContractUI : MonoBehaviour
{
    public Contract myContract;

    [Header("References")]
    public LayoutGroup SellList;
    public Image logo;
    public TMP_Text time;
    public TMP_Text quota;
    public TMP_Text  reward;



    [Header("Prefabs")] public Sprite[] logos;
    public GameObject sellUIPrefab;

    public void Init(Contract c){
        myContract = c;
        
        time.text = $"{(int)(myContract.TimeGiven / 60f)}:{(int)(myContract.TimeGiven % 60):00}";
        quota.text = "$"+ myContract.requiredQuota;
        reward.text = "$" + myContract.reward;
        
        
        foreach (Transform child in SellList.transform){
           Destroy(child.gameObject);
        }
        foreach (Item i in myContract.sellList){
            ItemStackUI ui =Instantiate(sellUIPrefab, SellList.transform).GetComponent<ItemStackUI>();
            ui.Init(i);
        }
        
        logo.sprite = logos[(int)myContract.sponsor];

    }
}
