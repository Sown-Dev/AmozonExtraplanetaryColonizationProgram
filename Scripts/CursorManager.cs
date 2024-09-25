using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

public class CursorManager : MonoBehaviour{
    public static CursorManager Instance;
    public GameObject crosshair; //show if playing

    private bool crosshairEnabled;
    public int uiDepth=0;
    private bool crosshairActive;
    
    
    private void Awake(){
        Instance = this;
        EnableUI();
    }


    public void OpenUI(){
        uiDepth++;
    }
    public void CloseUI(){
        uiDepth--;

        StartCoroutine(CloseUICR());
        //GC.Collect();
    }
    //Added delay for animations and difficulty
    public IEnumerator CloseUICR(){
        yield return new WaitForSecondsRealtime(0.5f);

    }
    
    private void Update(){
        if (crosshairEnabled){
            crosshairActive = uiDepth <= 0;

            //UnityEngine.Cursor.visible = !crosshairActive;
            //crosshair.SetActive(crosshairActive);
            
            Time.timeScale = crosshairActive ? 1 : 0;

        }
        else{
            //UnityEngine.Cursor.visible = true;
            crosshair.SetActive(false);
        
            Time.timeScale = 1;

        }

       
    }
    public void DisableUI(){
        crosshairEnabled= false;
        uiDepth = 100;
    }
    
    public void EnableUI(){
        crosshairEnabled= true;
        uiDepth = 0;
    }
    public bool isUIOpen(){
        return uiDepth > 0;
    }
    
    
}