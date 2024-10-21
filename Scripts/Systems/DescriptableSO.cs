using UnityEngine;

namespace Systems{
    public class DescriptableSo : ScriptableObject, IToolTippable{
        [field: SerializeField][field:TextArea(3,5)] public string description{ get; set; }
        [field: SerializeField] public Sprite icon{ get; set; }
    }
}