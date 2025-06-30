using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Systems.Items;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Systems.Round{
    public class RoundManager : MonoBehaviour{
        public static RoundManager Instance;

        public List<Item[]> contractItems = new List<Item[]>();

        //References
        [SerializeField] private RoundInfoUI infoUI;
        [SerializeField] private Transform importantUIs;

        //Prefabs
        [SerializeField] private GameObject RoundCompleteUIPrefab;
        [SerializeField] private GameObject ContractSelectUIPrefab;

        //Round info
        [FormerlySerializedAs("allOffers")] public List<ShopOffer> blockOffers;
        public List<ShopOffer> itemOffers;
        public List<ShopTier> shopTiers;
        public UpgradeSO[] upgrades;
        public Contract currentContract;

        public int money;
        public int roundNum{ private set; get; }
        public float roundTime;

        public float prevCDTime = 0f;
        private bool isInCooldown = false;

        //loans
        public int debt;
        public int loansTaken;
        public int loanLimit = 3;
        public int loanAmount = 100;
        public bool loansUnlocked = true;


        public float addTime;

        int lives = 0;
        bool lostGame;


        public WorldStats roundStats;

        public LoseGameUI loseGameUI;


        void Awake(){
            Instance = this;

            shopTiers = new List<ShopTier>();

            roundStats = new WorldStats();
            blockOffers.Sort((a, b) => a.tier.CompareTo(b.tier));

            if (!GameManager.Instance.currentWorld.generated){
                InitRound();
            }
            else{
                LoadRoundData(GameManager.Instance.currentWorld.roundData);
            }
        }

        private void Start(){
            /*StartRound(new Contract{
                quota = 0, requiredQuota = 300, sellList = ItemManager.Instance.GetRandomItemsByTier(0, 3).ToList(), reward = 50,
                signBonus = 0, TimeGiven = 450, sponsor = Sponsor.Amozon
            });*/
            infoUI.Refresh();

            CursorManager.Instance.uiDepth = 0;
        }

        public void InitRound(){
            StartCooldown(-1);
            roundNum = -1;
        
            
            
            loansTaken = 0;
            loanAmount = 100 * (roundNum + 2);

            money = 100;
            infoUI.Refresh();
        }


        // Modified FixedUpdate to handle cooldown
        private void FixedUpdate(){
            if (!isInCooldown && currentContract != null){
                // Handle contract timer
                if (roundTime > 0){
                    roundTime -= Time.deltaTime;
                }
                else if (!lostGame){
                    LoseRound();
                }
            }
            else{
                if (isInCooldown){
                    //indefinite cooldown
                    if (roundTime == -1){ }
                    else{
                        // Handle cooldown timer
                        prevCDTime = roundTime;
                        roundTime -= Time.deltaTime;
                        if (prevCDTime > 3.5f && roundTime <= 3.5f){
                            Player.Instance.Popup("Contract Incoming!", Color.yellow);
                        }

                        if (roundTime <= 0){
                            isInCooldown = false;
                            ChooseContract();
                            //open contract ui
                        }
                    }
                }
            }

            if (GameManager.Instance.settings.DevMode){
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
        public void Sell(ItemStack stack, float rate=1){
            AddMoney(Mathf.RoundToInt(stack.item.value * stack.amount * rate));
            stack.amount = 0;
        }

        public bool CanSell(ItemStack stack){
            return currentContract.sellList.Contains(stack.item.name);
        }

        public void AddMoney(int amount, bool countTowardsQuota = true){
            money += amount;

            if (amount > 0){
                Player.Instance.Popup("+" + amount + "$", new Color(0.1f, 1f, 0.5f));
            }

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

        public void CompleteRound(){
            StartCooldown(50);
            GenerateNewShopTier(roundNum + 1);
            infoUI.Refresh();
            
            loansTaken = 0;
            loanAmount = 100 * (roundNum + 1);

        }       

        // Modified StartRound to handle null contracts
        private bool firstRound = true;

        public void StartRound(Contract newContract){
            if (newContract == null){
                Debug.LogWarning("Tried to start round with null contract!");
                return;
            }

            GC.Collect();


            roundComplete = false;
            roundNum++;
            currentContract = newContract;
            roundTime = currentContract.TimeGiven;

            addTime += Player.Instance.finalStats[Statstype.ExtraTime];
            
            if (addTime > 0){
                roundTime += addTime;
                addTime = 0;
            }
            int extraMoney = Mathf.RoundToInt(Player.Instance.finalStats[Statstype.ExtraMoney]);
            if(extraMoney > 0){
               AddMoney(extraMoney,false);
            }

            if (firstRound){
                firstRound = false;
                GenerateNewShopTier(roundNum);
            }


            infoUI.Refresh();
            
            TutorialManager.Instance.StartTutorial("firstcontract", 0.1f);

        }

        public void GenerateNewShopTier(int tierNum){
            shopTiers.Add(GenerateShop(tierNum));
            Player.Instance?.Popup("New items available in shop!", Color.yellow);
        }

        public void StartCooldown(float duration){
            currentContract = null;
            roundTime = duration;
            isInCooldown = true;
            infoUI.Refresh();
        }

        public void ChooseContract(){
            int contractNum = roundNum > -1 ? 3 : 2;
            ContractSelectUI ui = Instantiate(ContractSelectUIPrefab, importantUIs).GetComponent<ContractSelectUI>();
            ui.Init(GenerateNewContracts(contractNum));
            TutorialManager.Instance.StartTutorial("contracts");
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
            Utils.Shuffle(blockOffers);
            Utils.Shuffle(itemOffers);
            List<ShopOffer> tierOffers = blockOffers.Where(t => t.tier == tier).ToList();

            // These will be pulled from world stats when added
            int logistics = tier > 0 ? 1 : 0;
            int electrical = tier >= 2 ? 1 : 0;
            int refine = tier == 0 ? 0 : 1;
            int production = 1;
            int misc = tier == 0 ? 3 : 2;
            int explosives = 1;

            // Method to calculate price adjustments
            int AdjustPrice(int roundNum) => Random.Range(-6, 12) - roundNum;

            // Select offers for logistics
            ShopOffer[] logisticsOffers = tierOffers
                .Where(t => !t.overrideCategory)
                .Where(t => (t.item as BlockItem)?.blockCategory.HasFlag(BlockCategory.Logistics) ?? false)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier * 2))
                .Take(logistics)
                .Union(
                    tierOffers
                        .Where(t => t.overrideCategory && t.blockCategory.HasFlag(BlockCategory.Logistics))
                        .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier * 2))
                        .Take(logistics)
                )
                .ToArray();

            tierOffers.RemoveAll(x => logisticsOffers.Any(y => y.item == x.item));

            // Select offers for electrical
            ShopOffer[] electricalOffers = tierOffers
                .Where(t => !t.overrideCategory)
                .Where(t => (t.item as BlockItem)?.blockCategory.HasFlag(BlockCategory.Electrical) ?? false)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(electrical)
                .Union(
                    tierOffers
                        .Where(t => t.overrideCategory && t.blockCategory.HasFlag(BlockCategory.Electrical))
                        .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                        .Take(electrical)
                )
                .ToArray();
            tierOffers.RemoveAll(x => electricalOffers.Any(y => y.item == x.item));

            // Select offers for refining
            ShopOffer[] refineOffers = tierOffers
                .Where(t => !t.overrideCategory)
                .Where(t => (t.item as BlockItem)?.blockCategory.HasFlag(BlockCategory.Refining) ?? false)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(refine)
                .Union(
                    tierOffers
                        .Where(t => t.overrideCategory && t.blockCategory.HasFlag(BlockCategory.Refining))
                        .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                        .Take(refine)
                )
                .ToArray();
            tierOffers.RemoveAll(x => refineOffers.Any(y => y.item == x.item));

            // Select offers for production
            ShopOffer[] productionOffers = tierOffers
                .Where(t => !t.overrideCategory)
                .Where(t => (t.item as BlockItem)?.blockCategory.HasFlag(BlockCategory.Production) ?? false)
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(production)
                .Union(
                    tierOffers
                        .Where(t => t.overrideCategory && t.blockCategory.HasFlag(BlockCategory.Production))
                        .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                        .Take(production)
                )
                .ToArray();
            tierOffers.RemoveAll(x => productionOffers.Any(y => y.item == x.item));

            // Select misc offers
            ShopOffer[] miscOffers = tierOffers
                .Select(t => new ShopOffer(t, AdjustPrice(roundNum), tier))
                .Take(misc)
                .ToArray();
            tierOffers.RemoveAll(x => miscOffers.Any(y => y.item == x.item));

            ShopOffer[] explosivesOffers = itemOffers
                .Where(t => (t.item?.category == ItemCategory.Explosive) && t.tier <= tier)
                .Select(t => new ShopOffer(t, (int)(t.price * (0.15f * tier)), t.stock * tier))
                .Take(explosives)
                .ToArray();

            UpgradeOffer u = new UpgradeOffer( (Upgrade)upgrades[Random.Range(0, upgrades.Length)].u.Clone(),
                ((tier + 1) * 100 + Random.Range(-20, 20)));

            ShopTier t = new ShopTier(logisticsOffers, electricalOffers, refineOffers, productionOffers, miscOffers, explosivesOffers, u, tier);
            ShopUI.Instance.Refresh(); //prob should be elsewhere but this works
            return t;
        }

        private float interestRate = 1.5f;

        public void TakeLoan(){
            if (loansTaken < loanLimit && loansUnlocked){
                AddMoney(loanAmount, false);
                debt += (int)(loanAmount * interestRate);
                loansTaken += 1;
            }
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
                allSponsors = new List<Sponsor>{ Sponsor.Amozon, Sponsor.Pivot, Sponsor.Anogen };
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
        }

        public RoundData SaveRoundData(){
            RoundData data = new RoundData();
            data.roundNum = roundNum;
            data.roundTime = roundTime;
            data.isInCooldown = isInCooldown;
            data.money = money;
            data.debt = debt;
            data.loansTaken = loansTaken;
            data.loanLimit = loanLimit;
            data.loanAmount = loanAmount;
            if (currentContract != null)
                data.currentContract = currentContract;
            else{
                data.currentContract = null;
            }

            data.shopTiers = shopTiers;

            Debug.Log("Saved round data, current contract is null: " + (currentContract == null));
            return data;
        }

        public void LoadRoundData(RoundData data){
            roundNum = data.roundNum;
            roundTime = data.roundTime;
            isInCooldown = data.isInCooldown;
            money = data.money;
            debt = data.debt;
            loansTaken = data.loansTaken;
            loanLimit = data.loanLimit;
            loanAmount = data.loanAmount;
            currentContract = data.currentContract;
            shopTiers = data.shopTiers;
        }
    }

    [Serializable]
    public class RoundData{
        public int roundNum;
        public bool isInCooldown;
        public float roundTime;
        public int money;
        public int debt;
        public int loansTaken;
        public int loanLimit = 3;
        public int loanAmount = 100;

        public Contract currentContract;
        public List<ShopTier> shopTiers = new();
    }
}