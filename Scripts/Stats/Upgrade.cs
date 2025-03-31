using System;
using Newtonsoft.Json;
using Systems;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Upgrade : ICloneable, IToolTippable{
    

    public Stats stats;

    public virtual void Init(Unit u){
    }

    public virtual void Remove(){
    }

    public object Clone(){
        Upgrade clone = MemberwiseClone() as Upgrade;
        clone.stats = (Stats)stats.Clone();
        return clone;
    }

    [field:SerializeField]public string name{ get; set; }
    [field:SerializeField] public string description{ get; set; }

    // Instead of a Sprite field, save the path to the Sprite in the Resources folder.
    // For example: "Sprites/Upgrades/SomeUpgradeIcon"
    [SerializeField] public string iconPath;

    // Cache the loaded Sprite (do not serialize this)
     [JsonIgnore] [FormerlySerializedAs("icon")]
    public Sprite iconField;

    // Property that loads the Sprite from Resources on first access.
    [JsonIgnore]
    public Sprite icon{
        get{
            
            if (!string.IsNullOrEmpty(iconPath)){
                return Resources.Load<Sprite>(iconPath);
            }
            else{
                return null;
            }
        }
        set{
            iconField = value;
            //truncate to just resources
            #if UNITY_EDITOR
            iconPath = Utils.GetResourcesPath(iconField);
            #endif
            // Optionally: update iconPath if you have a naming convention.
        }
    }
}