using System;

namespace Systems.Items{
    [Serializable]
    public struct ContainerProperties{
        public int size;
        public string name;
        public int gridWidth;
        public bool scaleDownGridIfSmaller;
    
        public ContainerType type;
        public ContainerProperties(int size){
            this.size = size;
            gridWidth = 8;
            scaleDownGridIfSmaller = true;
            name = "Container";
            type = ContainerType.FIFO;
        }
    }
}