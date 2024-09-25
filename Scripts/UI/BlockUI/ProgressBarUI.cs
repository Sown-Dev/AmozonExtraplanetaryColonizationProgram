using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour{

    public Image fill;
    
    public ProgressBar bar;
    
    private void Update(){
        fill.fillAmount = (float) bar.progress / (float) bar.maxProgress +0f;
        //fill.fillAmount = Mathf.Lerp( fill.fillAmount, (float) bar.progress / (float) bar.maxProgress, Time.unscaledDeltaTime * 24f); // not good
    }
}