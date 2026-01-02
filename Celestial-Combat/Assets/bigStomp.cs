using UnityEngine;

public class bigStomp : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    bool doOnce;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        doOnce = true;
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 0.39 && doOnce){
            if (animator.name == "animatedSun"){        //smo v animatorju od Sonca (soncev Ultimate)
                Debug.Log("sun ultimate");
                animator.GetComponent<PlayerController>().gameManager.StompCameraShake(true);
            } 
            else{                                       //smo v animatorju od Zemlje (zemljin Ultimate)
                Debug.Log("earth ultimate");
                animator.GetComponent<OponentController>().gameManager.StompCameraShake(false);
                animator.GetComponent<OponentController>().MoonSwitch();
            }
            
            doOnce = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
