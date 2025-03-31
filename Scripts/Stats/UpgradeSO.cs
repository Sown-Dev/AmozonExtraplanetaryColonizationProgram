using System;
using Unity.VisualScripting;
using UnityEngine;


//IMPORTANT: we don't actually use this scriptableobject, its just necessary for custom upgrades to inherit from
public class UpgradeSO : ScriptableObject{
    public virtual Upgrade u{ get; set; }
    
    public virtual Upgrade Create(){
        return (Upgrade)u.Clone();
    }

    #if UNITY_EDITOR
    public void OnValidate(){
        u.name = name;
        u.iconPath = Utils.GetResourcesPath(u.iconField);
    }
    #endif
}