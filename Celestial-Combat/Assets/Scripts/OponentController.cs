using System;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
        jump = 7, 
        flyingPunch = 8,
        ultimate = 9,
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
        int action = UnityEngine.Random.Range(2, 4);
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

    float decisionTimer = 0.8f;
    float moveTimer = 0.5f;
    float hSpeed;
    public int orientation = -1;
    bool weAreHit = false;
    bool blocking = false;
    public bool canChangeOrientation = true;


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
        if (!blocking && grounded){
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

    public GameObject ogMoon;
    public GameObject handMoon;
    public GameObject airMoon;
    public void MoonSwitch(){
        ogMoon.SetActive(false);
        handMoon.SetActive(true);
    }

    public void MoonThrow(){
        airMoon.GetComponent<Transform>().position = new Vector3(transform.position.x + 1.405f*orientation, transform.position.y + 1.619f, transform.position.z + 0.479f*(-orientation));
        handMoon.SetActive(false);
        airMoon.SetActive(true);
        airMoon.GetComponent<Rigidbody>().useGravity = true;
        float velocityY = -250f + gameManager.GetDistance()*30f;    
        airMoon.GetComponent<Rigidbody>().AddForce(new Vector3(700f*orientation, velocityY, 60f*orientation));
    }

    public void BringBackMoon(){
        ogMoon.SetActive(true);
    }        

    float knockDownSpeed;
    float decayFactor;
    bool initKDS = true;

    //spremenljivke za flipe
    public bool grounded = true;
    bool paramsInit = false;
    float gravity;
    float vSpeed;


    void Update()
    {
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        knockDownMeter -= decayRate * Time.unscaledDeltaTime;
        if (knockDownMeter < 0) knockDownMeter = 0;
        //Debug.Log(knockDownMeter);

        if (!weAreHit){
            decisionTimer -= Time.deltaTime;
            if (decisionTimer < 0){
                decisionTimer = 0.8f;
                MakeAction();
            }

            // moveTimer -= Time.deltaTime;
            // if (moveTimer < 0){
            //     moveTimer = 0.5f;
            //     Move();
            // }

            if (animInfo.IsName("Armature|stepForward")){
                transform.position += new Vector3(hSpeed*Time.deltaTime*orientation, 0, 0);
            }

            if (animInfo.IsName("Armature|block")){
                blocking = true;
            } else{
                blocking = false;
            }   

            string curAnimPlaying = clipInfo[0].clip.name;

            if (curAnimPlaying == "Armature_backflip"){  //0.67, 1.37
                grounded = false;
                float currentTime = animInfo.normalizedTime * animInfo.length;
                float extraHSpeed = 3.0f;
                //nastavimo zacetne parametre
                if (!paramsInit){
                    gravity = 60.0f;
                    vSpeed = gravity*(0.632f*animInfo.length - 0.254f*animInfo.length)/2;       //v oklepajih je dolzina skoka (sekunde)
                    paramsInit = true;
                }

                //ce smo znotraj pravega casovnega okvirja, potem zacnemo premikati objekt v loku
                if (currentTime > 0.254*animInfo.length && currentTime < 0.632*animInfo.length){
                    vSpeed -= gravity * Time.deltaTime;
                    transform.position += new Vector3(-(hSpeed+extraHSpeed)*Time.deltaTime*orientation, vSpeed*Time.deltaTime, 0.0f);
                    
                    if (transform.position.y < 1){
                        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                    }
                }
            }

            //premikanje ob predvajanju animacije | TODO: prestavi zacetni del kode direktno pod animacijo (OnStateEnter)
            if (curAnimPlaying == "Armature_frontFlip"){
                grounded = false;
                float currentTime = animInfo.normalizedTime * animInfo.length;
                float extraHSpeed = 3.5f;             //dodatek k horizontalni hitrosti
                //nastavimo zacetne parametre
                if (!paramsInit){
                    gravity = 40.0f;
                    vSpeed = gravity*(0.880f*animInfo.length - 0.282f*animInfo.length)/2;       //v oklepajih je dolzina skoka (sekunde)
                    paramsInit = true;
                }
                transform.position += new Vector3(hSpeed*1.6f*Time.deltaTime*orientation, 0, 0);
                //ce smo znotraj pravega casovnega okvirja, potem zacnemo premikati objekt v loku
                if (currentTime > 0.282*animInfo.length && currentTime < 0.880*animInfo.length){
                    vSpeed -= gravity * Time.deltaTime;
                    transform.position += new Vector3(extraHSpeed*Time.deltaTime*orientation, vSpeed*Time.deltaTime, 0.0f);
                    
                    if (transform.position.y < 1){
                        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                    }
                }
            }

            if (curAnimPlaying == "Armature_jump"){
                grounded = false;
                
                if (!paramsInit){
                    paramsInit = true;
                    gravity = 130.0f;
                    vSpeed = gravity*(0.6f*animInfo.length - 0.257f*animInfo.length)/2;
                }

                if (animInfo.normalizedTime > 0.257f && animInfo.normalizedTime < 0.6f){
                    vSpeed -= gravity * Time.deltaTime;
                    transform.position += new Vector3(0.0f, vSpeed*Time.deltaTime, 0.0f);
                    
                    if (transform.position.y < 1){
                        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                    }
                }
            }

            if (curAnimPlaying == "Armature_flyingPunch"){
                if (animInfo.normalizedTime < 0.65f){
                    transform.position += new Vector3((hSpeed+1.0f)*Time.deltaTime*orientation, 0f, 0f);
                }
            }

            if (curAnimPlaying == "Armature|idle"){
                paramsInit = false;
                grounded = true;
            }
            
        }

        if (animInfo.IsName("Armature_knockdown")){
            if (initKDS){
                knockDownSpeed = initKnockdownSpeed;
                decayFactor = initDecay;
                initKDS = false;
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
