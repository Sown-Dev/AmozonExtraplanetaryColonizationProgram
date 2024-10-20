using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Items;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Systems.Round{
    public class RoundManager : MonoBehaviour{
        public static RoundManager Instance;


        //References
        [SerializeField] private RoundInfoUI infoUI;

        //Prefabs
        [SerializeField] private GameObject RoundCompleteUIPrefab;

        //Round info
        public List<ShopOffer> allOffers;

        public List<ShopTier> shopTiers;

        public UpgradeSO[] upgrades;


        public List<Item> sellList;

        public int quotaRequired;
        public int quota;

        public int money;
        public int roundNum{ private set; get; }

        public float roundTime;

        int lives = 0;

        public WorldStats roundStats;
        public WorldStats myStats;

        public LoseGameUI loseGameUI;


        bool lostGame;


        void Awake(){
            roundStats = new WorldStats();
            shopTiers = new List<ShopTier>();
        }

        private void Start(){
            Instance = this;
            roundNum = -1;
            StartRound();


            money = 100;
            infoUI.Refresh();
            
            CursorManager.Instance.uiDepth = 0;
        }

        private void FixedUpdate(){
            if (roundTime >= 0){
                roundTime -= Time.deltaTime;
            }

            if (roundTime <= 0 && !lostGame){
                //lose game
                LoseRound();
            }
#if ALLITEMS1
            if (Input.GetKey(KeyCode.F1)){
                roundTime -= 1;
            }

            if (Input.GetKey(KeyCode.F2)){
                AddMoney(95);
            }

            if (Input.GetKeyDown(KeyCode.F4)){
                RegenerateShop();
            }

            if (Input.GetKeyDown(KeyCode.F5)){
                RegenerateRoundShop();
            }
#endif
        }


        public void SaveStats(){
            string json = JsonUtility.ToJson(myStats);
            PlayerPrefs.SetString("PlayerStats", json);
            PlayerPrefs.Save();
        }

        public void LoadStats(){
            if (PlayerPrefs.HasKey("PlayerStats")){
                string json = PlayerPrefs.GetString("PlayerStats");
                myStats = JsonUtility.FromJson<WorldStats>(json);
            }
            else{
                myStats = new WorldStats(); // Default if no data is saved
            }
        }

        //only call after checking if the item is in the sell list
        public void Sell(ItemStack stack){
            AddMoney(stack.item.value * stack.amount);
            stack.amount = 0;
        }

        public bool CanSell(ItemStack stack){
            return sellList.Contains(stack.item);
        }

        public void AddMoney(int amount, bool countTowardsQuota=true){
            money += amount;
            if (countTowardsQuota){
                quota += amount;
                if (quota >= quotaRequired){
                    //trigger quota reached
                    if (!roundComplete)
                        QuotaReach();
                }
                
            }

            infoUI.Refresh();
        }

        public bool SpendMoney(int amount){
            if (money >= amount){
                money -= amount;
                infoUI.Refresh();
                return true;
            }

            return false;
        }
        
        
        bool roundComplete;
        //Triggers when we have reached the quota
        public void QuotaReach(){
                        CursorManager.Instance.OpenUI();


            RoundCompleteUI rc = Instantiate(RoundCompleteUIPrefab, infoUI.transform.parent)
                .GetComponent<RoundCompleteUI>();
            
            rc.Init(quota, roundTime);
            roundComplete = true;
        }

        public void StartRound(){
            roundComplete = false;
            
            roundNum++;
            quota = 0;
            if (roundNum > 0){
                quotaRequired = (int)(500f * ((roundNum + 1f) * (roundNum / 2f)));
            }
            else{
                quotaRequired = 300; //low first quota to not be boring
            }

            roundTime = 420f + roundNum * 30;

            //get new sell list
            sellList = new List<Item>(ItemManager.Instance.GetRandomItemsByTier(roundNum, 3 + roundNum / 2));

            //get new shop list
            /*shopList = new List<ShopOffer>();

            //delete this if its slow
            Utils.Shuffle(allOffers);

            shopList.AddRange(allOffers.Where(t => t.tier == roundNum).Take(Random.Range(3, 5)).Select(t => new ShopOffer(t, Random.Range(-6, 12) - roundNum)));
            shopList.AddRange(allOffers.Where(t => t.tier == roundNum - 1).Take(1).Select(t => new ShopOffer(t, Random.Range(-6, 12) - roundNum)));
            shopList.Add(new ShopOffer( Utils.Instance.dynamight, 100, roundNum, roundNum*3+3 ));
            allOffers.Sort((a, b) => a.tier.CompareTo(b.tier));*/

            shopTiers.Add(GenerateShop(roundNum));


            infoUI.Refresh();
        }

        public void RegenerateRoundShop(){
            shopTiers[roundNum] = GenerateShop(roundNum);
            infoUI.Refresh();
        }

        public void RegenerateShop(){
            for (int i = 0; i <= roundNum; i++){
                shopTiers[i] = GenerateShop(i);
            }

            infoUI.Refresh();
        }

        public ShopTier GenerateShop(int tier){
            // Shuffle the list
            Utils.Shuffle(allOffers);
            List<ShopOffer> tierOffers = allOffers.Where(t => t.tier == tier).ToList();

// These will be pulled from world stats when added
            int logistics = 1;
            int electrical =  tier >= 2 ? 1 : 0;
            int refine = tier == 0 ? 0 : 1;
            int production = 1;
            int misc = 2;

// Method to calculate price adjustments
            int AdjustPrice(int roundNum) => Random.Range(-6, 12) - roundNum;

// Select offers for logistics
            ShopOffer[] logisticsOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory == BlockCategory.Logistics)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum)))
                .Take(logistics)
                .ToArray();
            tierOffers.RemoveAll(x => logisticsOffers.Any(y => y.item == x.item));
            
// Select offers for electrical

            ShopOffer[] electricalOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory == BlockCategory.Electrical)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum)))
                .Take(electrical)
                .ToArray();
            tierOffers.RemoveAll(x => electricalOffers.Any(y => y.item == x.item));

// Select offers for refining
            ShopOffer[] refineOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory == BlockCategory.Refining)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum)))
                .Take(refine)
                .ToArray();
            tierOffers.RemoveAll(x => refineOffers.Any(y => y.item == x.item));

// Select offers for production
            ShopOffer[] productionOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory == BlockCategory.Production)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum)))
                .Take(production)
                .ToArray();
            tierOffers.RemoveAll(x => productionOffers.Any(y => y.item == x.item));

// Select misc offers
            ShopOffer[] miscOffers = tierOffers
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum)))
                .Take(misc)
                .ToArray();
            tierOffers.RemoveAll(x => miscOffers.Any(y => y.item == x.item));


            UpgradeOffer u = new UpgradeOffer(upgrades[Random.Range(0, upgrades.Length)],
                ((tier + 1) * 100 + Random.Range(-20, 20)));


            ShopTier t = new ShopTier(logisticsOffers, electricalOffers,refineOffers, productionOffers, miscOffers, u, tier);
            return t;
        }

        public void LoseRound(){
            if (lives > 0){
                lives -= 1;
                money /= 2;
                money -= 50;
                roundTime += 150;
            }
            else{
                lostGame = true;
                //lose for real
                var ls = Instantiate(loseGameUI.gameObject, infoUI.transform.parent).GetComponent<LoseGameUI>();
                ls.transform.SetAsLastSibling();
                ls.LoseScreen(roundStats.moneyEarned, roundStats.ItemsDiscovered.Select(pair => pair.Key).ToList());
            }
        }

        private void OnValidate(){
            //sort by tier.
            allOffers.Sort((a, b) => a.tier.CompareTo(b.tier));
        }
    }
}