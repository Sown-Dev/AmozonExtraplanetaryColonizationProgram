using UnityEngine;

namespace Systems.BlockUI{
    public class Label : IBlockUI
    {
        public int Priority { get; set; }
        public bool Hidden { get; set; }
        public string text;
        public bool background;
        public Color textColor = Color.white;
        
    }
}