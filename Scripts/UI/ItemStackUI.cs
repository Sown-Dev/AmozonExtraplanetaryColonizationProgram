using Systems.Items;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//Meant to be an icon representing an itemstack, but not meant to be interactable. For an interactable UI representation of a slot use SlotUI;
public class ItemStackUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    public ItemStack myItemStack;

    [SerializeField] public Image icon;
    [SerializeField] public TMP_Text countText;

    public void Init(ItemStack itemStack){
        myItemStack = itemStack;
        icon.sprite = itemStack.item.icon;
        if (itemStack.amount > 1)
            countText.text = itemStack.amount.ToString();
        else
            countText.text = "";
    }

    public void Init(Item item){
        myItemStack = new ItemStack(item, 1);
        icon.sprite = item.icon;
        countText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData){
        ItemInfoUI.Instance.Select(myItemStack);
    }

    public void OnPointerExit(PointerEventData eventData){
        ItemInfoUI.Instance.Deselect();
    }
}