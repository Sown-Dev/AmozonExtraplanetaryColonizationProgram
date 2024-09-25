using System.Collections;
using System.Collections.Generic;
using Systems.Items;
using UnityEngine;

public class ItemDropCollector : MonoBehaviour{
    [SerializeReference] public Player p;

    private void OnTriggerEnter2D(Collider2D other){
        ItemDrop itemDrop = other.gameObject.GetComponent<ItemDrop>();

        if (itemDrop == null){
            return;
        }

        var itemStack = itemDrop.Collect();
        if (p.Inventory.Insert(ref itemStack)){
            Destroy(itemDrop.gameObject);
        }
    }
}