using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class PowerConsumerUI : ElectricityUI
{
    public IPowerConsumer producer;
    
    public Image plug;
    public Button plugButton;
    public Sprite plugOn;
    public Sprite plugOff;
    public TMP_Text powerText;

   
    public void Init (IPowerConsumer _producer){
        this.producer = _producer;
        plug.sprite = producer.myGrid != null ? plugOn : plugOff;
        plugButton.onClick.AddListener(() => WindowManager.Instance.CreateGridWindow( producer.myGrid));

    }
   
    void Update(){
        powerText.text =  producer.providedPower >0 ? $"{producer.providedPower}W/{producer.needed}W" : $"{producer.providedPower}W/{producer.needed}W";

    }
}

