using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public partial class PlayerUnit
{
    private void StartClimbingMove()
    {
        Vector3 near;
        Line line = new Line();
        Line.DetectLine(CapsuleStart, CapsuleEnd, CapsuleRadius, transform, out near, ref line);
    }

    private void EndClimbMove()
    {
        SetClimbMove(false);
        Vector3 near;
        Line line = new Line();
        Line.DetectLine(CapsuleStart,CapsuleEnd,CapsuleRadius,transform, out near, ref line);
    }

    private void EndGetUp()
    {
        ChangeState(defaultState);
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
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>(); 
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

    private void GetupSound()
    {
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1017; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
    }

    private void LandingSound()
    {
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1004; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
    }

    private void EndRespawn()
    {
        _animator.SetBool("Respawn", false);
        ChangeState(defaultState);
    }

    private void StartLedupAdjust()
    {
        LedgeUpAdjust = true;
    }

    private void JogFootStep(int left)
    {
        Vector3 footStepPosition;
        if (left == 0)
            footStepPosition = _leftFootTransform.position;
        else
            footStepPosition = _rightFootTransform.position;

        SoundPlayData soundData = MessageDataPooling.GetMessageData<SoundPlayData>();
        soundData.id = 1000; soundData.position = footStepPosition; soundData.returnValue = false; soundData.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundData);
    }

    private void RunFootStep(int left)
    {
        Vector3 footStepPosition;
        if (left == 0)
            footStepPosition = _leftFootTransform.position;
        else
            footStepPosition = _rightFootTransform.position;

        SoundPlayData soundData = MessageDataPooling.GetMessageData<SoundPlayData>();
        soundData.id = 1001; soundData.position = footStepPosition; soundData.returnValue = false; soundData.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundData);
    }
}
