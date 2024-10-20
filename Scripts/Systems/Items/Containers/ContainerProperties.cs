using System;
using UnityEngine;

namespace Systems.Items{
    [Serializable]
    public struct ContainerProperties{
        public int size;
        public string name;
        public int gridWidth;
        public bool scaleDownGridIfSmaller;
        public TooltipFlags ttFlags;
        
        //not currently being used
        [HideInInspector]public ContainerType type;
        public ContainerProperties(int _size, string _name=""){
            size = _size;
            gridWidth = 8;
            scaleDownGridIfSmaller = true;
            name = _name;
            type = ContainerType.FIFO;
            ttFlags = new TooltipFlags();
        }
    }

    public class TooltipFlags{
        private bool isBurner;
        private bool isElectric;
        private bool isContainer;
        
    }
}