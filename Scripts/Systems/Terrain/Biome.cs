using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Terrain{
    [CreateAssetMenu(fileName = "biome", menuName = "ScriptableObjects/Biome", order = 0)]
    public class Biome : ScriptableObject{
        public Threshold threshold;
        public List< BiomeTerrainInfo> terrains;
        public List<BiomeBlockInfo> blocks;

        public float wallModifier = 1;
        public RuleTile customWallTile;
    }
    [Serializable]
    public class BiomeTerrainInfo{
        public int priority;
        public TerrainProperties terrain;
        public Threshold threshold;
        
    }
    [Serializable]
    public class BiomeBlockInfo{
        public Block.Block block;
        public float chance;
        public Threshold threshold;
        
    }
    [Serializable]
    public struct Threshold{
        public float perlin2threshold; 
        
        public bool invertHeight; 
        public float height;
        
        public bool invertHeat; 
        public float heat;
        
        public bool invertWetness;
        public float wetness;
        
        public bool invertWind; 
        public float wind;
    }
}