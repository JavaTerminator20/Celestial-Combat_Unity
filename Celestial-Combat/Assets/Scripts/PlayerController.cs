using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.Animations;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.UI;
using NUnit.Framework.Constraints;


public class PlayerController : CharacterBase
{

    int rawDir = 0;
    int stepForward = 0;
    int stepBack = 0;
    int jump = 0;
    int action = 0;
    int oldAction = 0;          //action za primerjavo - iz prejsnjega frame-a

    public enum FighterAction
    {
        idle = 0,
        punch = 1,
        hook = 2,
        kick = 3,
        block = 4,
        frontFlip = 5,
        backFlip = 6,
        flyingPunch = 7,
        releaseBlock = 8,
        ult = 9,
        stumble = 10,
    }
    public ParticleSystem blockSparks;
    public ParticleSystem beam;
    public ParticleSystem beamEnd;
    public ParticleSystem beamEndOponent;
    public bool usingUltimate = false;
    private Dictionary<FighterAction, int> priority = new Dictionary<FighterAction, int>        //ce se dve stvari zgodita hkrati, da se doloci katera ima prednost
    {
        { FighterAction.idle, 0 },
        { FighterAction.block, 10 },
        { FighterAction.punch, 10 },
        { FighterAction.kick, 10 },
        { FighterAction.hook, 10 }
    };

    public ParticleSystem OponentLeftHand;         //to do komponente od nasprotnika
    public ParticleSystem OponentRightHand;
    public ParticleSystem OponentRightLeg;
    //int ultimateMeter = 0;

    public void OnStepForward(InputValue value)
    { stepForward = value.isPressed ? 1 : 0; }

    public void OnStepBack(InputValue value)
    { stepBack = value.isPressed ? 1 : 0; }

    public void OnJump(InputValue value)
    { jump = value.isPressed ? 1 : 0; 
      action = value.isPressed ? action : (int)FighterAction.idle; }
    
    public void OnPunch(InputValue value)
    { action = value.isPressed ? (int)FighterAction.punch: (int)FighterAction.idle; }

    public void OnHook(InputValue value)
    { action = value.isPressed ? (int)FighterAction.hook : (int)FighterAction.idle; }

    public void OnKick(InputValue value)
    { action = value.isPressed ? (int)FighterAction.kick : (int)FighterAction.idle; }

    public void OnBlock(InputValue value)
    { action = value.isPressed ? (int)FighterAction.block : (int)FighterAction.releaseBlock; }

    public void OnUlt(InputValue value)
    { 
        if (!grounded) return;
        if(ultimateMeter < ultimateThreshold) return;

        action = value.isPressed ? (int)FighterAction.ult : (int)FighterAction.releaseBlock; 
    
        if (value.isPressed) {
            ultimateMeter = 0;
        }
    }


    public void Stumble(){  //ko zemlja izvede svoj ultimate
        animator.SetBool("hit", true);
        Invoke("clearHit", 1.0f); 
        action = (int)FighterAction.stumble;
        Debug.Log("action is stumble");
        Invoke("releaseStumble", 1.0f);

    }

    void releaseStumble(){
        action = 0;
    }

    public void Ultimate(){
        usingUltimate = true;
        playerIsUsingUltimate = true;

        Vector3 oponentPos = gameManager.getOponentTransform().position;
        float distance = Mathf.Abs(oponentPos.x - (transform.position.x + 0.8f*orientation))-0.2f;
        var beamMain = beam.main;
        beamMain.startSizeX = distance;
        //Debug.Log(distance);
        float avgX = (oponentPos.x + (transform.position.x + 0.8f*orientation))/2;
        float avgY = (oponentPos.y+1.5f + transform.position.y+1.0f)/2;
        Debug.Log(avgY);
        beam.transform.position = new Vector3(avgX, avgY, 5);

        float angle = Mathf.Atan2(oponentPos.y+1.5f -(transform.position.y+1.0f), distance+0.2f) * Mathf.Rad2Deg;
        Debug.Log("angle is: " + angle);
        beam.transform.rotation = Quaternion.Euler(0f, 0f, angle*orientation);
        beam.Play();
        beamEnd.Play();
        beamEndOponent.Play();
        StartCoroutine(gameManager.sustainedShake(10));
        Invoke("stopBeamEnd", 4.0f);
    }

    void stopBeamEnd(){
        beamEnd.Stop();
        beamEndOponent.Stop();
    }

    public void EndUltimate() {
        playerIsUsingUltimate = false;
    }

    //to je couroutine - namesto invoka je uporabljena zato, da lahko passamo argumente
    IEnumerator<WaitForSeconds> clearAction(Queue<int> actionQueue, int action){
        yield return new WaitForSeconds(0.3f);
        if (actionQueue.Count > 0){
            if (actionQueue.Peek() == action){
                actionQueue.Dequeue();
                //Debug.Log("dequeued(time): " + action);
            }
        }
    }

    //pol sekunde po pristanku smo invincible -- obsolete ker sem implementiral feature, da se udarec zacne in konca v isti smeri
    public IEnumerator<WaitForSeconds> clearInvincible(){
        yield return new WaitForSeconds(0.5f);
        invincible = false;
    }
    

    void clearHit(){
        animator.SetBool("hit", false);
    }

    void GettingHit(int damage, float hitStopTime, float shakeIntensity, string bloodBodyPart){

        if (grounded && !invincible){         //TODO: naredi da bo player pri pristanku (fron/back flip) se nekaj casa invincible
            health -= damage;
            Debug.Log("player got hit, health: " + health);
            animator.SetBool("hit", true);
            Invoke("clearHit", 0.4f);
            StartCoroutine(HitFreeze(hitStopTime));        //to pozene HitFreeze
            CameraShake(shakeIntensity, false);
            playBloodVFX(bloodBodyPart, OponentLeftHand, OponentRightHand, null, OponentRightLeg);

        } else if (blocking){
            //Debug.Log("shake when blocking");
            CameraShake(shakeIntensity, true);
            blockSparks.Play();
            
        }   
    }

    public void CheckForMiss()
    {
        Debug.Log("CheckForMiss called.");
        foreach (HitBox hitbox in GetComponentsInChildren<HitBox>())
        {
            hitbox.CheckForMiss();
        }
    }

    

    private Animator animator;
    void Start()
    {   
        animator = GetComponent<Animator>();
        OnHitReceived += GettingHit;        //dodamo metodo na seznam "poslusalcev" - potrebno bi bilo sicer ga odstraniti pri OnDisable(){OnHitReceived -= GettingHit;}
        hSpeed = base.hSpeedUniversal;      //spremenljivka hSpeedUniversal je v nadrejenem razredu, da lahko obema igralcema nastavimo isto hitrost
        gameManager = FindFirstObjectByType<GameManager>();
        invincible = false;
    }

    float hSpeed;
    static float gravity = 60.0f;
    float vSpeed = 0.0f;
    public bool canMove = true;                        //se nastavi na false v skripti posamezne animacije (ko se ta zacne izvajati)
    public bool grounded = true;
    public bool invincible = false;                     //bool ki pove da ne moremo prejeti udarca (ko pristanemo pri fron/back flipu, ko smo knocked-down)
    public int orientation = 1;                        //int ki pove v katero smer gledamo: 1:desno, -1:levo
    public bool canChangeOrientation = true;           //bool ki pove, da se lahko zamenja rotacija (potreben zato, da se udarec zacne in konca v isti smeri - ne da se igralec obrne med izvajanjem nekega dolgega udarca)
    public bool blocking = false;                      //bool ki je true, ko je pritisnjena tipka za block - NE POMENI DA JE ANIMACIJA BLOCK AKTIVNA
    int dir = 0;

    bool paramsInit = false;            //boolean ki se uporablja zato, da se parametre pri front-back flipu nastavi samo pri prvem frame-u
    bool keyUp = true;

    public Queue<int> actionQueue = new Queue<int>();
    public Queue<int> comboQueue = new Queue<int>();

    float knockDownSpeed;
    float decayFactor;
    bool initKDS;

    void Update()
    {
        rawDir = stepForward - stepBack;
        dir = rawDir * orientation;
        animator.SetInteger("dir", dir);

        //Debug.Log("Player ultimate meter:" + ultimateMeter);

        if (jump == 1){
            if (dir == 1){
                action = (int)FighterAction.frontFlip;
            }
            if (dir == -1){
                action = (int)FighterAction.backFlip;
            }
        }

        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string curAnimPlaying = clipInfo[0].clip.name;

        if (curAnimPlaying == "Armature|backflip"){  //0.67, 1.37
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
        if (curAnimPlaying == "Armature|frontFlip"){
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

        if (curAnimPlaying == "Armature_flyingPunch"){
            if (animInfo.normalizedTime < 0.65f){
                transform.position += new Vector3((hSpeed+1.0f)*Time.deltaTime*orientation, 0f, 0f);
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

        //s tem gremo ven iz blocking polozaja (brez tega bi ostali v blocking polozaju)
        // if (curAnimPlaying == "Armature_block_faster"){
        //     if (action != 4){              
        //         animator.SetInteger("action", (int)FighterAction.idle);
        //     }
        // }

        if (animInfo.IsName("Armature_hitStepBackBlended")){
            if (animInfo.normalizedTime < 0.15f){
                transform.position += new Vector3(-(hSpeed+hitExtraSpeed)*Time.deltaTime*orientation, 0, 0);    //dejanski step back
            }
        }

        if (canMove){
            transform.position += new Vector3(rawDir * hSpeed, 0f, 0f) * Time.deltaTime;
        }

        //premikanje naprej ob jab udarcu
        // if (curAnimPlaying == "Armature|punchBlended"){
        //     transform.position += new Vector3(hSpeed*Time.deltaTime*orientation, 0, 0);
        // }

        //-------------------------------------------INPUT PROCESSING-------------------------------------------------------------------------------------------------
        //ce smo zaznali neko akcijo (pritisnjena tipka) in ta akcija ni Idle, potem jo damo v Queue in zazenemo rutino, ki jo bo pobrisala po nekem casovnem obdobju
        //key up nam sluzi da moramo tipko spustiti in se enkrat pritisniti, ce hocem akcijo se enkrat izvesti

        //ta del kode nam omogoci da pritisnemo dva (ali vec) zaporedna gumba ne da bi pri tem spustili predhodnega (in se bo oba registriralo) 
        if (action != oldAction && action != 0 && oldAction != 0){
            keyUp = true;
        }
        oldAction = action;

        //Debug.Log("ActionQueue: [" + string.Join(", ", actionQueue) + "]");

        //ce smo pritisnili releaseBlock potem nehamo blokirati
        if (action == (int)FighterAction.releaseBlock){
            action = 0;
            blocking = false;
            animator.SetInteger("action", (int)FighterAction.releaseBlock);
        }
        
        if (action == 0){keyUp = true;}     //ko spustimo gumb bo action == 0, zato resetiramo keyUp.
        
        if (action != 0 && keyUp){
            //izjema za akcijo block
            if (action == (int)FighterAction.block){
                animator.SetInteger("action", (int)FighterAction.block);
                blocking = true;
            }

            //filamo akcije v actionQueue 
            else {
                actionQueue.Enqueue(action);
                StartCoroutine(clearAction(actionQueue, action));
                comboQueue.Enqueue(action);
            }
            keyUp = false;
        }

        //transition v novo akcijo (ce je ta v Queue-ju)
        if (animInfo.IsName("Armature|idle") || animInfo.IsName("Armature|stepBack") || animInfo.IsName("Armature|stepForward")){
            grounded = true;
            paramsInit = false;
            jump = 0;

            //pisanje akcij v spremenljivko v animatorju
            if (actionQueue.Count > 0){                         //ce je kaksna akcija v vrsti, potem nastavimo animatorja na to akcijo (prvo v vrsti)
                int dqAction = actionQueue.Peek();
                animator.SetInteger("action", dqAction);        //ce je character v idle animaciji, in je neka akcija v actionQueue, potem se bo ta akcija posredovala v animator
                
            } else if (action != (int)FighterAction.block){  //ta pogoj je zato, da ne gremo v idle ko drzimo "block"
                animator.SetInteger("action", (int)FighterAction.idle);     //ce ni nobene akcije, potem nastavi animatorja na "idle"
                blocking = false;
            }
        }


        //---------------------------------------------------------------------------------------------------------------------------------------------------------
        
    }
}
