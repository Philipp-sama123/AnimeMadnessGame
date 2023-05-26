using UnityEngine;

public class ResetBoolStates : StateMachineBehaviour
{
    public string isUsingRootMotionBool = "IsUsingRootMotion";
    public string canDoComboBool = "CanDoCombo";

    public bool isUsingRootMotionStatus = false;
    public bool canDoComboStatus = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(isUsingRootMotionBool, isUsingRootMotionStatus);
        animator.SetBool(canDoComboBool, canDoComboStatus);
    }
}