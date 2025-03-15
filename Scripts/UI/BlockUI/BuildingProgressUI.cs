using System;
using System.Collections.Generic;
using UnityEngine;
using Systems.Items;
using TMPro;
using UnityEngine.UI;

public class BuildingProgressUI : MonoBehaviour
{
    public BuildingProgress buildingProgress;
    
    public Button buildButton;
    
    public Transform resourceList;
    public GameObject resourcePrefab;
    
    
    public void Init(BuildingProgress bp)
    {
        buildingProgress = bp;
        
        
        
        for (int i = 0; i < buildingProgress.resourcesNeeded.Length; i++)
        {
            ItemStack stack = buildingProgress.resourcesNeeded[i];
            if (stack == null) continue;
            
            GameObject go = GameObject.Instantiate(resourcePrefab, resourceList);
            go.GetComponentInChildren<ItemStackUI>().Init(stack);
            go.GetComponentInChildren<TMP_Text>().text = buildingProgress.progress[i] + "/" + stack.amount;
        }
        
        buildButton.transform.SetAsLastSibling();
    }

    private void Update(){
        //set text
        for (int i = 0; i < buildingProgress.resourcesNeeded.Length; i++)
        {
            ItemStack stack = buildingProgress.resourcesNeeded[i];
            if (stack == null) continue;
            
            resourceList.GetChild(i).GetComponentInChildren<TMP_Text>().text = buildingProgress.progress[i] + "/" + stack.amount;
        }
    }

    public void Build(){

        if (buildingProgress.Build(Player.Instance.Inventory)){
            
            
        }
    }
    
    
    


    
}