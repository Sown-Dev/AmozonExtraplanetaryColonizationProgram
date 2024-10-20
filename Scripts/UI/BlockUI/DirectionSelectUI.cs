using System;
using Systems.Block;
using Systems.BlockUI;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BlockUI{
    public class DirectionSelectUI: MonoBehaviour{

        public Sprite insertButtonOn;
        public Sprite insertButtonOff;
        public Sprite extractButtonOn;
        public Sprite extractButtonOff;
        
    
        public DirectionSelect directionSelect;

        public Button upInsert;
        public Button downInsert;
        public Button leftInsert;
        public Button rightInsert;
        
        public Button upExtract;
        public Button downExtract;
        public Button leftExtract;
        public Button rightExtract;

        private void Start(){
            //set button actions
            upInsert.onClick.AddListener(() => SetInputOrientation((int)Orientation.Up));
            downInsert.onClick.AddListener(() => SetInputOrientation((int)Orientation.Down));
            leftInsert.onClick.AddListener(() => SetInputOrientation((int)Orientation.Left));
            rightInsert.onClick.AddListener(() => SetInputOrientation((int)Orientation.Right));

            // Set button actions for extract buttons
            upExtract.onClick.AddListener(() => SetOutputOrientation((int)Orientation.Up));
            downExtract.onClick.AddListener(() => SetOutputOrientation((int)Orientation.Down));
            leftExtract.onClick.AddListener(() => SetOutputOrientation((int)Orientation.Left));
            rightExtract.onClick.AddListener(() => SetOutputOrientation((int)Orientation.Right));
        }

        private void Update(){
            upInsert.image.sprite = directionSelect.inputOrientation == Orientation.Up ? insertButtonOn : insertButtonOff;
            downInsert.image.sprite = directionSelect.inputOrientation == Orientation.Down ? insertButtonOn : insertButtonOff;
            leftInsert.image.sprite = directionSelect.inputOrientation == Orientation.Left ? insertButtonOn : insertButtonOff;
            rightInsert.image.sprite = directionSelect.inputOrientation == Orientation.Right ? insertButtonOn : insertButtonOff;
            
            upExtract.image.sprite = directionSelect.outputOrientation == Orientation.Up ? extractButtonOn : extractButtonOff;
            downExtract.image.sprite = directionSelect.outputOrientation == Orientation.Down ? extractButtonOn : extractButtonOff;
            leftExtract.image.sprite = directionSelect.outputOrientation == Orientation.Left ? extractButtonOn : extractButtonOff;
            rightExtract.image.sprite = directionSelect.outputOrientation == Orientation.Right ? extractButtonOn : extractButtonOff;
        }
        
        public void SetInputOrientation(int orientation){
            directionSelect.inputOrientation = orientation.GetOrientationInt();
        }
        
        public void SetOutputOrientation(int orientation){
            directionSelect.outputOrientation = orientation.GetOrientationInt();
        }
    }
}