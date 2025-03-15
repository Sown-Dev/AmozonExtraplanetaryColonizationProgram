using UnityEngine;
using UnityEngine.UI;

public class UIFollowRect : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect; // The UI object to follow
    [SerializeField] private bool matchSize = true; // Option to match width and height
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (targetRect == null) return;

        // Convert world position of the target to the local position of the follower's parent
        Vector3 worldPos = targetRect.position;
        rectTransform.position = worldPos;
        rectTransform.rotation = targetRect.rotation;

        // Sync size without affecting anchors
        if (matchSize)  
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetRect.rect.width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetRect.rect.height);
        }

        // Optional: Force layout update if necessary
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    public void SetTarget(RectTransform target)
    {
        targetRect = target;
    }

    public void SetMatchSize(bool match)
    {
        matchSize = match;
    }
}