using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class PauseManager : MonoBehaviour{
    private CursorManager cmr;
    public CanvasGroup cg;
    public Animator am;

    [SerializeField] public UIWindow settingsWindow;
    
    private void Start(){
        Close();
        cmr = CursorManager.Instance;
        settingsWindow.Hide();
    }
    
   

    public void Toggle(){
        if (open){
            Close();
        }else{
            Open();
        }
        
        
    }

    private bool open;
    private float toAlpha;

    public void Open(){
        if (cmr.isUIOpen())
            return;

        if (open)
            return;

        open = true;
        cmr.OpenUI();
        
        am.SetBool("Open", open);
        cg.interactable = open;
        cg.blocksRaycasts = open;
    }

    public void Close(){
        if (!open)
            return;
        open = false;
        cmr.CloseUI();
        
        am.SetBool("Open", open);
        cg.interactable = open;
        cg.blocksRaycasts = open;
        
        settingsWindow.Hide();
    }
    //Button Functions:
    public void Settings(){
        settingsWindow.Toggle();
    }
    public void Die(){
        Player.Instance?.Die();
        Close();
    }
    public void Quit(){
        Application.Quit();
    }
}