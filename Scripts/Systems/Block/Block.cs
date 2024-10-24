using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Systems.Block.BlockStates;
using Systems.Items;
using UnityEditor;
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

//#if UNITY_EDITOR  // i know this looks pointless, since it doesnt run in builds anyways, but this is more so it doesnt happen while running the game in editor
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
//#endif

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
            rotation = properties.invertRotation && properties.rotatable ? orientation.GetOpposite() : orientation;
            if(currentState.rotateable){
                currentState.SetOrientation(rotation);
            }
            UpdateSprite();
            
            
            bc.enabled = properties.collidable;

            if (properties.myItem){
                lootTable.Add(new ItemStack(properties.myItem, 1));
            }
            

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
            
            
            foreach (var blockpos in GetPositions()){
                TerrainManager.Instance.BlockLayerRemove(blockpos);
            }


            if (dropLoot){


                /*if (properties?.myItem != null)  We no longer do this. instead add itemdrop to loot table on init
                    Utils.Instance.CreateItemDrop(new ItemStack(properties.myItem, 1));*/
                foreach (ItemStack itemStack in lootTable){
                    Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.125f, 0.125f), UnityEngine.Random.Range(-0.125f, 0.125f));
                    Utils.Instance.CreateItemDrop(itemStack, transform.position + offset);
                }

            }
            return true;
        }
        
        public virtual StringBuilder GetDescription(){
            return new StringBuilder(properties.description);
        }

        public Block GetDirection(Orientation rot){
            return TerrainManager.Instance.GetBlock(
                Vector2Int.RoundToInt((Vector2)transform.position + rot.GetVector2()));
        }
        public List<Block> GetAdjascent(){
            return TerrainManager.Instance.GetAdjacentBlocks(origin, properties.size.x, properties.size.y);
        }

        public List<Vector2Int> GetPositions(){
            return TerrainManager.Instance.GetBlockPositions(origin, properties.size.x, properties.size.y);
        }
        
        public virtual List<Vector2> GetHighlights(){
            return new List<Vector2>();
        }
        
        
        

        public Action UpdateUI;
        
#if UNITY_EDITOR
        void OnDrawGizmos() 
        {
            Handles.Label(transform.position, rotation.ToString());
        }
        #endif
    }

    public enum Orientation{
        Up =0,
        Down =1,
        Left =2,
        Right=3
    }

    public static class OrientationFunction{
        public static Vector2 GetVector2(this Orientation orientation){
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
                    return 0;
                case Orientation.Down:
                    return 180;
                case Orientation.Left:
                    return 90;
                case Orientation.Right: 
                    return 270;
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
                    return Orientation.Left;
                case Orientation.Left:
                    return Orientation.Down;
                case Orientation.Down:
                    return Orientation.Right;
                case Orientation.Right:
                    return Orientation.Up;
                default:
                    return Orientation.Up;
            }
        }
        
        /// <summary>
        /// Rotates a list of Vector2 points around a given origin based on the provided orientation.
        /// </summary>
        /// <param name="points">The list of Vector2 points to rotate.</param>
        /// <param name="orientation">The orientation to rotate to (in increments of 90 degrees).</param>
        /// <param name="origin">The point around which the rotation occurs.</param>
        /// <returns>The rotated list of Vector2 points.</returns>
        public static IEnumerable<Vector2Int> RotateList(this IEnumerable<Vector2Int> points, Orientation orientation, Vector2Int origin)
        {
            float angle = orientation.GetAngle(); // Get the angle in degrees based on the orientation
            float radians = angle * Mathf.Deg2Rad; // Convert the angle to radians
        
            return points.Select(point => RotatePointAroundOrigin(point, radians, origin));
        }

        /// <summary>
        /// Rotates a single Vector2 point around a given origin by the specified angle.
        /// </summary>
        /// <param name="point">The Vector2 point to rotate.</param>
        /// <param name="radians">The angle in radians to rotate by.</param>
        /// <param name="origin">The origin point around which the rotation occurs.</param>
        /// <returns>The rotated Vector2 point.</returns>
        private static Vector2Int RotatePointAroundOrigin(Vector2Int point, float radians, Vector2Int origin)
        {
            // Translate point to origin
            Vector2Int translatedPoint = point - origin;

            // Rotate the point
            float cosTheta = Mathf.Cos(radians);
            float sinTheta = Mathf.Sin(radians);
            float xNew = translatedPoint.x * cosTheta - translatedPoint.y * sinTheta;
            float yNew = translatedPoint.x * sinTheta + translatedPoint.y * cosTheta;

            // Translate the point back to the original position
            Vector2Int rotatedPoint = Vector2Int.RoundToInt( new Vector2(xNew, yNew) + origin);

            return rotatedPoint;
        }

    }
}