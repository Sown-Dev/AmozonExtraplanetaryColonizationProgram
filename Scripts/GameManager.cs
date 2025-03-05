using System;
using System.Collections;
using System.Collections.Generic;
using NewRunMenu;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour{
    public static GameManager Instance;


    public Character selectedChar;
    public Character[] allCharacters;

    public WorldStats myStats;

    public PauseManager pauseManager;

    public static bool DevMode;

    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
            return;
        }

        CharacterSelectScreen();

        LoadStats();
        InvokeRepeating(nameof(SaveStats), 300f, 300f);
        
        StartCoroutine(PreloadLocalization());

        
        //steamworks

#if STEAMWORKS1
        try{
            Steamworks.SteamClient.Init(3305330);
        }
        catch (Exception e){
            Debug.LogError(e);
        }

        Debug.Log($"Steamworks Connected, username is {Steamworks.SteamClient.Name}");
#endif


    }
    
    private IEnumerator PreloadLocalization() {
        yield return LocalizationSettings.InitializationOperation;

        yield return LocalizationSettings.StringDatabase.PreloadTables("tutorial"); // Preload your table name here

        Debug.Log("Localization Preloaded!");
    }


    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            pauseManager.Toggle();
        }

        if (Input.GetKeyDown(KeyCode.F11)){
            StartRun();
        }

        if (Input.GetKeyDown(KeyCode.F10)){
            GC.Collect();
        }

#if STEAMWORKS1
        Steamworks.SteamClient.RunCallbacks();

        if (Input.GetKeyDown(KeyCode.F8)){
            ClearAllAchievements();
        }

        if (Input.GetKeyDown(KeyCode.F9)){
            Steamworks.SteamClient.Shutdown();
        }
#endif
    }

    public List<string> Achievements = new List<string>();


    public void CharacterSelectScreen(){
        SceneManager.LoadScene("Scenes/Run Start");
    }

    public void StartRun(){
        SceneManager.LoadScene("Game");
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

    private void OnApplicationQuit(){
        SaveStats();
        //close steamworks
#if STEAMWORKS1
        Steamworks.SteamClient.Shutdown();
#endif
    }

    //Steamworks helper functions

    public bool IsAchievementUnlocked(string id){
#if STEAMWORKS1
        var ac = new Steamworks.Data.Achievement(id);
        return ac.State;
#endif
        return false;
    }

    public void UnlockAchievement(string id){
#if STEAMWORKS1
        var ac = new Steamworks.Data.Achievement(id);
        ac.Trigger();

        Debug.Log($"Achievement {ac.Name} is now {ac.State}");
#endif
    }

    public void ClearAchievement(string id){
#if STEAMWORKS1
        var ac = new Steamworks.Data.Achievement(id);
        ac.Clear();
        Debug.Log($"Cleared achievement {ac.Name}");
#endif
    }

    public void ClearAllAchievements(){
        foreach (var achievement in Achievements){
            ClearAchievement(achievement);
        }
    }
}