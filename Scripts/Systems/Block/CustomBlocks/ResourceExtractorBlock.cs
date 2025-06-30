using System.Collections.Generic;
using System.Linq;
using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ResourceExtractorBlock : ContainerBlock{
        public int DrillTime = 80;
        public int DrillAmount;


        public ProgressBar progressBar = new ProgressBar(-10);

        public List<Vector2Int> ExtractPositions;

        private ResourceBlock currentResource = null;

        public List<ResourceBlock> whiteList = new List<ResourceBlock>();

        public override void Init(Orientation orientation){
            base.Init(orientation);
           
        }

        public override void Tick(){
            base.Tick();
            if (currentResource == null){
                if (FindResource() == null) return;
            }

            progressBar.progress++;

            if (progressBar.progress >= progressBar.maxProgress){
                progressBar.progress = 0;
                ResourceExtract();
            }
        }
        public bool CanExtract(){
            if (currentResource == null){
                if (FindResource() == null) return false;
                else{
                    return true;
                }
            }
            else{
                return true;
            }
        }

        public void ResourceExtract(){
            ItemStack s = currentResource.Extract(DrillAmount);
            if (s != null)
                Insert(ref s);
        }

       
        public ResourceBlock FindResource(){
            foreach (Vector2Int pos in ExtractPositions.RotateList(data.rotation, Vector2Int.zero)){
                if (TerrainManager.Instance.GetBlock(data.origin + pos) is ResourceBlock block){
                    if (whiteList.Any(t => t.addressableKey == block.addressableKey)  || whiteList.Count == 0){
                        currentResource = block;
                        return block;
                    }
                }
            }

            return null;
        }

        public override List<TileIndicator> GetIndicators(){
            return new List<TileIndicator>(){ new TileIndicator(ExtractPositions.Select((t) => t).ToArray(), IndicatorType.Harvesting) };
        }
    }
}