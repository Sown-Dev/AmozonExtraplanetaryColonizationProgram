using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using Systems.Block;
using Systems.Items;
using Systems.Terrain;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Terrain = Systems.Terrain.Terrain;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public partial class TerrainManager : MonoBehaviour{
    public static TerrainManager Instance;

    [Header("Manager Stuff:")] private Layer<Block> blockLayer;
    private Layer<Terrain> terrainLayer;
    private Layer<Ore> oreLayer;
    private List<TickingBlock> tickingBlocks;
    public Dictionary<Vector2Int, IPowerConnector> powerClaims = new Dictionary<Vector2Int, IPowerConnector>();
    
    public Dictionary<string, TerrainProperties> terrainProperties = new Dictionary<string, TerrainProperties>();

    
    [Header("References")]
    [SerializeField] private Tilemap terrainTilemap;
    [SerializeField] private Tilemap oreTilemap;
    [SerializeField] private Tilemap blockTilemap;
    [SerializeField] private Tilemap wallTilemap;
    
    [SerializeField] private AudioSource audioSource;


    [SerializeField] private GameObject blockDebrisPrefab;

    private void Awake(){
        totalTicksElapsed = 0;
        Instance = this;
        blockLayer = new Layer<Block>();
        terrainLayer = new Layer<Terrain>();
        oreLayer = new Layer<Ore>();
        tickingBlocks = new List<TickingBlock>();
        powerClaims = new Dictionary<Vector2Int, IPowerConnector>();
        
        // Load terrain properties
       var  allProperties = Resources.LoadAll<TerrainProperties>("Terrain");
        
       terrainProperties = allProperties.ToDictionary(p => p.name, p => p);


        QuantumContainerBlock.InitContainers(); //cant think of a better place to put this
    }

    private void Start(){
        if (!GameManager.Instance.currentWorld.generated){
            //generate world
            GenerateWorld();
            GameManager.Instance.Save();
        }
        else{
            //load world
            LoadWorld();
        }

        calculateStats();
        InitializeSunCurve();
    }

    [CanBeNull]
    public Block GetBlock(Vector2Int pos){
        return blockLayer.Get(pos);
    }

    [CanBeNull]
    public Terrain GetTerrain(Vector2Int pos){
        return terrainLayer.Get(pos);
    }
    
    public TerrainProperties GetTerrainProperties(Vector2Int pos){
        Terrain t = GetTerrain(pos);
        if (t == null)
            return null;
        return terrainProperties[t.myProperties];
    }
    public TerrainProperties GetTerrainProperties(string name){
        if (terrainProperties.ContainsKey(name))
            return terrainProperties[name];
        return null;
    }

    public void SetWall(RuleTile tile, Vector3Int pos){
        if (GetBlock((Vector2Int)pos) != null)
            return;
        wallTilemap.SetTile(pos, tile);
    }

    public bool IsWall(Vector3Int pos){
        return wallTilemap.GetTile(pos) != null;
    }

    public void DestroyWall(Vector3Int pos){
        if (!IsWall(pos))
            return;
        wallTilemap.SetTile(pos, null);
        Instantiate(blockDebrisPrefab, pos, Quaternion.identity);
    }

    
    public void SetTerrain(Vector2Int pos, string properties){

        SetTerrain(pos, terrainProperties[properties]);
    }

    public void SetTerrain(Vector2Int pos, TerrainProperties properties){
        Terrain t = new Terrain(properties);

        SetTerrain(pos, t);
    }

    public void SetTerrain(Vector2Int pos, Terrain terrain){
        Vector3Int position3D = (Vector3Int)pos;
        
       TerrainProperties p = terrainProperties[terrain.myProperties];
        
        _terrainTileBuffer[position3D] = p.tile;
        terrainLayer.Set(pos, terrain);
    }


    //NEW buffered terrain generation
    private Dictionary<Vector3Int, TileBase> _terrainTileBuffer = new Dictionary<Vector3Int, TileBase>();


    public void SetOre(Vector2Int pos, OreProperties props, int amount = -1){
        Vector3Int position3D = (Vector3Int)pos;


        var ore = new Ore(props, pos, amount);
        oreLayer.Set(pos, ore);
        oreTilemap.SetTile((Vector3Int)pos, props.tile);
    }


    private void ApplyBufferedTiles(){
        // Apply terrain tiles
        var terrainPositions = _terrainTileBuffer.Keys.ToArray();
        var terrainTiles = terrainPositions.Select(p => _terrainTileBuffer[p]).ToArray();
        terrainTilemap.SetTiles(terrainPositions, terrainTiles);
    }


    public bool RemoveBlock(Vector2Int pos, bool dropItems = true){
        Block block = blockLayer.Get(pos);
        if (block == null){
            return false;
        }

        if (!block.BlockDestroy(dropItems))
            return false;


        if (block.tile != null){
            blockTilemap.SetTile((Vector3Int)pos, null);
            //remove adjascent tiles
        }


        CreateBlockDebris(block.transform.position, block.baseColor);

        if (block is TickingBlock){
            tickingBlocks.Remove((TickingBlock)block);
        }

        return true;
    }

    public ParticleSystem CreateBlockDebris(Vector2 pos, Color color){
        ParticleSystem ps = blockDebrisPrefab.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule psMain = ps.main;
        psMain.startColor = color;

        return Instantiate(blockDebrisPrefab, pos, Quaternion.identity).GetComponent<ParticleSystem>();
    }


    public Ore GetOre(Vector2Int pos){
        return oreLayer.Get(pos);
    }

    public ItemStack ExtractOre(Vector2Int pos, int oreAmt){
        Ore ore = GetOre(pos);
        if (ore == null){
            return null;
        }

        ItemStack stack = ore.ExtractOre(oreAmt);
        if (ore.amount <= 0){
            oreTilemap.SetTile((Vector3Int)pos, null);
            oreLayer.Remove(pos);
        }

        if (stack.amount > 0)
            CreateBlockDebris(pos, new Color(0.3f, 0.25f, 0.2f));

        return stack;
    }

    public bool PlaceBlock(Block blockPrefab, Vector2Int position,  Orientation rot = Orientation.Up,BlockData data=null){
        //Debug.Log("Placing block w rot" + rot);
        int sizex = blockPrefab.properties.size.x;
        int sizey = blockPrefab.properties.size.y;

        if ((rot == Orientation.Left || rot == Orientation.Right) && blockPrefab.properties.rotatable){
            //swap dimensions
            (sizex, sizey) = (sizey, sizex);
        }

        if (sizex < 1 || sizex > 32 || sizey < 1 || sizey > 32){
            return false;
        }

        // Get the block positions using the new helper function
        List<Vector2Int> positions = GetBlockPositions(position, sizex, sizey); //don't use rotation here since we already swapped dimensions

        // Check if all the positions are valid (e.g., not overlapping with other blocks or tiles)
        foreach (Vector2Int pos in positions){
            if ( /*terrainLayer.Get(pos) != null && */
                blockLayer.Get(pos) == null && wallTilemap.GetTile((Vector3Int)pos) == null) //remove terrain check bc its pointless
            {
                // Valid position, continue
            }
            else{
                return false; // Invalid position, abort placement
            }
        }

        // Calculate the spawn position (with adjustment for even-sized blocks)
        Vector3 spawnPos = (Vector2)position + new Vector2(
            sizex % 2 == 0 ? (sizex / 2f) - 0.5f : 0,
            sizey % 2 == 0 ? (sizey / 2f) - 0.5f : 0
        );
        // Instantiate the block
        Block block;
        GameObject blockGO = null;
        try{
            blockGO = Instantiate(blockPrefab.gameObject, spawnPos, Quaternion.identity);
        }
        catch (Exception e){
            Debug.LogError($"Failed to instantiate block {blockPrefab.name}: {e}");
        }

        block = blockGO?.GetComponent<Block>();
    
        
        if(data != null){
            block.Load(data); // Load the block data if provided
        }
        else{
            block.InitializeData();
        }
        block.properties.size = new Vector2Int(sizex, sizey); // Set the block's size incase rotated

        block.Init(rot);

        block.data.origin = position; // Set the block's origin
        
        
        //sound effect
        if( data == null && GameManager.Instance.currentWorld.generated){
           audioSource.transform.position = spawnPos;
           audioSource.Play();
        }


        // Handle ticking blocks
        if (blockPrefab is TickingBlock){
            tickingBlocks.Add((TickingBlock)block);
        }

        // Place the block in the blockLayer and blockTilemap
        foreach (Vector2Int pos in positions){
            blockLayer.Set(pos, block);
            blockTilemap.SetTile((Vector3Int)pos, block.tile ?? null);
        }

        return true;
    }


    /// <summary>
    /// Gets the positions for placing a block based on its size and position.
    /// </summary>
    public List<Vector2Int> GetBlockPositions(Vector2Int position, int sizex, int sizey, Orientation rot = Orientation.Up){
        List<Vector2Int> positions = new List<Vector2Int>();

        // Calculate offset based on the block size (handles even and odd block sizes)
        Vector2Int offset = new(Mathf.FloorToInt(sizex / 2f), Mathf.FloorToInt(sizey / 2f));

        // Calculate block positions based on size and offset
        if (sizex % 2 == 0) // if x size is even
        {
            for (int i = 0; i < sizex; i++){
                if (sizey % 2 == 0) // if y size is even
                {
                    for (int j = 0; j < sizey; j++){
                        positions.Add(new Vector2Int(position.x + i, position.y + j));
                    }
                }
                else // if y size is odd
                {
                    for (int j = -offset.y; j <= offset.y; j++){
                        positions.Add(new Vector2Int(position.x + i, position.y + j));
                    }
                }
            }
        }
        else // if x size is odd
        {
            for (int i = -offset.x; i <= offset.x; i++){
                if (sizey % 2 == 0) // if y size is even
                {
                    for (int j = 0; j < sizey; j++){
                        positions.Add(new Vector2Int(position.x + i, position.y + j));
                    }
                }
                else // if y size is odd
                {
                    for (int j = -offset.y; j <= offset.y; j++){
                        positions.Add(new Vector2Int(position.x + i, position.y + j));
                    }
                }
            }
        }

        return positions.RotateList(rot, position).ToList();
    }


    /// <summary>
    /// Calculates the min and max x, y positions for a block based on origin and size.
    /// </summary>
    public (int minX, int maxX, int minY, int maxY) GetBlockBounds(Vector2Int position, int sizex, int sizey){
        // Calculate the min and max X positions based on block size and position
        int minX = (sizex % 2 == 0) ? position.x : position.x - Mathf.FloorToInt(sizex / 2);
        int maxX = (sizex % 2 == 0) ? position.x + sizex - 1 : position.x + Mathf.FloorToInt(sizex / 2);

        // Calculate the min and max Y positions based on block size and position
        int minY = (sizey % 2 == 0) ? position.y : position.y - Mathf.FloorToInt(sizey / 2);
        int maxY = (sizey % 2 == 0) ? position.y + sizey - 1 : position.y + Mathf.FloorToInt(sizey / 2);

        return (minX, maxX, minY, maxY);
    }

    /// <summary>
    /// Gets Adjacent positions (Vector2Int) based on origin and size, getting all positions along the perimeter.
    /// </summary>
    public Vector2Int[] GetAdjacentPositions(Vector2Int position, int sizex, int sizey){
        List<Vector2Int> adjacentPositions = new List<Vector2Int>();

        // Use the helper to get the block's boundaries
        var (minX, maxX, minY, maxY) = GetBlockBounds(position, sizex, sizey);

        // Add positions on the top and bottom edges
        for (int x = minX; x <= maxX; x++){
            Vector2Int topPosition = new Vector2Int(x, maxY + 1);
            Vector2Int bottomPosition = new Vector2Int(x, minY - 1);
            adjacentPositions.Add(topPosition);
            adjacentPositions.Add(bottomPosition);
        }

        // Add positions on the left and right edges
        for (int y = minY; y <= maxY; y++){
            Vector2Int leftPosition = new Vector2Int(minX - 1, y);
            Vector2Int rightPosition = new Vector2Int(maxX + 1, y);
            adjacentPositions.Add(leftPosition);
            adjacentPositions.Add(rightPosition);
        }

        return adjacentPositions.ToArray();
    }


    /// <summary>
    /// Gets Adjacent blocks based on the positions returned from GetAdjacentPositions method.
    /// </summary>
    public List<Block> GetAdjacentBlocks(Vector2Int position, int sizex, int sizey){
        List<Block> adjacentBlocks = new List<Block>();

        // Get the adjacent positions using the new method
        Vector2Int[] adjacentPositions = GetAdjacentPositions(position, sizex, sizey);

        // Iterate through the positions and add the corresponding blocks
        foreach (Vector2Int adjacentPosition in adjacentPositions){
            AddBlockIfNotPresent(adjacentBlocks, adjacentPosition);
        }

        return adjacentBlocks;
    }


// Helper method to add a block to the list if it's not already present
    private void AddBlockIfNotPresent(List<Block> blocks, Vector2Int position){
        Block block = GetBlock(position); // Assuming this retrieves a block based on its grid position
        if (block != null && !blocks.Contains(block)){
            blocks.Add(block);
        }
    }


    public void BlockLayerRemove(Vector2Int pos){
        blockLayer.Remove(pos);
    }

    //------------------TICK LOGIC--------------------------------

    public ulong totalTicksElapsed = 0;
    private float tickTimer;
    private float tickTime = 1 / 20f;

    public List<PowerGrid> powerGrids = new List<PowerGrid>();

    private void Update(){
        if (generatedWorld){
            generatedWorld = false;
            //log total time it took
            float endTime = Time.realtimeSinceStartup;
            Debug.Log("World generated in " + (endTime - startTime) + " seconds");
#if UNITY_EDITOR
            //add to file
            System.IO.File.AppendAllText("worldgenlog.txt", $"World of size {worldSize} and seed {currentSeed} generated in {(endTime - startTime)} seconds\n");
#endif
        }

        UpdateTime();

        tickTimer += Time.deltaTime;
        if (tickTimer >= tickTime){
            tickTimer -= tickTime;
            totalTicksElapsed++;
            bool resetActuate = totalTicksElapsed % 2 == 0;
            if (resetActuate){
                foreach (TickingBlock tickingBlock in tickingBlocks.ToList()){
                    tickingBlock.ResetActuated(); //can prob add an empty interface to only check things that need to be reset
                }
            }

            foreach (TickingBlock tickingBlock in tickingBlocks.ToList()){
                //tolist could be very bad. idk the performance impact, but need some way otherwise to destroy blocks at end frame

                //tickingBlock.Tick();
                try{
                    tickingBlock.Tick();
                }
                catch (Exception e){
                    Debug.LogWarning(e);
                }
            }

            foreach (PowerGrid powerGrid in powerGrids){
                powerGrid.GridTick();
            }
        }
    }


    private void OnDrawGizmos(){
        Gizmos.color = Color.magenta;

        foreach (var pair in powerClaims != null ? powerClaims : new Dictionary<Vector2Int, IPowerConnector>()){
           // Debug.Log("Drawing power claim for" + pair.Value.myBlock.name);
            Gizmos.DrawLine((Vector2)pair.Key, (Vector2)pair.Value.myBlock.data.origin);
        }

        Gizmos.color = Color.green;
        if (blockLayer == null)
            return;
        foreach (var pair in blockLayer.GetDictionary()){
            //Gizmos.DrawLine((Vector2)pair.Key, (Vector2)pair.Value.origin);
        }
    }

    //SAVE/LOAD

    public void SaveWorld(){
        
        GameManager.Instance.currentWorld.blocks.Clear();
        GameManager.Instance.currentWorld.ticksElapsed = totalTicksElapsed; // Save the total ticks elapsed
        // Save blocks
        foreach (var block in blockLayer.GetDictionary().Values){
            block.hasSaved = false; // Reset the hasSaved flag for all blocks
        }


        foreach (var block in blockLayer.GetDictionary().Values){
            if (!block.hasSaved){
                BlockLoadData blockData = new BlockLoadData{
                    data = block.Save(),
                    addressableKey = block.addressableKey, // Save addressable key
                };
                GameManager.Instance.currentWorld.blocks.Add(blockData); // Add to the world's block list
                block.hasSaved = true;
            }
        }

        Debug.Log("Saved Blocks");

        GameManager.Instance.currentWorld.ores.Clear();
        foreach (var pair in oreLayer.GetDictionary()){
            OreData data = new OreData{
                position = pair.Key,
                oreName = pair.Value.myProperties.name, // Store the asset name
                amount = pair.Value.amount
            };
            GameManager.Instance.currentWorld.ores.Add(data);
        }

        // Save terrain
        GameManager.Instance.currentWorld.terrain.Clear();
        foreach (var pair in terrainLayer.GetDictionary()){
            GameManager.Instance.currentWorld.terrain.Add(new TerrainData{
                pos = pair.Key,
                t = pair.Value // Store the asset name
            });
        }


        Debug.Log("Saved Terrain and Ores");


        // Initialize flattened walls array
        int totalCells = GameManager.Instance.currentWorld.worldSize.x * GameManager.Instance.currentWorld.worldSize.y;
        GameManager.Instance.currentWorld.walls = new bool[totalCells];

        // Save walls using 1D index
        int halfX = GameManager.Instance.currentWorld.worldSize.x / 2;
        int halfY = GameManager.Instance.currentWorld.worldSize.y / 2;

        for (int i = -halfX; i < halfX; i++){
            for (int j = -halfY; j < halfY; j++){
                Vector3Int pos = new Vector3Int(i, j, 0);
                bool hasWall = wallTilemap.GetTile(pos) != null;

                // Calculate 1D index
                int xIndex = i + halfX;
                int yIndex = j + halfY;
                int flatIndex = yIndex * GameManager.Instance.currentWorld.worldSize.x + xIndex;

                GameManager.Instance.currentWorld.walls[flatIndex] = hasWall;
            }
        }


        Debug.Log("Saved Terrain");
    }

    public void LoadWorld(){
        totalTicksElapsed = GameManager.Instance.currentWorld.ticksElapsed; // Load the total ticks elapsed

        // Load walls
        int halfX = GameManager.Instance.currentWorld.worldSize.x / 2;
        int halfY = GameManager.Instance.currentWorld.worldSize.y / 2;

        for (int i = -halfX; i < halfX; i++){
            for (int j = -halfY; j < halfY; j++){
                Vector3Int pos = new Vector3Int(i, j, 0);

                // Calculate 1D index
                int xIndex = i + halfX;
                int yIndex = j + halfY;
                int flatIndex = yIndex * GameManager.Instance.currentWorld.worldSize.x + xIndex;

                bool hasWall = GameManager.Instance.currentWorld.walls[flatIndex];
                if (hasWall){
                    SetWall(rockWall, pos);
                }
            }
        }

        Debug.Log("Loaded Walls");


        // Load ores

        foreach (OreData data in GameManager.Instance.currentWorld.ores){
            OreProperties properties = ItemManager.Instance.GetOreProperties(data.oreName);

            if (properties != null){
                SetOre(data.position, properties, data.amount);
            }
        }

        // Load terrain

        foreach (TerrainData data in GameManager.Instance.currentWorld.terrain){
            SetTerrain(data.pos, data.t);
        }

        ApplyBufferedTiles();


        //load blocks
        int errors = 0;
        foreach (BlockLoadData blockData in GameManager.Instance.currentWorld.blocks){

            try{
                string key = blockData.addressableKey;

                // Load as GameObject first
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);

                GameObject prefabObj = handle.WaitForCompletion();

                if (prefabObj != null){
                    // Get Block component from prefab
                    Block blockPrefab = prefabObj.GetComponent<Block>();

                    if (blockPrefab != null){
                        Vector2Int pos = blockData.data.origin;
                        if (GetBlock(pos) != null){
                            Debug.LogWarning($"Block exists at {pos}");
                            continue;
                        }

                        PlaceBlock(blockPrefab, pos, blockData.data.rotation, blockData.data); // Place the block at the specified position
                        Block block = GetBlock(pos);
                        if (blockPrefab == null){
                            Debug.LogError("prefab is null");
                        }

                        if (block == null){
                            Debug.LogError("block is null");
                        }

                        block.Load(blockData.data); // Load the block data
                        
                    }
                    else{
                        Debug.LogError($"Prefab {key} is missing Block component");
                    }
                }
                else{
                    Debug.LogError($"Failed to load Addressable: {key}");
                }
            }catch (Exception e){
                Debug.LogError($"Error loading block with key {blockData.addressableKey}: {e}");
                errors++;
            }
        }

        Debug.Log($"Loaded {GameManager.Instance.currentWorld.blocks.Count} blocks with {errors} errors");
    }
}