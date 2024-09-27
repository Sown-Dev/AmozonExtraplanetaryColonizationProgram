using System.Collections.Generic;
using System.Linq;
using Systems.Items;
using UI;
using UnityEngine;

namespace Systems.Round{
    public class RoundManager : MonoBehaviour{
        public static RoundManager Instance;


        //References
        [SerializeField] private RoundInfoUI infoUI;


        //Round info
        public List<ShopOffer> allOffers;

        public List<ShopTier> shopTiers;


        public List<Item> sellList;

        public int quotaRequired;
        public int quota;

        public int money;
        private int roundNum;

        public float roundTime;

        int lives = 0;

        public Stats roundStats;
        public Stats myStats;

        public LoseGameUI loseGameUI;


        bool lostGame;


        void Awake(){
            roundStats = new Stats();
            shopTiers = new List<ShopTier>();
        }

        private void Start(){
            Instance = this;
            roundNum = -1;
            StartRound();


            money = 100;
            infoUI.Refresh();
        }

        private void Update(){
            if (roundTime >= 0){
                roundTime -= Time.deltaTime;
            }

            if (roundTime <= 0 && !lostGame){
                //lose game
                LoseRound();
            }
        }


        public void SaveStats(){
            string json = JsonUtility.ToJson(myStats);
            PlayerPrefs.SetString("PlayerStats", json);
            PlayerPrefs.Save();
        }

        public void LoadStats(){
            if (PlayerPrefs.HasKey("PlayerStats")){
                string json = PlayerPrefs.GetString("PlayerStats");
                myStats = JsonUtility.FromJson<Stats>(json);
            }
            else{
                myStats = new Stats(); // Default if no data is saved
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

        public void AddMoney(int amount){
            money += amount;
            quota += amount;
            if (quota >= quotaRequired){
                quota = 0;
                //next round;
                StartRound();
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

        public void StartRound(){
            roundNum++;
            quota = 0;
            if (roundNum > 0){
                quotaRequired = 600 * ((roundNum + 1) * roundNum + 1);
            }
            else{
                quotaRequired = 300; //low first quota to not be boring
            }

            roundTime = 370f;

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

        public ShopTier GenerateShop(int tier){
            Utils.Shuffle(allOffers);
            ShopOffer[] tierOffers = allOffers.Where(t => t.tier == tier).ToArray();

            //these will be pulled from world stats when added
            int logistics = 1;
            int refine = 1;
            int storage = 1;

            ShopOffer[] logisticsOffers = tierOffers
                .Where(t => (t.item as BlockItem).blockCategory == BlockCategory.Logistics)
                .Select(t => new ShopOffer(t, Random.Range(-6, 12) - roundNum)).Take(logistics).ToArray();
            ShopOffer[] refineOffers = tierOffers
                .Where(t => (t.item as BlockItem).blockCategory == BlockCategory.Refining)
                .Select(t => new ShopOffer(t, Random.Range(-6, 12) - roundNum)).Take(refine).ToArray();


            ShopTier t = new ShopTier(logisticsOffers, refineOffers, null, tier);
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
    }
}