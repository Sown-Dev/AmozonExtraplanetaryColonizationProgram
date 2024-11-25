using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour{
    public GameObject blackBG;
    public Transform tutorialParent;

    public List<Tutorial> tutorials;
    
   private Tutorial currentTutorial;
    

    

    private void Awake(){
 
        
        blackBG.SetActive(false);
        blackBG.transform.SetParent(tutorialParent);
        blackBG.transform.SetAsFirstSibling();
        foreach (Transform child in tutorialParent){
            child.gameObject.SetActive(false);
        }
        foreach (Tutorial tut in tutorials){
                foreach (GameObject step in tut.steps){
                    step.transform.parent = tutorialParent;
                    step.SetActive(false);
                }
            
        }
    }

    private void Start(){
        currentTutorial = null;
    }

    private void Reset(){
        currentTutorial = null; //whatever genuine retard at unity made me have to do this needs to be shot in the head until they can come up with a better idea.
    }


    private void Update(){
        if (Input.anyKeyDown){
            StartTutorial("controls");
        }                                                
        
        if (Input.GetKeyDown(KeyCode.Space) && currentTutorial != null){
            NextStep();
        }
        
        Debug.Log(currentTutorial == null ? "No tutorial running" : $"Current tutorial: {currentTutorial.title}");
        
    }
    
    public void StartTutorial(string title){
        Debug.Log($"Starting tutorial {title}");
        Tutorial t = tutorials.Find(tut => tut.title == title);
        if (t == null || t == currentTutorial || t.completed){
            Debug.LogError($"Tutorial of name '{title}' not found");
            return;
        }
        if(currentTutorial != null){
            Debug.Log($"Ending current tutorial {currentTutorial.title}");
            EndTutorial();
        }
        CursorManager.Instance.OpenUI();
        blackBG.SetActive(true);
        Debug.Log($"Starting tutorial {t.title}");
        currentTutorial = t;
        t.currentStep = 0;
        
        //set all children to null except currentTutorial[currentStep]

        foreach (Tutorial tut in tutorials){
            if (tut != t){
                foreach (GameObject step in tut.steps){
                    step.SetActive(false);
                }
            }
        }
        currentTutorial.steps[t.currentStep].SetActive(true);
        
    }
    public void NextStep(){
        Debug.Log($"Next step on {currentTutorial.title}. Current step: {currentTutorial.currentStep}/{currentTutorial.steps.Length-1}, next step will be {currentTutorial.currentStep+1}");
        if (currentTutorial == null) return;
        if (currentTutorial.currentStep >= currentTutorial.steps.Length-1){
            EndTutorial();
            return;
        }
        currentTutorial.steps[currentTutorial.currentStep].SetActive(false);
        currentTutorial.currentStep++;
        currentTutorial.steps[currentTutorial.currentStep].SetActive(true);
    }
    
    public void EndTutorial(){
        
        CursorManager.Instance.CloseUI();
        blackBG.SetActive(false);
        
        currentTutorial.steps[currentTutorial.currentStep].SetActive(false);
        currentTutorial.completed = true;
        currentTutorial = null;
    }
    
    [Serializable]
    public class Tutorial{
        public string title;
        public GameObject[] steps;
        [HideInInspector]public int currentStep;
        [HideInInspector]public bool completed;

        public Tutorial(){
            
        }
        
    }
}