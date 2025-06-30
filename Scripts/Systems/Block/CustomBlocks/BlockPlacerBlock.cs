using System;
using System.Collections.Generic;
using Systems.Items;
using UI.BlockUI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Block{
    public class BlockPlacerBlock : ContainerBlock{

        public BlockUIButton button;

        

        public override void Init(Orientation orientation){
            base.Init(orientation);


        }

        protected override void Start(){
            base.Start();

            button = new BlockUIButton(Click, null);
            

        }
        
        

      

        

        public void Click(){
            Block nextBlock = TerrainManager.Instance.GetBlock(data.origin + data.rotation.GetVectorInt());
            
            if (nextBlock == null){
                Debug.Log("place block");

                //get first block in container
                foreach (var s in output.containerList){
                    if (s.ItemStack?.item is BlockItem){
                        //use item
                        s.ItemStack.item.Use(  data.origin + data.rotation.GetVectorInt(), null, s);
                        return;
                    }
                }
            }

            
        }

        public override List<TileIndicator> GetIndicators(){
            var e = base.GetIndicators();
            e.Add(new TileIndicator(new Vector2Int[]{ data.rotation.GetVectorInt() }, IndicatorType.InsertingTo));

            return e;
        }
    }

  
}