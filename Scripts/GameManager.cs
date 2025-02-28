using System;
using NewRunMenu;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager: MonoBehaviour{
     public static GameManager Instance;

     
     public Character selectedChar;
    public Character[] allCharacters;

    public WorldStats myStats;

     private void Awake(){
         if (Instance == null){
             Instance = this;
             DontDestroyOnLoad(gameObject);
             SceneManager.LoadScene("Scenes/Run Start");
         }
         else{
             Destroy(gameObject);
         }
         LoadStats();
         InvokeRepeating(nameof(SaveStats), 300f, 300f);
     }


     public void SaveStats(){
         string json = JsonUtility.ToJson(myStats);
         PlayerPrefs.SetString("PlayerStats", json);
         
         PlayerPrefs.Save();
     }

     public void LoadStats(){
         if (PlayerPrefs.HasKey("PlayerStats")){
             string json = PlayerPrefs.GetString("PlayerStats");
             myStats = JsonUtility.FromJson<WorldStats>(json);
         }
         else{
             myStats = new WorldStats(); // Default if no data is saved
         }
     }

}