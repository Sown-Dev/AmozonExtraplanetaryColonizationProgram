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
   
   [Header("Flags")]
   public bool indestructible;
   public bool rotatable = false; //used by cursor
   [ConditionalField("rotatable")]public bool invertRotation;
   public bool actuatable;
   public bool collidable = true;
   public float destroyTime = 1;

   public TooltipFlags ttFlags;
   
   
   //public float blockHeight = 1;
   public Vector2Int size= new (1,1);

   public string description = "";
   [Header("BlockState")]
   public BlockStateSO bso;
}
[Flags]
public enum TooltipFlags{
   None=0,
   //used to be electric, but we dont need it just use item category.
 
   isBurner=4,
   isActuatable=8,
   
        
}
