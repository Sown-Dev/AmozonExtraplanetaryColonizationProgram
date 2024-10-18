using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Round;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RoundCompleteUI : MonoBehaviour{

    public CanvasGroup earnedBarCG;
    public TMP_Text earnedBarText;
    public Image earnedBarFill;
    private int earned;
    private int toEarned;

    
    public CanvasGroup timeCG;
    public TMP_Text timeText;
    private float timeRemaining;
    private float toRemaining;
    private int timeBonus;
    private int realTimeBonus;
    
    public Button continueButton;


    private void Awake(){
        continueButton.onClick.AddListener(Continue);
    }

    public void Init(int earn, float time){
        //set all elements to hide
        earnedBarCG.alpha = 0;
        
        timeCG.alpha = 0;
        
        continueButton.interactable = false;
        
        StartCoroutine(StartSequence(earn, time));
    }
    //coroutine to start each element in order
    IEnumerator StartSequence(int _earned, float _timeRemaining){
        //TODO play sound on each start
        yield return new WaitForSecondsRealtime(0.5f);
        StartEarned(_earned);
        yield return new WaitForSecondsRealtime(2f);
        StartTime(_timeRemaining);
        yield return new WaitForSecondsRealtime(2f);
        continueButton.interactable = true;
    }

    int lerpFactor = 4;

    private void FixedUpdate(){
        Debug.Log(Time.timeScale);
        earnedBarText.text = earned + " / " + RoundManager.Instance.quotaRequired;
        earnedBarFill.rectTransform.offsetMax =
            new Vector2(-2 + (earned / RoundManager.Instance.quotaRequired) * 160, -2);

        timeText.text = $"Time Remaining: <color=#226633ff>{(int)(timeRemaining / 60)}:{(timeRemaining % 60).ToString("00")}</color>\nTime Bonus:    <color=#118811ff>+{timeBonus}</color>";

        earned = Mathf.RoundToInt(Mathf.Lerp(earned, toEarned, Time.fixedDeltaTime* lerpFactor));
        //if(earned/toEarned >= 0.98f)
           // earned = toEarned;
        
        timeRemaining = Mathf.Lerp(timeRemaining, toRemaining, Time.fixedDeltaTime * lerpFactor);
        timeBonus = Mathf.RoundToInt(Mathf.Lerp(timeBonus, realTimeBonus, timeRemaining/RoundManager.Instance.roundTime));
        /*if(timeRemaining/toRemaining >= 0.98f){
            timeRemaining = toRemaining;
            timeBonus = realTimeBonus;
        }*/
    }

    public void StartEarned(int _earned){
        earned = 0;
        toEarned = _earned;
        
        earnedBarCG.alpha = 1;
    }

    public void StartTime(float _timeRemaining){
        timeRemaining = _timeRemaining;
        toRemaining = 0;
        realTimeBonus = (int)timeRemaining * (10 * RoundManager.Instance.roundNum+1);
        timeBonus = 0;
        
        timeCG.alpha = 1;
    }

    public void Continue(){
        Destroy(gameObject);
        RoundManager.Instance.StartRound();
    }
}