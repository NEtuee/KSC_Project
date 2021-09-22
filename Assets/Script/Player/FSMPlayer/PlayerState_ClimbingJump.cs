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
            OnGrab(dummy , playerUnit, animator);
        }
    }
    

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void OnGrab(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if (playerUnit.ClimbingLineManager == null)
            return;

        Vector3 nearPosition = new Vector3();
        Line line = new Line();

        bool detect = false;
        ClimbingLine detectLine = null;
        Line detectLineElement = new Line();
        Vector3 prevNearPosition = new Vector3();
        Vector3 finalNearPosition = new Vector3();
        foreach (var climbingLine in playerUnit.ClimbingLineManager.climbingLines)
        {
            if (climbingLine.DetectLine(playerUnit.CapsuleStart, playerUnit.CapsuleEnd, playerUnit.CapsuleRadius, playerUnit.Transform, out nearPosition, ref line))
            {
                detect = true;
                if (detectLine == null)
                {
                    detectLine = climbingLine;
                    detectLineElement = line;
                    prevNearPosition = nearPosition;
                    finalNearPosition = nearPosition;
                }
                else
                {
                    if (Vector3.SqrMagnitude(nearPosition - playerUnit.CapsuleStart) < Vector3.SqrMagnitude(prevNearPosition - playerUnit.CapsuleStart))
                    {
                        detectLine = climbingLine;
                        detectLineElement = line;
                        prevNearPosition = nearPosition;
                        finalNearPosition = nearPosition;
                    }
                }
            }
        }


        if (detect == true)
        {
            playerUnit.Line = detectLine;
            playerUnit.nearPointMarker.position = finalNearPosition;
            playerUnit.StartLineClimbing(finalNearPosition);
            //Vector3 playerToP1 = (playerUnit.Line.points[detectLineElement.p1].position - playerUnit.Transform.position).normalized;
            //Vector3 playerForward = playerUnit.Transform.forward;
            //playerForward.y = 0.0f;
            //playerForward.Normalize();
            //Vector3 cross = Vector3.Cross(playerToP1, playerForward);
            //if (Vector3.Dot(cross, Vector3.up) > 0)
            //{
            //    playerUnit.rightPointNum = detectLineElement.p2;
            //    playerUnit.leftPointNum = detectLineElement.p1;
            //}
            //else
            //{
            //    playerUnit.rightPointNum = detectLineElement.p1;
            //    playerUnit.leftPointNum = detectLineElement.p2;
            //}

            if(playerUnit.Line.directionType == DirectionType.LeftMin)
            {
                playerUnit.leftPointNum = Mathf.Min(detectLineElement.p1, detectLineElement.p2);
                playerUnit.rightPointNum = Mathf.Max(detectLineElement.p1, detectLineElement.p2);
            }
            else
            {
                playerUnit.leftPointNum = Mathf.Max(detectLineElement.p1, detectLineElement.p2);
                playerUnit.rightPointNum = Mathf.Min(detectLineElement.p1, detectLineElement.p2);
            }
        }
    }
}
