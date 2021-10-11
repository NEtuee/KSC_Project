using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_DashEnd : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        transform.position += animator.deltaPosition;
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        animator.SetBool("DashEnd", true);
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        animator.SetBool("DashEnd", false);
        animator.updateMode = AnimatorUpdateMode.Normal;
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if(playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
        {
            animator.SetBool("DashEnd", false);
            playerUnit.ChangeState(PlayerUnit.defaultState);
        }
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
