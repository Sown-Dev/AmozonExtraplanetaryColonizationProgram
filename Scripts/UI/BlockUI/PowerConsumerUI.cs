using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerConsumerUI : MonoBehaviour
{
   public IPowerConsumer producer;

   public Image plug;
   public Sprite plugOn;
    public Sprite plugOff;
    
   
   public void Init (IPowerConsumer _producer){
       this.producer = _producer;
       plug.sprite = producer.myGrid != null ? plugOn : plugOff;
   }
}
