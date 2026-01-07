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

    protected int health = 15;
  
    public HealthBar healthBar;
    
    protected float initKnockdownSpeed = 17.0f;
    protected float initDecay = 60.0f;

    //audio
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Punch Sounds")]
    public AudioClip punchSound;

    [Header("Punch Miss Sound")]
    public AudioClip punchMissSound;

    [Header("Kick Sound")]
    public AudioClip kickSound;

    [Header("Kick Miss Sound")]
    public AudioClip kickMissSound;

    [Header("Block Sound")]
    public AudioClip blockSound;

    [Header("Hook Sound")]
    public AudioClip hookSound;

    [Header("Hook Miss Sound")]
    public AudioClip hookMissSound;

    [Header("Flying Punch Sound")]
    public AudioClip flyingPunchSound;

    [Header("Flying Punch Miss Sound")]
    public AudioClip flyingPunchMissSound;

    [Header("Sun Ultimate Sounds")]
    public AudioClip sunStompSound;
    public AudioClip beamSound;

    [Header("Earth Ultimate Sounds")]
    public AudioClip earthStompSound;
    public AudioClip moonThrowSound;

    //dictionary ki mapira Int(damageLevel) -> Tuple(damageAmount, hitStopTime, CameraShakeIntensity)
    protected Dictionary<int, Tuple<int, float, float>> damageDefinition = new Dictionary<int, Tuple<int, float, float>>{
        {1, new Tuple<int, float, float>(2, 0.04f, 0.1f)},
        {2, new Tuple<int, float, float>(5, 0.07f, 0.15f)},
        {3, new Tuple<int, float, float>(8, 0.10f, 0.2f)},
        {4, new Tuple<int, float, float>(10, 0.13f, 0.25f)},
    };
    

    //metoda, ki jo poklice HurtBox ko prejme udarec. Ce je hitCooldown vredu, potem poklicemo metodo preko notification systema
    public void TakeDamage(int damageLevel, string bodyPart){
        
        bool isBlocking = false;

        // Check PlayerController
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null && pc.blocking)
            isBlocking = true;

        // Check OpponentController
        OponentController oc = GetComponent<OponentController>();
        if (oc != null && oc.blocking)
            isBlocking = true;

        // If blocking, play block sound and exit
        if (isBlocking)
        {
            PlayBlockSound();
            Debug.Log("BLOCKED!");
        }

        
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

    void Start()  
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }
        
        

    }

    public void PlayPunchSound()
    {
        if (punchSound != null)
            audioSource.PlayOneShot(punchSound);
    }

    public void PlayPunchMissSound()
    {
        if (punchMissSound != null)
            audioSource.PlayOneShot(punchMissSound);
    }

    public void PlayKickSound()
    {
        if (kickSound != null)
            audioSource.PlayOneShot(kickSound);
    }

    public void PlayKickMissSound()
    {
        if (kickMissSound != null)
            audioSource.PlayOneShot(kickMissSound);
    }

    public void PlayBlockSound()
    {
        if (blockSound != null && audioSource != null)
            audioSource.PlayOneShot(blockSound);
    }

    public void PlayHookSound()
    {
        if (hookSound != null)
            audioSource.PlayOneShot(hookSound);
    }

    public void PlayHookMissSound()
    {
        if (hookMissSound != null)
            audioSource.PlayOneShot(hookMissSound);
    }

    public void PlayFlyingPunchSound()
    {
        if (flyingPunchSound != null)
            audioSource.PlayOneShot(flyingPunchSound);
    }

    public void PlayFlyingPunchMissSound()
    {
        if (flyingPunchMissSound != null)
            audioSource.PlayOneShot(flyingPunchMissSound);
    }

    public void PlaySunStompSound() 
    {
        if(sunStompSound != null) {
            audioSource.clip = sunStompSound;
            audioSource.time = 1f;
            audioSource.Play();
        }
    }

    public void PlayBeamSound()
    {
        if(beamSound != null)
            audioSource.PlayOneShot(beamSound);
    }

    public void PlayEarthStompSound()
    {
        if(earthStompSound != null) {
            audioSource.clip = earthStompSound;
            audioSource.time = 1f;
            audioSource.Play();
        }
    }

    public void PlayMoonHitSound()
    {
        if(moonThrowSound != null)
            audioSource.PlayOneShot(moonThrowSound);
    }

}
