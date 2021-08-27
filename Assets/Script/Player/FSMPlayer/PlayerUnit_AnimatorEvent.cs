using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public partial class PlayerUnit
{
    private void EndClimbMove()
    {
        SetClimbMove(false);
    }

    private void Left()
    {
        _animator.SetBool("Left", false);
    }

    private void Right()
    {
        _animator.SetBool("Left", true);
    }

    private void StartClimbingJump()
    {
        ChangeState(climbingJumpState);
    }

    private void CanCancelReadyClimbing()
    {
        isCanReadyClimbingCancel = true;
    }

    private void CanClimbingCancel()
    {
        SetCanClimbingCancel(true);
    }

    private void ClimbingSound()
    {
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>(); ;
        soundData.id = 1006; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
    }

    private void StartLedgeUp()
    {
        //handIk.DisableLeftHandIk();
        //handIk.DisableRightHandIk();
    }

    private void EndLedgeUp()
    {
        ChangeState(defaultState);
    }

    private void EndReadyGrab()
    {
        ChangeState(grabState);
    }

    private void TraceUp(int left)
    {

    }

    private void TraceDown(int left)
    {

    }

    private void TraceUpLeft()
    {

    }

    private void TraceUpRight()
    {

    }

    private void TraceDownLeft()
    {

    }

    private void TraceDownRight()
    {

    }

    private void TraceLedge()
    {

    }

    private void UpdateRightHandPos()
    {

    }

    private void RightTrace()
    {

    }

    private void UpdateLeftHandPos()
    {

    }

    private void LeftTrace()
    {

    }
}
