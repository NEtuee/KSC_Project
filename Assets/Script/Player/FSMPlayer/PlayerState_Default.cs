using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MD;

public class PlayerState_Default : PlayerState
{
    private Vector3 prevDir;
    private float rotationAngle;

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Default";

        playerUnit.CapsuleCollider.isTrigger = false;
        animator.applyRootMotion = false;
        animator.SetBool("IsGrab", false);
        animator.SetBool("IsLedge", false);
        animator.SetTrigger("Landing");
        playerUnit.FootIK.EnableFeetIk();
        playerUnit.HandIK.DisableHandIK();
        playerUnit.CapsuleCollider.height = 1.898009f;
        playerUnit.CapsuleCollider.center = new Vector3(0.0f, 0.95622f, 0.0f);

        playerUnit.AirTime = 0.0f;

        playerUnit.InitVelocity();

        StringData data = MessageDataPooling.GetMessageData<StringData>();
        data.value = "Default";
        playerUnit.SendMessageEx(MessageTitles.cameramanager_setfollowcameradistance, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), data);

        if (playerUnit.GetPrevState == PlayerUnit.jumpState)
        {
            AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundData.id = 1004; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
            playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);
        }
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.HorizonWeight = 0.0f;
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {

    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.CurrentSpeed >= playerUnit.WalkSpeed)
            playerUnit.AddEnergy(playerUnit.RunRestoreEnergyValue * Time.fixedDeltaTime);
        else if(playerUnit.CurrentSpeed > 0.0f)
            playerUnit.AddEnergy(playerUnit.WalkSpeed * Time.fixedDeltaTime);

        if (playerUnit.IsGround == true)
        {
            playerUnit.CurrentJumpPower = 0.0f;
            playerUnit.InitVelocity();
        }
        else
        {
            playerUnit.CurrentJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
            playerUnit.CurrentJumpPower = Mathf.Clamp(playerUnit.CurrentJumpPower, playerUnit.MinJumpPower, 50.0f);
        }

        if (playerUnit.IsGround == false)
        {
            playerUnit.ChangeState(PlayerUnit.jumpState);
            return;
        }

        Vector3 lookDir;
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

        if(playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
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

        RaycastHit hit;
        if (Physics.Raycast(playerUnit.transform.position + Vector3.up, Vector3.down, out hit, 2f, playerUnit.GrounLayer))
        {
            playerUnit.MoveDir = (Vector3.ProjectOnPlane(playerUnit.transform.forward, hit.normal)).normalized;
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
            targetRotation = Quaternion.LookRotation(playerUnit.transform.forward, Vector3.up);
            playerUnit.transform.rotation = Quaternion.Lerp(playerUnit.transform.rotation,
                Quaternion.LookRotation(playerUnit.transform.forward, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }

        if(playerUnit.CurrentSpeed > playerUnit.WalkSpeed && targetRotation != Quaternion.identity)
        {
            if (playerUnit.LookDir == Vector3.zero)
                playerUnit.LookDir = playerUnit.Transform.forward;

            rotationAngle=(int)Quaternion.Angle(playerUnit.Transform.rotation, Quaternion.LookRotation(playerUnit.LookDir, Vector3.up));
            if (Vector3.Dot(Vector3.Cross(transform.forward, playerUnit.LookDir), transform.up) < 0)
            {
                rotationAngle = -rotationAngle;
            }

            playerUnit.HorizonWeight = Mathf.Lerp(playerUnit.HorizonWeight, rotationAngle, Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }
        else
        {
            playerUnit.HorizonWeight = Mathf.Lerp(playerUnit.HorizonWeight, 0.0f, Time.fixedDeltaTime * 12f);
        }


        playerUnit.MoveDir *= playerUnit.CurrentSpeed;

        if (Physics.Raycast(playerUnit.transform.position + playerUnit.CapsuleCollider.center, playerUnit.MoveDir,
                    playerUnit.CapsuleCollider.radius + playerUnit.CurrentSpeed * Time.fixedDeltaTime, playerUnit.FrontCheckLayer) == false)
        {
            playerUnit.Move(playerUnit.MoveDir, Time.fixedDeltaTime);
        }

        playerUnit.PrevDir = playerUnit.LookDir;
    }

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void OnJump(PlayerUnit playerUnit, Animator animator)
    {
        if(playerUnit.JumpStart == false)
        {
            playerUnit.JumpStart = true;
            animator.SetTrigger("Jump");
        }
    }

    public override void OnAim(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if (value.action.IsPressed())
        {
            if (playerUnit.AimLock == false)
            {
                playerUnit.ChangeState(PlayerUnit.aimingState);
            }
        }
    }

    //public override void OnGrab(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    //{
    //    Vector3 point1;
    //    RaycastHit hit;
    //    Transform playerTransform = playerUnit.Transform;
    //    if (playerUnit.stamina.Value > 0.0f)
    //    {
    //        point1 = playerTransform.position + playerUnit.CapsuleCollider.center - playerTransform.forward;
    //        if (Physics.SphereCast(point1, playerUnit.CapsuleCollider.radius * 1.5f, playerTransform.forward, out hit, 3f, playerUnit.DetectionLayer))
    //        {
    //            if (playerUnit.DetectionCanClimbingAreaByVertexColor(point1, playerTransform.forward, 3f) == true)
    //            {
    //                return;
    //            }

    //            if (Physics.Raycast(playerTransform.position + playerTransform.TransformDirection(playerUnit.WallUnderCheckOffset), playerTransform.forward, 3f, playerUnit.DetectionLayer) == false)
    //            {
    //                return;
    //            }

    //            playerTransform.SetParent(hit.collider.transform);
    //            playerUnit.Attach();

    //            if (playerUnit.LedgeChecker.IsDetectedLedge() == false)
    //            {
    //                playerUnit.ChangeState(PlayerUnit.readyGrabState);
    //            }
    //            else
    //            {
    //                playerUnit.ChangeState(PlayerUnit.grabState);
    //                playerUnit.ChangeState(PlayerUnit.hangLedgeState);

    //            }

    //            playerUnit.Transform.rotation = Quaternion.LookRotation(-hit.normal);
    //            playerUnit.Transform.position = (hit.point - playerUnit.Transform.up * (playerUnit.CapsuleCollider.height * 0.5f)) + (hit.normal * 0.05f);

    //            playerUnit.MoveDir = Vector3.zero;

    //            return;
    //        }
    //        else
    //        {
    //            point1 = playerTransform.position + Vector3.up;
    //            if (Physics.Raycast(point1, -playerTransform.up, out hit, 1.5f, playerUnit.DetectionLayer))
    //            {
    //                point1 += playerTransform.forward;
    //                if (Physics.Raycast(point1, -playerTransform.up, 1.5f, playerUnit.DetectionLayer) == false)
    //                    return;

    //                playerTransform.rotation = Quaternion.LookRotation(-hit.normal, playerTransform.forward);
    //                playerTransform.position = (hit.point) + (hit.normal) * playerUnit.CapsuleCollider.radius;

    //                playerTransform.SetParent(hit.collider.transform);
    //                playerUnit.Attach();
    //                playerUnit.MoveDir = Vector3.zero;

    //                playerUnit.ChangeState(PlayerUnit.readyGrabState);

    //                return;
    //            }
    //        }

    //        point1 = playerTransform.position + playerTransform.up * playerUnit.CapsuleCollider.height * 0.5f - playerTransform.forward;
    //        if (Physics.SphereCast(point1, playerUnit.CapsuleCollider.radius, playerTransform.forward, out hit, 5f, playerUnit.LedgeAbleLayer))
    //        {
    //            RaycastHit ledgePointHit;
    //            point1 = playerTransform.position + playerTransform.up * playerUnit.CapsuleCollider.height * 2;
    //            if (Physics.SphereCast(point1, playerUnit.CapsuleCollider.radius * 2f, -playerTransform.up, out ledgePointHit,
    //                playerUnit.CapsuleCollider.height * 2, playerUnit.AdjustAbleLayer))
    //            {
    //                if (Vector3.Distance(ledgePointHit.point, playerTransform.position) > playerUnit.HangAbleEdgeDist)
    //                {
    //                    return;
    //                }

    //                playerTransform.rotation = Quaternion.LookRotation(-hit.normal);
    //                playerTransform.position = (hit.point - playerTransform.up * (playerUnit.CapsuleCollider.height * 0.5f)) + (hit.normal) * 0.05f;

    //                playerUnit.InitVelocity();
    //                playerUnit.MoveDir = Vector3.zero;

    //                playerTransform.SetParent(hit.collider.transform);
    //                playerUnit.Attach();

    //                playerUnit.ChangeState(PlayerUnit.grabState);
    //                playerUnit.ChangeState(PlayerUnit.hangEdgeState);

    //                return;
    //            }
    //        }
    //    }
    //}

    public override void OnDash(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.CurrentSpeed < playerUnit.WalkSpeed)
            return;

        playerUnit.ChangeState(PlayerUnit.dashState);
    }
}
