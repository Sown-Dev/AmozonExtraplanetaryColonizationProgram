using System.Collections.Generic;
using System.Linq;
using Systems.Terrain;
using UnityEngine;

namespace Systems.Items{
    public class ItemManager:MonoBehaviour{
        //Singleton responsible for loading all items for anything that needs them (ie not serialized)
    
        public static ItemManager Instance;
    
    
        //data structure that holds all items. by default, they are sorted by tier
        public Dictionary<int, List<Item>> itemDict = new();

        public BlockItem[] blocks;

        public OreProperties[] allOres;
        public List<Item> ores;
        public List<Item> burnables;
    
        void Awake(){
            Instance = this;
            LoadItems();
        }
    
        void LoadItems(){
            allOres= Resources.LoadAll<OreProperties>("Ore");
            Debug.Log("Ores: Loaded" + allOres.Length);

            foreach (OreProperties ore in allOres){
                ores.Add(ore.ore.oreItem);
                
            }

            Item[] items = Resources.LoadAll<Item>("Items");
            
            foreach (Item item in items){
                //Debug.Log("Loaded item " + item.name + " with tier " + item.tier);
                if (!itemDict.ContainsKey(item.tier)){
                    itemDict.Add(item.tier, new List<Item>());
                }
                itemDict[item.tier].Add(item);
                
                if (item.fuelValue > 0){
                    burnables.Add(item);
                }
            
            
            }
            Debug.Log("Items: Loaded " + itemDict.Count);
            blocks=  Resources.LoadAll<BlockItem>("Items/Blocks");
        }

        private void OnValidate(){
            blocks=  Resources.LoadAll<BlockItem>("Items/Blocks");
        }

        public Item[] GetRandomItemsByTier(int tier, int amount){
            //make sure to get randomly
            Utils.Shuffle(itemDict[tier]);
            return itemDict[tier].ToArray().TakeLast(amount).ToArray();
        }
    }
}