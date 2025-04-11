using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIObject: UIWindow, IPointerDownHandler, IPointerUpHandler{
    
    
    
    public override void OnBeginDrag(PointerEventData eventData){
        base.OnBeginDrag(eventData);
        windowRectTransform.anchoredPosition = windowRectTransform.anchoredPosition + new Vector2(0, 6);
        shadow.effectDistance = new Vector2(0, -7);
    }

    public override void OnDrag(PointerEventData eventData){
        base.OnDrag(eventData);
        windowRectTransform.anchoredPosition = windowRectTransform.anchoredPosition + new Vector2(0, 6);
        shadow.effectDistance = new Vector2(0, -7);
    }

    public override void OnEndDrag(PointerEventData eventData){
        base.OnEndDrag(eventData);
        shadow.effectDistance = new Vector2(0, -1);

    }
    
    public virtual void OnPointerUp(PointerEventData eventData){
        windowRectTransform.anchoredPosition = windowRectTransform.anchoredPosition + new Vector2(0, -6);
        shadow.effectDistance = new Vector2(0, -1);
    }
    public virtual void OnPointerDown(PointerEventData eventData){
        windowRectTransform.anchoredPosition = windowRectTransform.anchoredPosition + new Vector2(0, 6);
        shadow.effectDistance = new Vector2(0, -7);
    }
    
    
}