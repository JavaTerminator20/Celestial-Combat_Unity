using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HitBox : MonoBehaviour
{

    private Collider collider1;
    int damageLevel = 0;
    string bloodBodyPart;
    public CharacterBase owner;
    bool hasHit = false;
    public string currentAction;

    //metoda ki se klice, ko hitbox zadane nek drug collider ki je oznacen kot "trigger"
    void OnTriggerEnter(Collider other){
        //Debug.Log("HITBOX TRIGGERED!");
        HurtBox hurtbox = other.GetComponent<HurtBox>();    //preverimo ce ima ta collider komponento "HurtBox"
        if (hurtbox != null){
            hurtbox.TakeDamage(damageLevel, bloodBodyPart);                     //ce je ima, potem poklicemo njeno metodo za damageLevel
            Debug.Log(owner.name + " meter: " + owner.ultimateMeter);

            hasHit = true;

            PlayHitSound();
            
            if (owner != null) {
                owner.ultimateMeter++;

                if(owner.ultimateMeter > owner.ultimateThreshold) {
                    owner.ultimateMeter = owner.ultimateThreshold;
                }
            }
        
            collider1.enabled = false;
        }
    }

    public void EnableColliderAndSetDamageLevel(int damageLevel, string bodyPart, string actionName){
        collider1.enabled = true;
        this.damageLevel = damageLevel;
        bloodBodyPart = bodyPart;
        currentAction = actionName;
        hasHit = false;
    }

    public void DisableCollider(){
        collider1.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider1 = GetComponent<Collider>();
        owner = GetComponentInParent<CharacterBase>();
        //Debug.Log("HitBox owner = " + (owner != null ? owner.name : "NULL"));
    }

    //sounds
    void PlayHitSound() {
        //Debug.Log("HIT SOUND: " + currentAction);
        if (owner == null) { 
            //Debug.Log("OWNER IS NULL");
            return;
        }

        switch (currentAction) {

            case "punch":
                owner.PlayPunchSound();
                break;

            case "hook":
                owner.PlayHookSound();
                break;

            case "kick":
                owner.PlayKickSound();
                break;

            default:
                Debug.Log("UNKNOWN ACTION: " + currentAction);
                break;

        }
    }

    void PlayMissSound() {
        if (owner == null) return;

        switch (currentAction)
        {
            case "punch":
                owner.PlayPunchMissSound();
                break;

            case "hook":
                owner.PlayHookMissSound();
                break;

            case "kick":
                owner.PlayKickMissSound();
                break;
        }
    }

    public void CheckForMiss()
    {
        //Debug.Log("CheckForMiss called. hasHit = " + hasHit + " action = " + currentAction);


        if(!hasHit) PlayMissSound();

        hasHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
