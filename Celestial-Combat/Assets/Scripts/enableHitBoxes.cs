using System;
using UnityEngine;

public class enableHitBoxes : StateMachineBehaviour
{
    string rightHand = "mixamorig:RightHandIndex1";
    //string leftHand = "mixamorig:LeftHandIndex1";
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
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)       //v tej funkciji najdemo pravi collider in ga omogocimo
    {   
        int damage = 0;

        //preverimo katera animacija se je zacela in omogicimo samo nujen hitbox (enega)
        if (stateInfo.IsName("Armature|punchBlended") || stateInfo.IsName("Armature|punch")){   //zaradi tega ker imata zemlja in sonce drugacno ime za punch
            hitbox = FindHitBox(rightHand, animator);
            damage = 5;
        }
        if (stateInfo.IsName("Armature|hookPunch")){
            hitbox = FindHitBox(rightHand, animator);
            damage = 10;
        }
       if (stateInfo.IsName("Armature|kick")){
            hitbox = FindHitBox(rightLeg, animator);
            damage = 15;
       }
        if (hitbox != null){
            hitbox.EnableColliderAndSetDamage(damage);
        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
