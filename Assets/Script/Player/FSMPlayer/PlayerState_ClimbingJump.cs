using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MD;

public class PlayerState_ClimbingJump : PlayerState
{
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

        playerUnit.stamina.Value -= playerUnit.ClimbingJumpConsumeValue;
        playerUnit.stamina.Value = Mathf.Clamp(playerUnit.stamina.Value, 0.0f, playerUnit.MaxStamina);
        playerUnit.AddEnergy(playerUnit.ClimbingJumpRestoreEnrgyValue);
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.IsGround == true)
        {
            playerUnit.ChangeState(PlayerUnit.defaultState);
            return;
        }

        Vector3 upDirect = Vector3.zero;
        switch (playerUnit.ClimbingJumpDirection)
        {
            case ClimbingJumpDirection.Up:
                playerUnit.CurrentClimbingJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
                playerUnit.CurrentClimbingJumpPower = Mathf.Clamp(playerUnit.CurrentClimbingJumpPower, playerUnit.MinJumpPower, 50f);
                playerUnit.MoveDir = transform.up;
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
                    float normalizeTime = (Time.time - playerUnit.ClimbingJumpStartTime) / playerUnit.KeepClimbingJumpTime;
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
                }
                break;
            case ClimbingJumpDirection.Right:
                {
                    playerUnit.MoveDir = transform.right;
                    float normalizeTime = (Time.time - playerUnit.ClimbingJumpStartTime) / playerUnit.KeepClimbingJumpTime;
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
                }
                break;
        }

        playerUnit.MoveDir *= playerUnit.CurrentClimbingJumpPower;
        Vector3 finalDir = playerUnit.MoveDir + upDirect;
        playerUnit.Move(finalDir,Time.fixedDeltaTime);

        if (Time.time - playerUnit.ClimbingJumpStartTime >= playerUnit.KeepClimbingJumpTime)
        {
            playerUnit.MoveDir = playerUnit.MoveDir.normalized * finalDir.magnitude;

            playerUnit.IsGround = false;
            playerUnit.ClimbingJump();
            playerUnit.ChangeState(PlayerUnit.jumpState);
            if (playerUnit.ClimbingJumpDirection != ClimbingJumpDirection.Left &&
                playerUnit.ClimbingJumpDirection != ClimbingJumpDirection.Right)
                playerUnit.CurrentJumpPower = playerUnit.CurrentClimbingJumpPower;

            InputAction.CallbackContext dummy = new InputAction.CallbackContext();
            OnGrab(dummy, playerUnit, animator);
        }
    }
    

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void OnGrab(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        Vector3 point1;
        RaycastHit hit;
        Transform playerTransform = playerUnit.Transform;
        if (playerUnit.stamina.Value > 0.0f)
        {
            point1 = playerTransform.position + playerUnit.CapsuleCollider.center - playerTransform.forward;
            if (Physics.SphereCast(point1, playerUnit.CapsuleCollider.radius * 1.5f, playerTransform.forward, out hit, 3f, playerUnit.DetectionLayer))
            {
                if (playerUnit.DetectionCanClimbingAreaByVertexColor(point1, playerTransform.forward, 3f) == true)
                {
                    return;
                }

                if (Physics.Raycast(playerTransform.position + playerTransform.TransformDirection(playerUnit.WallUnderCheckOffset), playerTransform.forward, 3f, playerUnit.DetectionLayer) == false)
                {
                    return;
                }

                playerUnit.Transform.SetParent(hit.collider.transform);
                playerUnit.Attach();

                if (playerUnit.LedgeChecker.IsDetectedLedge() == false)
                {
                    playerUnit.ChangeState(PlayerUnit.readyGrabState);
                }
                else
                {
                    playerUnit.ChangeState(PlayerUnit.grabState);
                    playerUnit.ChangeState(PlayerUnit.hangLedgeState);

                }

                playerUnit.Transform.rotation = Quaternion.LookRotation(-hit.normal);
                playerUnit.Transform.position = (hit.point - playerUnit.Transform.up * (playerUnit.CapsuleCollider.height * 0.5f)) + (hit.normal) * 0.05f;

                playerUnit.MoveDir = Vector3.zero;

                return;
            }
            else
            {
                point1 = playerTransform.position + Vector3.up;
                if (Physics.Raycast(point1, -playerTransform.up, out hit, 1.5f, playerUnit.DetectionLayer))
                {
                    point1 += playerTransform.forward;
                    if (Physics.Raycast(point1, -playerTransform.up, 1.5f, playerUnit.DetectionLayer) == false)
                        return;

                    playerTransform.rotation = Quaternion.LookRotation(-hit.normal, playerTransform.forward);
                    playerTransform.position = (hit.point) + (hit.normal) * playerUnit.CapsuleCollider.radius;

                    playerTransform.SetParent(hit.collider.transform);
                    playerUnit.Attach();
                    playerUnit.MoveDir = Vector3.zero;

                    playerUnit.ChangeState(PlayerUnit.readyGrabState);

                    return;
                }
            }

            point1 = playerTransform.position + playerTransform.up * playerUnit.CapsuleCollider.height * 0.5f - playerTransform.forward;
            if (Physics.SphereCast(point1, playerUnit.CapsuleCollider.radius, playerTransform.forward, out hit, 5f, playerUnit.LedgeAbleLayer))
            {
                RaycastHit ledgePointHit;
                point1 = playerTransform.position + playerTransform.up * playerUnit.CapsuleCollider.height * 2;
                if (Physics.SphereCast(point1, playerUnit.CapsuleCollider.radius * 2f, -playerTransform.up, out ledgePointHit,
                    playerUnit.CapsuleCollider.height * 2, playerUnit.AdjustAbleLayer))
                {
                    if (Vector3.Distance(ledgePointHit.point, playerTransform.position) > playerUnit.HangAbleEdgeDist)
                    {
                        return;
                    }

                    playerTransform.rotation = Quaternion.LookRotation(-hit.normal);
                    playerTransform.position = (hit.point - playerTransform.up * (playerUnit.CapsuleCollider.height * 0.5f)) + (hit.normal) * 0.05f;

                    playerUnit.InitVelocity();
                    playerUnit.MoveDir = Vector3.zero;

                    playerTransform.SetParent(hit.collider.transform);
                    playerUnit.Attach();

                    playerUnit.ChangeState(PlayerUnit.grabState);
                    playerUnit.ChangeState(PlayerUnit.hangEdgeState);

                    return;
                }
            }
        }
    }
}
