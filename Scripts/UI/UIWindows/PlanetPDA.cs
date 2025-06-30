using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlanetPDA : UIWindow{
    public TMP_InputField planetName;

    public Transform planetTags;
    public PlanetTag planetTagPrefab;
    
    public Image backgroundImage;
    public Image clouds;
    public Image continents;

    public override void Awake(){
        base.Awake();
        Refresh();
    }

    public void Refresh(){
        planetName.text = GameManager.Instance.currentWorld.name;
        
        foreach (Transform child in planetTags){
            Destroy(child.gameObject);
        }
        PlanetTag type = Instantiate(planetTagPrefab, planetTags);
        var rt = type.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        type.tagText.text = GameManager.Instance.currentWorld.planetType.ToString();
        
        continents.color = new Color(1f, 1f, 1f);

        backgroundImage.color = new Color(0.4f, 0.6f, 0.9f);

        switch ( GameManager.Instance.currentWorld.planetType){
            case PlanetType.GasGiant:
                backgroundImage.color = new Color(0.4f, 0.8f, 0.3f);
                type.tagBG.color = new Color(0.5f, 0.8f, 0.4f);
                break;
            case PlanetType.Rocky:
                backgroundImage.color = new Color(0.7f, 0.5f, 0.4f);
                type.tagBG.color = new Color(0.9f, 0.65f, 0.5f);
                break;
            case PlanetType.Forest:
                type.tagBG.color = new Color(0.5f, 1f, 0.5f);
                continents.color = new Color(0f, 0.9f, 0f);
                break;
            case PlanetType.Ocean:
                type.tagBG.color =  new Color(0.5f, 0.75f, 1f);
                break;
            case PlanetType.Tundra:
                type.tagBG.color =  new Color(0.55f, 0.95f, 0.9f);
                backgroundImage.color = new Color(0.7f, 0.95f, 0.95f);

                break;
            default:
                type.tagBG.color = new Color(1f, 0.5f, 0.5f);
                break;
        }
        //set cloud and continent offset based on seed. use the image's actual size to determine the range
        float cloudWidth = clouds.rectTransform.rect.width;
        float cloudHeight = clouds.rectTransform.rect.height;
        clouds.transform.localPosition = new Vector3(Random.Range(-cloudWidth / 3, cloudWidth / 3), Random.Range(-cloudHeight /3, cloudHeight / 3), 0);
        float continentWidth = continents.rectTransform.rect.width;
        float continentHeight = continents.rectTransform.rect.height;
        //set continents offset based on seed
        continents.transform.localPosition = new Vector3(Random.Range(-continentWidth / 3, continentWidth / 3), Random.Range(-continentHeight / 3, continentHeight / 3), 0);        
        
        
        
        type.tagBG.color = new Color(type.tagBG.color.r, type.tagBG.color.g, type.tagBG.color.b, 0.1f);

        type.gameObject.SetActive(false);
        type.gameObject.SetActive(true);
        //refresh type for content size fitter
        type.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        //rebuild ui
        LayoutRebuilder.ForceRebuildLayoutImmediate(planetTags.GetComponent<RectTransform>());
        
    
    }

    

    public void GenerateNewPlanet(){
        GameManager.Instance.CreateNewWorld();
        Refresh();
    }

    public void SetPlanetName(string name){
        GameManager.Instance.currentWorld.name = name;
        planetName.text = name;
    }
}

