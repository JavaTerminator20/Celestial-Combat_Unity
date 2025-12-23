using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HurtBox : MonoBehaviour
{

    CharacterBase characterBase;

    public void TakeDamage(int amount){
        characterBase.TakeDamage(amount);   //poklicemo metodo v BaseCharacterju za taking damage ... on se potem odloci ali se to zgodi ali ne (blocking, grounded...)
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterBase = GetComponentInParent<CharacterBase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
