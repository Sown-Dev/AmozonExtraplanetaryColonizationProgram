using System;
using System.Collections.Generic;
using Systems.Block.BlockStates;
using Systems.Items;
using UnityEngine;

[Serializable]

//Represents IMMUTABLE properties of a block, not stored in the blockitem
public class BlockProperties{
   
   //do not set (duh, cant be private though)
   [field:SerializeField]public BlockItem myItem{ get; set; }
   [HideInInspector] public string name; //set in block prefab OnValidate()
   public bool indestructible;
   public bool rotateable = false; //used by cursor
   public bool actuatable;
   public bool collidable = true;
   //public float blockHeight = 1;
   public Vector2Int size= new (1,1);

   public string description = "";
   public BlockStateSO bso;
}
