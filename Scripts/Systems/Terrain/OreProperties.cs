using System;
using Systems.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Systems.Terrain{
    [CreateAssetMenu(fileName = "ore props", menuName = "ScriptableObjects/OreProperties", order = 0)]
    public class OreProperties : ScriptableObject{
        [Header("Ore Patch Rarity")]
        public float threshold;
        [Header("Ore Patch Size")]
        public float scale;
        
        public int minAmount;
        public int maxAmount;
        public int variance;


        public Ore ore;


        private void OnValidate(){
            ore.myProperties = this;
        }
    }
}