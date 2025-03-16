using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Systems.Block.BlockStates;
using Systems.Items;
using UI.BlockUI;
using UnityEditor;
using UnityEngine;

namespace Systems.Block{
    public class Block <DataType> : MonoBehaviour where DataType: BlockData, new(){
        public SpriteRenderer sr;
        [HideInInspector] public BoxCollider2D bc;
        protected Animator am;
        protected Material mat;


        public Color baseColor;
        public BlockProperties properties;
        
        public BlockState currentState;
        protected BlockStateHolder stateHolder;

        public RuleTile tile;


        public DataType data;
        


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
            data.rotation = properties.invertRotation && properties.rotatable ? orientation.GetOpposite() : orientation;
            if(currentState.rotateable){
                currentState.SetOrientation(data.rotation);
            }
            UpdateSprite();
            
            
            bc.enabled = properties.collidable;

            if (properties.myItem){
                data.lootTable.Add(new ItemStack(properties.myItem, 1));
            }
            

        }
        public void UpdateSprite(){
            sr.sprite = currentState.CurrentSprite();
        }

        public virtual void Use(Unit user){
            TileIndicatorManager.Instance.DrawIndicators(GetIndicators(), data.origin, data.rotation);
            if(properties.blockUI)
                BlockUIManager.Instance.GenerateBlockUI(this);

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
                foreach (ItemStack itemStack in data.lootTable){
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
            return TerrainManager.Instance.GetAdjacentBlocks(data.origin, properties.size.x, properties.size.y);
        }

        public List<Vector2Int> GetPositions(){
            return TerrainManager.Instance.GetBlockPositions(data.origin, properties.size.x, properties.size.y);
        }
        
        public virtual List<TileIndicator> GetIndicators(){
            return new List<TileIndicator>();
        }

        //prob a more elegant way to do this, but this works
        public virtual void OnUIClose(){
            
        }
        
        
        

        public Action UpdateUI;
        
#if UNITY_EDITOR
        void OnDrawGizmos() 
        {
            Handles.Label(transform.position, data.rotation.ToString());
        }
        #endif


        public virtual void Load(BlockData d){
            data = d;
            
        }
    }
    
    [Serializable]
    public class BlockData{
        
        public List<ItemStack> lootTable;

        
        public Orientation rotation;
        [HideInInspector]public Vector2Int origin; // the origin is kind of the center, except since we can have even sized objects, it 

    }
    

   
}