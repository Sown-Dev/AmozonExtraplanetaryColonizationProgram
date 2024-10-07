using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerProducerUI : MonoBehaviour
{
   public IPowerProducer producer;

   public Image plug;
   public Sprite plugOn;
    public Sprite plugOff;
    
   
   public void Init (IPowerProducer _producer){
       this.producer = _producer;
       plug.sprite = producer.myGrid != null ? plugOn : plugOff;
   }
}
