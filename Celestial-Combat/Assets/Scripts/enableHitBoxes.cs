using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class enableHitBoxes : StateMachineBehaviour
{
    string rightHand = "mixamorig:RightHandIndex1";
    string leftHand = "mixamorig:LeftHandIndex1";
    string rightLeg = "mixamorig:RightToeBase";
    //string leftLeg = "mixamorig:LeftToeBase";


    //to metodo rabimo, zato da najdemo hitbox pri pravem characterju - ker imata oba enako poimenovanje - moramo iskati pri otroku objekta ki ima animator
    HitBox FindHitBox(string name, Animator animator){                  
        HitBox[] hitboxes = animator.GetComponentsInChildren<HitBox>();
        foreach (HitBox hitbox in hitboxes){
            if (hitbox.name == name){
                return hitbox;
            }
        }
        return null;
    }

    HitBox hitbox;
    int damageLevel;
    bool doOnce;
    bool disableOnce;
    float enableTime = 0.0f;
    float disableTime = 1.0f;
    string bodyPart;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)       //v tej funkciji najdemo pravi collider in ga omogocimo
    {   
        damageLevel = 0;
        doOnce = true;
        disableOnce = true;

        //preverimo katera animacija se je zacela in omogicimo samo nujen hitbox (enega)
        if (stateInfo.IsName("Armature|punchBlended") || stateInfo.IsName("Armature|punch")){   //zaradi tega ker imata zemlja in sonce drugacno ime za punch
            hitbox = FindHitBox(rightHand, animator);
            enableTime = 0.2f;
            disableTime = 0.7f;
            damageLevel = 1;
            bodyPart = "RH";    //right hand
        }
        if (stateInfo.IsName("Armature|hookPunch")){
            hitbox = FindHitBox(rightHand, animator);
            enableTime = 0.3f;
            disableTime = 0.7f;
            damageLevel = 2;
            bodyPart = "RH";    //right hand
        }
        if (stateInfo.IsName("Armature|kick")){
            hitbox = FindHitBox(rightLeg, animator);
            enableTime = 0.25f;
            disableTime = 0.6f;
            damageLevel = 3;
            bodyPart = "RL";     //right leg

        }
        if (stateInfo.IsName("Armature_flyingPunch")){
            hitbox = FindHitBox(leftHand, animator);
            Debug.Log("flying punch");
            enableTime = 0.45f;
            disableTime = 0.7f;
            damageLevel = 4;
            bodyPart = "LH";    //right leg
        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //ko pridemo v pravilni time frame potem omogocimo hit boxe
        if (hitbox != null && stateInfo.normalizedTime > enableTime && doOnce){ 
            hitbox.EnableColliderAndSetDamageLevel(damageLevel, bodyPart);
            doOnce = false;
        }

        //onemogocimo hit boxe po nekem casovnem obdobju
        if (hitbox != null && stateInfo.normalizedTime > disableTime && disableOnce){
            disableOnce = false;
            hitbox.DisableCollider();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       hitbox.DisableCollider();    //nazaj onemogocimo hitbox
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
