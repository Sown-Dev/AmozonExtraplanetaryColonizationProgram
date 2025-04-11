using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayInfo : MonoBehaviour
{
   
   public TerrainManager terrainManager;
   
   public TMP_Text dayText;
   public Image sunFill;

   private bool show;

   private void Awake(){
       show = false;
       gameObject.SetActive( GameManager.Instance.settings.DevMode && show);
   }

   private void Update(){
       if (Input.GetKeyDown(KeyCode.Alpha0)){
              show = !show;
              gameObject.SetActive(show && GameManager.Instance.settings.DevMode);
       }

       dayText.text = $"TotalTicks:{terrainManager.totalTicksElapsed}\n" +
                      $"DayProgress:{terrainManager.GetDayProgress()}\n";
      sunFill.fillAmount = terrainManager.GetSolarIntensity();
   }
}
