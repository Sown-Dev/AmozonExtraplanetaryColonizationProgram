using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour{
    public List<Tutorial> tutorials;
    
    public Tutorial currentTutorial;


    private void Awake(){
         currentTutorial = null;
    }

    private void Update(){
        //next step if any pressed and has tutorial;
        if (Input.GetKeyDown(KeyCode.Space) && currentTutorial != null){
            NextStep();
        }
        
    }
    
    public void StartTutorial(string title){
        Tutorial t = tutorials.Find(tut => tut.title == title);
        if (t == null) return;
        currentTutorial = t;
        t.currentStep = 0;
        
        //set all children to null except currentTutorial[currentStep]

        foreach (GameObject child in transform){
            if(child != currentTutorial.steps[t.currentStep]){
                child.SetActive(false);
            }
            else{
                child.SetActive(true);
            }
        }
        
    }
    public void NextStep(){
        if (currentTutorial == null) return;
        if (currentTutorial.currentStep >= currentTutorial.steps.Length){
            EndTutorial();
            return;
        }
        currentTutorial.steps[currentTutorial.currentStep].SetActive(false);
        currentTutorial.currentStep++;
        if (currentTutorial.currentStep >= currentTutorial.steps.Length) return;
        currentTutorial.steps[currentTutorial.currentStep].SetActive(true);
    }
    
    public void EndTutorial(){
        currentTutorial.steps[currentTutorial.currentStep].SetActive(false);
        currentTutorial = null;
    }
    
    [Serializable]
    public class Tutorial{
        public string title;
        public GameObject[] steps;
        public int currentStep;
        
    }
}