using System.Collections.Generic;
using System.Linq;
using Systems.Items;
using UnityEngine;

namespace Systems.Round{
    public class Contract{
        public int quota;
        public int requiredQuota;
        public List<Item> sellList;
        public int reward;
        public int signBonus;

        public float TimeGiven;
        public Sponsor sponsor;

        public Contract(){ }

        public Contract(int tier, int itemsAmt, Sponsor s){
            requiredQuota = (int)((500f * ((tier + 1f) * (tier / 2f)) + 300) / 25) * 25;

            sponsor = s; // (Sponsor)Random.Range(0, Sponsor.GetValues(typeof(Sponsor)).Length);

            quota = 0;


            // Generate random items to be part of the contract
            sellList = new List<Item>();
            // Randomly generate reward and sign bonus for the contract
            reward = (Random.Range(80, 160) + (3 - sellList.Count) * 40) * tier;
            signBonus = 0;

            TimeGiven = 440 + (tier * (100 + Random.Range(0, 20))) + Random.Range(-20, 20);

            switch (s){
                case Sponsor.CorbCO:
                    requiredQuota = (int)(requiredQuota * 1.1f);
                    reward += 100;
                    TimeGiven += 10;
                    break;
                case Sponsor.Anogen:
                    reward /= 2;
                    requiredQuota += 200*tier;
                    TimeGiven += 45*tier;
                    signBonus += 100;
                    break;
                case Sponsor.Silus:
                    TimeGiven -= 25;
                    reward *= 2;
                    requiredQuota -= 100;
                    itemsAmt-=1;
                    sellList.AddRange(ItemManager.Instance.GetRandomItemsByTier(tier + 1, 1));

                    break;
                case Sponsor.Toyoma:
                    sellList.AddRange(ItemManager.Instance.GetRandomItemsByTier(tier - 1, 1));
                    TimeGiven -= 60;
                    reward = 0;
                    TimeGiven *= 1.2f;

                    break;
                case Sponsor.Amozon:
                    reward += 100; //Partner bonus
                    break;
                case Sponsor.Pivot:
                    itemsAmt = 1;
                    TimeGiven -= 45;
                    reward = 500 * tier +250;
                    requiredQuota /= 2;
                    requiredQuota += 200;

                    break;
            }
            
            sellList = ItemManager.Instance.GetRandomItemsByTier(tier, itemsAmt).ToList();


            TimeGiven = Mathf.RoundToInt(TimeGiven / 15f) * 15;
            requiredQuota = requiredQuota / 50 * 50;
            reward = reward / 10 * 10;
            // Debug.Log($"New contract generated for tier {tier} with {itemsAmt} items. Required quota: {requiredQuota}, Reward: {reward}, SignBonus: {signBonus}");
        }
    }

    public enum Sponsor{
        CorbCO,
        Anogen,
        Silus,
        Toyoma,
        Amozon,
        Pivot,
    }
}