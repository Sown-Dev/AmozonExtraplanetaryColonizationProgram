using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Block;
using Systems.Items;
using Systems.Terrain;
using UnityEngine;
using Random = UnityEngine.Random;
using Terrain = Systems.Terrain.Terrain;

public partial class TerrainManager{
    [SerializeField] private RuleTile rockWall;

    [Header("Terrain")] [SerializeField] private TerrainProperties Grass;
    [SerializeField] private TerrainProperties Stone;


    [Header("Blocks")] 
    [SerializeField] private Block Tree;
    [SerializeField] private Block Sapling;
    [SerializeField] private Block Crystal;
    [SerializeField] private Block Mushroom;
    [SerializeField] private Block LootCrate;
    [SerializeField] private Block CoalNode;
    

    [SerializeField] private Block SellBlock;
    [SerializeField] private Block BigCrate;


    //[SerializeField] private Block Rock1x;
    [SerializeField] private Block Rock2x;


    public void GenerateTerrain(){
        float noiseOffset = 50000 * Random.value + 10000;


        //Start by placing important blocks first, so their positions can't be occupied

        PlaceBlock(SellBlock, new Vector2Int(0, 1));

#if ALLITEMS1
        PlaceBlock(BigCrate, new Vector2Int(0, -4));
        ContainerBlock c = GetBlock(new Vector2Int(0, -4)) as ContainerBlock;

        foreach (List<Item> list in ItemManager.Instance.itemDict.Values){
            foreach (Item item in list){
                ItemStack i = new ItemStack(item, item.stackSize);
                c.output.Insert(ref i);
            }
        }
#endif


        float centerNoise = 0;
        for (int i = -200; i < 200; i++){
            for (int j = -200; j < 200; j++){
                Vector2Int position = new Vector2Int(i, j);

                try{
                    // Sample Perlin noise for this position

                    bool placedOre = false;

                    float perlin1 = Mathf.PerlinNoise(i * 0.2f - (noiseOffset / 3 * 3), j * 0.2f - noiseOffset * 2) +
                                    Random.Range(-0.01f, 0.01f);

                    float perlin2 = Mathf.PerlinNoise(i * 0.088f + (noiseOffset * 3), j * 0.09f + noiseOffset) +
                                    Random.Range(-0.01f, 0.01f);

                    float perlin3 = Mathf.PerlinNoise(i * 0.035f - (noiseOffset), j * 0.04f + (noiseOffset / 2));


                    if (i == 0 && j == 0){
                        centerNoise = perlin2;
                    }

                    if (perlin2 > 0.25f){
                        SetTerrain(position, Stone);
                        if (perlin2 * (perlin2) > 0.65f + Random.Range(0, 0.05f)){
                            SetTerrain(position, Grass);
                        }
                        else
                            SetTerrain(position, Stone);


                        for (int k = 0; k < ItemManager.Instance.allOres.Length; k++){
                            float perlinValue = Mathf.PerlinNoise(
                                i * ItemManager.Instance.allOres[k].scale + noiseOffset + (10000 * k),
                                j * ItemManager.Instance.allOres[k].scale + noiseOffset + (20000 * k));

                            OreProperties oreProperties = ItemManager.Instance.allOres[k];
                            if (perlinValue > oreProperties.threshold + Random.Range(-0.01f, 0.01f)){
                                int amount = (int)(oreProperties.minAmount +
                                                   (oreProperties.maxAmount - oreProperties.minAmount) *
                                                   (perlinValue - oreProperties.threshold) / 0.3f +
                                                   Random.Range(-oreProperties.variance, oreProperties.variance));
                                SetOre(position, oreProperties.ore, amount);
                                //placedOre = true;
                                break;
                            }
                        }
                    }
                    else{
                        SetTerrain(position, Stone);
                    }


                    //forest
                    if (perlin3 < 0.195f + Random.Range(-0.01f, 0f)){
                        SetTerrain(position, Grass);
                        if (Random.Range(0, 1.5f) < 0.18f - perlin3 / 3 && perlin3 < 0.19){
                            if (Random.value < 0.3f){
                                PlaceBlock(Tree, position);
                            }
                            else if (Random.value > 0.4f){
                                PlaceBlock(Mushroom, position);
                            }else{
                                PlaceBlock(Sapling, position);
                            }
                        }
                    }

                    if (((perlin2 * perlin2 < 0.32f && perlin2 > 0.03f) || (perlin1 < 0.25f && perlin2 < 0.42f)) &&
                        perlin3 > 0.25f){
                        SetWall(rockWall, (Vector3Int)position);
                    }

                    if (Random.value > 0.995f){
                        PlaceBlock(Rock2x, position - Vector2Int.one);
                    }

                    if (Random.value < 0.032f && perlin1 < 0.34f){
                        PlaceBlock(Mushroom, position);
                    }
                }
                catch (Exception e){
                    Debug.LogError("Error at " + position + ": " + e.Message);
                }
            }
        }

        bool inverse = false;
        inverse = centerNoise < 0.5f;

        int a = 18;
        for (int i = -a - 4; i <= a + 4; i++){
            for (int j = -a - 4; j <= a + 4; j++){
                float perlin2 = Mathf.PerlinNoise(i * 0.095f + noiseOffset * 3, j * 0.09f + noiseOffset) +
                                Random.Range(-0.01f, 0.01f) + j / 1000f;
                float myDist = Mathf.Sqrt(i * i + j * (j / 2));
                float myPerlin = perlin2 + (inverse ? myDist : -myDist) / (a * 2);
                if (inverse ? myPerlin < 0.75f : myPerlin > 0.25f){
                    SetWall(null, new Vector3Int(i, j, 0));
                }
            }
        }


        //now place worldgen:

        for (int i = 0; i < 600; i++){
            Vector2Int pos = new Vector2Int(Random.Range(-200, 200), Random.Range(-200, 200));
            if (!IsWall((Vector3Int)pos)){
                if (GetTerrain(pos).myProperties == Grass){
                    PlaceBlock(Tree, pos);
                }
                else{
                    if (Random.value > 0.6f){
                        PlaceBlock(CoalNode, pos);
                    }

                    PlaceBlock(Crystal, pos);
                }
            }
        }

        for (int i = 0; i < 20; i++){
            Vector2Int pos = new Vector2Int(Random.Range(-200, 200), Random.Range(-200, 200));
            SetWall(null, (Vector3Int)pos);
            for (int j = 0; j < 7; j++){
                SetWall(null, (Vector3Int)pos + Vector3Int.RoundToInt(Random.insideUnitCircle * Random.Range(0.5f,2f)));
            }


            PlaceBlock(Crystal, pos);
        }


        GC.Collect();
    }
}