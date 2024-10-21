using UnityEngine;

public interface IToolTippable{
    public string name{ get; }
    [field: SerializeField]public string description{get;}
    public Sprite icon{get;}
}