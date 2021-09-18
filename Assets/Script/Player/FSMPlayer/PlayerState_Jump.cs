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
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

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
