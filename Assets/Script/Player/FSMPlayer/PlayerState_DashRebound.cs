using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_DashRebound : PlayerState
{

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        transform.position += animator.deltaPosition;
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "DashRebound";

        animator.SetBool("DashRebound",true);

        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        animator.SetBool("DashRebound", false);

        animator.updateMode = AnimatorUpdateMode.Normal;
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
