using System;
using Systems.Terrain;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Items{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/TerrainItem", order = 0)]
    public class TerrainItem : Item{
        public TerrainProperties terrain;

        public override void Use(Vector2Int pos, Unit user, Slot slot){
            base.Use(pos, user, slot);

            if (TerrainManager.Instance.GetTerrainProperties(pos) != terrain){

                TerrainManager.Instance.SetTerrain(pos, terrain, true);
                TerrainManager.Instance.ApplyBufferedTiles();
                slot.ItemStack.amount--;
                if (slot.ItemStack.amount <= 0){
                    slot.ItemStack = null;
                }
            }
        }
    }
}