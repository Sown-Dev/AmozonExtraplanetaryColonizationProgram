using System.Collections.Generic;
using Systems.Items;
using UnityEngine;

namespace Systems.Block{
    public class DrillBlock : ContainerBlock{
        public int DrillTime = 80;
        public int DrillAmount;

        public List<Vector2Int> DrillPositions;

        public ProgressBar progressBar = new ProgressBar(21);

        public List<GameObject> DrillFX;


        protected override void Awake(){
            base.Awake();
            
            progressBar.maxProgress = DrillTime;
        }

        public override void Tick(){
            base.Tick();

            bool canMine = CanMine();
            DrillFX.ForEach(fx => fx.SetActive(canMine));

            if (!canMine) return;

            progressBar.progress++;

            if (progressBar.progress >= progressBar.maxProgress){
                progressBar.progress = 0;
                Drill();
            }
        }

        private int i = 0;

        public virtual void Drill(){
            ItemStack item =
                TerrainManager.Instance.ExtractOre(origin+ DrillPositions[i], 1);
            while (TerrainManager.Instance.GetOre(origin+ DrillPositions[i]) ==
                   null){
                i++;
                i %= DrillPositions.Count;
            }

            if (item != null)
                output.Insert(ref item);
        }

        public virtual bool CanMine(){
            if(output.isFull()) return false;
            
            foreach (Vector2Int pos in DrillPositions){
                if (TerrainManager.Instance.GetOre(origin+ pos) != null){
                    if (TerrainManager.Instance.GetOre(origin+ pos).amount > 0){
                        return true;
                    }
                }
            }

            return false;
        }
    }
}