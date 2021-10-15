using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MD;

public class PlayerState_HangLedge : PlayerState
{
    private float _ledgeUpInputTime = 0.0f;
    private float _ledgeUpTriggerTime = 0.3f;

    private bool _canRelease = false;
    private TimeCounterEx _grabTimer = new TimeCounterEx();
    private float _minKeepGrabTime = 0.05f;
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        float deltaMagnitude = animator.deltaPosition.magnitude;

        //if (Mathf.Abs(playerUnit.InputHorizontal) == 0.0f)
        //{
        //    Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
        //    if (planInfo != null)
        //    {
        //        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(-planInfo.forward), Time.deltaTime * 5.0f);
        //    }
        //    return;
        //}

        if (deltaMagnitude == 0.0f)
            return;

        Vector3 leftPoint;
        Vector3 rightPoint;
        Vector3 moveDir = new Vector3();
        Vector3 adjustForward = new Vector3();
        Vector3 destPointPosition = new Vector3();
        Vector3 u = new Vector3();
        Vector3 v = new Vector3();

        if (playerUnit.climbDir == ClimbDir.Left)
        {
            leftPoint = playerUnit.CurrentFollowLine.points[playerUnit.leftPointNum].position;
            rightPoint = playerUnit.CurrentFollowLine.points[playerUnit.rightPointNum].position;
            moveDir = leftPoint - rightPoint;
            moveDir.Normalize();
            adjustForward = Vector3.Cross(-moveDir, Vector3.up);
            moveDir *= deltaMagnitude;

            destPointPosition = playerUnit.LineTracker.position + moveDir;
            destPointPosition = rightPoint + Intersection.ShortestPointLineSegmentAndPoint(rightPoint, leftPoint, destPointPosition);

            u = leftPoint - rightPoint;
            v = destPointPosition - rightPoint;
        }
        else if (playerUnit.climbDir == ClimbDir.Right)
        {
            leftPoint = playerUnit.CurrentFollowLine.points[playerUnit.leftPointNum].position;
            rightPoint = playerUnit.CurrentFollowLine.points[playerUnit.rightPointNum].position;
            moveDir = rightPoint - leftPoint;
            moveDir.Normalize();
            adjustForward = Vector3.Cross(moveDir, Vector3.up);
            moveDir *= deltaMagnitude;

            destPointPosition = playerUnit.LineTracker.position + moveDir;
            destPointPosition = leftPoint + Intersection.ShortestPointLineSegmentAndPoint(leftPoint, rightPoint, destPointPosition);

            u = rightPoint - leftPoint;
            v = destPointPosition - leftPoint;
        }
        else if(playerUnit.prevClimbDir == ClimbDir.Left)
        {
            leftPoint = playerUnit.CurrentFollowLine.points[playerUnit.leftPointNum].position;
            rightPoint = playerUnit.CurrentFollowLine.points[playerUnit.rightPointNum].position;
            moveDir = leftPoint - rightPoint;
            moveDir.Normalize();
            adjustForward = Vector3.Cross(-moveDir, Vector3.up);
            moveDir *= deltaMagnitude;

            destPointPosition = playerUnit.LineTracker.position + moveDir;
            destPointPosition = rightPoint + Intersection.ShortestPointLineSegmentAndPoint(rightPoint, leftPoint, destPointPosition);

            u = leftPoint - rightPoint;
            v = destPointPosition - rightPoint;
        }
        else if(playerUnit.prevClimbDir == ClimbDir.Right)
        {
            leftPoint = playerUnit.CurrentFollowLine.points[playerUnit.leftPointNum].position;
            rightPoint = playerUnit.CurrentFollowLine.points[playerUnit.rightPointNum].position;
            moveDir = rightPoint - leftPoint;
            moveDir.Normalize();
            adjustForward = Vector3.Cross(moveDir, Vector3.up);
            moveDir *= deltaMagnitude;

            destPointPosition = playerUnit.LineTracker.position + moveDir;
            destPointPosition = leftPoint + Intersection.ShortestPointLineSegmentAndPoint(leftPoint, rightPoint, destPointPosition);

            u = rightPoint - leftPoint;
            v = destPointPosition - leftPoint;
        }

        float s = u.x != 0.0f ? v.x / u.x : (u.y != 0.0f ? v.y / u.y : v.z / u.z);

        if (s > 1.0f)
        {
            if (playerUnit.climbDir == ClimbDir.Left)
            {
                if (playerUnit.CurrentFollowLine.PassLeft(ref playerUnit.leftPointNum, ref playerUnit.rightPointNum))
                {
                    playerUnit.LineTracker.position = destPointPosition;
                    Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
                    Vector3 pos = new Vector3();
                    if (planInfo != null)
                    {
                        pos = destPointPosition + (planInfo.up * playerUnit.DetectionOffset.y);
                        pos -= planInfo.forward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(-planInfo.forward), Time.deltaTime * 5.0f);
                    }
                    else
                    {
                        pos = destPointPosition + (playerUnit.Transform.up * playerUnit.DetectionOffset.y);
                        pos += adjustForward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
                    }
                    playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
                }
            }
            else if(playerUnit.climbDir == ClimbDir.Right)
            {
                if (playerUnit.CurrentFollowLine.PassRight(ref playerUnit.leftPointNum, ref playerUnit.rightPointNum))
                {
                    playerUnit.LineTracker.position = destPointPosition;
                    Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
                    Vector3 pos = new Vector3();
                    if (planInfo != null)
                    {
                        pos = destPointPosition + (planInfo.up * playerUnit.DetectionOffset.y);
                        pos -= planInfo.forward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(-planInfo.forward), Time.deltaTime * 5.0f);
                    }
                    else
                    {
                        pos = destPointPosition + (playerUnit.Transform.up * playerUnit.DetectionOffset.y);
                        pos += adjustForward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
                    }
                    playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
                }
            }
            else if(playerUnit.prevClimbDir == ClimbDir.Left)
            {
                if (playerUnit.CurrentFollowLine.PassLeft(ref playerUnit.leftPointNum, ref playerUnit.rightPointNum))
                {
                    playerUnit.LineTracker.position = destPointPosition;
                    Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
                    Vector3 pos = new Vector3();
                    if (planInfo != null)
                    {
                        pos = destPointPosition + (planInfo.up * playerUnit.DetectionOffset.y);
                        pos -= planInfo.forward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(-planInfo.forward), Time.deltaTime * 5.0f);
                    }
                    else
                    {
                        pos = destPointPosition + (playerUnit.Transform.up * playerUnit.DetectionOffset.y);
                        pos += adjustForward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
                    }
                    playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
                }
            }
            else if(playerUnit.prevClimbDir == ClimbDir.Right)
            {
                if (playerUnit.CurrentFollowLine.PassRight(ref playerUnit.leftPointNum, ref playerUnit.rightPointNum))
                {
                    playerUnit.LineTracker.position = destPointPosition;
                    Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
                    Vector3 pos = new Vector3();
                    if (planInfo != null)
                    {
                        pos = destPointPosition + (planInfo.up * playerUnit.DetectionOffset.y);
                        pos -= planInfo.forward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(-planInfo.forward), Time.deltaTime * 5.0f);
                    }
                    else
                    {
                        pos = destPointPosition + (playerUnit.Transform.up * playerUnit.DetectionOffset.y);
                        pos += adjustForward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
                    }
                    playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
                }
            }
        }
        else
        {
            playerUnit.LineTracker.position = destPointPosition;
            Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
            Vector3 pos = new Vector3();
            if (planInfo != null)
            {
                pos = destPointPosition + (planInfo.up * playerUnit.DetectionOffset.y);
                pos -= planInfo.forward * playerUnit.DetectionOffset.z;
                playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(-planInfo.forward), Time.deltaTime * 5.0f);
            }
            else
            {
                pos = destPointPosition + (playerUnit.Transform.up * playerUnit.DetectionOffset.y);
                pos += adjustForward * playerUnit.DetectionOffset.z;
                playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
            }
            playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
        }
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "HangLedge";

        playerUnit.IsLedge = true;
        playerUnit.IsClimbingMove = false;
        //animator.SetBool("IsLedge", true);
        animator.SetBool("IsGrab", true);
        //playerUnit.HandIK.ActiveLedgeIK(true);

        playerUnit.CurrentJumpPower = 0.0f;
        playerUnit.CurrentSpeed = 0.0f;

        playerUnit.CapsuleCollider.height = 1f;
        playerUnit.CapsuleCollider.center = new Vector3(0.0f, 0.5f, 0.0f);
        playerUnit.IsJump = false;
        playerUnit.CurrentJumpPower = 0.0f;

        playerUnit.InitVelocity();
        playerUnit.FootIK.DisableFeetIk();

        _grabTimer.InitTimer("MinGrabTime", 0.0f, _minKeepGrabTime);
        _canRelease = false;
        //playerUnit.AdjustLedgeOffset();
        StringData data = MessageDataPooling.GetMessageData<StringData>();
        data.value = "Grab";
        playerUnit.SendMessageEx(MessageTitles.cameramanager_setfollowcameradistance, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), data);

        playerUnit.Attach();
        //Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
        //Vector3 pos = new Vector3();
        //if (planInfo != null)
        //{
        //    pos = playerUnit.LineTracker.position + (planInfo.up * playerUnit.DetectionOffset.y);
        //    pos -= planInfo.forward * playerUnit.DetectionOffset.z;
        //    playerUnit.Transform.rotation = Quaternion.LookRotation(-planInfo.forward);
        //}
        //else
        //{
        //    pos = playerUnit.LineTracker.position + (playerUnit.Transform.up * playerUnit.DetectionOffset.y);
        //    pos += playerUnit.Transform.forward * playerUnit.DetectionOffset.z;
        //    playerUnit.Transform.rotation = Quaternion.LookRotation(playerUnit.Transform.forward);
        //}
        //playerUnit.Transform.position = pos;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        animator.SetBool("IsLedge", false);
        playerUnit.IsLedge = false;

        playerUnit.LineTracker.SetParent(null);
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        //playerUnit.UpdateGrab();
        playerUnit.InitVelocity();

        if (playerUnit.climbDir != ClimbDir.Stop)
           playerUnit.AddEnergy(playerUnit.ClimbingJumpRestoreEnrgyValue * Time.fixedDeltaTime);
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.UpdateClimbingInput();

        _grabTimer.IncreaseTimerSelf("MinGrabTime", out bool limit, Time.deltaTime);
        if(limit)
        {
            _canRelease = true;
        }

        if (playerUnit.InputVertical >= 0.5f)
        {
            _ledgeUpInputTime += Time.deltaTime;
        }
        else
            _ledgeUpInputTime = 0f;

        if (_ledgeUpInputTime >= _ledgeUpTriggerTime)
        {
            LedgeUp(playerUnit, animator);
        }
    }

    private void LedgeUp(PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.IsLedge == true && playerUnit.SpaceChecker.Overlapped() == false)
        {
            playerUnit.IsLedge = false;
            animator.SetTrigger("LedgeUp");
            //animator.SetBool("IsLedge", false);
            animator.SetBool("IsGrab", false);

            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.x = 0.0f;
            currentRot.z = 0.0f;
            transform.rotation = Quaternion.Euler(currentRot);

            playerUnit.ChangeState(PlayerUnit.ledgeUpState);
        }
    }

    public override void OnGrabRelease(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if (_canRelease == false)
            return;

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
        if (_canRelease == false)
            return;

        if (playerUnit.InputVertical >= 0.5f || playerUnit.InputHorizontal >= 0.5f || playerUnit.InputHorizontal <= -0.5f)
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
