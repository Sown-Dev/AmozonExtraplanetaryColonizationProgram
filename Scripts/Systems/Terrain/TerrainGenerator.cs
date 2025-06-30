using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Block;
using Systems.Items;
using Systems.Terrain;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Terrain = Systems.Terrain.Terrain;

public partial class TerrainManager{
    [SerializeField] private RuleTile rockWall;
    [SerializeField] private RuleTile iceWall;

    private int currentSeed;

    [FormerlySerializedAs("size")] [SerializeField]
    private int worldSize = 500;

    [Header("Biomes")] [SerializeField] private List<Biome> allBiomes;
    [SerializeField] private List<Biome> biomes;


    [Header("Terrain")] [SerializeField] private TerrainProperties Grass;
    [SerializeField] private TerrainProperties Stone;
    [SerializeField] private TerrainProperties Sand;
    [SerializeField] private TerrainProperties Water;


    [Header("Blocks")] [SerializeField] private Block Sapling;
    [SerializeField] private Block Mushroom;
    [SerializeField] private Block LootCrate;
    [SerializeField] private Block SupplyCrate;


    [SerializeField] private Block Crystal;
    [SerializeField] private Block Tree;
    [SerializeField] private Block CoalNode;
    [SerializeField] private Block rockNode;
    [SerializeField] public List<Block> resourceBlocks;

    [SerializeField] private Block SellBlock;
    [SerializeField] private Block BigCrate;
    [SerializeField] private Block Rock2x;


    // construction sites (unused)
    /*[SerializeField] private Block SolarFurnaceSite;
    [SerializeField] private Block WaterPumpSite;
    [SerializeField] private Block RailWorkbenchSite;*/

    public Dictionary<string, float[][]> worldMaps = new Dictionary<string, float[][]>(); //maps for height, heat, 

    [SerializeField] private Block[] constructionSites;

    private float startTime;
    private bool generatedWorld;

    private float noiseOffset = 0;

    public void GenerateWorld(){
        startTime = Time.realtimeSinceStartup;
        generatedWorld = true;
        _terrainTileBuffer.Clear();

        allBiomes = Resources.LoadAll<Biome>("Biomes").ToList();
        biomes = allBiomes;

        Random.State originalState = Random.state;
        currentSeed = GameManager.Instance.currentWorld.seed;
        Debug.Log($"Generating terrain with seed {currentSeed}");
        Random.InitState(currentSeed);

        BoundsInt bounds = new BoundsInt(
            new Vector3Int(-worldSize, -worldSize, 0),
            new Vector3Int(worldSize * 2, worldSize * 2, 1)
        );

        float noiseTime = Time.realtimeSinceStartup;

        float noiseOffset = 50000 * Random.value + 10000;
        float noiseOffset2 = 5000 * Random.value + 22000;


        string[] keys = new string[]{ "perlin1", "perlin2", "perlin3", "height", "wetness", "heat", "wind", "simplex" };

        // Initialize the dictionary with empty jagged arrays.
        worldMaps.Clear();
        foreach (string key in keys){
            // Create a jagged array with 'dimension' rows.
            float[][] map = new float[bounds.size.x][];
            for (int i = 0; i < bounds.size.x; i++){
                // Allocate each row as a new float array.
                map[i] = new float[bounds.size.y];
            }

            worldMaps.Add(key, map);
        }


        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        noise.SetFrequency(0.1f);

        float maxHeat = 1;
        float minHeat = 0.25f;

        float maxWetness = 1;
        float minWetness = 0;

        switch (GameManager.Instance.currentWorld.planetType){
            case PlanetType.GasGiant:
                break;
            case PlanetType.Ocean:
                minWetness = 0.3f;
                break;
            case PlanetType.Rocky:
                maxHeat = 1.2f;
                break;
            case PlanetType.Tundra:
                minHeat = 0f;
                maxHeat = 0.9f;
                break;
            default:
                minHeat = 0.25f;
                break;
        }

        //generate noise maps:
        for (int i = -worldSize; i < worldSize; i++){
            for (int j = -worldSize; j < worldSize; j++){
                Vector2Int position = new Vector2Int(i, j);
                float perlin1 = Mathf.PerlinNoise(i * 0.09f - (noiseOffset / 3 * 3), j * 0.09f - noiseOffset * 2)
                                + Random.Range(-0.01f, 0.01f);
                float perlin2 = Mathf.PerlinNoise(i * 0.08f + (noiseOffset * 3), j * 0.08f + (noiseOffset * 3))
                                + Random.Range(-0.01f, 0.01f);
                float perlin3 = Mathf.PerlinNoise(i * 0.04f - noiseOffset, j * 0.04f + (noiseOffset / 2));


                //heat is from 0-1, but we want this to be in our scale of max and minheat, ie if max is 2, itll be from 0-2, etc
                float heat = Mathf.PerlinNoise(i * 0.02f - noiseOffset2, j * 0.02f - noiseOffset2);
                heat = Mathf.Lerp(minHeat, maxHeat, heat);


                float height = Mathf.PerlinNoise(i * 0.008f - noiseOffset2, j * 0.008f - noiseOffset2);
                float wetness = Mathf.PerlinNoise(i * 0.015f - noiseOffset2, j * 0.015f - noiseOffset2);
                wetness = Mathf.Lerp(minWetness, maxWetness, wetness);
                float wind = Mathf.PerlinNoise(i * 0.03f - noiseOffset2, j * 0.03f - noiseOffset2);
                float simplex = noise.GetNoise(i, j);

                worldMaps["perlin1"][i + worldSize][j + worldSize] = perlin1;
                worldMaps["perlin2"][i + worldSize][j + worldSize] = perlin2;
                worldMaps["perlin3"][i + worldSize][j + worldSize] = perlin3;

                worldMaps["height"][i + worldSize][j + worldSize] = height;
                worldMaps["wetness"][i + worldSize][j + worldSize] = wetness;
                worldMaps["heat"][i + worldSize][j + worldSize] = heat;
                worldMaps["wind"][i + worldSize][j + worldSize] = wind;
                worldMaps["simplex"][i + worldSize][j + worldSize] = simplex;
            }
        }

        Debug.Log($"Noise generation took {Time.realtimeSinceStartup - noiseTime} seconds");


        int numSites = Mathf.FloorToInt((worldSize * worldSize) / 2000f);
        numSites = Mathf.Clamp(numSites, 1, 20); // Limit maximum sites
        numSites = 13;
        List<Vector2Int> placedSites = new List<Vector2Int>();

        int halfSize = worldSize / 2;


        //set resource blocks based on flags:
        resourceBlocks = new List<Block>();
        resourceBlocks.Add(Tree);
        resourceBlocks.Add(Crystal);

        if (GameManager.Instance.currentWorld.flags.HasFlag(PlanetFlags.RockCoal)){
            resourceBlocks.Add(CoalNode);
        }

        if (GameManager.Instance.currentWorld.flags.HasFlag(PlanetFlags.StoneNodes)){
            resourceBlocks.Add(rockNode);
        }


        // Initial block placements
        //PlaceBlock(SellBlock, new Vector2Int(0, 1));
        PlaceBlock(SupplyCrate, new Vector2Int(Random.Range(-2, 2), -1));

        if (GameManager.Instance.settings.DevMode){
            PlaceBlock(BigCrate, new Vector2Int(0, -4));
            if (GetBlock(new Vector2Int(0, -4)) is ContainerBlock c){
                foreach (List<Item> list in ItemManager.Instance.itemDict.Values){
                    foreach (Item item in list){
                        ItemStack i = new ItemStack(item, item.stackSize);
                        c.Insert(ref i);
                    }
                }
            }
        }

        float siteStartTime = Time.realtimeSinceStartup;
        while (placedSites.Count < numSites){
            int randomX = Random.Range(-halfSize, halfSize);
            int randomY = Random.Range(-halfSize, halfSize);

            Vector2Int pos = new Vector2Int(randomX, randomY);
            float perlin2 = Mathf.PerlinNoise(randomX * 0.2f - (noiseOffset / 3 * 3), randomY * 0.2f - noiseOffset * 2)
                            + Random.Range(-0.01f, 0.01f);
            if (perlin2 > 0.7f && constructionSites.Length > 0 && placedSites.Count < numSites){
                bool validPosition = true;

                // Check minimum distance from other sites
                foreach (var site in placedSites){
                    if (Vector2Int.Distance(pos, site) < 40){
                        validPosition = false;
                        break;
                    }
                }

                if (validPosition){
                    Debug.Log($"Placing construction site at {pos}");

                    // Place random construction site
                    Block sitePrefab = constructionSites[Random.Range(0, constructionSites.Length)];
                    PlaceBlock(sitePrefab, pos);
                    placedSites.Add(pos);
                }
            }
        }

        Debug.Log($"Placed {placedSites.Count} construction sites in {Time.realtimeSinceStartup - siteStartTime} seconds");

        float terrainStartTime = Time.realtimeSinceStartup;

        // World gen
        float centerNoise = 0;
        for (int i = -halfSize; i < halfSize; i++){
            for (int j = -halfSize; j < halfSize; j++){
                Vector2Int position = new Vector2Int(i, j);
                try{
                    GeneratePosition(i, j);

                    if (i == 0 && j == 0){
                        centerNoise = 0;
                    }
                }
                catch (Exception e){
                    Debug.LogError($"Error at {position}: {e.StackTrace}");
                }
            }
        }

        Debug.Log($"Terrain generation took {Time.realtimeSinceStartup - terrainStartTime} seconds");

        // Central area generation
        bool inverse = centerNoise < 0.5f;
        int a = 20;
        for (int i = -a - 4; i <= a + 4; i++){
            for (int j = -a - 4; j <= a + 4; j++){
                float perlin2 = Mathf.PerlinNoise(i * 0.095f + noiseOffset * 3, j * 0.09f + noiseOffset)
                                + Random.Range(-0.01f, 0.01f) + j / 1000f;
                float myDist = Mathf.Sqrt(i * i + j * (j / 2));
                float myPerlin = perlin2 + (inverse ? myDist : -myDist) / (a * 2);
                if (inverse ? myPerlin < 0.75f : myPerlin > 0.25f){
                    SetWall(null, new Vector3Int(i, j, 0));
                }
            }
        }

        // Random block placement
        for (int i = 0; i < (halfSize * halfSize) / 130; i++){
            Vector2Int pos = new Vector2Int(
                Random.Range(-halfSize, halfSize),
                Random.Range(-halfSize, halfSize)
            );

            if (IsWall((Vector3Int)pos)){
                i--;
                continue;
            }

            if (Random.value < 0.065f){
                PlaceBlock(LootCrate, pos);
                continue;
            }

            if (GetTerrainProperties(pos) == Grass) PlaceBlock(Tree, pos);
            else{
                PlaceBlock(resourceBlocks[Random.Range(0, resourceBlocks.Count)], pos);
            }
        }

        // Crystal cluster generation
        for (int i = 0; i < 25; i++){
            Vector2Int pos = new Vector2Int(
                Random.Range(-halfSize, halfSize),
                Random.Range(-halfSize, halfSize)
            );

            SetWall(null, (Vector3Int)pos);
            for (int j = 0; j < 9; j++){
                SetWall(null, (Vector3Int)pos +
                              Vector3Int.RoundToInt(Random.insideUnitCircle * Random.Range(0.5f, 2.2f)));
            }

            PlaceBlock(Crystal, pos);
        }

        foreach (Vector2Int site in placedSites){
            for (int i = -12; i <= 12; i++){
                for (int j = -12; j <= 12; j++){
                    float distance = Mathf.Sqrt(i * i + j * j);

                    // Function: Completely remove walls within radius 5, taper off from 5 to 10
                    float falloff = Mathf.Clamp01(1 - (distance - 5) / 5);
                    float zoom = 0.05f;
                    float perlin = Mathf.PerlinNoise((site.x + i) * zoom, (site.y + j) * zoom);
                    if ((falloff > 0.25 && perlin > 0.5) || falloff > 0.8f){
                        SetWall(null, new Vector3Int(site.x + i, site.y + j, 0));
                        if (Random.value < 0.8f && distance < 2){
                            SetTerrain(new Vector2Int(site.x + i, site.y + j), Sand);
                        }
                    }
                }
            }
        }

        GameManager.Instance.currentWorld.generated = true;

        Random.state = originalState;

        ApplyBufferedTiles();
        GC.Collect();
    }

    public void GeneratePosition(int i, int j){
        bool placedOre = false;
        Vector2Int position = new Vector2Int(i, j);

        float perlin2 = worldMaps["perlin2"][i + worldSize][j + worldSize];
        float perlin1 = worldMaps["perlin1"][i + worldSize][j + worldSize];
        float perlin3 = worldMaps["perlin3"][i + worldSize][j + worldSize];

        float wallModifier = 1;

        bool placedTerrain = false;

        Biome biome = null;

        foreach (var vbiome in biomes){
            if (CheckThreshold(vbiome.threshold, i, j)){
                biome = vbiome;
                wallModifier *= biome.wallModifier;

                break;
            }
        }

        switch (GameManager.Instance.currentWorld.planetType){
            case PlanetType.GasGiant:
                break;
            case PlanetType.Ocean:
                //if distance from center is greater than 200, set to water
                if (Mathf.Sqrt(i * i + j * j) > 200){
                    SetTerrain(position, Water);
                    return;
                }

                break;
            case PlanetType.Rocky:
                break;
            case PlanetType.Tundra:
                break;
            default:
                break;
        }

        if (((perlin2 * perlin2 < 0.35f * wallModifier && perlin2 > 0.03f) || (perlin1 < 0.25f * wallModifier && perlin2 < 0.42f * wallModifier)) &&
            perlin3 * wallModifier > 0.25f){
            if (biome?.customWallTile != null){
                SetWall(biome.customWallTile, (Vector3Int)position);
            }
            else{
                SetWall(rockWall, (Vector3Int)position);
            }

            if (Random.value > 0.5f){
                //set terrain to stone
                SetTerrain(position, Stone);
            }
        }

        if (biome != null){
            //we are now in this biome


            biome.terrains = biome.terrains.OrderByDescending(x => x.priority).ToList();
            foreach (BiomeTerrainInfo terrainInfo in biome.terrains){
                if (CheckThreshold(terrainInfo.threshold, i, j)){
                    SetTerrain(position, terrainInfo.terrain);
                    placedTerrain = true;
                    break;
                }
            }

            foreach (BiomeBlockInfo blockInfo in biome.blocks){
                if (Random.value < blockInfo.chance){
                    if (CheckThreshold(blockInfo.threshold, i, j)){
                        if (PlaceBlock(blockInfo.block, position)){ }

                        break;
                    }
                }
            }
        }

        if (perlin2 > 0.25f){
            if (!placedTerrain){
                if (perlin2 < 0.6f && perlin1 < 0.2f && perlin3 > 0.5f){
                    SetTerrain(position, Sand);
                }
                else{
                    SetTerrain(position, perlin2 * perlin2 > 0.65f + Random.Range(0, 0.05f) ? Grass : Stone);
                }

                placedTerrain = true;
            }

            int k = 0;

            //can't place on collision terrains (water)
            if (!GetTerrainProperties(position).collider){
                foreach (OreProperties oreProperties in ItemManager.Instance.allOres){
                    if (placedOre) break;
                    k++;
                    float offset = 5000 * k;
                    float perlinValue = Mathf.PerlinNoise(
                        i * oreProperties.scale + noiseOffset + offset,
                        j * oreProperties.scale + noiseOffset + offset);

                    if (oreProperties.threshold < perlinValue + Random.Range(-0.01f, 0.01f) &&
                        oreProperties.heatThreshold < getNoisePosition(position, "heat") &&
                        oreProperties.heightThreshold < getNoisePosition(position, "height") &&
                        oreProperties.chance > Random.value){
                        int amount = (int)(oreProperties.minAmount +
                                           (oreProperties.maxAmount - oreProperties.minAmount) *
                                           (perlinValue - oreProperties.threshold) / 0.3f +
                                           Random.Range(-oreProperties.variance, oreProperties.variance));
                        SetOre(position, oreProperties, amount);
                        placedOre = true;
                        break;
                    }
                }
            }
        }
        else{
            if (!placedTerrain)
                SetTerrain(position, Stone);
        }

        //forest
        if (perlin3 < 0.195f + Random.Range(-0.01f, 0f)){
            SetTerrain(position, Grass);
            if (Random.Range(0, 1.6f) < 0.18f - perlin3 / 3 && perlin3 < 0.19){
                if (Random.value < 0.3f) PlaceBlock(Tree, position);
                else if (Random.value > 0.4f) PlaceBlock(Mushroom, position);
                else PlaceBlock(Sapling, position);
            }
        }


        if (Random.value > 0.995f) PlaceBlock(Rock2x, position - Vector2Int.one);
        if (Random.value < 0.032f && perlin1 < 0.34f) PlaceBlock(Mushroom, position);
    }

    Texture2D CreateTextureFromNoiseMap(Dictionary<Vector2Int, float> noiseMap, int textureSize){
        Texture2D texture = new Texture2D(textureSize, textureSize);

        int offset = textureSize / 2;

        for (int x = 0; x < texture.width; x++){
            for (int y = 0; y < texture.height; y++){
                Vector2Int worldPos = new Vector2Int(x - offset, y - offset);

                float value = 0f;
                if (noiseMap.TryGetValue(worldPos, out value)){
                    Color col = new Color(value, value, value);
                    texture.SetPixel(x, y, col);
                }
                else{
                    texture.SetPixel(x, y, Color.black);
                }
            }
        }

        texture.Apply();
        return texture;
    }

    public bool CheckThreshold(Threshold t, int x, int y){
        float perlin2 = worldMaps["perlin2"][x + worldSize][y + worldSize];
        float height = worldMaps["height"][x + worldSize][y + worldSize];
        float heat = worldMaps["heat"][x + worldSize][y + worldSize];
        float wetness = worldMaps["wetness"][x + worldSize][y + worldSize];
        float wind = worldMaps["wind"][x + worldSize][y + worldSize];

        if (t.invertHeight) height = 1 - height;
        if (t.invertHeat) heat = 1 - heat;
        if (t.invertWetness) wetness = 1 - wetness;
        if (t.invertWind) wind = 1 - wind;

        return perlin2 > t.perlin2threshold && height > t.height && heat > t.heat && wetness > t.wetness &&
               wind > t.wind;
    }

    public float getNoisePosition(Vector2Int pos, string map){
        if (worldMaps.ContainsKey(map)){
            return worldMaps[map][pos.x + worldSize][pos.y + worldSize];
        }
        else{
            Debug.LogError($"Map {map} not found");
            return 0;
        }
    }
}