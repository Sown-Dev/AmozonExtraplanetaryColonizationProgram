using System;
using Systems.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Systems.Terrain{
    [CreateAssetMenu(fileName = "ore props", menuName = "ScriptableObjects/OreProperties", order = 0)]
    public class OreProperties : ScriptableObject
    {
        [Header("Generation Settings")]
        public float threshold = 0.5f;
        public float scale = 50f;
        public int minAmount = 1;
        public int maxAmount = 10;
        public int variance = 2;
        
        [Header("Visual Settings")]
        public RuleTile tile;
        public Item oreItem;
    }
}