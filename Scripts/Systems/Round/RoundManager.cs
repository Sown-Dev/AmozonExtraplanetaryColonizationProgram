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
        [SerializeField] private Transform importantUIs;

        //Prefabs
        [SerializeField] private GameObject RoundCompleteUIPrefab;

        //Round info
        public List<ShopOffer> allOffers;

        public List<ShopTier> shopTiers;

        public UpgradeSO[] upgrades;

        public Contract currentContract;

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
            StartRound(new Contract{
                quota = 0, requiredQuota = 300, sellList = ItemManager.Instance.GetRandomItemsByTier(0, 3).ToList(), reward = 50,
                signBonus = 0, TimeGiven = 450, sponsor = Sponsor.Amozon
            });


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
            return currentContract.sellList.Contains(stack.item);
        }

        public void AddMoney(int amount, bool countTowardsQuota = true){
            money += amount;
            if (countTowardsQuota){
                currentContract.quota += amount;
                if (currentContract.quota >= currentContract.requiredQuota){
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


            RoundCompleteUI rc = Instantiate(RoundCompleteUIPrefab, importantUIs)
                .GetComponent<RoundCompleteUI>();

            rc.Init(currentContract.quota, roundTime);
            roundComplete = true;
        }

        public void StartRound(Contract newContract){
            roundComplete = false;

            roundNum++;

            currentContract = newContract;


            roundTime = currentContract.TimeGiven;


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
            int electrical = tier >= 2 ? 1 : 0;
            int refine = tier == 0 ? 0 : 1;
            int production = 1;
            int misc = 2;

// Method to calculate price adjustments
            int AdjustPrice(int roundNum) => Random.Range(-6, 12) - roundNum;

// Select offers for logistics
            ShopOffer[] logisticsOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory == BlockCategory.Logistics)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier * 2))
                .Take(logistics)
                .ToArray();
            tierOffers.RemoveAll(x => logisticsOffers.Any(y => y.item == x.item));

// Select offers for electrical

            ShopOffer[] electricalOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory == BlockCategory.Electrical)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(electrical)
                .ToArray();
            tierOffers.RemoveAll(x => electricalOffers.Any(y => y.item == x.item));

// Select offers for refining
            ShopOffer[] refineOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory == BlockCategory.Refining)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(refine)
                .ToArray();
            tierOffers.RemoveAll(x => refineOffers.Any(y => y.item == x.item));

// Select offers for production
            ShopOffer[] productionOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory == BlockCategory.Production)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(production)
                .ToArray();
            tierOffers.RemoveAll(x => productionOffers.Any(y => y.item == x.item));

// Select misc offers
            ShopOffer[] miscOffers = tierOffers
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(misc)
                .ToArray();
            tierOffers.RemoveAll(x => miscOffers.Any(y => y.item == x.item));


            UpgradeOffer u = new UpgradeOffer(upgrades[Random.Range(0, upgrades.Length)],
                ((tier + 1) * 100 + Random.Range(-20, 20)));


            ShopTier t = new ShopTier(logisticsOffers, electricalOffers, refineOffers, productionOffers, miscOffers, u,
                tier);
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
                var ls = Instantiate(loseGameUI.gameObject, importantUIs).GetComponent<LoseGameUI>();
                ls.transform.SetAsLastSibling();
                ls.LoseScreen(roundStats.moneyEarned, roundStats.ItemsDiscovered.Select(pair => pair.Key).ToList());
            }
        }

        //generates new contracts for next round
        public Contract[] GenerateNewContracts(int numContracts){
            Contract[] contracts = new Contract[numContracts];
            for (int i = 0; i < numContracts; i++){
                contracts[i] = new Contract(roundNum + 1, Random.Range(2, 4));
            }

            return contracts;
        }

        private void OnValidate(){
            //sort by tier.
            //allOffers.Sort((a, b) => a.tier.CompareTo(b.tier));
        }
    }

    public class Contract{
        public int quota;
        public int requiredQuota;
        public List<Item> sellList;
        public int reward;
        public int signBonus;

        public int TimeGiven;
        public Sponsor sponsor;

        public Contract(){ }

        public Contract(int tier, int itemsAmt){
            requiredQuota = (int)((400f * ((tier + 1f) * (tier / 2f)) + 300) / 50) * 50;
            
            sponsor = (Sponsor)Random.Range(0, Sponsor.GetValues(typeof(Sponsor)).Length);
            
            quota = 0;

            

            // Generate random items to be part of the contract
            sellList = ItemManager.Instance.GetRandomItemsByTier(tier, itemsAmt).ToList();

            // Randomly generate reward and sign bonus for the contract
            reward = (Random.Range(80, 160) + (3-sellList.Count)*40) * tier;
            signBonus = 0;

            TimeGiven = 420 + (tier * (90+Random.Range(-60, 45)));

            switch ( sponsor){
                case Sponsor.CorbCO:
                    requiredQuota = (int)(requiredQuota * 1.1f);
                    reward += 100;
                    TimeGiven += 10;
                    break;
                case Sponsor.Anogen:
                    reward /= 2;
                    requiredQuota += 100;
                    TimeGiven += 80;
                    signBonus += 40;
                    break;
                case Sponsor.Silus:
                    TimeGiven -= 25;
                    reward *= 2;
                    requiredQuota -= 100;
                    sellList.RemoveAt(0);
                    sellList.AddRange(ItemManager.Instance.GetRandomItemsByTier(tier+1, 1));
                    
                    break;
                case Sponsor.Toyoma:
                    sellList.AddRange(ItemManager.Instance.GetRandomItemsByTier(tier-1, 1));
                    TimeGiven -= 15;
                    reward = 0;

                    break;
                case Sponsor.Amozon:
                    reward += 25; //Partner bonus
                    break;
                case Sponsor.Pivot:
                    sellList.RemoveRange(1, sellList.Count - 1);
                    TimeGiven -= 45;
                    reward += 500 * tier;
                    requiredQuota/=2;
                    requiredQuota += 200;
                    
                    break;
            }
            
            TimeGiven =  Mathf.RoundToInt(TimeGiven / 15f) * 15;
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