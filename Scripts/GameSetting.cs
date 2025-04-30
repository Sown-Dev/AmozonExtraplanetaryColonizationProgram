using System;
using System.Collections.Generic;
using UnityEngine;

    //[CreateAssetMenu(fileName = "FILENAME", menuName = "Settings", order = 0)]
    [Serializable]
    public class GameSettings{
        public bool DevMode;
        public List<string> completedTutorials = new List<string>();

        public float masterVolume = 0.5f;
        public float musicVolume = 0.9f;
        public float sfxVolume = 0.9f;

        public bool TerrainInfo = false;
        public bool OreInfo = true;
        public bool BlockInfo = true;
        
    }
