using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_HangEdge : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        var p = playerUnit.Transform.position;
        p += animator.deltaPosition;
        playerUnit.Transform.position = p;
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "HangEdge";

        playerUnit.IsLedge = true;
        playerUnit.IsClimbingMove = false;
        animator.SetBool("IsLedge", true);
        //handIK.ActiveLedgeIK(true);
        playerUnit.AdjustLedgeOffset();
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        animator.SetBool("IsLedge", false);
        playerUnit.IsLedge = false;
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.UpdateGrab();
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.UpdateClimbingInput();
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

    public override void OnJump(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.InputVertical == 1.0f)
        {
            playerUnit.ChangeState(PlayerUnit.readyClimbingJumpState);
            return;
        }

        if (playerUnit.DetectLedgeCanHangLedgeByVertexColor() == true)
            return;

        if (playerUnit.IsLedge == true && playerUnit.IsClimbingMove == false && playerUnit.SpaceChecker.Overlapped() == false)
        {
            playerUnit.IsLedge = false;
            animator.SetTrigger("LedgeUp");
            animator.SetBool("IsLedge", false);

            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.x = 0.0f;
            currentRot.z = 0.0f;
            transform.rotation = Quaternion.Euler(currentRot);

            playerUnit.ChangeState(PlayerUnit.ledgeUpState);
        }
    }
}
