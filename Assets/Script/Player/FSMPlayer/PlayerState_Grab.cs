using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;
using UnityEngine.InputSystem;

public class PlayerState_Grab : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.CheckCanClimbingMoveByVertexColor() == false)
            return;

        var p = playerUnit.Transform.position;
        p += animator.deltaPosition;

        if (playerUnit.IsClimbingGround == false)
        {
            playerUnit.Transform.position = p;
            return;
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool detect = false;
        if (stateInfo.IsName("Climbing.Up_LtoR") || stateInfo.IsName("Climbing.Up_RtoL"))
        {
            if (Physics.Raycast(p + playerUnit.Transform.up * playerUnit.CapsuleCollider.height, playerUnit.Transform.forward, 3f))
                detect = true;
        }
        else if (stateInfo.IsName("Climbing.Down_LtoR") || stateInfo.IsName("Climbing.Down_RtoL"))
        {
            if (Physics.Raycast(p, playerUnit.Transform.forward, 3f))
                detect = true;
        }
        else if (stateInfo.IsName("Climb_Left") || stateInfo.IsName("Climb_Right"))
        {
            if (Physics.Raycast(p + playerUnit.Transform.up * playerUnit.CapsuleCollider.height * 0.5f, playerUnit.Transform.forward, 3f))
                detect = true;
        }

        if (detect == true)
            playerUnit.Transform.position = p;
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Grab";

        animator.SetBool("IsGrab", true);
        playerUnit.CurrentJumpPower = 0.0f;
        playerUnit.CurrentSpeed = 0.0f;

        playerUnit.CapsuleCollider.height = 1f;
        playerUnit.CapsuleCollider.center = new Vector3(0.0f,0.5f,0.0f);

        //playerUnit.IsClimbingMove = false;
        playerUnit.IsJump = false;
        playerUnit.CurrentJumpPower = 0.0f;

        playerUnit.InitVelocity();
        playerUnit.HandIK.ActiveHandIK(true);
        playerUnit.HandIK.ActiveLedgeIK(false);
        playerUnit.FootIK.DisableFeetIk();

        StringData data = MessageDataPooling.GetMessageData<StringData>();
        data.value = "Grab";
        playerUnit.SendMessageEx(MessageTitles.cameramanager_setfollowcameradistance, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), data);
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.IsClimbingMove = false;
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.InitVelocity();

        if(playerUnit.IsCanClimbingCancel == true)
        {
            if(playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f )
            {
                animator.SetTrigger("ClimbingCancel");
                playerUnit.IsCanClimbingCancel = false;
            }
        }

        if(playerUnit.stamina.Value <= 0.0f)
        {
            playerUnit.IsClimbingMove = false;
            playerUnit.IsLedge = false;

            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.x = 0.0f;
            currentRot.z = 0.0f;
            playerUnit.Transform.rotation = Quaternion.Euler(currentRot);

            playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.Falling;

            playerUnit.Detach();

            playerUnit.ChangeState(PlayerUnit.defaultState);
            return;
        }

        playerUnit.CheckLedge();
        float climbingPlaneAngle = Vector3.Dot(Vector3.Cross(playerUnit.Transform.up, Vector3.right), Vector3.forward);
        playerUnit.IsClimbingGround = climbingPlaneAngle > -15f * Mathf.Deg2Rad;

        playerUnit.AddEnergy(playerUnit.IsClimbingMove == true ? playerUnit.ClimbingJumpRestoreEnrgyValue * Time.fixedDeltaTime : 0.0f);

        //playerUnit.UpdateGrab();
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        //playerUnit.UpdateClimbingInput();
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
    }

    public override void OnJump(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.InputVertical == 0.0f && playerUnit.InputHorizontal == 0.0f)
            return;

        playerUnit.ChangeState(PlayerUnit.readyClimbingJumpState);
    }
}
