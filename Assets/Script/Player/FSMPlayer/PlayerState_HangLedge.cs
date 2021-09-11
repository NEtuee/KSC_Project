using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_HangLedge : PlayerState
{
    private float _ledgeUpInputTime = 0.0f;
    private float _ledgeUpTriggerTime = 0.3f;

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        var p = playerUnit.Transform.position;
        p += animator.deltaPosition;
        playerUnit.Transform.position = p;
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "HangLedge";

        playerUnit.IsLedge = true;
        playerUnit.IsClimbingMove = false;
        animator.SetBool("IsLedge", true);
        playerUnit.HandIK.ActiveLedgeIK(true);
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

        if (playerUnit.InputVertical == 1.0f)
        {
            _ledgeUpInputTime += Time.deltaTime;
        }
        else
            _ledgeUpInputTime = 0f;

        if(_ledgeUpInputTime >= _ledgeUpTriggerTime)
        {
            LedgeUp(playerUnit, animator);
        }
    }

    //public override void OnGrab(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    //{
    //    playerUnit.IsClimbingMove = false;
    //    playerUnit.IsLedge = false;

    //    Vector3 currentRot = transform.rotation.eulerAngles;
    //    currentRot.x = 0.0f;
    //    currentRot.z = 0.0f;
    //    transform.rotation = Quaternion.Euler(currentRot);

    //    playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.Falling;

    //    playerUnit.Detach();

    //    playerUnit.ChangeState(PlayerUnit.defaultState);
    //}

    private void LedgeUp(PlayerUnit playerUnit, Animator animator)
    {
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

    public override void OnGrabRelease(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
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

        playerUnit.GrabRelease = true;
    }

    public override void OnJump(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.InputVertical == 1.0f)
        {
            playerUnit.ChangeState(PlayerUnit.readyClimbingJumpState);
            return;
        }

        //if (playerUnit.DetectLedgeCanHangLedgeByVertexColor() == true)
        //    return;

        //if (playerUnit.IsLedge == true && playerUnit.IsClimbingMove == false && playerUnit.SpaceChecker.Overlapped() == false)
        //{
        //    playerUnit.IsLedge = false;
        //    animator.SetTrigger("LedgeUp");
        //    animator.SetBool("IsLedge", false);

        //    Vector3 currentRot = transform.rotation.eulerAngles;
        //    currentRot.x = 0.0f;
        //    currentRot.z = 0.0f;
        //    transform.rotation = Quaternion.Euler(currentRot);

        //    playerUnit.ChangeState(PlayerUnit.ledgeUpState);
        //}
    }
}
