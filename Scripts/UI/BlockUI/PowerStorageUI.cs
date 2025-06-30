using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class PowerStorageUI : ElectricityUI{
    public IPowerBattery battery;
    
    public TMP_Text powerText;
    
   
    public void Init (IPowerBattery _bat){
        this.battery = _bat;
        plug.sprite = battery.myGrid != null ? plugOn : plugOff;
        plugButton.onClick.AddListener(() => {
            if(battery.myGrid != null)
                WindowManager.Instance.CreateGridWindow(battery.myGrid);
        });

    }
   
    void Update(){
        powerText.text =  battery.storedPower.ToString("0") + " / " + battery.capacity.ToString("0") + "W";
    }
}

