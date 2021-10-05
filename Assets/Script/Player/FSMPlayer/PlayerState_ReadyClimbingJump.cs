using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_ReadyClimbingJump : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "ReadyClimbingJump";

        if (playerUnit.InputVertical >= 0.5f)
        {
            playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.Up;
            if(playerUnit.CheckUpClimbingLine() == true)
            {
                playerUnit.ChangeState(PlayerUnit.climbingUpperLineState);
                //Debug.Log(playerUnit.Transform.position);
                return;
            }
        }
        else
        {
            if (playerUnit.InputHorizontal >= 0.5f)
            {
                playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.Right;
            }
            else if (playerUnit.InputHorizontal <= -0.5f)
            {
                playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.Left;
            }
        }

        playerUnit.InitVelocity();

        //playerUnit.HandIK.DisableHandIK();
        animator.SetBool("IsGrab", false);
        animator.SetTrigger("ClimbingJump");
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        //Debug.Log(playerUnit.Transform.position);
    }
}
