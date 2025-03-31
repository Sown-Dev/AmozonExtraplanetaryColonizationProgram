using System;
using UI.BlockUI;
using UnityEngine;

namespace Systems.Block
{
    public class TickingBlock : Block
    {
        //public new TickingBlockData data => (TickingBlockData)myData;

        public bool actuatedThisTick;
        
        public virtual void Tick() {
            if (TerrainManager.Instance.totalTicksElapsed % 2 == 0){

                if (properties.actuatable)
                    mat.SetColor("_AddColor", new Color(0, 0, 0, 0));

                currentState.SetNextSprite();
                UpdateSprite();
            }
        }

        
       
        public void ResetActuated()
        {
            actuatedThisTick = false;
            if (properties.actuatable)
                mat.SetColor("_AddColor", new Color(0, 0, 0, 0));

            currentState.SetNextSprite();
            UpdateSprite();
        }

        public override void Actuate()
        {
            if (actuatedThisTick)
                return;
            actuatedThisTick = true;
            base.Actuate();
            if (properties.actuatable)
                mat.SetColor("_AddColor", new Color(0.25f, 0.1f, 0.05f, 0));
        }

        public override void Use(Unit user)
        {
            base.Use(user);
        }

        public override BlockData Save(){
            var s = base.Save();
            s.data.SetBool("actuatedThisTick", actuatedThisTick);
            return s;
        }
        
        public override void Load(BlockData blockData)
        {
            base.Load(blockData);
           actuatedThisTick = blockData.data.GetBool("actuatedThisTick");
        }
    }
    [Serializable]
    public class TickingBlockData : BlockData
    {
        public bool actuatedThisTick = false;
    }
}
