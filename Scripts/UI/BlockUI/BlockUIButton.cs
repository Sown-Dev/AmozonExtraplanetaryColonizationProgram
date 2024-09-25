using System;
using Systems.BlockUI;
using UnityEngine;

namespace UI.BlockUI{
    
    //represents a button that can be clicked
    public class BlockUIButton: IBlockUI{
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }
        
        public BlockUIButton( Action onClick, Sprite icon, int priority=0){
            this.OnClick = onClick;
            this.icon = icon;
            this.Priority = priority;
        }

        public Action OnClick;
        public Sprite icon;
    }
}