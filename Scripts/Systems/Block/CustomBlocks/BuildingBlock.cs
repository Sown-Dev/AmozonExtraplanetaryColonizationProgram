// BuildingBlock.cs
using UnityEngine;
using Systems.Block;
using Systems.Items;
using UI.BlockUI;

namespace Systems.Block
{
    public class BuildingBlock : Block
    {
        public BuildingProgress buildingProgress;
        public Block blockPrefab;

        protected override void Awake()
        {
            base.Awake();
            
            // Subscribe to events
            buildingProgress.OnBuildComplete += Build;
            
            buildingProgress.progress = new int[buildingProgress.resourcesNeeded.Length];
        }

    


        public void Build()
        {
            //don't need to close. auto closes when block is destroyed

            // Replace block
            TerrainManager.Instance.RemoveBlock(origin, false);
            TerrainManager.Instance.PlaceBlock(blockPrefab, origin, rotation);
            
            // Cleanup before destruction
            buildingProgress.ClearCallbacks();
        }

        private void OnDestroy()
        {
            // Critical memory leak prevention
            buildingProgress.OnBuildComplete -= Build;
        }
    }
}