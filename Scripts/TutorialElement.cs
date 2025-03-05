using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TutorialElement : MonoBehaviour {
    [SerializeField] private TMP_Text text;
    public Transform highlight;

    public void Init(string key, Transform highlight) {
        if (text == null) {
            text = GetComponentInChildren<TMP_Text>();
        }

        text.text = LocalizationSettings.StringDatabase.GetLocalizedString("tutorial", key);

        RectTransform rect = GetComponent<RectTransform>();
        rect.localPosition = new Vector3(
            Mathf.RoundToInt(rect.localPosition.x),
            Mathf.RoundToInt(rect.localPosition.y),
            0);

        // Force content size fitter refresh if applicable
        LayoutGroup layout = GetComponent<LayoutGroup>();
        if (layout != null) {
            layout.enabled = false;
            layout.enabled = true;
        }
    }
}