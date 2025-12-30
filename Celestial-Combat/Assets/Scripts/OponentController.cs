using System;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class OponentController : CharacterBase
{
    public enum FighterAction
    {
        idle = 0,
        punch = 1,
        hook = 2,
        kick = 3,
        block = 4,
        frontFlip = 5,
        backFlip = 6,
        jump = 7
    }

    private Animator animator;
    public ParticleSystem blockSparks;

    public ParticleSystem PlayerLeftHand;         //to do komponente od igralca
    public ParticleSystem PlayerRightHand;
    public ParticleSystem PlayerRightLeg;

    void Start()
    {
        OnHitReceived += GettingHit;        //dodamo metodo na seznam "poslusalcev" - potrebno bi bilo sicer ga odstraniti pri OnDisable(){OnHitReceived -= GettingHit;}
        animator = GetComponent<Animator>();
        hSpeed = base.hSpeedUniversal;
    }

    void MakeAction(){
        int action = UnityEngine.Random.Range(0, 5);
        animator.SetInteger("action", action);

    }

    public void SunUltFloating(){
        animator.SetBool("sunUlt", true);
        animator.SetBool("hit", true);      //zato da se prekine akcija ki se trenutno izvaja
        animator.SetInteger("action", 0);
        animator.SetInteger("dir", 0);
        weAreHit = true;                //preprecimo izvajanje akcij
        Invoke("clearHit", 0.4f);
    }
    void Move(){
        int dir = (int)Time.time % 2;
        if (dir == 0){dir = -1;}
        animator.SetInteger("dir", dir);   
    }

    // Update is called once per frame
    float decisionTimer = 0.8f;
    float moveTimer = 0.5f;
    float hSpeed;
    public int orientation = -1;
    bool weAreHit = false;
    bool blocking = false;

    void clearHit(){
        animator.SetBool("hit", false);
        weAreHit = false;
        animator.SetBool("knockdown", false);
    }
    
    float knockDownMeter;
    float decayRate = 2.0f;
    float lastHitTime = 0.0f;
    void checkKnockDown(int damage){
        float comboBonus = 1.0f;
        if (Time.time - lastHitTime < 1.5f){    //ce damo dva zaporedna udarca, potem dodaj combo bonus na knockdownmeter
            comboBonus = 2.0f;
            Debug.Log("we got combo bonus!!");
        }
        if (Time.time - lastHitTime > 3.5f){    //resetiramo knockdown meter ce pretece prevec casa med udarci
            knockDownMeter = 0;
        }

        knockDownMeter += damage*comboBonus;

        if (knockDownMeter > 20.0f){
            knockDownMeter += damage;
            animator.SetBool("knockdown", true);
            animator.SetInteger("action", 0);
            animator.SetInteger("dir", 0);
            knockDownMeter = 0.0f;
            
        } else{
            animator.SetBool("hit", true);
            animator.SetInteger("action", 0);
            animator.SetInteger("dir", 0);
        }
    }

    void GettingHit(int damage, float hitStopTime, float cameraShakeIntensity, string bloodBodyPart){
        if (!blocking){
            health -= damage;
            Debug.Log("oponent got hit, current health: " + health);
            checkKnockDown(damage);
            lastHitTime = Time.time;
            weAreHit = true;
            Invoke("clearHit", 0.4f);
            StartCoroutine(HitFreeze(hitStopTime));                                 //zazenemo hitFreeze efekt
            CameraShake(cameraShakeIntensity, false);
            playBloodVFX(bloodBodyPart, PlayerLeftHand, PlayerRightHand, null, PlayerRightLeg);
        } else{
            CameraShake(cameraShakeIntensity, true);
            blockSparks.Play();
        }
    }        

    float knockDownSpeed;
    float decayFactor;
    bool initKDS = true;

    void Update()
    {
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        knockDownMeter -= decayRate * Time.unscaledDeltaTime;
        if (knockDownMeter < 0) knockDownMeter = 0;
        //Debug.Log(knockDownMeter);

        if (false){
            decisionTimer -= Time.deltaTime;
            if (decisionTimer < 0){
                decisionTimer = 0.8f;
                MakeAction();
            }

            moveTimer -= Time.deltaTime;
            if (moveTimer < 0){
                moveTimer = 0.5f;
                Move();
            }

            if (animInfo.IsName("Armature|stepForward")){
                transform.position += new Vector3(hSpeed*Time.deltaTime*orientation, 0, 0);
            }

            if (animInfo.IsName("Armature|block")){
                blocking = true;
            } else{
                blocking = false;
            }
            
        }

        if (animInfo.IsName("Armature_knockdown")){
            if (initKDS){
                knockDownSpeed = 17.0f;
                initKDS = false;
                decayFactor = 50.0f;
            }
            if (animInfo.normalizedTime < 0.5f){
                transform.position += new Vector3(-knockDownSpeed * Time.deltaTime*orientation, 0f, 0f);
                knockDownSpeed -= decayFactor*Time.deltaTime;
                if (knockDownSpeed < 0){knockDownSpeed = 0;}
                //Debug.Log(knockDownSpeed);
            }
            //Debug.Log(knockDownSpeed);
        } else{
            initKDS = true;
        }

        if (animInfo.IsName("Armature_hitStepBackBlended")){
            if (animInfo.normalizedTime < 0.15f){
                transform.position += new Vector3(-(hSpeed+hitExtraSpeed)*Time.deltaTime*orientation, 0, 0);    //dejanski step back
            }
        }
    }
}
