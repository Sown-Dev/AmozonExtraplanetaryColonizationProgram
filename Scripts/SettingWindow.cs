using System;
using System.Collections.Generic;
using UI;
using UnityEngine.UI;

public class SettingWindow: UIWindow{
    
    public Button resetTutorialButton;
    public void ResetTutorial(){

        GameManager.Instance.settings.completedTutorials = new List<string>();
    }

    private void Update(){
        resetTutorialButton.interactable = GameManager.Instance.settings.completedTutorials.Count > 0;
    }
}