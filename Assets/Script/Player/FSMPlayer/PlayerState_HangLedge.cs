using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_HangLedge : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        //var p = playerUnit.Transform.position;
        //p += animator.deltaPosition;
        //playerUnit.Transform.position = p;

        if(playerUnit.climbDir == ClimbDir.Left)
        {
            Vector3 leftPoint = playerUnit.Line.points[playerUnit.leftPointNum].position;
            Vector3 rightPoint = playerUnit.Line.points[playerUnit.rightPointNum].position;
            Vector3 moveDir = leftPoint - rightPoint;
            moveDir.Normalize();
            Vector3 adjustForward = Vector3.Cross(-moveDir, Vector3.up);
            moveDir *= animator.deltaPosition.magnitude;
            //playerUnit.Transform.position += moveDir;
            //playerUnit.nearPointMarker.position += moveDir;

            Vector3 destPointPosition = playerUnit.nearPointMarker.position + moveDir;

            Vector3 u = leftPoint - rightPoint;
            Vector3 v = destPointPosition - rightPoint;
            float s;
            if (u.x != 0.0f)
                s = v.x / u.x;
            else if(u.y != 0.0f)
                s = v.y / u.y;
            else
                s = v.z / u.z;

            //Debug.Log(s);
            if (s > 1.0f)
            {
                if(playerUnit.Line.PassLeft(ref playerUnit.leftPointNum, ref playerUnit.rightPointNum))
                {
                    playerUnit.nearPointMarker.position = destPointPosition;
                    //playerUnit.Transform.position += moveDir;
                    Transform planInfo = playerUnit.Line.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
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
                        pos -= adjustForward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
                    }
                    playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
                }
            }
            else
            {
                playerUnit.nearPointMarker.position = destPointPosition;
                //playerUnit.Transform.position += moveDir;
                Transform planInfo = playerUnit.Line.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
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
                    pos -= adjustForward * playerUnit.DetectionOffset.z;
                    playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
                }
                playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
            }
        }
        else
        {
            Vector3 leftPoint = playerUnit.Line.points[playerUnit.leftPointNum].position;
            Vector3 rightPoint = playerUnit.Line.points[playerUnit.rightPointNum].position;
            Vector3 moveDir = rightPoint - leftPoint;
            moveDir.Normalize();
            Vector3 adjustForward = Vector3.Cross(moveDir, Vector3.up);
            moveDir *= animator.deltaPosition.magnitude;
            //playerUnit.Transform.position += moveDir;
            //playerUnit.nearPointMarker.position += moveDir;

            Vector3 destPointPosition = playerUnit.nearPointMarker.position + moveDir;

            Vector3 u = rightPoint - leftPoint;
            Vector3 v = destPointPosition - leftPoint;
            float s;
            if (u.x != 0.0f)
                s = v.x / u.x;
            else if (u.y != 0.0f)
                s = v.y / u.y;
            else
                s = v.z / u.z;
            //Debug.Log(s);
            if (s > 1.0f)
            {
                if(playerUnit.Line.PassRight(ref playerUnit.leftPointNum, ref playerUnit.rightPointNum))
                {
                    playerUnit.nearPointMarker.position = destPointPosition;
                    //playerUnit.Transform.position += moveDir;
                    Transform planInfo = playerUnit.Line.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
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
                        pos -= adjustForward * playerUnit.DetectionOffset.z;
                        playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
                    }
                    playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
                }
            }
            else
            {
                playerUnit.nearPointMarker.position = destPointPosition;
                //playerUnit.Transform.position += moveDir;
                Transform planInfo = playerUnit.Line.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);
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
                    pos -= adjustForward * playerUnit.DetectionOffset.z;
                    playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, Quaternion.LookRotation(adjustForward), Time.deltaTime * 5.0f);
                }
                playerUnit.Transform.position = Vector3.Lerp(playerUnit.Transform.position, pos, Time.deltaTime * 50.0f);
            }
        }
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "HangLedge";

        playerUnit.IsLedge = true;
        playerUnit.IsClimbingMove = false;
        animator.SetBool("IsLedge", true);
        playerUnit.HandIK.ActiveLedgeIK(true);
        //playerUnit.AdjustLedgeOffset();
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        Debug.Log("Exit HangLedge");
        animator.SetBool("IsLedge", false);
        playerUnit.IsLedge = false;
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        //playerUnit.UpdateGrab();
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.UpdateClimbingInput();
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
        //if (playerUnit.InputVertical == 1.0f)
        //{
        //    playerUnit.ChangeState(PlayerUnit.readyClimbingJumpState);
        //    return;
        //}

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
