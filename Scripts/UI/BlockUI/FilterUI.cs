using System;
using Systems.Items;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class FilterUI: MonoBehaviour{
    public Filter myFilter;
    public ItemStackUI itemStackUI;
    public Button filterButton;
    public void Init(Filter f){
        myFilter = f;
        itemStackUI.Init(f.filter);
        filterButton.onClick.AddListener(() => {
            WindowManager.Instance.CreateFilterSelectWindow(myFilter);
            
        });
    }

    public void Update(){
        if(itemStackUI.myItemStack?.item != myFilter.filter){
            itemStackUI.Init(myFilter.filter);
        }
    }
}