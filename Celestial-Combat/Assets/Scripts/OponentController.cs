using System;
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

    void clearHit(){
        animator.SetBool("hit", false);
        weAreHit = false;
    }

    void GettingHit(int damage){
        health -= damage;
        Debug.Log("oponent got hit, current health: " + health);
        animator.SetBool("hit", true);
        animator.SetInteger("action", 0);
        animator.SetInteger("dir", 0);
        weAreHit = true;
        Invoke("clearHit", 0.5f);
        StartCoroutine(HitFreeze(damage));                                 //zazenemo hitFreeze efekt
    }        

    void Update()
    {
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!weAreHit){
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

            
            // if (animInfo.IsName("Armature|stepBack")){
            //     transform.position += new Vector3(hSpeed*Time.deltaTime*orientation, 0, 0);
            // }

            if (animInfo.IsName("Armature|stepForward")){
                transform.position += new Vector3(hSpeed*Time.deltaTime*orientation, 0, 0);
            }
            
        }

        if (animInfo.IsName("Armature_hitStepBackBlended")){
            if (animInfo.normalizedTime < 0.15f){
                transform.position += new Vector3(-(hSpeed+hitExtraSpeed)*Time.deltaTime*orientation, 0, 0);    //dejanski step back
            }
        }
    }
}
