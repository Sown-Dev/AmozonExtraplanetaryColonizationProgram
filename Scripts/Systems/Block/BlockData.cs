using System;
using System.Collections.Generic;
using Systems.Items;
using UnityEngine;

namespace Systems.Block{
    [Serializable]
    public class BlockData{
        public List<ItemStack> lootTable = new();

        public Orientation rotation;

        public Vector2Int origin; // the origin is kind of the center, except since we can have even sized objects, it

        //public string typeName; // Stores the class type

        public DataStorage data = new DataStorage();

        public BlockData(){
            //typeName = GetType().AssemblyQualifiedName; // Save full type name
        }
    }
}