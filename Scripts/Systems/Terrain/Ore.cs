using System;
using Systems.Items;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Systems.Terrain{
    [Serializable]
    
    public class Ore
    {
        public readonly OreProperties myProperties;
        public int amount;
        public Vector2Int position;

        public Ore(OreProperties properties, Vector2Int position, int amount)
        {
            this.myProperties = properties;
            this.position = position;
            this.amount = amount;
        }

        public ItemStack ExtractOre(int requestedAmount)
        {
            int extracted = Mathf.Min(requestedAmount, amount);
            extracted = ApplyStatsModifiers(extracted);
        
            amount -= extracted;
            return new ItemStack(myProperties.oreItem, extracted);
        }

        private int ApplyStatsModifiers(int baseAmount)
        {
            float modified = baseAmount * TerrainManager.Instance.finalStats[GStatstype.OreYieldMult];
            modified += TerrainManager.Instance.finalStats[GStatstype.OreYieldAdd];
            return Mathf.RoundToInt(modified);
        }
    }
}