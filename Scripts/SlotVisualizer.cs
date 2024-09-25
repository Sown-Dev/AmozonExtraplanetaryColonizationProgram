using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Items;
using UnityEngine;

public class SlotVisualizer : MonoBehaviour
{
    public Slot mySlot;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void SetSlot(Slot s)
    {
        mySlot = s;
        if (mySlot != null)
            mySlot.OnChange += Refresh;
        Refresh();
    }

    public void Refresh()
    {
        if (mySlot?.ItemStack != null)
        {
            spriteRenderer.sprite = mySlot.ItemStack.item.icon;
            float spriteWidthInUnits = 16f / spriteRenderer.sprite.texture.width;
            float spriteHeightInUnits = 16f / spriteRenderer.sprite.texture.height;

            spriteRenderer.transform.localScale = new Vector3(spriteWidthInUnits, spriteHeightInUnits, 1f);
        }
        else
        {
            spriteRenderer.sprite = null;
        }



    }
}
