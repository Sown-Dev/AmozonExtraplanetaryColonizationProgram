using System;
using UnityEngine;
using MemoryPack;

namespace Systems.Items{
    [Serializable]
    public partial struct ContainerProperties{
        public int size;
        public string name;
        public int gridWidth;
        public bool scaleDownGridIfSmaller;
        
        //not currently being used
        [HideInInspector]public ContainerType type;
        [MemoryPackConstructor]
        public ContainerProperties(int _size, string _name=""){
            size = _size;
            gridWidth = 8;
            scaleDownGridIfSmaller = true;
            name = _name;
            type = ContainerType.FIFO;
        }
    }

    
}