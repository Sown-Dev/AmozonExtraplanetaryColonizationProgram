using System;
using Systems.Block;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class BlockInfoUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private Image icon;
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private Transform iconScaleParent; // New scaling parent

        [HideInInspector] public Block block;
        private Vector2 lastIconSize;

        private void Update()
        {
            if (block != null && !EventSystem.current.IsPointerOverGameObject())
            {
                UpdateInfoDisplay();
                HandleIconScaling();
                ForceLayoutRefresh();
            }
            else
            {
                ResetDisplay();
            }
        }

        private void UpdateInfoDisplay()
        {
            cg.alpha = 1;
            cg.interactable = true;
            
            nameText.text = block.properties.name;
            descText.text = block.GetDescription().ToString();
            
            if (icon.sprite != block.sr.sprite)
            {
                icon.sprite = block.sr.sprite;
                icon.SetNativeSize();
                lastIconSize = icon.rectTransform.sizeDelta;
            }
        }

        private void HandleIconScaling()
        {
            if (iconScaleParent == null) return;

            bool needsScaling = lastIconSize.x > 64 || lastIconSize.y > 64;
            iconScaleParent.localScale = needsScaling ? 
                new Vector3(0.5f, 0.5f, 1f) : 
                Vector3.one;
        }

        private void ForceLayoutRefresh()
        {
            if (layoutElement.ignoreLayout)
            {
                layoutElement.ignoreLayout = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(
                    (RectTransform)transform.parent
                );
            }
        }

        private void ResetDisplay()
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
            
            if (!layoutElement.ignoreLayout)
            {
                layoutElement.ignoreLayout = true;
                LayoutRebuilder.ForceRebuildLayoutImmediate(
                    (RectTransform)transform.parent
                );
            }
        }
    }
}