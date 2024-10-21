using System;
using Systems.Items;
using TMPro;
using UI.BlockUI;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler{
    public Slot Slot;
    [SerializeField] public Draggable draggable;


    [SerializeField] private Image tint;
    [SerializeField] private Image background;
    [SerializeField] private Image slotImg;
    [SerializeField] private Image foreground;
    [SerializeField] private TMP_Text amount;


    public bool AllowDrag = true;

    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite filtered;
    
   

    void Start(){
        Refresh();
        slotImg.color = new Color(0.9f, 0.9f, 0.9f, 1f);
    }

    public void Refresh(){
        if (Slot == null) return;

        if (Slot.ItemStack != null && Slot.ItemStack?.item == null){
            Debug.Log("ERROR: ITEM CANNOT BE NULL, there is bugged a not null itemstack somewhere");
        }

        draggable.myImg.sprite = Slot.ItemStack?.item != null && Slot.ItemStack?.item.icon
            ? Slot.ItemStack?.item.icon
            : Utils.Instance.blankIcon;
        amount.text = Slot.ItemStack?.amount > 1 ? Slot.ItemStack?.amount.ToString() : "";

        switch(Slot?.ItemStack?.item.category){
            case ItemCategory.Building:
                tint.color = new Color(0.9f, 0.6f, 0.1f, 0.2f);
                break;
            case ItemCategory.Material:
                tint.color = new Color(0.1f, 0.8f, 0.3f, 0.2f);
                break;
            default:
                tint.color = Color.clear;
                break;

        }
    }

    private void Update(){
       // if (Slot.dirty){
            Refresh();
            slotImg.sprite = Slot.filter ? filtered : normal;
            foreground.sprite = Slot.Selected ? selected : Utils.Instance.blankIcon;
            background.sprite = Slot.filter ? Slot.filter.icon : Utils.Instance.blankIcon;
            Slot.dirty = false;
        //}
    }

    public void OnDrop(PointerEventData eventData){
        if (eventData.pointerDrag.GetComponent<Draggable>() != null){
            Debug.Log("Dropped On Slot");

            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();


            if (d.mySlot.Slot.ItemStack?.item == Slot.ItemStack?.item){
                Slot.Insert(d.mySlot.Slot);
            }
            else{
                Slot.Swap(d.mySlot.Slot);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData){
        if (eventData.button == PointerEventData.InputButton.Middle){
            Player.Instance.SelectSlot(Slot);
        }

        if (eventData.button == PointerEventData.InputButton.Left){
            if (Input.GetKey(KeyCode.LeftShift)){
                if (BlockUIManager.Instance.currentBlockUI?.block is IContainerBlock container){
                    Debug.Log("Shift Click Container");
                    if (Player.Instance.Inventory.Contains(Slot)){
                        CU.Transfer(Slot, container);
                    }
                    else{
                        CU.Transfer(Slot,Player.Instance.Inventory );
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        slotImg.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData){
        slotImg.color = new Color(0.85f, 0.85f, 0.85f, 1f);
    }
}