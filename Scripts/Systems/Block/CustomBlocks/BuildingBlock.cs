// BuildingBlock.cs

using Newtonsoft.Json;
using UnityEngine;
using Systems.Block;
using Systems.Items;
using UI.BlockUI;
using UnityEngine.Serialization;

namespace Systems.Block
{
    public class BuildingBlock : Block
    {
        public BuildingProgress buildingProgress;

        //public new BuildingBlockData data => (BuildingBlockData)base.data;
        public Block buildingbPrefab;

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
            TerrainManager.Instance.RemoveBlock(data.origin, false);
            TerrainManager.Instance.PlaceBlock(buildingbPrefab, data.origin, data.rotation);
            
            // Cleanup before destruction
            buildingProgress.ClearCallbacks();
        }

        private void OnDestroy()
        {
            // Critical memory leak prevention
            buildingProgress.OnBuildComplete -= Build;
        }

        public override BlockData Save(){
            BlockData d = base.Save();
            d.data.SetString( "buildingProgress", JsonConvert.SerializeObject(buildingProgress.progress, GameManager.JSONsettings));
            return d;
        }
        
        public override void Load(BlockData d){
            base.Load(d);
            buildingProgress.progress = JsonConvert.DeserializeObject<int[]>(d.data.GetString("buildingProgress"), GameManager.JSONsettings);
        }
    }
    public class BuildingBlockData : BlockData
    {
       // public BuildingProgress buildingProgress;
    }
}