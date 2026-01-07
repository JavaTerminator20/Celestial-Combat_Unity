using System;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
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
  

    //int ultimateMeter = 0;

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
        //Invoke("clearHit", 0.4f);
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
    public bool blocking = false;

    //float distance = Mathf.Abs(player.position.x - transform.position.x);
    float punchRange = 1.385f;
    float hookRange = 1.4f;
    float kickRange = 2f;
    float safeRange = 2.5f;
    int rawDir = 0;
    int dir = 0;
    public bool canMove = true;

    float DistanceToPlayer() {
        return Mathf.Abs(player.position.x - transform.position.x);
    }


    public bool canChangeOrientation = true;

    public void playDead(){
        animator.SetBool("dead", true);
        animator.SetInteger("dir", 0);
        animator.SetInteger("action", 0);
        AIdisabled = true;
        gameManager.disablePlayer();
    }

    void clearHit(){
        animator.SetBool("hit", false);
        weAreHit = false;
        Debug.Log("cleared hit");
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
            //knockDownMeter += damage;
            weAreHit = false;
            blocking = false;
            CancelInvoke("clearHit");
            animator.SetBool("hit", false);
            canMove = false;
            
            animator.SetBool("knockdown", true);
            animator.SetInteger("action", 0);
            animator.SetInteger("dir", 0);
            knockDownMeter = 0.0f;
            
            return;
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

    public void SendDamage(int value){
        Debug.Log("oponent recieved ultimate damage");
        health -= value;
        if (health < 0){
            playDead();
        }

        if (healthBar != null)
        {    
            healthBar.SetHealth(health);
        }
    }

    void GettingHit(int damage, float hitStopTime, float cameraShakeIntensity, string bloodBodyPart){
        if (animator.GetBool("knockdown")) return;

        if (!blocking && grounded){

            health -= damage;
            if (healthBar != null)
            {    
                healthBar.SetHealth(health);
            }

            if (health <= 0){
                playDead();
                return;
            }

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

    public void OnUltimateFired()
    {
        ultimateMeter = 0;
    }

    public void BringBackMoon(){
        ogMoon.SetActive(true);
    }        

    float knockDownSpeed;
    float decayFactor;
    bool initKDS = true;

    float thinkTimer = 0f;

    void DecideNextAction()
    {
        float distance = DistanceToPlayer();

        float punchIn = punchRange;
        float punchOut = punchRange + 0.2f;

        float kickIn = kickRange;
        float kickOut = kickRange + 0.2f;

        if(distance > kickOut)
        {
            float r = UnityEngine.Random.value;

            if(r < 0.4f) {
                currentState = AIState.Approach;

            } else if( r < 0.8f){
                currentState = AIState.Attack; 
            } else {
                currentState = AIState.Evade;
            }

            return;
        }

        if(distance > punchOut && distance <= kickIn)
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

        if( distance <= punchIn) {

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

        if(DistanceToPlayer() > safeRange)
        {
            currentState = AIState.Idle;
        }

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
            if (dist > kickRange && UnityEngine.Random.value < 1f) {
                animator.SetInteger("action", (int)FighterAction.ultimate);
                //ultimateMeter = 0;
                currentState = AIState.Recover;
                return;
            }

        }

        if (dist > punchRange && dist < safeRange && UnityEngine.Random.value < 0.5f) {
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

    public void CheckForMiss()
    {
        Debug.Log("CheckForMiss called.");
        foreach (HitBox hitbox in GetComponentsInChildren<HitBox>())
        {
            hitbox.CheckForMiss();
        }
    }

    //spremenljivke za flipe
    public bool grounded = true;
    bool paramsInit = false;
    float gravity;
    float vSpeed;

    public bool AIdisabled = false;

    void Update()
    {
        if (AIdisabled){animator.SetInteger("action", 0); animator.SetInteger("dir", 0); return;}

        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);

        //Debug.Log("AI State: " + currentState + " | Distance: "+ DistanceToPlayer());
        rawDir = 0;
        float dx = player.position.x - transform.position.x;
        //Debug.Log("NPC ultimate meter:" + ultimateMeter);

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

        if (weAreHit) {
            animator.SetInteger("action", 0);
            animator.SetInteger("dir", 0);
            canMove = false;
            return;

        }

        if (playerIsUsingUltimate)
        {
            canMove = false;
            rawDir = 0;

            return;
        } else {
            canMove = true;
        }


        if (currentState == AIState.Approach)
        {
            rawDir = dx > 0 ? 1 : -1;
        } else if (currentState == AIState.Retreat) {
            rawDir = dx > 0 ?-1: 1;
        } else {
            rawDir = 0;
        }

        //orientation = player.position.x > transform.position.x ? 1 : -1;

        dir = rawDir * orientation;
        animator.SetInteger("dir", dir);

        if (canMove) {
            transform.position += new Vector3(rawDir * hSpeed, 0f, 0f) * Time.deltaTime;
        }

        
        if(animator.GetBool("knockdown")) {
            return;
        }

        thinkTimer -= Time.deltaTime;

        if(thinkTimer <= 0f)
        {
            if (currentState == AIState.Idle || currentState == AIState.Approach || currentState == AIState.Retreat) {
                thinkTimer = UnityEngine.Random.Range(0.1f, 0.5f);
                DecideNextAction();
            } else {
                thinkTimer = UnityEngine.Random.Range(0.1f, 0.2f);
            }
        }

        RunCurrentState();
        //HandleMovementAnimations(animInfo);

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        knockDownMeter -= decayRate * Time.unscaledDeltaTime;
        if (knockDownMeter < 0) knockDownMeter = 0;
        //Debug.Log(knockDownMeter);

        // decisionTimer -= Time.deltaTime;
        // if (decisionTimer < 0){
        //     decisionTimer = 0.8f;
        //     MakeAction();
        // }

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

        bool isJumpingAnim = curAnimPlaying == "Armature_jump" ||
            curAnimPlaying == "Armature_frontFlip" ||
            curAnimPlaying == "Armature_backflip";

        if(!isJumpingAnim) {
            paramsInit = false;
            grounded = true;

            if (transform.position.y > 1.01f)
            {
                transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            }
        }

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
        

        if (animInfo.IsName("Armature_hitStepBackBlended")){
            if (animInfo.normalizedTime < 0.15f){
                transform.position += new Vector3(-(hSpeed+hitExtraSpeed)*Time.deltaTime*orientation, 0, 0);    //dejanski step back
            }
        }
    }
}
