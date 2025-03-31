
using System;

namespace Systems.Round{
    [Serializable]
    public class ShopTier{
        public int tier;
        public ShopOffer[] logistics;
        public ShopOffer[] electrical;
        public ShopOffer[] refinement;
        public ShopOffer[] production;
        public ShopOffer[] misc;
        public ShopOffer[] explosives;

        
        
        public UpgradeOffer upgradeOffer;

        
        public ShopTier(){
        }

        public ShopTier(ShopOffer[] _logistics, ShopOffer[] _electrical, ShopOffer[] _refinement, ShopOffer[] _production, ShopOffer[] _misc, ShopOffer[] _explosives,
            UpgradeOffer _upgrade, int _tier){
            logistics = _logistics;
            electrical = _electrical;
            refinement = _refinement;
            production = _production;
            explosives = _explosives;
            misc = _misc;
            upgradeOffer = _upgrade;
            tier = _tier;
        }
    }
}