using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AnimationComponent : MonoBehaviour{
    public Image image;
    private Color originalImageColor;
    public Vector4 ExtraColor;
        
    private void Awake(){
        originalImageColor = image.color;
    }

    public Vector4 ImageColorAdd{
        get{ return ExtraColor; }
        set{
            if (ExtraColor != value){
                ExtraColor = value;
                if (Application.isPlaying){
                    image.color = originalImageColor + (Color)ExtraColor;
                }
            }
        }
    }

    private void Update(){
        image.color = originalImageColor + (Color)ExtraColor;
    }
}