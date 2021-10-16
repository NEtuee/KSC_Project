using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MD;

public class PlayerState_ClimbingJump : PlayerState
{
    private float _minKeepJumpTime = 0.1f;

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "ClimbingJump";

        playerUnit.ClimbingJumpStartTime = Time.time;

        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1007; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
        playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);

        if (playerUnit.ClimbingJumpDirection == ClimbingJumpDirection.Left || playerUnit.ClimbingJumpDirection == ClimbingJumpDirection.Right)
            playerUnit.CurrentClimbingJumpPower = playerUnit.ClimbingHorizonJumpPower;
        else
            playerUnit.CurrentClimbingJumpPower = playerUnit.ClimbingUpJumpPower;

        playerUnit.ClimbingJump();

        playerUnit.AddEnergy(playerUnit.ClimbingJumpRestoreEnrgyValue);

        playerUnit.HandIK.DisableHandIK();
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        switch (playerUnit.ClimbingJumpDirection)
        {
            case ClimbingJumpDirection.Up:
                animator.SetInteger("ReadyClimbNum", 0);
                break;
            case ClimbingJumpDirection.Left:
                animator.SetInteger("ReadyClimbNum", 1);
                break;
            case ClimbingJumpDirection.Right:
                animator.SetInteger("ReadyClimbNum", 2);
                break;
        }
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.IsGround == true)
        {
            playerUnit.ChangeState(PlayerUnit.defaultState);
            return;
        }

        Vector3 upDirect = Vector3.zero;
        float keepJumpTime = 0.0f;
        switch (playerUnit.ClimbingJumpDirection)
        {
            case ClimbingJumpDirection.Up:
                playerUnit.CurrentClimbingJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
                playerUnit.CurrentClimbingJumpPower = Mathf.Clamp(playerUnit.CurrentClimbingJumpPower, playerUnit.MinJumpPower, 50f);
                playerUnit.MoveDir = transform.up;
                keepJumpTime = playerUnit.KeepClimbingUpJumpTime;
                break;
            case ClimbingJumpDirection.UpLeft:
                playerUnit.CurrentClimbingJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
                playerUnit.CurrentClimbingJumpPower = Mathf.Clamp(playerUnit.CurrentClimbingJumpPower, playerUnit.MinJumpPower, 50f);
                if (playerUnit.CurrentClimbingJumpPower > 0)
                {
                    playerUnit.MoveDir = (playerUnit.Transform.up + -playerUnit.Transform.right).normalized;
                }
                else
                {
                    playerUnit.MoveDir = playerUnit.Transform.up;
                }

                break;
            case ClimbingJumpDirection.UpRight:
                playerUnit.CurrentClimbingJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
                playerUnit.CurrentClimbingJumpPower = Mathf.Clamp(playerUnit.CurrentClimbingJumpPower, playerUnit.MinJumpPower, 50f);
                if (playerUnit.CurrentClimbingJumpPower > 0)
                {
                    playerUnit.MoveDir = (playerUnit.Transform.up + playerUnit.Transform.right).normalized;
                }
                else
                {
                    playerUnit.MoveDir = playerUnit.Transform.up;
                }

                break;
            case ClimbingJumpDirection.Left:
                {
                    playerUnit.MoveDir = -playerUnit.Transform.right;
                    float normalizeTime = (Time.time - playerUnit.ClimbingJumpStartTime) / playerUnit.KeepClimbingHorizonJumpTime;
                    if (normalizeTime < 0.5f)
                    {
                        upDirect = transform.up * 3f;
                    }
                    else
                    {
                        upDirect = -transform.up * 3f;
                    }

                    playerUnit.CurrentClimbingJumpPower = playerUnit.ClimbingHorizonJumpPower *
                                               playerUnit.ClimbingHorizonJumpSpeedCurve.Evaluate(normalizeTime);
                    keepJumpTime = playerUnit.KeepClimbingHorizonJumpTime;
                }
                break;
            case ClimbingJumpDirection.Right:
                {
                    playerUnit.MoveDir = transform.right;
                    float normalizeTime = (Time.time - playerUnit.ClimbingJumpStartTime) / playerUnit.KeepClimbingHorizonJumpTime;
                    if (normalizeTime < 0.5f)
                    {
                        upDirect = transform.up * 3f;
                    }
                    else
                    {
                        upDirect = -transform.up * 3f;
                    }

                    playerUnit.CurrentClimbingJumpPower = playerUnit.ClimbingHorizonJumpPower *
                                               playerUnit.ClimbingHorizonJumpSpeedCurve.Evaluate(normalizeTime);
                    keepJumpTime = playerUnit.KeepClimbingHorizonJumpTime;
                }
                break;
        }

        playerUnit.MoveDir *= playerUnit.CurrentClimbingJumpPower;
        Vector3 finalDir = playerUnit.MoveDir + upDirect;
        playerUnit.Move(finalDir,Time.fixedDeltaTime);

        if (Time.time - playerUnit.ClimbingJumpStartTime >= keepJumpTime)
        {
            if (playerUnit.ClimbingJumpDirection != ClimbingJumpDirection.Up)
            {
                InputAction.CallbackContext dummy = new InputAction.CallbackContext();
                if(OnGrabClimbingJump(dummy,playerUnit,animator))
                {
                    return;
                }
            }

            playerUnit.MoveDir = playerUnit.MoveDir.normalized * finalDir.magnitude;

            playerUnit.IsGround = false;
            playerUnit.ClimbingJump();
            playerUnit.ChangeState(PlayerUnit.jumpState);
            if (playerUnit.ClimbingJumpDirection != ClimbingJumpDirection.Left &&
                playerUnit.ClimbingJumpDirection != ClimbingJumpDirection.Right)
                playerUnit.CurrentJumpPower = playerUnit.CurrentClimbingJumpPower;
            
            //InputAction.CallbackContext dummy = new InputAction.CallbackContext();
            //OnGrab(dummy, playerUnit, animator);
        }
    }
    

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void OnGrab(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if (Time.time - playerUnit.ClimbingJumpStartTime < _minKeepJumpTime)
            return;
        playerUnit.TryGrab();
    }

    public bool OnGrabClimbingJump(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if (Time.time - playerUnit.ClimbingJumpStartTime < _minKeepJumpTime)
            return false;
        return playerUnit.TryGrab();
    }
}
