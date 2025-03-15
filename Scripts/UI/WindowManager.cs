using System;
using Systems.Items;
using UnityEngine;

namespace UI{
    public class WindowManager: MonoBehaviour{


        public static WindowManager Instance;
        
        public GameObject GridWindowPrefab;
        public UIWindow FilterSelectWindowPrefab;
        
        [HideInInspector]public PowerGridUI currentGridUI;
        
        [HideInInspector] public FilterSelectWindow currentFilterSelectWindow; 
        

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

        public GameObject CreateWindow(UIWindow prefab, Vector2 pos =default){
            GameObject go = Instantiate(prefab, transform).gameObject;
            UIWindow win = go.GetComponent<UIWindow>();
            win.transform.localPosition = pos;
            win.manager = this;
            return go;
        }
        
        public FilterSelectWindow CreateFilterSelectWindow(Filter f){
            if (currentFilterSelectWindow){
                currentFilterSelectWindow.Close();
                currentFilterSelectWindow=null;
            }
            
            FilterSelectWindow win = CreateWindow(FilterSelectWindowPrefab).GetComponent<FilterSelectWindow>();
            currentFilterSelectWindow = win;
            win.Init(f);
            return win;
        }

    }
}