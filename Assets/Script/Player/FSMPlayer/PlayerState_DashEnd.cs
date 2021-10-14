using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_DashEnd : PlayerState
{
    private bool _runEnd;

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        if (_runEnd == false)
        {
            transform.position += animator.deltaPosition;
        }
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "DashEnd";

        if (playerUnit.InputVertical == 0.0f && playerUnit.InputHorizontal == 0.0f)
        {
            animator.SetBool("DashEnd", true);
            _runEnd = false;
        }
        else
        {
            _runEnd = true;
            animator.SetBool("DashRunEnd", true);
        }

        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        animator.SetBool("DashEnd", false);
        animator.SetBool("DashRunEnd", false);
        animator.updateMode = AnimatorUpdateMode.Normal;
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if (_runEnd == false)
        {
            if (playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
            {
                animator.SetBool("DashEnd", false);
                playerUnit.ChangeState(PlayerUnit.defaultState);
            }
        }
        else
        {
            playerUnit.Move(playerUnit.Transform.forward * playerUnit.CurrentSpeed, Time.fixedDeltaTime);
        }
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
