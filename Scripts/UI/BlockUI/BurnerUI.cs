using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class BurnerUI : MonoBehaviour{
    public SlicedFilledImage fill;
    public ContainerUI containerUI;

    public Burner burner;

    public void Init(Burner b){
        burner = b;
        containerUI.Init(burner.fuelContainer);
    }

    private void Update(){
        if (burner == null)
            return;
        fill.fillAmount = (float)burner.fuelTime / (float)burner.maxFuelTime;
    }
}