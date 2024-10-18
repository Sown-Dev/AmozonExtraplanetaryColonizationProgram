using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerConsumerUI : MonoBehaviour
{
   public IPowerConsumer producer;

   public Image plug;
   public Sprite plugOn;
    public Sprite plugOff;
    public TMP_Text powerText;
    
   
   public void Init (IPowerConsumer _producer){
       this.producer = _producer;
       plug.sprite = producer.myGrid != null ? plugOn : plugOff;
   }
   
   void Update(){
       powerText.text = producer.providedPower + "W";
   }
}
