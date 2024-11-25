using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class TutorialElement : MonoBehaviour{
    public GameObject text;

    public Transform highLight;
    
    

    public void Init(string key, Transform highlight){
        GameObject go = Instantiate(text, transform);
        go.GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("tutorial", key);
    }
}
