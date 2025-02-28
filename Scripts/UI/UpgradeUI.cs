using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeUI:MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    
    public UpgradeSO myUpgrade;
    public Image icon;
    
    public void Init(UpgradeSO u){
        myUpgrade = u;
        icon.sprite = u.u.icon;
    }
    public void OnPointerEnter(PointerEventData eventData){
            
        TooltipManager.Instance.Show(myUpgrade.u, transform.position + new Vector3(0,-32), this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData){
        TooltipManager.Instance.Hide();
    }
}