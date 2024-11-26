using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

public class CursorManager : MonoBehaviour{
    public static CursorManager Instance;

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
        yield return new WaitForSecondsRealtime(0.25f);

    }
    
    private void Update(){
        Time.timeScale = uiDepth>0 ? 0 : 1;
        
        

       
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