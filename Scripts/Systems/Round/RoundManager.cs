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
        [SerializeField] private GameObject ContractSelectUIPrefab;

        //Round info
        public List<ShopOffer> allOffers;
        public List<ShopTier> shopTiers;
        public UpgradeSO[] upgrades;
        public Contract currentContract;

        public int money;
        public int roundNum{ private set; get; }
        public float roundTime;

        public float cooldownTimer = 0f;
        private bool isInCooldown = false;


        public float addTime;

        int lives = 0;
        bool lostGame;


        public WorldStats roundStats;

        public LoseGameUI loseGameUI;


        void Awake(){
            roundStats = new WorldStats();
            shopTiers = new List<ShopTier>();
            roundStats = new WorldStats();
        }

        private void Start(){
            Instance = this;
            roundNum = -1;
            /*StartRound(new Contract{
                quota = 0, requiredQuota = 300, sellList = ItemManager.Instance.GetRandomItemsByTier(0, 3).ToList(), reward = 50,
                signBonus = 0, TimeGiven = 450, sponsor = Sponsor.Amozon
            });*/

            StartCooldown(30f);


            money = 100;
            infoUI.Refresh();

            CursorManager.Instance.uiDepth = 0;
        }


        // Modified FixedUpdate to handle cooldown
        private void FixedUpdate(){
            if (currentContract != null){
                // Handle contract timer
                if (roundTime > 0){
                    roundTime -= Time.deltaTime;
                }
                else if (!lostGame){
                    LoseRound();
                }
            }
            else if (isInCooldown){
                //indefinite cooldown
                if (cooldownTimer == -1){ }
                else{
                    // Handle cooldown timer
                    cooldownTimer -= Time.deltaTime;
                    if (cooldownTimer <= 0){
                        isInCooldown = false;
                        ChooseContract();
                        //open contract ui
                    }
                }
            }

            if (GameManager.DevMode){
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

            if (money >= 1000){
                GameManager.Instance.UnlockAchievement("1000_DOLLARS");
            }

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

        public void AddTime(float time){
            roundTime += time;
        }


        bool roundComplete;

        // Modified QuotaReach to trigger cooldown
        public void QuotaReach(){
            CursorManager.Instance.OpenUI();

            if (roundNum == 0){
                GameManager.Instance.UnlockAchievement("FIRST_ROUND");
            }

            RoundCompleteUI rc = Instantiate(RoundCompleteUIPrefab, importantUIs)
                .GetComponent<RoundCompleteUI>();

            rc.Init(currentContract.quota, roundTime);
            roundComplete = true;

           
        }

        // Modified StartRound to handle null contracts
        public void StartRound(Contract newContract){
            if (newContract == null){
                Debug.LogWarning("Tried to start round with null contract!");
                return;
            }
            
            

            roundComplete = false;
            roundNum++;
            currentContract = newContract;
            roundTime = currentContract.TimeGiven;

            if (addTime > 0){
                roundTime += addTime;
                addTime = 0;
            }

            shopTiers.Add(GenerateShop(roundNum));


            infoUI.Refresh();
            
            TutorialManager.Instance.StartTutorial("firstcontract", 0.1f);
        }

        public void StartCooldown(float duration){
            currentContract = null;
            cooldownTimer = duration;
            isInCooldown = true;
            infoUI.Refresh();
        }

        public void ChooseContract(){
            int contractNum = roundNum > -1 ? 3 : 2;
            ContractSelectUI ui = Instantiate(ContractSelectUIPrefab, importantUIs).GetComponent<ContractSelectUI>();
            ui.Init(GenerateNewContracts(contractNum));
            TutorialManager.Instance.StartTutorial("contracts",0.2f);

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
            int logistics = tier > 0 ? 1 : 0;
            int electrical = tier >= 2 ? 1 : 0;
            int refine = tier == 0 ? 0 : 1;
            int production = 1;
            int misc = 2;

// Method to calculate price adjustments
            int AdjustPrice(int roundNum) => Random.Range(-6, 12) - roundNum;

// Select offers for logistics
            ShopOffer[] logisticsOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory.HasFlag(BlockCategory.Logistics) ?? false)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier * 2))
                .Take(logistics)
                .ToArray();
            tierOffers.RemoveAll(x => logisticsOffers.Any(y => y.item == x.item));

// Select offers for electrical

            ShopOffer[] electricalOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory.HasFlag(BlockCategory.Electrical) ?? false)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(electrical)
                .ToArray();
            tierOffers.RemoveAll(x => electricalOffers.Any(y => y.item == x.item));

// Select offers for refining
            ShopOffer[] refineOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory.HasFlag(BlockCategory.Refining) ?? false)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(refine)
                .ToArray();
            tierOffers.RemoveAll(x => refineOffers.Any(y => y.item == x.item));

// Select offers for production
            ShopOffer[] productionOffers = tierOffers
                .Where(t => (t.item as BlockItem)?.blockCategory.HasFlag(BlockCategory.Production) ?? false)
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
            List<Sponsor> allSponsors = Enum.GetValues(typeof(Sponsor)).Cast<Sponsor>().ToList();
            if (roundNum == -1){
                allSponsors = new List<Sponsor>{Sponsor.Amozon, Sponsor.Pivot, Sponsor.Anogen};
            }
            for (int i = 0; i < numContracts; i++){
                
                Sponsor s = allSponsors[Random.Range(0, allSponsors.Count)];
               
                contracts[i] = new Contract(roundNum + 1, Random.Range(2, 4), s);
                allSponsors.Remove(s);
            }

            return contracts;
        }

        private void OnValidate(){
            //sort by tier.
            //allOffers.Sort((a, b) => a.tier.CompareTo(b.tier));
        }
    }
}