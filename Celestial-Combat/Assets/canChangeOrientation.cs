using UnityEngine;

public class canChangeOrientation : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

    // ta skripta je potrebna, da ko se zacne izvajati neka akcija, da se izvede v isti smeri - ne da se character med izvajanjem...
    // ...obrne, ce se zamenjata pozicije igralcev
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.name == "animatedSun"){
            animator.GetComponent<PlayerController>().canChangeOrientation = true;
        }
        else{
            animator.GetComponent<OponentController>().canChangeOrientation = true;
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
       if (animator.name == "animatedSun"){
            animator.GetComponent<PlayerController>().canChangeOrientation = false;
        }
        else{
            animator.GetComponent<OponentController>().canChangeOrientation = false;
        }
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
