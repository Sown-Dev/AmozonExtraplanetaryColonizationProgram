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

    public partial class TerrainManager
    {
        [SerializeField] private RuleTile rockWall;
        
        
        private int currentSeed;
        [FormerlySerializedAs("size")] [SerializeField] private int worldSize = 500;

        [Header("Terrain")] 
        [SerializeField] private TerrainProperties Grass;
        [SerializeField] private TerrainProperties Stone;
        [SerializeField] private TerrainProperties Sand;
        

        [Header("Blocks")] 
        [SerializeField] private Block Sapling;
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

        [SerializeField] private Block[] constructionSites;
        
        private float startTime;
        private bool generatedWorld;

        public void GenerateWorld()
        {
            
            startTime = Time.realtimeSinceStartup;
            generatedWorld = true;
            _terrainTileBuffer.Clear();
            
            Random.State originalState = Random.state;
            currentSeed = GameManager.Instance.currentWorld.seed;
            Debug.Log($"Generating terrain with seed {currentSeed}");
            Random.InitState(currentSeed);
            
            int numSites = Mathf.FloorToInt((worldSize * worldSize) / 2000f);
            numSites = Mathf.Clamp(numSites, 1, 20); // Limit maximum sites
            numSites = 13;
            List<Vector2Int> placedSites = new List<Vector2Int>();

            float noiseOffset = 50000 * Random.value + 10000;
            int halfSize = worldSize / 2;
            
            
            
            //set resource blocks based on flags:
            resourceBlocks = new List<Block>();
            resourceBlocks.Add(Tree);
            resourceBlocks.Add( Crystal);
            
            if (GameManager.Instance.currentWorld.flags.HasFlag(PlanetFlags.RockCoal)) 
            {
                resourceBlocks.Add(CoalNode);
            }
            if(GameManager.Instance.currentWorld.flags.HasFlag(PlanetFlags.StoneNodes))
            {
                resourceBlocks.Add(rockNode);
            }
            
            
        
            
            
            
            

            // Initial block placements
            //PlaceBlock(SellBlock, new Vector2Int(0, 1));
            PlaceBlock(SupplyCrate, new Vector2Int(Random.Range(-4, 4), -1));

            if (GameManager.Instance.settings.DevMode)
            {
                PlaceBlock(BigCrate, new Vector2Int(0, -4));
                if (GetBlock(new Vector2Int(0, -4)) is ContainerBlock c)
                {
                    foreach (List<Item> list in ItemManager.Instance.itemDict.Values)
                    {
                        foreach (Item item in list)
                        {
                            ItemStack i = new ItemStack(item, item.stackSize);
                            c.Insert(ref i);
                        }
                    }
                }
            }
            float siteStartTime = Time.realtimeSinceStartup;
            while (placedSites.Count < numSites)
            {
                int randomX = Random.Range(-halfSize, halfSize);
                int randomY = Random.Range(-halfSize, halfSize);
                
                Vector2Int pos = new Vector2Int(randomX, randomY);
                float perlin2 = Mathf.PerlinNoise(randomX * 0.2f - (noiseOffset / 3 * 3), randomY * 0.2f - noiseOffset * 2) 
                              + Random.Range(-0.01f, 0.01f);
                if (perlin2 > 0.7f && constructionSites.Length > 0 && placedSites.Count < numSites)
                {
                    bool validPosition = true;
                            
                    // Check minimum distance from other sites
                    foreach (var site in placedSites)
                    {
                        if (Vector2Int.Distance(pos, site) < 40)
                        {
                            validPosition = false;
                            break;
                        }
                    }

                    if (validPosition)
                    {
                        Debug.Log($"Placing construction site at {pos}");

                        // Place random construction site
                        Block sitePrefab = constructionSites[Random.Range(0, constructionSites.Length)];
                        PlaceBlock(sitePrefab, pos);
                        placedSites.Add(pos);
                    }
                }

            }
            Debug.Log($"Placed {placedSites.Count} construction sites in {Time.realtimeSinceStartup - siteStartTime} seconds");

            // Terrain generation
            float centerNoise = 0;
            for (int i = -halfSize; i < halfSize; i++)
            {
                for (int j = -halfSize; j < halfSize; j++)
                {
                    Vector2Int position = new Vector2Int(i, j);
                    try
                    {
                        bool placedOre = false;
                        float perlin1 = Mathf.PerlinNoise(i * 0.2f - (noiseOffset / 3 * 3), j * 0.2f - noiseOffset * 2) 
                                      + Random.Range(-0.01f, 0.01f);
                        float perlin2 = Mathf.PerlinNoise(i * 0.088f + (noiseOffset * 3), j * 0.09f + noiseOffset) 
                                      + Random.Range(-0.01f, 0.01f);
                        float perlin3 = Mathf.PerlinNoise(i * 0.035f - noiseOffset, j * 0.04f + (noiseOffset / 2));

                        if (i == 0 && j == 0) centerNoise = perlin2;
                        
                        
                      
                        
                        
                        if (perlin2 > 0.25f)
                        {

                            if (perlin2 <0.6f && perlin1 < 0.2f && perlin3 > 0.5f){
                                SetTerrain(position, Sand);
                            }else{
                                SetTerrain(position, perlin2 * perlin2 > 0.65f + Random.Range(0, 0.05f) ? Grass : Stone);

                            }

                            int k=0;
                            foreach (OreProperties oreProperties in ItemManager.Instance.allOres){
                                k++;
                                float offset =5000 * k;
                                float perlinValue = Mathf.PerlinNoise(
                                    i * oreProperties.scale + noiseOffset + offset,
                                    j * oreProperties.scale + noiseOffset+offset);

                                if (perlinValue > oreProperties.threshold + Random.Range(-0.01f, 0.01f))
                                {
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
                        else
                        {
                            SetTerrain(position, Stone);
                        }

                        if (perlin3 < 0.195f + Random.Range(-0.01f, 0f))
                        {
                            SetTerrain(position, Grass);
                            if (Random.Range(0, 1.6f) < 0.18f - perlin3 / 3 && perlin3 < 0.19)
                            {
                                if (Random.value < 0.3f) PlaceBlock(Tree, position);
                                else if (Random.value > 0.4f) PlaceBlock(Mushroom, position);
                                else PlaceBlock(Sapling, position);
                            }
                        }

                        if (((perlin2 * perlin2 < 0.35f && perlin2 > 0.03f) || (perlin1 < 0.25f && perlin2 < 0.42f)) &&
                            perlin3 > 0.25f)
                        {
                            SetWall(rockWall, (Vector3Int)position);
                        }

                        if (Random.value > 0.995f) PlaceBlock(Rock2x, position - Vector2Int.one);
                        if (Random.value < 0.032f && perlin1 < 0.34f) PlaceBlock(Mushroom, position);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error at {position}: {e.Message}");
                    }
                }
            }

            // Central area generation
            bool inverse = centerNoise < 0.5f;
            int a = 20;
            for (int i = -a - 4; i <= a + 4; i++)
            {
                for (int j = -a - 4; j <= a + 4; j++)
                {
                    float perlin2 = Mathf.PerlinNoise(i * 0.095f + noiseOffset * 3, j * 0.09f + noiseOffset)
                                  + Random.Range(-0.01f, 0.01f) + j / 1000f;
                    float myDist = Mathf.Sqrt(i * i + j * (j / 2));
                    float myPerlin = perlin2 + (inverse ? myDist : -myDist) / (a * 2);
                    if (inverse ? myPerlin < 0.75f : myPerlin > 0.25f)
                    {
                        SetWall(null, new Vector3Int(i, j, 0));
                    }
                }
            }

            // Random block placement
            for (int i = 0; i <  (halfSize*halfSize)/130; i++)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(-halfSize, halfSize),
                    Random.Range(-halfSize, halfSize)
                );

                if (IsWall((Vector3Int)pos)) { i--; continue; }

                if (Random.value < 0.065f)
                {
                    PlaceBlock(LootCrate, pos);
                    continue;
                }

                if (GetTerrainProperties(pos) == Grass) PlaceBlock(Tree, pos);
                else{
                    PlaceBlock( resourceBlocks[Random.Range(0, resourceBlocks.Count)], pos);
                }
                
            }

            // Crystal cluster generation
            for (int i = 0; i < 20; i++)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(-halfSize, halfSize),
                    Random.Range(-halfSize, halfSize)
                );

                SetWall(null, (Vector3Int)pos);
                for (int j = 0; j < 9; j++)
                {
                    SetWall(null, (Vector3Int)pos + 
                        Vector3Int.RoundToInt(Random.insideUnitCircle * Random.Range(0.5f, 2.2f)));
                }
                PlaceBlock(Crystal, pos);
            }
            
            foreach (Vector2Int site in placedSites)
            {
                for (int i = -12; i <= 12; i++)
                {
                    for (int j = -12; j <= 12; j++)
                    {
                        float distance = Mathf.Sqrt(i * i + j * j);

                        // Function: Completely remove walls within radius 5, taper off from 5 to 10
                        float falloff = Mathf.Clamp01(1 - (distance - 5) / 5);
                        float zoom =0.05f;
                        float perlin = Mathf.PerlinNoise((site.x + i) *zoom, (site.y+ j) *zoom);
                        if ((falloff >0.25 && perlin > 0.5) || falloff>0.8f)
                        {
                            SetWall(null, new Vector3Int(site.x + i, site.y + j, 0));
                            if(Random.value < 0.8f && distance<2)   
                            {
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

     
        
    }