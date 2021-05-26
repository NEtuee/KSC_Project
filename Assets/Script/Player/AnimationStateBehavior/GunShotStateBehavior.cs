using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShotStateBehavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(3, 1.0f);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetTrigger("ShotComplete");
        animator.SetLayerWeight(3, 0.0f);
    }
}
