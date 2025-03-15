using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "Settings", order = 0)]
    public class GameSettings : ScriptableObject{
        public bool DevMode;
        public List<string> completedTutorials = new List<string>();
        
    }
}