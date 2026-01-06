using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{   
    protected event Action<int, float, float, string> OnHitReceived;      //definiramo listener
    bool hitCooldown = false;

    protected float hSpeedUniversal = 1.3f;     //horizontal speed obeh igralcev
    protected float hitExtraSpeed = 3.0f;       //konckback speed - koliko se premakne nazaj ko je hittan
    public GameManager gameManager;
    public int ultimateMeter = 0;
    public int ultimateThreshold = 10;
    public bool playerIsUsingUltimate = false;

    protected int health = 100;

    protected float initKnockdownSpeed = 17.0f;
    protected float initDecay = 60.0f;

    //dictionary ki mapira Int(damageLevel) -> Tuple(damageAmount, hitStopTime, CameraShakeIntensity)
    protected Dictionary<int, Tuple<int, float, float>> damageDefinition = new Dictionary<int, Tuple<int, float, float>>{
        {1, new Tuple<int, float, float>(2, 0.04f, 0.1f)},
        {2, new Tuple<int, float, float>(5, 0.07f, 0.15f)},
        {3, new Tuple<int, float, float>(8, 0.10f, 0.2f)},
        {4, new Tuple<int, float, float>(10, 0.13f, 0.25f)},
    };
       

    //metoda, ki jo poklice HurtBox ko prejme udarec. Ce je hitCooldown vredu, potem poklicemo metodo preko notification systema
    public void TakeDamage(int damageLevel, string bodyPart){
        int damage = damageDefinition[damageLevel].Item1;
        float hitStoptime = damageDefinition[damageLevel].Item2;
        float shakeIntensity = damageDefinition[damageLevel].Item3;

        //if (!hitCooldown){
        //    OnHitReceived?.Invoke(damage, hitStoptime, shakeIntensity, bodyPart);        //if anyone is listening to this event, tell them it happened (zaradi argumenta uporabimo Listener)
        //    hitCooldown = true;
        //    Invoke("ClearHitCooldown", 0.3f);       //0.5s cooldawna med hiti
        //}

        OnHitReceived?.Invoke(damage, hitStoptime, shakeIntensity, bodyPart);
    }

    //metoda hit freeze - na podlagi damage (s tem tudi vrste udarca) se doloci freeze time
    protected IEnumerator<WaitForSecondsRealtime> HitFreeze(float hitStopTime){
        yield return new WaitForSecondsRealtime(0.0f);         //delay da bo hitFreeze malo kasneje zacel - ko bo nasprotnik ze v polozaju za "taking hit"
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopTime);
        Time.timeScale = 1f;
    }

    protected void CameraShake(float strength, bool block){
        gameManager.CameraShake(strength, block);
    }

    void ClearHitCooldown(){
        hitCooldown = false;
    }

    protected void playBloodVFX(string bloodBodyPart, ParticleSystem LH, ParticleSystem RH, ParticleSystem LL, ParticleSystem RL){
        switch(bloodBodyPart){
            case "LH":
                LH.Play();
                break;
            
            case "RH":
                RH.Play();
                break;

            case "RL":
                RL.Play();
                break;
        }
    }

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        health = 100;
    }
}
