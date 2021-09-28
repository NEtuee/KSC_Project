using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MD;

public class PlayerState_Jump : PlayerState
{

    public override void Enter(PlayerUnit playerUnit,Animator animator)
    {
        if (playerUnit.GetPrevState != PlayerUnit.climbingJumpState)
            playerUnit.MoveDir = playerUnit.Transform.forward * playerUnit.CurrentSpeed;
        playerUnit.currentStateName = "Jump";
        playerUnit.HorizonWeight = 0.0f;
        playerUnit.ClimbingJumpDirection = ClimbingJumpDirection.Up;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
       
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.CurrentJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
        playerUnit.CurrentJumpPower = Mathf.Clamp(playerUnit.CurrentJumpPower, playerUnit.MinJumpPower, 50.0f);

        playerUnit.AirTime += Time.fixedDeltaTime;

        if(playerUnit.IsGround == true)
        {
            //Debug.Log(playerUnit.AirTime);
            if (playerUnit.AirTime >= playerUnit.LandingFactor)
            {
                playerUnit.ChangeState(PlayerUnit.highLandingState);
                return;
            }
            else
            {
                playerUnit.ChangeState(PlayerUnit.defaultState);
                return;
            }
        }

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        camForward.Normalize();

        if (playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
        {
            playerUnit.MoveDir = (camForward * playerUnit.InputVertical) + (camRight * playerUnit.InputHorizontal);
            playerUnit.MoveDir.Normalize();
            playerUnit.LookDir = playerUnit.MoveDir;
        }
        else
        {
            playerUnit.MoveDir = playerUnit.PrevDir;
            playerUnit.MoveDir.Normalize();
            playerUnit.LookDir = playerUnit.MoveDir;
        }

        Quaternion targetRotation = Quaternion.identity;
        if (playerUnit.CurrentSpeed != 0.0f && playerUnit.LookDir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(playerUnit.LookDir, Vector3.up);
            playerUnit.transform.rotation = Quaternion.Lerp(playerUnit.transform.rotation,
                Quaternion.LookRotation(playerUnit.LookDir, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }
        else
        {
            Vector3 targetDir = playerUnit.transform.forward;
            if (Vector3.Cross(playerUnit.transform.up, Vector3.up).normalized.x != 0.0f)
            {
                targetDir.Set(playerUnit.transform.up.x, 0.0f, playerUnit.transform.up.z);
                targetDir.Normalize();
            }

            targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            playerUnit.transform.rotation = Quaternion.Lerp(playerUnit.transform.rotation, targetRotation, Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }

        playerUnit.MoveDir *= playerUnit.CurrentSpeed;

        if (Physics.Raycast(playerUnit.transform.position + playerUnit.CapsuleCollider.center, playerUnit.MoveDir,
                    playerUnit.CapsuleCollider.radius + playerUnit.CurrentSpeed * Time.fixedDeltaTime, playerUnit.FrontCheckLayer))
        {
            playerUnit.Move(Vector3.up * playerUnit.CurrentJumpPower,Time.fixedDeltaTime);
        }
        else
        {
            playerUnit.Move(playerUnit.MoveDir + (Vector3.up * playerUnit.CurrentJumpPower), Time.fixedDeltaTime);        
        }

        playerUnit.PrevDir = playerUnit.LookDir;
    }

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void OnAim(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if (value.action.IsPressed())
        {
            if (playerUnit.AimLock == false)
            {
                AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                soundData.id = 1008; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);

                AttachSoundPlayData chargeSoundPlayData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                chargeSoundPlayData.id = 1013; chargeSoundPlayData.localPosition = Vector3.up; chargeSoundPlayData.parent = transform; chargeSoundPlayData.returnValue = true;
                playerUnit.SendMessageQuick(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), chargeSoundPlayData);

                playerUnit.ChangeState(PlayerUnit.aimingState);
            }
        }
    }

    public override void OnJump(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.IsNearGround == true && playerUnit.JumpStart == false)
        {
            playerUnit.JumpStart = true;
            animator.SetTrigger("Jump");
        }
    }

    public override void OnGrab(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.TryGrab();
    }
}
