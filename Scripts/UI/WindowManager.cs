using System;
using UnityEngine;

namespace UI{
    public class WindowManager: MonoBehaviour{


        public static WindowManager Instance;
        
        public GameObject GridWindowPrefab;
        
        [HideInInspector]public PowerGridUI currentGridUI;
        

        private void Awake(){
            
            Instance = this;
        }
        public PowerGridUI CreateGridWindow(PowerGrid grid){
            if(currentGridUI != null)
                Destroy(currentGridUI?.gameObject);
            
            PowerGridUI gridUI = Instantiate(GridWindowPrefab, transform).GetComponent<PowerGridUI>();
            gridUI.Init(grid);
            currentGridUI = gridUI;
            return gridUI;
        }

        public UIWindow CreateWindow(UIWindow prefab, Vector2 pos =default){
            UIWindow win = Instantiate(prefab, transform).GetComponent<UIWindow>();
            win.transform.localPosition = pos;
            win.manager = this;
            return win;
        }

    }
}