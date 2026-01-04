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

    public enum AIState
    {
        Idle,
        Approach,
        Attack,
        Retreat,
        Block,
        Evade,
        Recover
    }

    private AIState currentState = AIState.Idle;

    private Animator animator;
    public ParticleSystem blockSparks;

    public Transform player;
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
    //float distance = Mathf.Abs(player.position.x - transform.position.x);
    float punchRange = 1.385f;
    float hookRange = 1.4f;
    float kickRange = 2f;
    float safeRange = 2.5f;
    int rawDir = 0;
    int dir = 0;
    public bool canMove = true;
    //public int ultimateMeter = 0;
    //public int ultimateThreshold = 10;

    float DistanceToPlayer() {
        return Mathf.Abs(player.position.x - transform.position.x);
    }

    void clearHit(){
        animator.SetBool("hit", false);
        weAreHit = false;
        //animator.SetBool("knockdown", false);
    }
    
    float knockDownMeter;
    float decayRate = 2.0f;
    float lastHitTime = 0.0f;
    void checkKnockDown(int damage){
        if (animator.GetBool("knockdown")) return;

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

    public void EndKnockDown() {
        animator.SetBool("knockdown", false);
        animator.SetBool("hit", false);

        weAreHit = false;
        blocking = false;

        animator.SetInteger("action", 0);
        animator.SetInteger("dir", 0);

        currentState = AIState.Idle;

    }

    void GettingHit(int damage, float hitStopTime, float cameraShakeIntensity, string bloodBodyPart){
        if (animator.GetBool("knockdown")) return;

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

    float knockDownSpeed;
    float decayFactor;
    bool initKDS = true;

    float thinkTimer = 0f;

    void DecideNextAction()
    {
        float distance = DistanceToPlayer();

        if(distance > kickRange + 0.5f)
        {
            float r = UnityEngine.Random.value;

            if(r < 0.7f) {
                currentState = AIState.Approach;

            } else {
                currentState = AIState.Evade;
            }

            return;
        }

        if(distance > punchRange && distance <= kickRange)
        {
            float r = UnityEngine.Random.value;

            if(r < 0.4f) {
                currentState = AIState.Attack;
            } else if(r < 0.6f) {
                currentState = AIState.Evade;

            } else if(r < 0.8f) {
                currentState = AIState.Approach;
            } else {
                currentState = AIState.Block;
            }

            return;
        }

        if( distance <= punchRange) {

            float r = UnityEngine.Random.value;

            if(r < 0.6f) {
                currentState = AIState.Attack;
            } else if ( r < 0.8f) {
                currentState = AIState.Block;
            } else {
                currentState = AIState.Retreat;
            }

            return;
        }

        currentState = AIState.Idle;
    }

    void RunCurrentState()
    {
        switch (currentState) {
            case AIState.Idle:
                animator.SetInteger("action", (int)FighterAction.idle);
                break;

            case AIState.Approach:
                ApproachPlayer();
                break;
        
            case AIState.Attack:
                PreformAttack();
                break;

            case AIState.Retreat:
                Retreat();
                break;


            case AIState.Block:
                Block();
                break;

            case AIState.Evade:
                Evade();
                break;

            case AIState.Recover:
                Recover();
                break;
        }
    }

    void ApproachPlayer() {
        orientation = player.position.x > transform.position.x ? 1 : -1;
        
        animator.SetInteger("action", (int)FighterAction.idle);

    }

    void Retreat() {
        orientation = player.position.x > transform.position.x ? 1: -1;

        animator.SetInteger("action", (int)FighterAction.idle);

    }

    void Block()
    {
        animator.SetInteger("action", (int)FighterAction.block);
        Invoke(nameof(StopBlocking), 0.5f);
    }

    void StopBlocking()
    {
        currentState = AIState.Idle;
    }

    void Evade() 
    {
        float r = UnityEngine.Random.value;

        if (r < 0.4f) {
            animator.SetInteger("action", (int)FighterAction.frontFlip);
        } else if (r < 0.8f) {
            animator.SetInteger("action", (int)FighterAction.backFlip);
        } else {
            animator.SetInteger("action", (int)FighterAction.jump);
        }

        currentState = AIState.Recover;
    }

    void PreformAttack()
    {
        float dist = DistanceToPlayer();

        if(ultimateMeter >= ultimateThreshold) {
            if (UnityEngine.Random.value < 0.8f) {
                animator.SetInteger("action", (int)FighterAction.ultimate);
                ultimateMeter = 0;
                currentState = AIState.Recover;
                return;
            }
        }

        if ( dist > punchRange && dist < safeRange && UnityEngine.Random.value < 0.4f) {
            animator.SetInteger("action", (int)FighterAction.flyingPunch);
            currentState = AIState.Recover;
            return;
        }


        if (dist <= punchRange)
        {
            // choose between punch or hook
            animator.SetInteger("action",
                UnityEngine.Random.value < 0.5f ?
                (int)FighterAction.punch :
                (int)FighterAction.hook);
        
        } else if (dist <= hookRange) {
            animator.SetInteger("action", (int)FighterAction.hook);
        } else if (dist <= kickRange) {
            animator.SetInteger("action", (int)FighterAction.kick);
        }

        currentState = AIState.Recover;
    }

    void Recover() {
        Invoke(nameof(EndRecover), 0.3f);
    }

    void EndRecover() {
        currentState = AIState.Idle;
    }

    void Update()
    {
        //Debug.Log("AI State: " + currentState + " | Distance: "+ DistanceToPlayer());
        rawDir = 0;
        float dx = player.position.x - transform.position.x;
        Debug.Log("NPC ultimate meter:"  + ultimateMeter);
        if (currentState == AIState.Approach)
        {
            rawDir = dx > 0 ? 1 : -1;
        } else if (currentState == AIState.Retreat) {
            rawDir = dx > 0 ?-1: 1;
        } else {
            rawDir = 0;
        }

        orientation = player.position.x > transform.position.x ? 1 : -1;

        dir = rawDir * orientation;
        animator.SetInteger("dir", dir);

        if (canMove) {
            transform.position += new Vector3(rawDir * hSpeed, 0f, 0f) * Time.deltaTime;
        }

        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);

        //if(animInfo.IsName("Armature_knockdown")) return;

        if (playerIsUsingUltimate)
        {
            canMove = false;
            rawDir = 0;

            return;
        } else {
            canMove = true;
        }

        if (animator.GetBool("knockdown")) {

            //HandleKnockdownMotion(animInfo);
            return;
        }
        //if(animInfo.IsName("Armature_knockdown")) return;     ta vrstica onemogoca da se zemlja premakne nazaj pri animaciji knockback

        thinkTimer -= Time.deltaTime;

        if(thinkTimer <= 0f)
        {
            thinkTimer = UnityEngine.Random.Range(0.4f, 0.9f);
            DecideNextAction();
        }

        RunCurrentState();
        //HandleMovementAnimations(animInfo);
        /*
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
        }*/
    }
}
