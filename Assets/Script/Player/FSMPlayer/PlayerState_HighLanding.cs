using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class PlayerState_HighLanding : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.CurrentSpeed = 0.0f;
        animator.SetFloat("Speed", 0.0f);
        animator.SetBool("HighLanding", true);
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1004; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
        playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);
        playerUnit.SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), null);
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
