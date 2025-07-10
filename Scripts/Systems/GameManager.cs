#define UNITYSERIALIZATION0

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NewRunMenu;
using Systems.Block;
using Systems.Items;
using Systems.Round;
using Systems.Terrain;
using UI.BlockUI;
using Unity.VisualScripting;
using UnityEditor;
using MemoryPack;
using UI; // Add this at the top
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
using Terrain = Systems.Terrain.Terrain;


public class GameManager : MonoBehaviour{
    public static GameManager Instance;


    public Character selectedChar;
    public Character[] allCharacters;

    [FormerlySerializedAs("myStats")] public WorldMetrics myMetrics = new WorldMetrics(); 

    //metrics for the current run
    [HideInInspector]
    [DoNotSerialize]
    public WorldMetrics runMetrics;


    public Vector4 windowMargin = new Vector4(0, 0, 0, 60);

    public GameData gameData;

    [HideInInspector] [DoNotSerialize] public List<World> worlds = new List<World>();
    //logic to know if we're creating a new world or loading one

    //make into property to avoid unity serialization issues
    private World _currentWorld;
    public GameSettings settings;


    [Header("References")] [SerializeField]
    private CanvasGroup saveIconCG;

    public PauseManager pauseManager;
    public UIWindow settingsWindow;

    // Track asynchronous saves
    [HideInInspector]
    [DoNotSerialize]
    private Coroutine saveCoroutine;
    [HideInInspector]
    [DoNotSerialize]
    private CancellationTokenSource saveCancellation;
    [HideInInspector]
    [DoNotSerialize]
    private bool isSaving;
    public bool IsSaving => isSaving;

    [HideInInspector]
    [DoNotSerialize]
    public World currentWorld{
        get{ return _currentWorld; }
        set{ _currentWorld = value; }
    }

    public bool inGame;


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
        saveIconCG.alpha = 0;


        LoadStats();
        InvokeRepeating(nameof(SaveStats), 300f, 300f);

        StartCoroutine(PreloadLocalization());


        //load settings
        if (PlayerPrefs.HasKey("GameSettings")){
            byte[] bytes = Convert.FromBase64String(PlayerPrefs.GetString("GameSettings"));
            settings = MemoryPackSerializer.Deserialize<GameSettings>(bytes);
        }
        else{
            settings = new GameSettings();
        }

        worlds = new List<World>();
        //load all worlds from PlayerPrefs
        try{
            if (PlayerPrefs.HasKey("AllWorlds")){
                byte[] listBytes = Convert.FromBase64String(PlayerPrefs.GetString("AllWorlds"));
                var worldNames = MemoryPackSerializer.Deserialize<List<string>>(listBytes);
                if (worldNames != null){
                    Debug.Log($"Loaded {worldNames.Count} world names from PlayerPrefs");

                    foreach (var name in worldNames){
                        if (PlayerPrefs.HasKey(name)){
                            byte[] worldBytes = Convert.FromBase64String(PlayerPrefs.GetString(name));
                            World world = MemoryPackSerializer.Deserialize<World>(worldBytes);

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

        //load gamedata
        if (PlayerPrefs.HasKey("GameData")){
            byte[] gbytes = Convert.FromBase64String(PlayerPrefs.GetString("GameData"));
            gameData = MemoryPackSerializer.Deserialize<GameData>(gbytes);
        }
        else{
            gameData = new GameData(); // Default if no data is saved
        }


        //steamworks

#if STEAMWORKS1
        try{
            Steamworks.SteamClient.Init(3305330);
            SyncStatsWithSteam();

        }
        catch (Exception e){
            Debug.LogError(e);
        }

        Debug.Log($"Steamworks Connected, username is {Steamworks.SteamClient.Name}");
#endif

        LoadTitleScreen();
    }

    public void Start(){
        settingsWindow.Hide();


        //stupid piece of code to disable dev mode unless we have the directive
        bool disableDevMode = true;
#if ALLITEMS1
        disableDevMode = false;
#endif
        if (disableDevMode){
            settings.DevMode = false;
        }
    }

    private IEnumerator PreloadLocalization(){
        yield return LocalizationSettings.InitializationOperation;

        yield return LocalizationSettings.StringDatabase.PreloadTables("tutorial"); // Preload your table name here

        Debug.Log("Localization Preloaded!");
    }

    public void Quit(){
        //this is a bit iffy
        if (TerrainManager.Instance != null){
            if(isSaving) CancelSave();
            SaveImmediate();
        }

        Application.Quit();
    }

    public void NewRunMenu(){
        inGame = false;
        CreateNewWorld();
        SceneManager.LoadScene("Scenes/Run Start");
    }

    public void LoadTitleScreen(){
        inGame = false;
        SceneManager.LoadScene("Scenes/Titlescreen");
    }

    public void ExitToMain(bool save = true){
        if(isSaving) CancelSave();

        if (TerrainManager.Instance != null && save){
            SaveImmediate();
        }
        else{
            SyncStatsWithSteam();
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
        ResetRunMetrics();
        SceneManager.LoadScene("Game");
    }

    public void CreateNewWorld(){
        Random.InitState((int)System.DateTime.Now.Ticks);
        inGame = true;
        currentWorld = new World(Random.Range(-100000, 100000));
        //currentWorld.name = "Kepler-" + (char) Random.Range(97,122) + Random.Range(0, 99);

        if (selectedChar)
            currentWorld.playerCharacter = selectedChar.name;
        currentWorld.oreProperties = ItemManager.Instance.allOres.Select(ore => ore.name).ToArray();
    }

    public void LoadWorld(World world){
        inGame = true;
        currentWorld = world;
        Debug.Log($"loading world: {currentWorld.name}");
        SceneManager.LoadScene("Game");
    }

    public Character GetCharacter(string name){
        return allCharacters.FirstOrDefault(c => c.name == name);
    }


    public IEnumerator SaveCR(){
        isSaving = true;
        saveIconCG.alpha = 1;
        saveCancellation = new CancellationTokenSource();
        byte[] settingsBytes = MemoryPackSerializer.Serialize(settings);
        PlayerPrefs.SetString("GameSettings", Convert.ToBase64String(settingsBytes));


        SaveStats();

        Debug.Log("Saving World");

        if (Player.Instance)
            currentWorld.playerData = Player.Instance.SavePlayer();

        if (RoundManager.Instance)
            currentWorld.roundData = RoundManager.Instance.SaveRoundData();

        yield return StartCoroutine(TerrainManager.Instance.SaveWorldCR());
        if(saveCancellation.IsCancellationRequested){
            CleanupSavingState();
            yield break;
        }

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


        // Save the current world to PlayerPrefs
        System.Threading.Tasks.Task<byte[]> serializeTask = System.Threading.Tasks.Task.Run(() => MemoryPackSerializer.Serialize(currentWorld), saveCancellation.Token);

        while(!serializeTask.IsCompleted){
            if(saveCancellation.IsCancellationRequested){
                CleanupSavingState();
                yield break;
            }
            yield return null;
        }
        byte[] worldBytes;
        try{
            worldBytes = serializeTask.Result;
        }catch(AggregateException ae) when(ae.InnerException is OperationCanceledException){
            CleanupSavingState();
            yield break;
        }
        PlayerPrefs.SetString(currentWorld.name, Convert.ToBase64String(worldBytes));


#if UNITY_STANDALONE_WIN && SAVEWORLDTOFILE
        string filePath = Path.Combine(Application.persistentDataPath, "WorldSave.txt");
        Exception fileException = null;
        var fileTask = System.Threading.Tasks.Task.Run(() => {
            try{
                if(!saveCancellation.IsCancellationRequested)
                    File.WriteAllBytes(filePath, worldBytes);
            }
            catch (Exception e){ fileException = e; }
        }, saveCancellation.Token);
        while(!fileTask.IsCompleted){
            if(saveCancellation.IsCancellationRequested){
                CleanupSavingState();
                yield break;
            }
            yield return null;
        }
        if (fileException != null) Debug.LogError($"Failed to save world to text file: {fileException}");
        else if(!saveCancellation.IsCancellationRequested) Debug.Log($"World saved to text file at {filePath}");
#endif

        SaveWorlds();

        Debug.Log("finished saving world");

        CleanupSavingState();
        saveCoroutine = null;
        saveCancellation.Dispose();
        saveCancellation = null;
        yield break;
    }

    public void Save(){
        if(isSaving) return;
        saveCoroutine = StartCoroutine(SaveCR());
    }

    public void CancelSave(){
        if(saveCoroutine != null){
            StopCoroutine(saveCoroutine);
            saveCoroutine = null;
        }
        if(saveCancellation != null){
            saveCancellation.Cancel();
            saveCancellation.Dispose();
            saveCancellation = null;
        }
        CleanupSavingState();
    }

    private void CleanupSavingState(){
        isSaving = false;
        saveIconCG.alpha = 0;
    }

    public void SaveImmediate(){
        isSaving = true;
        saveIconCG.alpha = 1;
        
        byte[] settingsBytes = MemoryPackSerializer.Serialize(settings);
        PlayerPrefs.SetString("GameSettings", Convert.ToBase64String(settingsBytes));

        SaveStats();

        if (Player.Instance)
            currentWorld.playerData = Player.Instance.SavePlayer();

        if (RoundManager.Instance)
            currentWorld.roundData = RoundManager.Instance.SaveRoundData();

        TerrainManager.Instance.SaveWorldImmediate();

        var existingWorld = worlds.FirstOrDefault(w => w.name == currentWorld.name);
        if (existingWorld == null){
            worlds.Add(currentWorld);
        } else {
            int index = worlds.IndexOf(existingWorld);
            worlds[index] = currentWorld;
        }

#if UNITYSERIALIZATION1
        // not used
#endif
        byte[] worldBytes = MemoryPackSerializer.Serialize(currentWorld);
        PlayerPrefs.SetString(currentWorld.name, Convert.ToBase64String(worldBytes));

#if UNITY_STANDALONE_WIN && SAVEWORLDTOFILE
        string filePath = Path.Combine(Application.persistentDataPath, "WorldSave.txt");
        try{
            File.WriteAllBytes(filePath, worldBytes);
            Debug.Log($"World saved to text file at {filePath}");
        } catch (Exception e){
            Debug.LogError($"Failed to save world to text file: {e}");
        }
#endif

        SaveWorlds();
        Debug.Log("finished saving world");

        CleanupSavingState();
    }

    private void SaveWorlds(){
        // Save all world names to PlayerPrefs
        List<string> worldNames = worlds.Select(w => w.name).ToList();
        byte[] listBytes = MemoryPackSerializer.Serialize(worldNames);
        PlayerPrefs.SetString("AllWorlds", Convert.ToBase64String(listBytes));
        PlayerPrefs.Save();
    }


    public void  SaveStats(){
        byte[] bytes = MemoryPackSerializer.Serialize(myMetrics);
        PlayerPrefs.SetString("PlayerStats", Convert.ToBase64String(bytes));

        PlayerPrefs.Save();
        SyncStatsWithSteam();
    }

    public void LoadStats(){
        if (PlayerPrefs.HasKey("PlayerStats")){
            byte[] bytes = Convert.FromBase64String(PlayerPrefs.GetString("PlayerStats"));
            myMetrics = MemoryPackSerializer.Deserialize<WorldMetrics>(bytes);
        }
        else{
            myMetrics = new WorldMetrics(); // Default if no data is saved
        }
        Debug.Log($"Loaded PlayerStats: {myMetrics}");
    }

    public void ResetRunMetrics(){
        runMetrics = new WorldMetrics();
    }

    public void ApplyRunMetrics(){
        if(currentWorld != null){
            currentWorld.worldMetrics += runMetrics;
        }

        runMetrics = new WorldMetrics();
    }

    public void SyncStatsWithSteam(){
#if STEAMWORKS1
        WorldMetrics total = myMetrics;
        Steamworks.SteamUserStats.SetStat("BlocksBroken", total.blocksBroken);
        Steamworks.SteamUserStats.SetStat("BlocksPlaced", total.blocksPlaced);
        Steamworks.SteamUserStats.SetStat("TerrainDestroyed", total.terrainDestroyed);
        Steamworks.SteamUserStats.SetStat("ItemsPickedUp", total.itemsPickedUp);
        Steamworks.SteamUserStats.SetStat("MoneyEarned", total.moneyEarned);
        Steamworks.SteamUserStats.SetStat("DistanceTraveled", (int)total.distanceTraveled);

        Steamworks.SteamUserStats.SetStat("CurrentRunMoney",runMetrics.moneyEarned);
        Steamworks.SteamUserStats.StoreStats();
#endif
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
            if(isSaving) CancelSave();
            SaveImmediate();
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


    public void ToggleSettings(){
        settingsWindow.Toggle();
    }

    public void OpenSettings(){
        settingsWindow.Show();
    }

    public void CloseSettings(){
        settingsWindow.Hide();
    }

    public void LoadScene(Scenum scene){
        switch (scene){
            case Scenum.RunStart:
                SceneManager.LoadScene("Scenes/Run Start");
                break;
            case Scenum.MainMenu:
                SceneManager.LoadScene("Scenes/MainMenu");
                break;
            case Scenum.Titlescreen:
                SceneManager.LoadScene("Scenes/Titlescreen");
                break;
        }
    }
}

public enum Scenum{
    None = -1,
    Titlescreen = 1,
    RunStart = 2,
    MainMenu = 3,
}

[MemoryPackable]
[Serializable]
public partial class World{
    //[JsonProperty] public Guid InstanceId = Guid.NewGuid();

    public string name;
    public int seed;
    public bool generated = false;

    //terrain data
    public ulong ticksElapsed;


    public string playerCharacter; //player character for this world
    public PlayerData playerData; //player data for this world
    public RoundData roundData; //round data for this world
    public WorldMetrics worldMetrics = new WorldMetrics(); //metrics persistent to this world

    public Vector2Int worldSize = new Vector2Int(500, 500); //default size


    //other things:world stats, player info, terrain data
    public List<BlockLoadData> blocks = new List<BlockLoadData>();
    public short[] walls;

    public List<OreData> ores = new List<OreData>(); //list of ores to spawn
    public List<TerrainData> terrain = new List<TerrainData>(); //list of terrain to spawn

    public string[] oreProperties; //list of ore properties to use for spawning


    //flags & planet properties
    public PlanetType planetType = PlanetType.Rocky; //default planet type

    public PlanetFlags flags = PlanetFlags.None;

    static string[] planetNames = new string[]{
        "Kepler", "Proxima", "Pluto", "Gemini", "Bezos", "Leporis", "Gliese", "Upsilon", "Librae", "Resonare", "Tau", "WASP", "Borealis", "Primus", "TESS",
        "CoRoT", "TRAPPIST-1", "Iora", "Gemmora", "APS", "APS2" 
    };

    //needed for deserialization
    public World() {
        worldMetrics = new WorldMetrics();
    }

    //generate world
    public World(int _seed){
        seed = _seed;
        //set random seed, then generate world
        Random.InitState(seed);
        planetType = (PlanetType)Random.Range(0, Enum.GetValues(typeof(PlanetType)).Length); //random planet type

        worldMetrics = new WorldMetrics();

        //planet name
        name = planetNames[Random.Range(0, planetNames.Length)] + "-" + Random.Range(0, 99) + (char)Random.Range(97, 122);

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
[MemoryPackable]
public partial class BlockLoadData{
    public BlockData data;
    public string addressableKey;
}

[Serializable]
[MemoryPackable]
public partial class OreData{
    public Vector2Int position;
    public string oreName; // Name of the OreProperties asset
    public int amount;
}

[Serializable]
[MemoryPackable]
public partial class TerrainData{
    public Terrain t;
    public Vector2Int pos;
}

[Serializable]
[MemoryPackable]
public partial class GameData{
    //level
    public int level;
    public double xp;
    public double maxXp;
}