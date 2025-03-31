#define UNITYSERIALIZATION0

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DefaultNamespace;
using NewRunMenu;
using Systems.Block;
using Systems.Items;
using Systems.Round;
using Systems.Terrain;
using UI.BlockUI;
using Unity.VisualScripting;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization; // Add this at the top
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Terrain = Systems.Terrain.Terrain;


public class GameManager : MonoBehaviour{
    public static GameManager Instance;


    public Character selectedChar;
    public Character[] allCharacters;

    public WorldStats myStats;

    public PauseManager pauseManager;

    public GameSettings settings;

    [HideInInspector] [DoNotSerialize] public List<World> worlds = new List<World>();
    //logic to know if we're creating a new world or loading one

    //make into property to avoid unity serialization issues
    private World _currentWorld;

    [HideInInspector]
    [DoNotSerialize]
    public World currentWorld{
        get{ return _currentWorld; }
        set{ _currentWorld = value; }
    }

    public bool inGame;

    public static JsonSerializerSettings JSONsettings = new JsonSerializerSettings{
        NullValueHandling = NullValueHandling.Include,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

        //DefaultValueHandling = DefaultValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver{
            // Ensure Unity serialization attributes are ignored
            IgnoreSerializableAttribute = true
        }
    };

    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
            return;
        }

        currentWorld = null;
        inGame = false;

        LoadStats();
        InvokeRepeating(nameof(SaveStats), 300f, 300f);

        StartCoroutine(PreloadLocalization());


        worlds = new List<World>();
        //load all worlds from PlayerPrefs
        try{
            if (PlayerPrefs.HasKey("AllWorlds")){
                string json = PlayerPrefs.GetString("AllWorlds");
                var worldNamesWrapper = JsonUtility.FromJson<Wrapper<List<string>>>(json);
                if (worldNamesWrapper != null && worldNamesWrapper.data != null){
                    List<string> worldNames = worldNamesWrapper.data;
                    Debug.Log($"Loaded {worldNames.Count} world names from PlayerPrefs");

                    foreach (var name in worldNames){
                        if (PlayerPrefs.HasKey(name)){
                            string worldJson = PlayerPrefs.GetString(name);
                            //World world = JsonUtility.FromJson<World>(worldJson);
                            World world;
#if UNITYSERIALIZATION1
                        world = JsonUtility.FromJson<World>(worldJson);
#else

                            world = JsonConvert.DeserializeObject<World>(worldJson, JSONsettings);
#endif

                            worlds.Add(world);
                            Debug.Log($"Loaded world: {world.name}");
                        }
                        else{
                            Debug.LogWarning($"World {name} was listed in AllWorlds but does not exist in PlayerPrefs!");
                        }
                    }
                }
            }
        }
        catch (Exception e){
            Debug.LogError($"Failed to load worlds from PlayerPrefs: {e.StackTrace}");
        }


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

        LoadTitleScreen();
    }

    private IEnumerator PreloadLocalization(){
        yield return LocalizationSettings.InitializationOperation;

        yield return LocalizationSettings.StringDatabase.PreloadTables("tutorial"); // Preload your table name here

        Debug.Log("Localization Preloaded!");
    }

    public void Quit(){
        //this is a bit iffy
        if (TerrainManager.Instance != null){
            Save();
        }

        Application.Quit();
    }

    public void NewRunMenu(){
        inGame = false;
        SceneManager.LoadScene("Scenes/Run Start");
    }

    public void LoadTitleScreen(){
        inGame = false;
        SceneManager.LoadScene("Scenes/Titlescreen");
    }

    public void ExitToMain(){
        if (TerrainManager.Instance != null){
            Save();
        }

        LoadTitleScreen();
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (BlockUIManager.Instance?.currentBlockUI == null){
                pauseManager.Toggle();
            }
        }

        if (Input.GetKeyDown(KeyCode.F11)){
            StartNewRun();
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


    public void StartNewRun(){
        Random.InitState((int)System.DateTime.Now.Ticks);
        inGame = true;
        currentWorld = new World(Random.Range(-100000, 100000));
        //currentWorld.name = "Kepler-" + (char) Random.Range(97,122) + Random.Range(0, 99);

        currentWorld.playerCharacter = selectedChar.name;
        currentWorld.oreProperties = ItemManager.Instance.allOres.Select(ore => ore.name).ToArray();

        SceneManager.LoadScene("Game");
    }

    public void LoadWorld(World world){
        inGame = true;
        currentWorld = world;
        Debug.Log("loading world:" + JsonConvert.SerializeObject(currentWorld.playerData, JSONsettings));
        SceneManager.LoadScene("Game");
    }

    public Character GetCharacter(string name){
        return allCharacters.FirstOrDefault(c => c.name == name);
    }


    public void Save(){
        Debug.Log("Saving World");
        SaveStats();

        TerrainManager.Instance.SaveWorld();

        // Check if the current world is already in the list
        var existingWorld = worlds.FirstOrDefault(w => w.name == currentWorld.name);
        if (existingWorld == null){
            // Add new world to the list
            worlds.Add(currentWorld);
        }
        else{
            // Update existing world
            int index = worlds.IndexOf(existingWorld);
            worlds[index] = currentWorld;
        }

        //player save
        if (Player.Instance)
            currentWorld.playerData = Player.Instance.SavePlayer();

        Debug.Log("saved:" + JsonConvert.SerializeObject(currentWorld.playerData, JSONsettings));

        //round save
        if (RoundManager.Instance)
            currentWorld.roundData = RoundManager.Instance.SaveRoundData();

        // Save the current world to PlayerPrefs

        string json = "";
#if UNITYSERIALIZATION1
         json = JsonUtility.ToJson(currentWorld, true);
#else
        json = JsonConvert.SerializeObject(currentWorld, Formatting.Indented, JSONsettings);
#endif
        //string json = JsonUtility.ToJson(currentWorld);
        PlayerPrefs.SetString(currentWorld.name, json);
        try{
            string filePath = Path.Combine(Application.persistentDataPath, "WorldSave.txt");
            string jsonPretty = json;
            //string jsonPretty = json; // Enable pretty print
            File.WriteAllText(filePath, jsonPretty);
            Debug.Log($"World saved to text file at {filePath}");
        }
        catch (Exception e){
            Debug.LogError($"Failed to save world to text file: {e.Message}");
        }

        SaveWorlds();

        Debug.Log("finished saving saved:" + JsonConvert.SerializeObject(currentWorld.playerData, JSONsettings));
    }

    private void SaveWorlds(){
        // Save all world names to PlayerPrefs
        List<string> worldNames = worlds.Select(w => w.name).ToList();
        string json = JsonUtility.ToJson(new Wrapper<List<string>>{ data = worldNames });
        PlayerPrefs.SetString("AllWorlds", json);
        PlayerPrefs.Save();
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

    public bool DeleteWorld(World world){
        if (worlds.Contains(world)){
            worlds.Remove(world);
            PlayerPrefs.DeleteKey(world.name);
            SaveWorlds();
            return true;
        }

        return false;
    }


    private void OnApplicationQuit(){
        //close steamworks
#if STEAMWORKS1
        Steamworks.SteamClient.Shutdown();
#endif
        if (TerrainManager.Instance != null){
            Save();
        }
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

[JsonObject(ItemNullValueHandling = NullValueHandling.Include)]
[Serializable]
public class World{
    //[JsonProperty] public Guid InstanceId = Guid.NewGuid();

    public string name;
    public int seed;
    public bool generated = false;

    //terrain data
    public ulong ticksElapsed;


    public string playerCharacter; //player character for this world
    public PlayerData playerData; //player data for this world
    public RoundData roundData; //round data for this world

    public Vector2Int worldSize = new Vector2Int(500, 500); //default size


    //other things:world stats, player info, terrain data
    public List<BlockLoadData> blocks = new List<BlockLoadData>();
    public bool[] walls;

    public List<OreData> ores = new List<OreData>(); //list of ores to spawn
    public List<TerrainData> terrain = new List<TerrainData>(); //list of terrain to spawn

    public string[] oreProperties; //list of ore properties to use for spawning


    //flags & planet properties
    public PlanetType planetType = PlanetType.Rocky; //default planet type

    public PlanetFlags flags = PlanetFlags.None;


    //generate world
    public World(int _seed){
        seed = _seed;
        //set random seed, then generate world
        Random.InitState(seed);
        planetType = (PlanetType)Random.Range(0, Enum.GetValues(typeof(PlanetType)).Length); //random planet type

        //planet name
        name = "Kepler-" + Random.Range(0, 99) + (char)Random.Range(97, 122);

        //each flag has a 50% chance of being applied
        foreach (PlanetFlags flag in System.Enum.GetValues(typeof(PlanetFlags))){
            if (flag == PlanetFlags.None) continue; // Skip the None flag
            if (Random.value > 0.5f) flags |= flag; // 50% chance to add this flag
        }
    }
}

[Serializable]
public enum PlanetType{
    Rocky,
    GasGiant,
    Tundra,
    Forest,
    Ocean
}

[Serializable]
[Flags]
public enum PlanetFlags{
    None = 0,
    RockCoal = 1,
    HasTin = 2,
    StoneNodes = 4,
}

[Serializable]
public class Wrapper<T>{
    public T data;
}

[Serializable]
public class BlockLoadData{
    public BlockData data;
    public string addressableKey;
}

[Serializable]
public class OreData{
    public Vector2Int position;
    public string oreName; // Name of the OreProperties asset
    public int amount;
}

[Serializable]
public class TerrainData{
    public Terrain t;
    public Vector2Int pos;
}