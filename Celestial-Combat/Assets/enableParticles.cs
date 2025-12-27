using System;
using System.Diagnostics;
using UnityEngine;

public class enableParticles : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    ParticleSystem[] ps;
    ParticleSystem punchPart;
    bool startPlaying;
    bool stopPlaying;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ps = animator.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps1 in ps){
            if (ps1.name == "bigPunchParticles"){
                punchPart = ps1;
            }
        }
        startPlaying = false;
        stopPlaying = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Armature_flyingPunch")){
            if (stateInfo.normalizedTime > 0.3 && !startPlaying){
                punchPart.Play();
                //UnityEngine.Debug.Log("enabled particles");
                startPlaying = true;
            }
            if (stateInfo.normalizedTime > 0.6 && !stopPlaying){
                punchPart.Stop();
                stopPlaying = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       punchPart.Stop();   //ce je slucajno animacija prekinjena se particli ugasnejo
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
