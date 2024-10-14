using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour{

    public Image fill;
    public Image bg;
    
    public ProgressBar bar;
    
    private void Update(){
        //fill.fillAmount = (float) bar.progress / (float) bar.maxProgress +0f;
        bg.color =bar.progress >= bar.maxProgress - 4? Color.white : new Color( 0.6f, 0.8f, 0.8f, 1f);
        fill.fillAmount = Mathf.Lerp( fill.fillAmount, (float) bar.progress / (float) bar.maxProgress, Time.unscaledDeltaTime * 64f); // not good
    }
}