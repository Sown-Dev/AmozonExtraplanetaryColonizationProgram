using Systems.Block.BlockStates;
using UnityEngine;

namespace Systems.Block.BlockStates{
    [CreateAssetMenu(fileName = "BlockState", menuName = "BlockState")]
    public class BlockStateSO:ScriptableObject{
        public BlockState defaultState;
        public virtual BlockStateHolder GenerateBlockState(){
           BlockStateHolder bsh = new BlockStateHolder();
           bsh.defaultState = defaultState;
           return bsh;
        }
    }
}