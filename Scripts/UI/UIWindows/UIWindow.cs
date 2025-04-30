using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // The offset in canvas local coordinates between the pointer and the window's anchoredPosition.
        private Vector2 offset;
        // Cache the window's RectTransform.
        protected RectTransform windowRectTransform;
        // Cache the parent Canvas's RectTransform.
        private RectTransform canvasRectTransform;
        

        [HideInInspector]
        public WindowManager manager;
        public Shadow shadow;
        
        private CanvasGroup cg;
        protected bool DragAllow = true;

        public Action OnClose;

        public virtual void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            windowRectTransform = transform as RectTransform;
            // Assumes the window is a child of a Canvas.
            canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
           
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            BringToFront();
            // Convert the pointer position into canvas local coordinates.
            Vector2 localPointerPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPointerPos
            );
            // Compute offset between the pointer and the current anchored position.
            offset = localPointerPos - windowRectTransform.anchoredPosition;
            OnDragStart();
            RecipeToolTip.Instance?.Close();  // Prevents unexpected behavior when dragging tooltips.
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!DragAllow) return;

            // Convert the current screen pointer position into canvas local coordinates.
            Vector2 localPointerPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPointerPos
            ))
            {
                // Compute the new anchored position in canvas space.
                Vector2 newAnchoredPos = localPointerPos - offset;
                
                // Retrieve custom margin from the GameManager.
                // Convention: margin order is left, top, right, bottom.
                Vector4 margin = GameManager.Instance.windowMargin;
                
                // Calculate the safe bounds relative to the canvas' local coordinate space.
                // The canvas RectTransform's rect is centered (xMin and yMin are negative).
                float leftBound = canvasRectTransform.rect.xMin + margin.x + (windowRectTransform.rect.width * windowRectTransform.pivot.x);
                float rightBound = canvasRectTransform.rect.xMax - margin.z - (windowRectTransform.rect.width * (1f - windowRectTransform.pivot.x));
                float bottomBound = canvasRectTransform.rect.yMin + margin.w + (windowRectTransform.rect.height * windowRectTransform.pivot.y);
                float topBound = canvasRectTransform.rect.yMax - margin.y - (windowRectTransform.rect.height * (1f - windowRectTransform.pivot.y));

                // Clamp the new position so the window does not leave the safe area.
                newAnchoredPos.x = Mathf.Clamp(newAnchoredPos.x, leftBound, rightBound);
                newAnchoredPos.y = Mathf.Clamp(newAnchoredPos.y, bottomBound, topBound);

                // Apply the calculated position.
                int r = 1;  //round to nearest r
                newAnchoredPos.x = Mathf.Round(newAnchoredPos.x/r)*r;
                newAnchoredPos.y = Mathf.Round(newAnchoredPos.y/r)*r;
                
                windowRectTransform.anchoredPosition = newAnchoredPos;
            }
            OnDragging();
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!DragAllow) return;
            OnDragEnd();
        }

        // Virtual methods for additional per-drag behavior.
        protected virtual void OnDragStart() { }
        protected virtual void OnDragging() { }
        protected virtual void OnDragEnd() { }

        public void BringToFront()
        {
            transform.SetAsLastSibling();
        }

        public virtual void Close()
        {
            OnClose?.Invoke();
            Destroy(gameObject);
        }

        private bool isOpen;

        public void Hide()
        {
            isOpen = false;
            CGRefresh();
        }

        public void Show()
        {
            BringToFront();
            isOpen = true;
            CGRefresh();
            Refresh();
        }

        public void Toggle()
        {
            if (isOpen)
                Hide();
            else
                Show();
        }

        public void CGRefresh()
        {
            cg.interactable = isOpen;
            cg.blocksRaycasts = isOpen;
            cg.alpha = isOpen ? 1 : 0;
            if (isOpen)
                Canvas.ForceUpdateCanvases();
        }

        public virtual void Refresh()
        {
            // Optional: Rebuild layout if needed.
            // LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }

      
    }
}
