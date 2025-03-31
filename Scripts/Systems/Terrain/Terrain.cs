using System;
using UnityEngine;

namespace Systems.Terrain{
    [Serializable]
    public class Terrain{
        
        //we put mutable variables in this class, and immmutable variables in TerrainProperties
        public bool owned;

        public bool instantiated{
            get;
            private set;
        }

        
        public string myProperties;

        public Terrain(){
            
        }
        
        public Terrain(TerrainProperties properties){
            myProperties = properties.name;
            owned = false;
            instantiated = true;
        }

        
    }

  
}