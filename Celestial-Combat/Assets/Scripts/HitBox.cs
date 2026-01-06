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

    //metoda ki se klice, ko hitbox zadane nek drug collider ki je oznacen kot "trigger"
    void OnTriggerEnter(Collider other){
        HurtBox hurtbox = other.GetComponent<HurtBox>();    //preverimo ce ima ta collider komponento "HurtBox"
        if (hurtbox != null){
            hurtbox.TakeDamage(damageLevel, bloodBodyPart);                     //ce je ima, potem poklicemo njeno metodo za damageLevel
            Debug.Log(owner.name + " meter: " + owner.ultimateMeter);

            if (owner != null) {
                owner.ultimateMeter++;

                if(owner.ultimateMeter > owner.ultimateThreshold) {
                    owner.ultimateMeter = owner.ultimateThreshold;
                }
            }
        
            collider1.enabled = false;
        }
    }

    public void EnableColliderAndSetDamageLevel(int damageLevel, string bodyPart){
        collider1.enabled = true;
        this.damageLevel = damageLevel;
        bloodBodyPart = bodyPart;
    }

    public void DisableCollider(){
        collider1.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider1 = GetComponent<Collider>();
        owner = GetComponentInParent<CharacterBase>();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
