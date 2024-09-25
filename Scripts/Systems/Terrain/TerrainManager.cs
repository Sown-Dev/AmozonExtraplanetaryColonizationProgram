using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.Block;
using Systems.Items;
using Systems.Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Terrain = Systems.Terrain.Terrain;

public partial class TerrainManager : MonoBehaviour{
    public static TerrainManager Instance;

[Header("Manager Stuff:")]
    private Layer<Block> blockLayer;
    private Layer<Terrain> terrainLayer;
    private Layer<Ore> oreLayer;
    private List<TickingBlock> tickingBlocks;

    [SerializeField] private Tilemap terrainTilemap;
    [SerializeField] private Tilemap oreTilemap;
        [SerializeField] private Tilemap blockTilemap;
        [SerializeField] private Tilemap wallTilemap;


    [SerializeField] private GameObject blockDebrisPrefab;

    private void Awake(){
        ticks = 0;
        Instance = this;
        blockLayer = new Layer<Block>();
        terrainLayer = new Layer<Terrain>();
        oreLayer = new Layer<Ore>();
        tickingBlocks = new List<TickingBlock>();


        QuantumContainerBlock.InitContainers(); //cant think of a better place to put this
    }

    private void Start(){
        GenerateTerrain();
        calculateStats();
    }

    [CanBeNull]
    public Block GetBlock(Vector2Int pos){
        return blockLayer.Get(pos);
    }

    [CanBeNull]
    public Terrain GetTerrain(Vector2Int pos){
        return terrainLayer.Get(pos);
    }

    public void SetWall(RuleTile tile, Vector3Int pos){
        wallTilemap.SetTile(pos,tile);
    }
    
    public bool IsWall(Vector3Int pos){
        return wallTilemap.GetTile(pos) != null;
    }

    public void DestroyWall(Vector3Int pos){
        if(!IsWall(pos))
            return;
        wallTilemap.SetTile(pos,null);
        Instantiate(blockDebrisPrefab, pos, Quaternion.identity);

    }

    public bool PlaceBlock(Block blockPrefab, Vector2Int position, Orientation rot = Orientation.Up){
        int sizex = blockPrefab.properties.size.x;
        int sizey = blockPrefab.properties.size.y;
        if (sizex < 1 || sizex > 32 || sizey < 1 || sizey > 32){
            return false;
        }

        List<Vector2Int> positions = new List<Vector2Int>();

        Vector2Int offset = new(Mathf.FloorToInt(sizex / 2f), Mathf.FloorToInt(sizey / 2f));

        //this dumb piece of shit code is to offset the block correctly based on its dimensions since odd and even blocks have completely different offsets (center, bottom left corner)

        if (sizex % 2 == 0){ //if x even
            for (int i = 0; i < sizex; i++){
                // y check
                if (sizey % 2 == 0){ //if y even
                    for (int j = 0; j < sizey; j++){
                        Vector2Int pos = new Vector2Int(position.x + i, position.y + j);
                        if (terrainLayer.Get(pos) !=null && blockLayer.Get(pos) == null && wallTilemap.GetTile((Vector3Int)pos) ==null){
                            //TODO: POSSIBLY ADD AIR BLOCK THAT CAN HOLD ITEMS SO U CAN PUT STUFF ON GROUND
                            positions.Add(pos);
                        }
                        else{
                            return false;
                        }
                    }
                }
                else{ //if y odd
                    for (int j = -offset.y; j <= offset.y; j++){
                        Vector2Int pos = new Vector2Int(position.x + i, position.y + j);
                        if (terrainLayer.Get(pos) !=null && blockLayer.Get(pos) == null && wallTilemap.GetTile((Vector3Int)pos) ==null){
                            positions.Add(pos);
                        }
                        else{
                            return false;
                        }
                    }
                }
            }
        }
        else{ //if x odd
            for (int i = -offset.x; i <= offset.x; i++){
                //same block as before
                if (sizey % 2 == 0){ //if y even
                    for (int j = 0; j < sizey; j++){
                        Vector2Int pos = new Vector2Int(position.x + i, position.y + j);
                        if (terrainLayer.Get(pos) != null && blockLayer.Get(pos) == null && wallTilemap.GetTile((Vector3Int)pos) ==null){
                            //TODO: POSSIBLY ADD AIR BLOCK THAT CAN HOLD ITEMS SO U CAN PUT STUFF ON GROUND
                            positions.Add(pos);
                        }
                        else{
                            return false;
                        }
                    }
                }
                else{ //if y odd
                    for (int j = -offset.y; j <= offset.y; j++){
                        Vector2Int pos = new Vector2Int(position.x + i, position.y + j);
                        if (terrainLayer.Get(pos) != null && blockLayer.Get(pos) == null && wallTilemap.GetTile((Vector3Int)pos) ==null){
                            positions.Add(pos);
                        }
                        else{
                            return false;
                        }
                    }
                }
            }
        }


        Vector3 spawnPos =
            (Vector2)position + new Vector2(sizex % 2 == 0 ? sizex / 4f : 0, sizey % 2 == 0 ? sizey / 4f : 0);

        Block block = Instantiate(blockPrefab.gameObject, spawnPos, Quaternion.identity).GetComponent<Block>();
        block.Init(rot);

        block.origin = position; //could make this part of init, but who cares

        if (blockPrefab is TickingBlock){
            tickingBlocks.Add((TickingBlock)block);
        }


        foreach (Vector2Int pos in positions){
            blockLayer.Set(pos, block);
            blockTilemap.SetTile((Vector3Int)pos, block.tile ?? null);
        }

        return true;
    }
    
    public void SetTerrain(Vector2Int pos, TerrainProperties t){
        SetTerrain(pos, t.terrain);
    }

    public void SetTerrain(Vector2Int pos, Terrain terrain){
        //Debug.Log("Setting terrain at " + pos);
        terrainTilemap.SetTile((Vector3Int)pos, terrain.myProperties.tile);
        Terrain t = new Terrain(terrain.myProperties);
        
        terrainLayer.Set(pos,t);
    }


    public bool RemoveBlock(Vector2Int pos, bool dropItems = true){
        Block block = blockLayer.Get(pos);
        if (block == null){
            return false;
        }

        if (!block.BlockDestroy(dropItems))
            return false;


        blockLayer.Remove(pos);
        if(block.tile !=null){
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

    public void SetOre(Vector2Int pos, Ore ore, int amount = -1){
        oreTilemap.SetTile((Vector3Int)pos, ore.tile);

        Ore o = ore.Clone();
        oreLayer.Set(pos, o);

        if (amount > 0){
            o.amount = amount;
        }
        else
            o.amount = 100;
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


    /// <summary>
    /// Gets Adjascent block based on origin and size, getting all blocks along perimiter
    /// </summary>
    public List<Block> GetAdjacentBlocks(Vector2Int position, int sizex, int sizey){
        List<Block> adjacentBlocks = new List<Block>();

        // Calculate the starting points (min) and end points (max) for the block's position
        // either do corner if even or center if odd
        int minX = (sizex % 2 == 0) ? position.x : position.x - Mathf.FloorToInt(sizex / 2);
        int maxX = (sizex % 2 == 0) ? position.x + sizex - 1 : position.x + Mathf.FloorToInt(sizex / 2);
        int minY = (sizey % 2 == 0) ? position.y : position.y - Mathf.FloorToInt(sizey / 2);
        int maxY = (sizey % 2 == 0) ? position.y + sizey - 1 : position.y + Mathf.FloorToInt(sizey / 2);

        // Add blocks on the top and bottom edges
        for (int x = minX; x <= maxX; x++){
            Vector2Int topPosition = new Vector2Int(x, maxY + 1);
            Vector2Int bottomPosition = new Vector2Int(x, minY - 1);
            AddBlockIfNotPresent(adjacentBlocks, topPosition);
            AddBlockIfNotPresent(adjacentBlocks, bottomPosition);
        }

        // Add blocks on the left and right edges
        for (int y = minY; y <= maxY; y++){
            Vector2Int leftPosition = new Vector2Int(minX - 1, y);
            Vector2Int rightPosition = new Vector2Int(maxX + 1, y);
            AddBlockIfNotPresent(adjacentBlocks, leftPosition);
            AddBlockIfNotPresent(adjacentBlocks, rightPosition);
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

    //------------------TICK LOGIC--------------------------------
    
    public static ulong ticks = 0;
    private float tickTimer;
    private float tickTime = 1 / 20f;

    public List<PowerGrid> powerGrids = new List<PowerGrid>();

    private void Update(){
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickTime){
            tickTimer -= tickTime;
            ticks++;
            foreach (TickingBlock tickingBlock in tickingBlocks){

                tickingBlock.Tick();
                /*try{
                    tickingBlock.Tick();
                }
                catch (Exception e){
                    Debug.LogError(e);
                }*/
            }

            foreach (PowerGrid powerGrid in powerGrids){
                powerGrid.GridTick();
            }
        }
    }
}