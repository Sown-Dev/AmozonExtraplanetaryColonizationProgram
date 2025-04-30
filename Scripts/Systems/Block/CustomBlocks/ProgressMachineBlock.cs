using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ProgressMachineBlock : TickingBlock, IMachineBlock{
        
        
        //public new ProgressMachineBlockData data => (ProgressMachineBlockData)base.data;
        
        [HideInInspector]public ProgressBar progressBar;

        
        public int progressPerCycle = 40;

        public int speed{ get; set; } = 1;
        [field: SerializeField]public int baseSpeed{ get; set; } = 1;


        protected override void Awake(){
            base.Awake();
            
        }
        
        public override void Init(Orientation orientation){
            base.Init(orientation);
            progressBar = new ProgressBar(23);
            progressBar.maxProgress = progressPerCycle;
        }

       
        public override void Tick(){
            base.Tick();
            if (!CanProgress()){
                //data.progressBar.progress=0; TODO idk if this is necessary, but well see
                return;
            }
            
            Progress();
        }

        public virtual void Progress(){
            speed = baseSpeed;
            progressBar.progress+= speed;
            if (progressBar.ResetIfFull()){
               CompleteCycle();
            }
        }

        public virtual bool CanProgress(){
            return true;
        }

        //what it does when progress complete
        public virtual void CompleteCycle(){ }
        
        public override void Load(BlockData d){
            base.Load(d);
            progressBar = JsonConvert.DeserializeObject<ProgressBar>(d.data.GetString("progressBar"), GameManager.JSONsettings);
        }
        
        public override BlockData Save(){
            BlockData b = base.Save();
            b.data.SetString( "progressBar", JsonConvert.SerializeObject(progressBar, GameManager.JSONsettings));
            return b;
        }
        
        
    }
    
    [Serializable]
    public class ProgressMachineBlockData : TickingBlockData{
        public ProgressBar progressBar;
    }
}

public interface IMachineBlock{
    public int baseSpeed{ get; set; }
    public int speed{ get; set; }
}