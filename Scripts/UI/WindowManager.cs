using System;
using UnityEngine;

namespace UI{
    public class WindowManager: MonoBehaviour{


        public static WindowManager Instance;

        private void Awake(){
            
            Instance = this;
        }


        public UIWindow CreateWindow(UIWindow prefab, Vector2 pos =default){
            UIWindow win = Instantiate(prefab, transform).GetComponent<UIWindow>();
            win.transform.localPosition = pos;
            win.manager = this;
            return win;
        }

    }
}