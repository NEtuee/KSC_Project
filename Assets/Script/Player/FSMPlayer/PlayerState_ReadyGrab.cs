using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_ReadyGrab : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        var p = playerUnit.Transform.position;
        p += animator.deltaPosition;
        playerUnit.Transform.position = p;
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "ReadyGrab";

        playerUnit.CurrentJumpPower = 0.0f;
        playerUnit.CurrentSpeed = 0.0f;
        playerUnit.InitVelocity();

        animator.SetBool("IsGrab", true);
        animator.SetInteger("ReadyClimbNum", (int)playerUnit.ClimbingJumpDirection);

        //playerUnit.HandIK.ActiveHandIK(true);
        playerUnit.HandIK.ActiveLedgeIK(false);
        playerUnit.FootIK.DisableFeetIk();
        playerUnit.IsCanReadyClimbingCancel = false;

        playerUnit.IsClimbingMove = false;
        playerUnit.IsJump = true;

        animator.ResetTrigger("RightClimbing");
        animator.ResetTrigger("LeftClimbing");
        animator.ResetTrigger("UpClimbing");
        animator.ResetTrigger("DownClimbing");
        animator.ResetTrigger("UpLeftClimbing");
        animator.ResetTrigger("UpRightClimbing");
        animator.ResetTrigger("DownLeftClimbing");
        animator.ResetTrigger("DownRightClimbing");

        playerUnit.AirTime = 0.0f;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if(playerUnit.LedgeChecker.IsDetectedLedge() == true)
        {
            playerUnit.ChangeState(PlayerUnit.grabState);
            playerUnit.ChangeState(PlayerUnit.hangLedgeState);
            return;
        }

        if(playerUnit.IsCanReadyClimbingCancel == true && (playerUnit.InputVertical != 0 || playerUnit.InputHorizontal != 0))
        {
            animator.SetTrigger("ReadyClimbCancel");
            playerUnit.ChangeState(PlayerUnit.grabState);
        }

        playerUnit.UpdateGrab();
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void OnGrab(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.IsClimbingMove = false;
        playerUnit.IsLedge = false;

        Vector3 currentRot = transform.rotation.eulerAngles;
        currentRot.x = 0.0f;
        currentRot.z = 0.0f;
        transform.rotation = Quaternion.Euler(currentRot);

        playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.Falling;

        playerUnit.Detach();

        playerUnit.ChangeState(PlayerUnit.defaultState);
    }
}
