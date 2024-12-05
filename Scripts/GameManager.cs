using System;
using NewRunMenu;
using UnityEngine;


public class GameManager: MonoBehaviour{
     public static GameManager Instance;

     
     public Character selectedChar;
    public Character[] allCharacters;

     private void Awake(){
         if (Instance == null){
             Instance = this;
             DontDestroyOnLoad(gameObject);
         }
         else{
             Destroy(gameObject);
         }
     }

     
     

}