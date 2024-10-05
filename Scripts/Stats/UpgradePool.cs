using System.Collections.Generic;
using UnityEngine;

namespace Upgrades{
    [CreateAssetMenu(fileName = "Upgrade Pool", menuName = "Upgrade Pool", order = 0)]
    public class UpgradePool : ScriptableObject{
        public string poolName;
        public List<UpgradeSO> upgrades;
    }
}