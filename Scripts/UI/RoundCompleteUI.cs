using System;
using System.Collections;
using Systems.Round;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundCompleteUI : MonoBehaviour
{
    public CanvasGroup earnedBarCG;
    public TMP_Text earnedBarText;
    public Image earnedBarFill;
    private float earned;
    int realEarned;
    private int toEarned;

    public CanvasGroup timeCG;
    public TMP_Text timeText;
    private float timeRemaining;
    private float toRemaining;
    private float timeBonus;
    private int realTimeBonus;
    
    public CanvasGroup addtimeCG;
    public TMP_Text addtimeText;
    private float addTime;

    public CanvasGroup rewardCG;
    public TMP_Text rewardText;
    
    public CanvasGroup debtCG;
    public TMP_Text debtText;

    public CanvasGroup totalCG;
    public TMP_Text totalText;

    public Button continueButton;

    [Header("Animation Settings")]
    public AnimationCurve easeInOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private float earnProgress;
    private float timeProgress;
    private float earnStartValue;
    private float timeStartValue;

    private void Awake()
    {
        continueButton.onClick.AddListener(Continue);
    }

    public void Init(int earn, float time)
    {
        earnedBarCG.alpha = 0;
        timeCG.alpha = 0;
        rewardCG.alpha = 0;
        totalCG.alpha = 0;
        addtimeCG.alpha = 0;
        debtCG.alpha = 0;

        continueButton.interactable = false;
        addTime = time / 2;
        RoundManager.Instance.addTime = addTime;

        StartCoroutine(StartSequence(earn, time));
    }

    private int totalBonus = 0;

    IEnumerator StartSequence(int _earned, float _timeRemaining)
    {
        yield return new WaitForSecondsRealtime(1f);
        StartEarned(_earned);
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(AnimateEarned());
        yield return new WaitForSecondsRealtime(2.5f);
        StartTime(_timeRemaining);
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(AnimateTime());
        yield return new WaitForSecondsRealtime(1.5f);
        
        addtimeText.text = $"Extra time for next contract:      <color=#118811ff>+{(int)(addTime/60)}:{(addTime % 60):00}</color>";
        addtimeCG.alpha = 1;
        yield return new WaitForSecondsRealtime(1f);
        rewardText.text = $"Contract Reward: <color=#118811ff>${RoundManager.Instance.currentContract.reward}</color>";
        rewardCG.alpha = 1;
        
        totalBonus = realTimeBonus + RoundManager.Instance.currentContract.reward;


        if (RoundManager.Instance.debt > 0){
            yield return new WaitForSecondsRealtime(1.5f);
            debtCG.alpha = 1;

            if (totalBonus > RoundManager.Instance.debt){
                totalBonus -= RoundManager.Instance.debt;
                debtText.text = $"Debt Collection: <color=#881111ff>-${RoundManager.Instance.debt}</color>";
                RoundManager.Instance.debt = 0;

            }
            else{
                debtText.text = $"Debt Collection:  <color=#118811ff>-${RoundManager.Instance.debt -totalBonus}</color>";
                RoundManager.Instance.debt -= totalBonus;
                totalBonus = 0;


            }

        }


        yield return new WaitForSecondsRealtime(1.5f);
        
        totalCG.alpha = 1;
        totalText.text = $"Total Bonus:    <color=#118811ff>${totalBonus}</color>";
        
        yield return new WaitForSecondsRealtime(1f);
        continueButton.interactable = true;
    }

    private void Update()
    {
        earnedBarText.text = (int)earned + " / " + RoundManager.Instance.currentContract.requiredQuota;
        earnedBarFill.rectTransform.offsetMax = 
            new Vector2(-2 + ((1 - (earned / RoundManager.Instance.currentContract.requiredQuota)) * -156), -2);

        timeText.text =
            $"Time Remaining: <color=#226633ff>{(int)(timeRemaining / 60)}:{(timeRemaining % 60):00}</color>\nTime Bonus:    <color=#118811ff>+${(int)timeBonus}</color>";
    }

    public void StartEarned(int _earned)
    {
        earned = 0;
        toEarned = _earned;
        realEarned = _earned;
        earnedBarCG.alpha = 1;
    }

    public void StartTime(float _timeRemaining)
    {
        timeRemaining = _timeRemaining;
        toRemaining = 0;
        realTimeBonus = (int)(timeRemaining) * (RoundManager.Instance.roundNum + 1);
        timeBonus = 0;
        timeCG.alpha = 1;
    }

    private IEnumerator AnimateEarned()
    {
        float duration = 2.5f;
        float startTime = Time.unscaledTime;
        float startValue = earned;

        while (Time.unscaledTime - startTime < duration)
        {
            float progress = (Time.unscaledTime - startTime) / duration;
            earned = Mathf.Lerp(startValue, toEarned, easeInOutCurve.Evaluate(progress));
            yield return null;
        }
        earned = toEarned;
    }

    private IEnumerator AnimateTime()
    {
        float duration = 1.5f;
        float startTime = Time.unscaledTime;
        float startValue = timeRemaining;

        while (Time.unscaledTime - startTime < duration)
        {
            float progress = (Time.unscaledTime - startTime) / duration;
            timeRemaining = Mathf.Lerp(startValue, 0, easeInOutCurve.Evaluate(progress));
            timeBonus = Mathf.Lerp(0, realTimeBonus, 
                1 - timeRemaining / RoundManager.Instance.roundTime);
            yield return null;
        }
        timeRemaining = 0;
        timeBonus = realTimeBonus;
    }

    public void Continue()
    {
        RoundManager.Instance.AddMoney(totalBonus, false);
        RoundManager.Instance.CompleteRound();
        CursorManager.Instance.CloseUI();
        Destroy(gameObject);
    }
}