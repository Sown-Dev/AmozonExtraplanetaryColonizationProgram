using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FixedSizeSprite : MonoBehaviour
{
    public float fixedWidth = 16f; // Desired fixed width in pixels
    public float fixedHeight = 16f; // Desired fixed height in pixels
    public float scaleModifier = 1f; // Scale modifier to adjust the size
    private void Start()
    {
        AdjustScale();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Adjust scale in editor mode for testing purposes
        
        //AdjustScale();
    }
#endif

    public void Update(){
        //AdjustScale();
        
    }

    void AdjustScale()
    {
        Camera camera = Camera.main;
        Sprite s= GetComponent<SpriteRenderer>().sprite;

        // Calculate the size of the sprite in world units
        float spriteWidthInUnits = fixedWidth /s.texture.width;
        float spriteHeightInUnits = fixedHeight / s.texture.height;

        transform.localScale = new Vector3(spriteWidthInUnits, spriteHeightInUnits, 1f) * scaleModifier;
    }
}