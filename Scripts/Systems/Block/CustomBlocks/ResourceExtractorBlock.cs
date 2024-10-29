using System.Collections.Generic;
using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ResourceExtractorBlock: ContainerBlock{
        public int DrillTime = 80;
        public int DrillAmount;



        public ProgressBar progressBar = new ProgressBar();
        
        public List<Vector2Int> ExtractPositions;

        private ResourceBlock currentResource;
        
        public override void Tick(){
            base.Tick();
            if (currentResource == null){
                if(FindResource()==null) return;
            }
            
            progressBar.progress++;

            if (progressBar.progress >= progressBar.maxProgress){
                progressBar.progress = 0;
                Extract();
            }
        }

        public void Extract(){
            ItemStack s = currentResource.Extract(DrillAmount);
            if (s != null)
                output.Insert(ref s);
            
        }

        public override void Init( Orientation orientation){
            base.Init( orientation);
            currentResource = GetDirection(orientation) as ResourceBlock;
        }
        public ResourceBlock FindResource(){
            foreach (Vector2Int pos in ExtractPositions){
                if (TerrainManager.Instance.GetBlock(origin + pos) is ResourceBlock block){
                    currentResource = block;
                    return block;
                }
            }

            return null;
        }
        public override List<TileIndicator> GetIndicators(){
            
            return new List<TileIndicator>(){ new TileIndicator(ExtractPositions.ToArray(), IndicatorType.Extracting)};
        }

        
    }
}