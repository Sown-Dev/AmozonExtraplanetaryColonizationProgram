using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Block.BlockStates;
using Systems.Items;
using UnityEngine;

namespace Systems.Block{
    public class Block : MonoBehaviour{
        public SpriteRenderer sr;
        [HideInInspector] public BoxCollider2D bc;
        protected Animator am;
        protected Material mat;


        public Color baseColor;
        public BlockProperties properties;
        
        public BlockState currentState;
        protected BlockStateHolder stateHolder;

        public List<ItemStack> lootTable;

        
        public Orientation rotation;
        public RuleTile tile;

        [HideInInspector]public Vector2Int origin; // the origin is kind of the center, except since we can have even sized objects, it 

#if UNITY_EDITOR
        private void OnValidate(){
            baseColor = /* TODO: MAYBE FIX THIS??? Utils.FindMostProminentColor(sr.sprite) ??*/
                new Color(0.35f, 0.32f, 0.27f);
            
            properties.name = name;

            sr = GetComponent<SpriteRenderer>();

            bc = GetComponent<BoxCollider2D>();

            try{
                properties.myItem = ItemManager.Instance.blocks.First(x => x.blockPrefab == this);
            }
            catch (Exception e){
               // Debug.LogError("Block " + name + " does not have a corresponding item");
            }
        }
#endif

        protected virtual void Awake(){
            if(currentState == null || currentState?.baseSprite.sprites?.Length == 0){
                currentState = new BlockState();
                currentState.baseSprite = new SpriteSheet(){sprites = new[] {sr.sprite}};
                
            }
            currentState.currentSprite = currentState.baseSprite;

            am = GetComponent<Animator>();
            mat = sr.material;
            Deselect();
            mat.SetFloat("_PixelsPerUnit", sr.sprite.texture.width);
        }

        public virtual void Init(Orientation orientation){
            bc.size = new Vector2(properties.size.x - 1 / 8f, properties.size.y - 1 / 8f); //remove 1 pixel on each
            rotation = orientation;
            if(currentState.rotateable){
                currentState.SetOrientation(rotation);
            }
            UpdateSprite();
            

        }
        public void UpdateSprite(){
            sr.sprite = currentState.CurrentSprite();
        }

        public virtual void Use(Unit user){
            Debug.Log("Used " + this.GetType().Name);
        }

        public virtual void Actuate(){
            Utils.Actuate(this);
        }

        public void Select(){
            mat.SetFloat("_OutlineThickness", 1);
            mat.SetColor("_OutlineColor", Color.white);
        }

        public void Deselect(){
            mat.SetFloat("_OutlineThickness", 0);
            mat.SetColor("_OutlineColor", new Color(22 / 255f, 21 / 255f, 24 / 255f));
        }


        public Action DestroyAction;

        public void OnDestroy(){
            DestroyAction?.Invoke();
        }
        
        //true if block gets destroyed
        public virtual bool BlockDestroy(bool dropLoot = true){
            Destroy(gameObject);
            TerrainManager.Instance.BlockLayerRemove(origin);


            if (dropLoot){


                if (properties?.myItem != null)
                    Utils.Instance.CreateItemDrop(new ItemStack(properties.myItem, 1), transform.position);
                foreach (ItemStack itemStack in lootTable){
                    Utils.Instance.CreateItemDrop(itemStack, transform.position);
                }

            }
            return true;
        }
        
        public virtual string GetDescription(){
            return properties.description;
        }

        public Block GetDirection(Orientation rot){
            return TerrainManager.Instance.GetBlock(
                Vector2Int.RoundToInt((Vector2)transform.position + rot.GetVector()));
        }
        
        

        public Action UpdateUI;
        
        
    }

    public enum Orientation{
        Up =0,
        Down =1,
        Left =2,
        Right=3
    }

    public static class OrientationFunction{
        public static Vector2 GetVector(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Vector2.up;
                case Orientation.Down:
                    return Vector2.down;
                case Orientation.Left:
                    return Vector2.left;
                case Orientation.Right:
                    return Vector2.right;
                default:
                    return Vector2.zero;
            }
        }
         public static Vector3 GetVector3(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Vector2.up;
                case Orientation.Down:
                    return Vector2.down;
                case Orientation.Left:
                    return Vector2.left;
                case Orientation.Right:
                    return Vector2.right;
                default:
                    return Vector2.zero;
            }
        }
        public static Vector2Int GetVectorInt(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Vector2Int.up;
                case Orientation.Down:
                    return Vector2Int.down;
                case Orientation.Left:
                    return Vector2Int.left;
                case Orientation.Right:
                    return Vector2Int.right;
                default:
                    return Vector2Int.zero;
            }
        }

        public static Orientation GetOrientation(this Vector2 vector){
            if (vector == Vector2.up){
                return Orientation.Up;
            }

            if (vector == Vector2.down){
                return Orientation.Down;
            }

            if (vector == Vector2.left){
                return Orientation.Left;
            }

            if (vector == Vector2.right){
                return Orientation.Right;
            }

            return Orientation.Up;
        }

        public static float GetAngle(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return 90;
                case Orientation.Down:
                    return 270;
                case Orientation.Left:
                    return 180;
                case Orientation.Right:
                    return 0;
                default:
                    return 0;
            }
        }

        public static Orientation GetOrientationInt(this int number){
            switch (number){
                case 0:
                    return Orientation.Up;
                case 1:
                    return Orientation.Down;
                case 2:
                    return Orientation.Left;
                case 3:
                    return Orientation.Right;
                default:
                    return Orientation.Up;
            }
        }
        
        public static Orientation GetOpposite(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Orientation.Down;
                case Orientation.Down:
                    return Orientation.Up;
                case Orientation.Left:
                    return Orientation.Right;
                case Orientation.Right:
                    return Orientation.Left;
                default:
                    return Orientation.Up;
            }
        }
        public static bool isVertical(this Orientation orientation){
            return orientation == Orientation.Up || orientation == Orientation.Down;
        }
        
        public static Orientation next(this Orientation orientation){
            switch (orientation){
                case Orientation.Up:
                    return Orientation.Right;
                case Orientation.Right:
                    return Orientation.Down;
                case Orientation.Down:
                    return Orientation.Left;
                case Orientation.Left:
                    return Orientation.Up;
                default:
                    return Orientation.Up;
            }
        }

    }
}