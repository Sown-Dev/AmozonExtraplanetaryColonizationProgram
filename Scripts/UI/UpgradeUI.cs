using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeUI:MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    
    public Upgrade myUpgrade;
    public Image icon;
    
    public void Init(UpgradeSO u){
        Init(u.u);
    }

    public void Init(Upgrade u){
        myUpgrade = u;
        icon.sprite = u.icon;
    }
    
    
    public void OnPointerEnter(PointerEventData eventData){
            
        TooltipManager.Instance.Show(myUpgrade, transform.position + new Vector3(0,-32), this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData){
        TooltipManager.Instance.Hide();
    }
}