using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler,
    IEndDragHandler{
    public bool isDragging;
    [SerializeField] public SlotUI mySlot;
    [SerializeField] public Image myImg;
    [SerializeField] private RectTransform rt;
    private Material mat;
    private Vector2 awakePos;


    private void Awake(){
        
        awakePos = rt.anchoredPosition;
        targetPos = awakePos;
        mat = Instantiate(myImg.material);
        myImg.material = mat;
        
    }


    private Vector3 targetPos;

    private void Update(){
        

        if (!isDragging){
            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, awakePos, Time.unscaledDeltaTime * 24);
        }
        else{
            transform.position = Vector3Int.RoundToInt(Vector3.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * 16));
        }

    }

    public void OnDrag(PointerEventData eventData){
        if (!mySlot.AllowDrag|| !isDragging ||mySlot.Slot.ItemStack==null) return;


        targetPos = eventData.position;
        myImg.raycastTarget = false;
    }

    public void OnBeginDrag(PointerEventData eventData){
        //temp
        if(mySlot.Slot.ItemStack==null) return;
        
        if (!mySlot.AllowDrag) return;

        transform.SetParent(PlayerUI.Instance.OnTop, false);
        transform.position = eventData.position;

        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData){
        if (!mySlot.AllowDrag) return;
        
        //if not dragging over anything, use item
        if (!IsPointerOverUIObject()){
            mySlot.Slot.ItemStack?.item.Use(Player.Instance.myCursor.currentPos, Player.Instance, mySlot.Slot);
        }
        
        myImg.raycastTarget = true;
        transform.SetParent(mySlot.transform, true);
        transform.localScale = Vector3.one;
        isDragging = false;
    }


    private bool mouseOver;

    public void OnPointerEnter(PointerEventData eventData){
        //Debug.Log("Mouse over");
        mouseOver = true;
        if(mySlot.Slot.ItemStack!=null)
            TooltipManager.Instance.ShowItem(mySlot.Slot.ItemStack.item, transform.position , gameObject);
            //ItemInfoUI.Instance.Select(mySlot.Slot.ItemStack);

    }

    public void OnPointerExit(PointerEventData eventData){
        //Debug.Log("Mouse exit");
        mouseOver = false;
        TooltipManager.Instance.Hide();
        //ItemInfoUI.Instance.Deselect();
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject != this.gameObject)
            {
                return true; // Mouse is over a UI element other than this gameObject
            }
        }

        return false; // Mouse is not over any UI element, or only over this gameObject
    }
}