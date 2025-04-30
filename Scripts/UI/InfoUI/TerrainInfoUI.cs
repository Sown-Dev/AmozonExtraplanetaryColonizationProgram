using Systems.Terrain;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TerrainInfoUI : MonoBehaviour
{
    public static TerrainInfoUI Instance;
    
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private TMP_Text nameText;
    [FormerlySerializedAs("speedText")] [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private LayoutElement layoutElement;

    [HideInInspector] public TerrainProperties terrain;
    
    
    private void Awake()
    {
        Instance = this;
        terrain = null;
    }

    private void Update()
    {
        
        if (terrain != null && !EventSystem.current.IsPointerOverGameObject())
        {
            UpdateInfoDisplay();
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

        nameText.text = terrain.name;
        descriptionText.text = $"Move Speed: {terrain.walkSpeed:0.##}";
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