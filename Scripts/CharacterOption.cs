using System;
using System.Collections;
using System.Collections.Generic;
using NewRunMenu;
using UnityEngine;
using UnityEngine.UI;

public class CharacterOption : MonoBehaviour{
   public Image icon;
   public Button button;
   
   public Character character;
   private CharacterSelect characterSelect;
   
   public void Init(Character c, CharacterSelect cs){
       icon.sprite = c.icon;
       button.onClick.AddListener(() => cs.SelectChar(c)); //cant believe i dont use this pattern more often
       character = c;
       characterSelect = cs;
   }

   public void Update(){
       if (characterSelect.selectedChar == character){
           button.Select();
       }
       
   }
}
