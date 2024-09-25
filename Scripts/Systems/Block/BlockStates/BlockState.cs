using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Block.BlockStates{
    [Serializable]
    public class BlockState{
        public SpriteSheet baseSprite;
        public bool rotateable;
        public Orientation orientation;
        public List<SpriteSheet> rotations;
        public SpriteSheet currentSprite;
        private int i;
        
        public BlockState(){
            currentSprite = baseSprite;
            i = 0;
        }
        
        public void SetOrientation(Orientation orientation){
            currentSprite = rotations[(int) orientation];
        }
        public Sprite SetNextSprite(){
            i++;
            if(i>=currentSprite.sprites.Length){
                i = 0;
            }
            return currentSprite.sprites[i];
        }
        public Sprite CurrentSprite(){
            if(i>=currentSprite.sprites.Length){
                i = 0;
            }
            return currentSprite.sprites[i];
        }

    }
    [Serializable]
    public struct SpriteSheet{
        public Sprite[] sprites;
        
    }
    public class BlockStateHolder{
        public BlockState defaultState;
        public BlockState currentState;
        public Dictionary<string, BlockState> states;
        
        public BlockStateHolder(){
            states = new Dictionary<string, BlockState>();
            SetState("default");
        }
        public BlockState SetState(string state){
            if (states.ContainsKey(state)){
                currentState = states[state];
            }
            return currentState;

        }
    }
}