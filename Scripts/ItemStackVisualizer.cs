using System;
using Systems.Items;
using UnityEngine;

public class ItemStackVisualizer : MonoBehaviour
{
    public ItemStack myItemStack;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Update()
    {
        // Ensure the sprite is always upright
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// Sets the ItemStack to visualize and subscribes to its change event.
    /// </summary>
    /// <param name="itemStack">The ItemStack to visualize.</param>
    public void SetItemStack(ItemStack itemStack)
    {
        
        myItemStack = itemStack;

        Refresh();
    }

    /// <summary>
    /// Refreshes the visual representation of the ItemStack.
    /// </summary>
    public void Refresh()
    {
        if (myItemStack != null && myItemStack.item != null)
        {
            // Set the sprite to the ItemStack's icon
            spriteRenderer.sprite = myItemStack.item.icon;

            // Calculate the scale to fit the sprite within a 16x16 unit space
            float spriteWidthInUnits = 16f / spriteRenderer.sprite.texture.width;
            float spriteHeightInUnits = 16f / spriteRenderer.sprite.texture.height;

            spriteRenderer.transform.localScale = new Vector3(spriteWidthInUnits, spriteHeightInUnits, 1f);
        }
        else
        {
            // Clear the sprite if the ItemStack is null or has no item
            spriteRenderer.sprite = null;
        }
    }

    
}