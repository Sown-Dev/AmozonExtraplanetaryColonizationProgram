using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour{
    public static TutorialManager Instance;
    
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
                
                for( int i = 0; i < tut.steps.Length; i++){
                    tut.steps[i].Init( tut.title + i, tut.steps[i].highlight);
                    tut.steps[i].transform.SetParent(tutorialParent);   
                    tut.steps[i].gameObject.SetActive(false);
                }

            
        }
        
        Instance = this;
        currentTutorial = null;

    }


    private void Reset(){
        currentTutorial = null; //whatever genuine retard at unity made me have to do this needs to be shot in the head until they can come up with a better idea.
    }


    private void Update(){
      
        if (Input.GetKeyDown(KeyCode.Space) && currentTutorial != null){
            NextStep();
        }
        
        
    }
    public void StartTutorial(string title, float delay = 0f){
        if(currentTutorial != null){
            Debug.Log($"Cannot start tutorial. one is already in progress");
            return;
        }

        if (GameManager.Instance.settings.completedTutorials.Contains(title)){
            //Debug.Log("tutorial already completed");
            return;
        }

        if (title.Length < 2){
            return;
        }
        Tutorial t = tutorials.Find(tut => tut.title == title);
        if (t == null || t == currentTutorial || t.completed){
            Debug.LogWarning($"Tutorial of name '{title}' not found");
            return;
        }

        if (t.prerequisite.Length > 1){  //check to see if we actualy have a prerequisite, not just empty string
            if (GameManager.Instance.settings.completedTutorials.Contains(t.prerequisite)){
                Debug.Log($"Prerequisite {t.prerequisite} already completed");
            }
            else{
                Debug.Log($"Prerequisite {t.prerequisite} not completed, skipping tutorial");
                return;
            }
        }
        
        StartCoroutine(StartTutorialCoroutine(t, delay));
    }

    private System.Collections.IEnumerator StartTutorialCoroutine(Tutorial t, float delay){
        yield return new WaitForSecondsRealtime(delay);
        
        
        //in case tutorial gets started between coroutine start and now
        if(currentTutorial != null){
            Debug.Log($"Cannot start tutorial. one is already in progress");
            yield break;
        }
        
        

        CursorManager.Instance.OpenUI();
        blackBG.SetActive(true);
        Debug.Log($"Starting tutorial {t.title}");
        
        currentTutorial = t;
        t.currentStep = 0;
        
        //set all children to null except currentTutorial[currentStep]

        foreach (Tutorial tut in tutorials){
            if (tut != t){
                for( int i = 0; i < tut.steps.Length; i++){
                    
                    tut.steps[i].gameObject.SetActive(false);
                }
                
            }
        }
        currentTutorial.steps[t.currentStep].gameObject.SetActive(true);
        
    }
    public void NextStep(){
        Debug.Log($"Next step on {currentTutorial.title}. Current step: {currentTutorial.currentStep}/{currentTutorial.steps.Length-1}");
        if (currentTutorial == null) return;
        if (currentTutorial.currentStep >= currentTutorial.steps.Length-1){
            EndTutorial();
            return;
        }
        currentTutorial.steps[currentTutorial.currentStep].gameObject.SetActive(false);
        currentTutorial.currentStep++;
        currentTutorial.steps[currentTutorial.currentStep].gameObject.SetActive(true);
    }
    
    public void EndTutorial(){
        
        CursorManager.Instance.CloseUI();
        blackBG.SetActive(false);
        String nextTutorial = currentTutorial.nextTutorial;
        GameManager.Instance.settings.completedTutorials.Add(currentTutorial.title);
        currentTutorial.steps[currentTutorial.currentStep].gameObject.SetActive(false);
        currentTutorial.completed = true;
        currentTutorial = null;
        
        StartTutorial(nextTutorial);
    }
    
    [Serializable]
    public class Tutorial{
        public string title;
        public TutorialElement[] steps;
        [HideInInspector]public int currentStep;
        [HideInInspector]public bool completed;
        
        
        public string prerequisite;
        [FormerlySerializedAs("NextTutorial")] public string nextTutorial;

        public Tutorial(){
            
        }
        
    }
}