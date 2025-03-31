using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BouncingScrollingBackground : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Use random direction for movement if true; otherwise, use movementDirection.")]
    public bool randomDirection = true;
    
    [Tooltip("Movement direction used if randomDirection is false (will be normalized).")]
    public Vector2 movementDirection = new Vector2(1f, 1f);
    
    [Tooltip("Speed at which the background scrolls.")]
    public float scrollSpeed = 100f;
    
    [Header("Margin Settings")]
    [Tooltip("Margin that ensures the image edge is never visible.")]
    public float edgeMargin = 50f;
    
    [Header("Mouse Influence")]
    [Tooltip("Amount that the mouse cursor influences movement.")]
    public float mouseInfluence = 5f;
    
    [Header("Boundary Settings")]
    [Tooltip("Optional boundary RectTransform. If not set, the parent's RectTransform or screen size is used.")]
    public RectTransform boundary;

    private RectTransform rectTransform;
    private Vector2 imageSize;
    private Vector2 velocity;
    private float safeRangeX;
    private float safeRangeY;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        imageSize = rectTransform.rect.size;

        // Determine boundary: if not set, try the parent's RectTransform, else fallback to screen size.
        if (boundary == null && transform.parent != null)
        {
            boundary = transform.parent.GetComponent<RectTransform>();
        }
        Vector2 boundarySize = boundary ? boundary.rect.size : new Vector2(Screen.width, Screen.height);

        // Calculate safe range (half range in which the image can move without showing its edge)
        safeRangeX = (imageSize.x - boundarySize.x) / 2f - edgeMargin;
        safeRangeY = (imageSize.y - boundarySize.y) / 2f - edgeMargin;
        safeRangeX = Mathf.Max(0, safeRangeX);
        safeRangeY = Mathf.Max(0, safeRangeY);

        // Determine movement direction.
        Vector2 direction = Vector2.zero;
        if (randomDirection)
        {
            float angle = Random.Range(0f, 2f * Mathf.PI);
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        else
        {
            if (movementDirection == Vector2.zero)
                direction = Vector2.right; // fallback
            else
                direction = movementDirection.normalized;
        }
        velocity = direction * scrollSpeed;

        // Set initial position on the opposite side of the movement direction (within safe boundaries).
        float startX = (velocity.x > 0) ? -safeRangeX : safeRangeX;
        float startY = (velocity.y > 0) ? -safeRangeY : safeRangeY;
        rectTransform.anchoredPosition = new Vector2(startX, startY);
    }

    void Update()
    {
        if( Time.timeScale <=0)
            return;
        
        // Calculate mouse offset: normalize the mouse position, center it around zero, then scale.
        Vector2 mousePos = Input.mousePosition;
        Vector2 mouseNorm = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
        Vector2 mouseOffset = (mouseNorm - new Vector2(0.5f, 0.5f)) * mouseInfluence;

        // Update the background's position.
        Vector2 pos = rectTransform.anchoredPosition;
        pos += velocity * Time.deltaTime + mouseOffset;

        // Bounce on horizontal safe bounds.
        if (pos.x > safeRangeX)
        {
            pos.x = safeRangeX;
            velocity.x = -Mathf.Abs(velocity.x);
        }
        else if (pos.x < -safeRangeX)
        {
            pos.x = -safeRangeX;
            velocity.x = Mathf.Abs(velocity.x);
        }

        // Bounce on vertical safe bounds.
        if (pos.y > safeRangeY)
        {
            pos.y = safeRangeY;
            velocity.y = -Mathf.Abs(velocity.y);
        }
        else if (pos.y < -safeRangeY)
        {
            pos.y = -safeRangeY;
            velocity.y = Mathf.Abs(velocity.y);
        }

        rectTransform.anchoredPosition = pos;
    }
}
