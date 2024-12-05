using System.Collections;
using System.Collections.Generic;
using NewRunMenu;
using UnityEngine;
using UnityEngine.UI;

public class CharacterOption : MonoBehaviour{
   public Image icon;
   public Button button;
   
   public void Init(Character c, CharacterSelect cs){
       icon.sprite = c.portrait;
       icon.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
       button.onClick.AddListener(() => cs.SelectChar(c));
   }
}
