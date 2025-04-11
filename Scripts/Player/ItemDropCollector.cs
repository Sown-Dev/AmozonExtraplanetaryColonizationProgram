using System.Collections;
using System.Collections.Generic;
using Systems.Items;
using UnityEngine;

public class ItemDropCollector : MonoBehaviour{
    [SerializeReference] public Player p;

    private void OnTriggerEnter2D(Collider2D other){
        ItemDrop itemDrop = other.gameObject.GetComponent<ItemDrop>();

        if (itemDrop == null || itemDrop.myItem == null || !itemDrop.enablePickup){
            return;
        }

        var itemStack = itemDrop.Collect();
        if (p.Insert(ref itemStack) || itemStack==null || itemStack?.amount == 0){
            Destroy(itemDrop.gameObject);
        }
    }
}