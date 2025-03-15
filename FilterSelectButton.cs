using System.Collections;
using System.Collections.Generic;
using Systems.Items;
using UnityEngine;

public class FilterSelectButton : MonoBehaviour
{
    public ItemStackUI itemStackUI;
    public Item myItem;

    public void Init(Item i){
        myItem = i;
        itemStackUI.Init(i);
    }
    
    
}
