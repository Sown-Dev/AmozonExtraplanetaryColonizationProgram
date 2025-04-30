using Systems.Block;
using TMPro;
using UnityEngine;

namespace UI.BlockUI{
    public class BlockUI: UIWindow{
        [HideInInspector] public Block block;

        
        [SerializeField] public TMP_Text nameText;

    }
}