using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

public class PlanetPDA : UIObject
{
    
    public TMP_InputField planetName;


    public override void Awake(){
        base.Awake();
        planetName.text = GameManager.Instance.currentWorld.name;
    }

    public void SetPlanetName(string name)
    {
        GameManager.Instance.currentWorld.name = name;
        planetName.text = name;
    }
    
}
