using UnityEngine;

public interface IDescriptable{
    public string name{ get; }
    [field: SerializeField]public string description{get;}
    public Sprite icon{get;}
}