using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeIcon: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    public Image icon;
    public WorldUpgrade myUpgrade;

    public void Set(WorldUpgrade u){
        myUpgrade = u;
        icon.sprite = u.icon;
    }
    
    public void OnPointerEnter(PointerEventData eventData){
        
    }

    public void OnPointerExit(PointerEventData eventData){
        
    }
}