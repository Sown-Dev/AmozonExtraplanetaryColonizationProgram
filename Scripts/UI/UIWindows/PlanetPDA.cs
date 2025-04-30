using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlanetPDA : UIWindow{
    public TMP_InputField planetName;

    public Transform planetTags;
    public PlanetTag planetTagPrefab;

    public override void Awake(){
        base.Awake();
        planetName.text = GameManager.Instance.currentWorld.name;
        PlanetTag type = Instantiate(planetTagPrefab, planetTags);
        type.tagText.text = GameManager.Instance.currentWorld.planetType.ToString();

        switch ( GameManager.Instance.currentWorld.planetType){
            case PlanetType.GasGiant:
                type.tagBG.color = new Color(0.5f, 0.5f, 1f);
                break;
            case PlanetType.Rocky:
                type.tagBG.color = new Color(0.9f, 0.65f, 0.5f);
                break;
            case PlanetType.Forest:
                type.tagBG.color = new Color(0.5f, 1f, 0.5f);
                break;
            case PlanetType.Ocean:
                type.tagBG.color =  new Color(0.5f, 0.75f, 1f);
                break;
            case PlanetType.Tundra:
                type.tagBG.color =  new Color(0.6f, 1f, 1f);
                break;
            default:
                type.tagBG.color = new Color(1f, 0.5f, 0.5f);
                break;
        }
        type.tagBG.color = new Color(type.tagBG.color.r, type.tagBG.color.g, type.tagBG.color.b, 0.1f);
    }

    public void SetPlanetName(string name){
        GameManager.Instance.currentWorld.name = name;
        planetName.text = name;
    }
}

