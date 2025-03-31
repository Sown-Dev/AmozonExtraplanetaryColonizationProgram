using System;
using Newtonsoft.Json;
using Systems.BlockUI;
using UnityEngine.Serialization;

namespace Systems.Items{
    [Serializable]
    public class Filter: IBlockUI{

        public string filterID;

        [JsonIgnore]
        public Item filter {
            get {
                if (string.IsNullOrEmpty(filterID))
                    return null;
                if (ItemManager.Instance)
                    return ItemManager.Instance.GetItemByID(filterID);
                return null;
            }
            set => filterID = ItemManager.Instance.GetItemID(value);
        }        public int Priority{ get; set; }
        public bool Hidden{ get; set; }
        
        public Filter(){
            Priority = 0;
            Hidden = false;
        }
        public Filter(Item filter){
            this.filter = filter;
            Priority = 0;
            Hidden = false;
        }
    }
}