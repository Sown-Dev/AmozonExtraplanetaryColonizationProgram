using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class Layer<ObjectType> where ObjectType: class{


    private Dictionary<Vector2Int, ObjectType> layer;


    public Layer(){
        layer = new Dictionary<Vector2Int, ObjectType>();
    }
    
    [CanBeNull]
    public ObjectType Get(Vector2Int position){
        if(layer.ContainsKey(position)){
            return layer[position];
        }
        return null;
    }
    
    public void Set(Vector2Int position, ObjectType obj){
        layer[position] = obj;
    }
    
    //TODO: NOT THIS
    public void Remove(ObjectType obj){
        foreach (var pair in layer){
            if (pair.Value == obj){
                layer.Remove(pair.Key);
                return;
            }
        }
    }
    
    public void Remove(Vector2Int position){
        if(layer.ContainsKey(position)){
            layer.Remove(position);
        }
    }
    public Dictionary<Vector2Int, ObjectType> GetDictionary(){
        return layer;
    }

}