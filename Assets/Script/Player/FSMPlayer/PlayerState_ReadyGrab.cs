using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_ReadyGrab : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
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
        //playerUnit.HandIK.ActiveLedgeIK(false);
        playerUnit.FootIK.DisableFeetIk();
        playerUnit.IsCanReadyClimbingCancel = false;

        playerUnit.IsClimbingMove = false;
        playerUnit.IsJump = true;

        playerUnit.CapsuleCollider.height = 1f;
        playerUnit.CapsuleCollider.center = new Vector3(0.0f, 0.5f, 0.0f);
        playerUnit.AirTime = 0.0f;

        Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
        Vector3 finalPosition;

        if (planInfo != null)
        {
            finalPosition = playerUnit.LineTracker.position + (planInfo.up * playerUnit.DetectionOffset.y);
            finalPosition -= planInfo.forward * playerUnit.DetectionOffset.z;

            playerUnit.Transform.rotation = Quaternion.LookRotation(-planInfo.forward);
            playerUnit.Transform.position = finalPosition;
        }
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
        Vector3 finalPosition;

        if (planInfo != null)
        {
            finalPosition = playerUnit.LineTracker.position + (planInfo.up * playerUnit.DetectionOffset.y);
            finalPosition -= planInfo.forward * playerUnit.DetectionOffset.z;

            playerUnit.Transform.rotation = Quaternion.LookRotation(-planInfo.forward);
            playerUnit.Transform.position = finalPosition;
        }
    }
}
