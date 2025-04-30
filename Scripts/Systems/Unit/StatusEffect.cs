using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class StatusEffect{
    public Debuffs.DebuffTypes type;
    public Sprite icon;
    public GameObject particlePrefab; //actual prefab
    [HideInInspector]public GameObject particles; //reference
    public float duration;
    public float strength =1;
    public bool isStats;
    public Stats stats;
    public float maxTime = 20;
    public bool addStrength;
    public bool isBuff =true;

    public StatusEffect(float dur, float str){
        duration = dur;
        strength = str;
    }


    private float tickElapsed;
    public virtual void Tick( IStatusEffectable st, float buffMult=1, float debuffMult=1){
        //reducing duration is done in debuffs
        float max = maxTime;
        if (isBuff){
            max *= buffMult;
        }
        else{
            max *= debuffMult;
        }
        if (duration > max){
            duration = max;
        }

       
    }

    public virtual void Init(Transform parent){
        particles = GameObject.Instantiate(particlePrefab, parent);
    }

    public virtual void Remove(){
        GameObject.Destroy(particles);
    }

    public virtual StatusEffect Clone(){
        StatusEffect clone = new StatusEffect( duration,strength);
        clone.particlePrefab = particlePrefab;
        clone.type = type;
        clone.isStats = isStats;
        clone.stats = stats; //don't clone so that we can compare
        clone.maxTime = maxTime;
        clone.addStrength = addStrength;
        clone.icon= icon;
        clone.isBuff = isBuff;
        

        return clone;
    }
}