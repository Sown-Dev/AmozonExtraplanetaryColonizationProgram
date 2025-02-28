using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupListUI : MonoBehaviour
{
    public GameObject popupPrefab; // Prefab with a TextMeshPro component
    public float spacing = 30f; // Vertical spacing between popups
    public float popupDuration = 3f; // Duration before popups fade out
    public float animationSpeed = 5f; // Speed of the position animation
    public int maxPopupCount = 5; // Maximum number of popups allowed

    private List<PopupItem> popups = new List<PopupItem>();

    void Update()
    {
        // Animate the position of each popup
        for (int i = 0; i < popups.Count; i++)
        {
            PopupItem popup = popups[i];
            Vector3 targetPosition =  new Vector3(0, (popups.Count + -i) * spacing, 0);
            popup.rectTransform.anchoredPosition = Vector3.Lerp(popup.rectTransform.anchoredPosition, targetPosition, Time.deltaTime * animationSpeed);

            // Update the timer and fade out if necessary
            popup.timer += Time.deltaTime;
            if (popup.timer >= popupDuration)
            {
                popup.canvasGroup.alpha = Mathf.Lerp(popup.canvasGroup.alpha, 0, Time.deltaTime * animationSpeed);
                if (popup.canvasGroup.alpha <= 0.01f)
                {
                    RemovePopup(popup);
                    i--; // Adjust index after removal
                }
            }
        }
    }

    public void AddPopup(string message)
    {
        // If the list is at max capacity, remove the oldest popup
        if (popups.Count >= maxPopupCount)
        {
            RemovePopup(popups[0]);
        }

        // Instantiate a new popup
        GameObject popupGO = Instantiate(popupPrefab, transform);
        
        TMP_Text textComponent = popupGO.GetComponent<TMP_Text>();
        textComponent.text = message;

        // Initialize the popup item
        PopupItem newPopup = new PopupItem
        {
            gameObject = popupGO,
            rectTransform = popupGO.GetComponent<RectTransform>(),
            canvasGroup = popupGO.AddComponent<CanvasGroup>(),
            timer = 0f
        };
        newPopup.rectTransform.anchoredPosition = new Vector3(0, -spacing, 0);

        // Add to the list
        popups.Add(newPopup);
    }

    private void RemovePopup(PopupItem popup)
    {
        // Destroy the popup game object and remove it from the list
        Destroy(popup.gameObject);
        popups.Remove(popup);
    }

    private class PopupItem
    {
        public GameObject gameObject;
        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;
        public float timer;
    }
}