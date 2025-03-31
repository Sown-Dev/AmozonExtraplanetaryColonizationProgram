using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

public class TitleScreen : MonoBehaviour{
    public UIWindow worldSelect;

    public Transform worldSelectList;
    public GameObject worldSelectPrefab;

    public TMP_Text tip;

    public string[] tips;
    
    public AudioSource music;
    
    void Start(){
        Refresh();
        worldSelect.Hide();
        tip.text = tips[Random.Range(0, tips.Length)];
        
        music.loop = true;
        music.PlayDelayed(2f);
        
    }

  
    
    
    public void Refresh(){
        foreach (Transform child in worldSelectList){
            Destroy(child.gameObject);
        }

        foreach (var world in GameManager.Instance.worlds){
            var go = Instantiate(worldSelectPrefab, worldSelectList);
            var worldButton = go.GetComponent<WorldButton>();
            worldButton.Init(world);
        }
    }
    
    public void NewRun(){
        GameManager.Instance.NewRunMenu();
    }

    public void LoadWorld(){
        worldSelect.Toggle();
    }
}
