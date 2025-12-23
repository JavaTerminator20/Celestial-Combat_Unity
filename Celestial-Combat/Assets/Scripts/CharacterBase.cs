using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{   
    protected event Action<int> OnHitReceived;
    bool hitCooldown = false;

    protected float hSpeedUniversal = 1.3f;
    protected float hitExtraSpeed = 2.0f;
    protected GameManager gameManager;

    protected int health = 100;

    //metoda, ki jo poklice HurtBox ko prejme udarec. Ce je hitCooldown vredu, potem poklicemo metodo preko notification systema
    public void TakeDamage(int damage){
        if (!hitCooldown){
            OnHitReceived?.Invoke(damage);        //if anyone is listening to this event, tell them it happened
            hitCooldown = true;
            Invoke("ClearHitCooldown", 0.5f);       //0.5s cooldawna med hiti
        }
    }

    //metoda hit freeze - na podlagi damage (s tem tudi vrste udarca) se doloci freeze time
    protected IEnumerator<WaitForSecondsRealtime> HitFreeze(int damage){
        float hitStopTime;
        switch (damage)
        {
            case 5:
                hitStopTime = 0.06f;
                break;
            case 10:
                hitStopTime = 0.14f;
                break;
            case 15:
                hitStopTime = 0.2f;
                break;
            default:
                hitStopTime = 0.3f;
                break;
        }
        yield return new WaitForSecondsRealtime(0.1f);         //delay da bo hitFreeze malo kasneje zacel - ko bo nasprotnik ze v polozaju za "taking hit"

        Debug.Log("freeze");
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(hitStopTime);
        Time.timeScale = 1f;
        Debug.Log("resume");
    }

    void ClearHitCooldown(){
        hitCooldown = false;
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        health = 100;
    }
}
