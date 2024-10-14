using System;
using System.Collections;
using System.Collections.Generic;
using Systems.BlockUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberSelectorUI : MonoBehaviour{
    public NumberSelector numSelector;

    public TMP_Text number;
    
    public Button buttonUp;
    public Button buttonDown;
    
    public void Init(NumberSelector ns){
        numSelector = ns;
        number.text = numSelector.value.ToString();
        
        buttonUp.onClick.AddListener(() => Change(1));
        buttonDown.onClick.AddListener(() => Change(-1));
    }
    
    public void Change (int value){
        numSelector.Change(value);
        number.text = numSelector.value.ToString();
    }

    private void Update(){
        //set number if number key is pressed
        for (int i = 0; i < 10; i++){
            if (Input.GetKeyDown(i.ToString())){
                numSelector.value = i;
                number.text = numSelector.value.ToString();
            }
        }
        buttonUp.interactable = numSelector.value < numSelector.max;
        buttonDown.interactable = numSelector.value > numSelector.min;
    }
}

public class NumberSelector: IBlockUI{
    public int value;
    public int min = 0;
    public int max = 32;
    
    public Action OnChange;
    
    public NumberSelector( Action change,int min = 0, int max = 32){
        this.min = min;
        this.max = max;
        value = min;
        if(change != null)
            this.OnChange = change;
    }
    
    public void Change(int value){
        this.value += value;
        if(this.value < min){
            this.value = min;
        }
        if(this.value > max){
            this.value = max;
        }
        OnChange?.Invoke();
    }

    public int Priority{ get; set; }
    public bool Hidden{ get; set; }
}
    