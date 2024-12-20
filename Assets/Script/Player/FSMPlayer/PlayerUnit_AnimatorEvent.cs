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
        //Line.DetectLine(CapsuleStart, CapsuleEnd, CapsuleRadius, transform, out near, ref line);
    }

    private void EndClimbMove()
    {
        SetClimbMove(false);
        Vector3 near;
        Line line = new Line();
        //Line.DetectLine(CapsuleStart,CapsuleEnd,CapsuleRadius,transform, out near, ref line);
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

        SendMessageEx(MessageTitles.gamepadVibrationManager_vibrationByKey, GetSavedNumber("GamepadVibrationManager"), "RightHand");
    }

    private void RightHand()
    {
        SendMessageEx(MessageTitles.gamepadVibrationManager_vibrationByKey, GetSavedNumber("GamepadVibrationManager"), "RightHand");

        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1006; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
    }

    private void LeftHand()
    {
        SendMessageEx(MessageTitles.gamepadVibrationManager_vibrationByKey, GetSavedNumber("GamepadVibrationManager"), "LeftHand");

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

    private void CanSkipRunToStopEvent()
    {
        CanSkipRunToStop = true;
    }

    private void Kick()
    {
        RaycastHit hit;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4f, kickLayer);
        if (colliders.Length != 0)
        {
            foreach(var coll in colliders)
            {
                MessageReceiver receiver;
                if (coll.TryGetComponent<MessageReceiver>(out receiver))
                {
                    Message msg = new Message();
                    msg.Set(MessageTitles.object_kick, receiver.uniqueNumber, this, this);
                    receiver.ReceiveMessage(msg);
                }
            }
        }
        //if (Physics.Raycast(transform.position + CapsuleCollider.center, transform.forward, out hit, 4f, kickLayer))
        //{
        //    MessageReceiver receiver;
        //    if (hit.collider.TryGetComponent<MessageReceiver>(out receiver))
        //    {
        //        Message msg = new Message();
        //        msg.Set(MessageTitles.object_kick, receiver.uniqueNumber, null, (Object)this);
        //        receiver.ReceiveMessage(msg);
        //    }
        //}
    }
}
