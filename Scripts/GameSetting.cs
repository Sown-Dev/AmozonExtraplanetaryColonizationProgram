using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "Settings", order = 0)]
    public class GameSettings : ScriptableObject{
        public bool DevMode;
        public List<string> completedTutorials = new List<string>();

        public float masterVolume = 0.5f;
        public float musicVolume = 0.9f;
        public float sfxVolume = 0.9f;

    }
}