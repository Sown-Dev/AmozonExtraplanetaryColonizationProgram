using System;
using Systems.Items;
using UnityEngine;

namespace Systems.Terrain{
    [Serializable]
    [CreateAssetMenu(fileName = "terr props", menuName = "ScriptableObjects/TerrainProperties", order = 0)]
    public class TerrainProperties : ScriptableObject{
      
        
        public RuleTile tile;
        public Terrain terrain;
        private void OnValidate(){
           terrain.myProperties = this;
        }
    }
}