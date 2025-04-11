using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour{
    private CursorManager cmr;
    public CanvasGroup cg;
    public Animator am;


    public Button saveButton;


    private void Start(){
        Close();
        cmr = CursorManager.Instance;
    }


    public void Toggle(){
        if (open){
            Close();
        }
        else{
            Open();
        }
    }

    private void Update(){
        //savebutton only if we have a world loaded
        saveButton.gameObject.SetActive(GameManager.Instance.inGame);
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

        GameManager.Instance.CloseSettings();
    }

    //Button Functions:
    public void Settings(){
        GameManager.Instance.ToggleSettings();
    }

    public void Save(){
        Close();
        GameManager.Instance.Save();
    }

    public void ExitMain(){
        Close();
        GameManager.Instance.ExitToMain();
    }

    public void Quit(){
        Close();
        GameManager.Instance.Quit();
    }
}