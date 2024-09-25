using UnityEngine;
using UnityEngine.UI;

public class SignatureCapture : MonoBehaviour
{
    public RawImage signatureImage;  // Reference to the RawImage UI component
    private Texture2D texture;
    private Texture2D originalTexture; // Store the original texture
    private bool isDrawing = false;
    private Vector2 lastPosition;

    public Color penColor = Color.black;  // Settings for pen color
    public Vector2 penSize = new Vector2(5, 5); // Brush size as a Vector2

    public bool HasSignature { get; private set; } = false; // True if there is a signature on the texture

    void Start()
    {
        // Check if the texture in the RawImage is readable
        if (signatureImage.texture is Texture2D tempOriginalTexture && tempOriginalTexture.isReadable)
        {
            originalTexture = new Texture2D(tempOriginalTexture.width, tempOriginalTexture.height, TextureFormat.RGBA32, false);
            originalTexture.SetPixels32(tempOriginalTexture.GetPixels32()); // Copy the original texture's pixels
            
            // Create a new texture for drawing
            texture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
            texture.SetPixels32(originalTexture.GetPixels32()); // Copy the original texture's pixels

            // Set filter and wrap modes
            texture.filterMode = FilterMode.Point; // Set to point filtering
            texture.wrapMode = originalTexture.wrapMode; // Copy wrap mode from original
        }
        else
        {
            // Create a new white texture if the original is not readable or does not exist
            texture = new Texture2D((int)signatureImage.rectTransform.rect.width, (int)signatureImage.rectTransform.rect.height, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[texture.width * texture.height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.white;

            texture.SetPixels32(pixels);
            texture.filterMode = FilterMode.Point; // Set to point filtering for the white texture
        }

        texture.Apply();

        // Assign the new texture to the RawImage
        signatureImage.texture = texture;
    }

    void Update()
    {
        
        // Capture drawing input when the user clicks and drags the mouse
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            lastPosition = GetMousePosition();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            // Reset last position to prevent drawing from a stale point
            lastPosition = Vector2.zero; 
        }

        if (isDrawing)
        {
            Vector2 mousePos = GetMousePosition();
            DrawLine(lastPosition, mousePos); // Draw line to the current position
            lastPosition = mousePos; // Update last position
        }
    }

    // Get the mouse position in texture space
    Vector2 GetMousePosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(signatureImage.rectTransform, Input.mousePosition, null, out Vector2 localPoint);
        
        // Convert local position to texture coordinates
        localPoint.x += signatureImage.rectTransform.rect.width / 2;
        localPoint.y += signatureImage.rectTransform.rect.height / 2;

        return new Vector2(localPoint.x, localPoint.y);
    }

    // Draw a line between two points
    void DrawLine(Vector2 from, Vector2 to)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(from, to)); // Number of steps to interpolate
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps; // Interpolation factor
            Vector2 point = Vector2.Lerp(from, to, t); // Interpolated point
            SetPixel(point); // Set the pixel for the interpolated point
        }
    }

    // Set a pixel on the texture at a given position
    void SetPixel(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        for (int i = -(int)penSize.x / 2; i < (int)penSize.x / 2; i++)
        {
            for (int j = -(int)penSize.y / 2; j < (int)penSize.y / 2; j++)
            {
                if (x + i < 0 || x + i >= texture.width || y + j < 0 || y + j >= texture.height)
                    continue;

                // Apply black tint to the pen color
                Color tintedColor = ApplyBlackTint(penColor);

                // If drawing for the first time, set HasSignature to true
                if (texture.GetPixel(x + i, y + j) == Color.white)
                {
                    HasSignature = true;
                }

                texture.SetPixel(x + i, y + j, tintedColor);
            }
        }

        texture.Apply();
    }

    // Apply a black tint to the current color
    private Color ApplyBlackTint(Color color)
    {
        float tintAmount = 0.5f; // Adjust this value to change the tint intensity
        return Color.Lerp(color, Color.black, tintAmount);
    }

    // Clear the signature area by restoring the original texture
    public void ClearSignature()
    {
        if (originalTexture != null)
        {
            texture.SetPixels32(originalTexture.GetPixels32()); // Restore the original pixels
            texture.Apply();
        }

        // Reset HasSignature to false after clearing
        HasSignature = false;
    }

    // Cleanup method to delete the texture when done
    private void OnDestroy()
    {
        if (texture != null)
        {
            Destroy(texture);
        }
        if (originalTexture != null)
        {
            Destroy(originalTexture);
        }
    }
}
