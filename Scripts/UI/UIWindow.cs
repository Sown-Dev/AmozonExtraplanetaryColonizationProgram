using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
        private Vector2 offset;

        [HideInInspector]public WindowManager manager;

        private CanvasGroup cg;
        
        public Action OnClose;
        
        public virtual void Awake(){
            cg = GetComponent<CanvasGroup>();
            gameObject.AddComponent<Shadow>().effectDistance = new Vector2(1, -1);
            
        }
        
        public void OnBeginDrag(PointerEventData eventData){
            BringToFront();
            // Calculate the offset between the mouse position and the element's anchor
            offset = eventData.position - (Vector2)transform.position;
            OnDragStart();
            RecipeToolTip.Instance.Close(); // prevents wierd behaviour when dragging recipe window
        }

        public void OnDrag(PointerEventData eventData){

            transform.position = Vector3Int.RoundToInt(eventData.position - offset);

            OnDragging();
        }

        public void OnEndDrag(PointerEventData eventData){
            OnDragEnd();
        }

        // These virtual methods allow derived classes to implement specific behaviors during the drag operation
        protected virtual void OnDragStart(){ }
        protected virtual void OnDragging(){ }
        protected virtual void OnDragEnd(){ }
        
        public void BringToFront(){
            transform.SetAsLastSibling();
        }

        public virtual void Close(){
            OnClose?.Invoke();
            Destroy(gameObject);
        }


        private bool isOpen;

        public void Hide(){
            isOpen = false;
            Refresh();
        }

        public void Show(){
            BringToFront();
            isOpen = true;
            Refresh();
        }
        
        public void Toggle(){
            if (isOpen) Hide();
            else Show();
        }

        public virtual void Refresh(){
            cg.interactable = isOpen;
            cg.blocksRaycasts = isOpen;
            cg.alpha = isOpen ? 1 : 0;
            if (isOpen)
                Canvas.ForceUpdateCanvases();
            //LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }
    }
}