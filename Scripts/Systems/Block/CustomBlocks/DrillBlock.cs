using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Systems.Items;
using UnityEngine;

namespace Systems.Block{
    public class DrillBlock : ContainerBlock{
        
        
        //public new DrillBlockData data => (DrillBlockData)base.data;
        
        public int DrillTime = 80;
        public int DrillAmount=1;

        public List<Vector2Int> DrillPositions;


        public List<GameObject> DrillFX;
        
        public ProgressBar progressBar = new ProgressBar(21);



        public override void Init(Orientation orientation){
            base.Init(orientation);
            progressBar = new ProgressBar(21);
            progressBar.progress = 0;
            progressBar.maxProgress = DrillTime;

            
        }

        public override void Tick(){
            base.Tick();

            bool canMine = CanMine();
            DrillFX.ForEach(fx => fx.SetActive(canMine));

            if (!canMine) return;

            progressBar.progress++;

            if (progressBar.progress >=progressBar.maxProgress){
                progressBar.progress = 0;
                Drill();
            }
        }

        private int i = 0;

        public virtual void Drill(){
            ItemStack item =
                TerrainManager.Instance.ExtractOre(data.origin+ DrillPositions[i], 1);
            while (TerrainManager.Instance.GetOre(data.origin+ DrillPositions[i]) ==
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
                if (TerrainManager.Instance.GetOre(data.origin+ pos) != null){
                    if (TerrainManager.Instance.GetOre(data.origin+ pos).amount > 0){
                        return true;
                    }
                }
            }

            return false;
        }

        public override List<TileIndicator> GetIndicators(){
            var e = base.GetIndicators();
            e.Add(new TileIndicator(DrillPositions.ToArray(), IndicatorType.Mining));
            return e;
        }

        public override BlockData Save(){
            BlockData d =base.Save();
            d.data.SetString( "progressBar", JsonConvert.SerializeObject( progressBar, GameManager.JSONsettings));
            return d;
        }
        public override void Load(BlockData d){
            base.Load(d);
            progressBar = JsonConvert.DeserializeObject<ProgressBar>(d.data.GetString("progressBar"), GameManager.JSONsettings);
        }
    }
    [Serializable]
    public class DrillBlockData : ContainerBlockData{
        
        
        public ProgressBar progressBar = new ProgressBar(21);

    }
    
}