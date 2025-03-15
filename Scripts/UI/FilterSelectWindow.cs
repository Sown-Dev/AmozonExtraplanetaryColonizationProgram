using System.Linq;
using Systems.BlockUI;
using Systems.Items;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class FilterSelectWindow : UIWindow{

        public Filter filter;
        
        public LayoutGroup filterSelectButtonContainer;
        public FilterSelectButton filterSelectButtonPrefab;
        

        public void Init(Filter f){
            filter = f;
            
            
            var empty = Instantiate(filterSelectButtonPrefab, filterSelectButtonContainer.transform);
            empty.Init(null);
            empty.GetComponent<Button>().onClick.AddListener(() => {
                filter.filter = null;
                Close();
            });
            
            
            foreach (var item in ItemManager.Instance.itemDict.Values.SelectMany(t => t)){
                if (item == null) continue;
                var button = Instantiate(filterSelectButtonPrefab, filterSelectButtonContainer.transform);
                button.Init(item);
                button.GetComponent<Button>().onClick.AddListener(() => {
                    filter.filter = item;
                    Close();
                });
            }
        }

    }
    

}
