using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HitBox : MonoBehaviour
{

    private Collider collider1;
    int damageLevel = 0;
    string bloodBodyPart;

    //metoda ki se klice, ko hitbox zadane nek drug collider ki je oznacen kot "trigger"
    void OnTriggerEnter(Collider other){
        HurtBox hurtbox = other.GetComponent<HurtBox>();    //preverimo ce ima ta collider komponento "HurtBox"
        if (hurtbox != null){
            hurtbox.TakeDamage(damageLevel, bloodBodyPart);                     //ce je ima, potem poklicemo njeno metodo za damageLevel
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
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
