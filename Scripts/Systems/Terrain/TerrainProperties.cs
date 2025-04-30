using System;
using Systems.Items;
using UnityEngine;

namespace Systems.Terrain{
    [Serializable]
    [CreateAssetMenu(fileName = "terr props", menuName = "ScriptableObjects/TerrainProperties", order = 0)]
    public class TerrainProperties : ScriptableObject{
        public CustomRuleTile tile;
        public Terrain terrain;

        public AudioClip[] footsteps;
        public Item myItem;

        private void OnValidate(){
            terrain.myProperties = this.name;
            tile.hasCollider = collider;
        }
        
        //stats
        public float walkSpeed = 1f;
        public bool collider = false;
    }
}