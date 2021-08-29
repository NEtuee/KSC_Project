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
        if (playerUnit.InputVertical >= 0.5f)
        {
            if (playerUnit.InputHorizontal == 0.0f)
                playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.Up;
            else if (playerUnit.InputHorizontal > 0.0f)
                playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.UpRight;
            else if (playerUnit.InputHorizontal < 0.0f)
                playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.UpLeft;
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
            else
            {
                Vector3 backVector = -transform.forward;
                backVector.y = 0f;
                transform.rotation = Quaternion.LookRotation(backVector);

                playerUnit.MoveDir = transform.forward;
                playerUnit.MoveDir.Normalize();
                playerUnit.CurrentSpeed = playerUnit.RunSpeed;
                playerUnit.MoveDir *= playerUnit.CurrentSpeed;
                playerUnit.CurrentJumpPower = playerUnit.JumpPower * 0.5f;
                transform.position = transform.position +
                                     (playerUnit.MoveDir + (Vector3.up * playerUnit.CurrentJumpPower)) * Time.deltaTime;

                animator.SetBool("IsGrab", false);

                //playerUnit.stamina.Value -= playerUnit.ClimbingJumpConsumeValue;
                //playerUnit.stamina.Value = Mathf.Clamp(playerUnit.stamina.Value, 0.0f, playerUnit.MaxStamina);

                playerUnit.SetVelocity(playerUnit.MoveDir);

                playerUnit.HandIK.DisableHandIK();
                playerUnit.Jump();
                playerUnit.ChangeState(PlayerUnit.jumpState);

                return;
            }
        }

        playerUnit.InitVelocity();

        playerUnit.HandIK.DisableHandIK();
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
    }
}
