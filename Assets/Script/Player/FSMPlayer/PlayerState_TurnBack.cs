using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class PlayerState_TurnBack : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        Vector3 move = playerUnit.Transform.position + (playerUnit.MoveDir.normalized * animator.deltaPosition.magnitude);
        Quaternion rot = playerUnit.Transform.rotation * animator.deltaRotation;
        playerUnit.Transform.SetPositionAndRotation(move, rot);
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "TurnBack";

        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1018; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
        playerUnit.SendMessageEx(MessageTitles.fmod_attachPlay, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);
        animator.SetTrigger("TurnBack");
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        ComputeMoveDir(playerUnit);

        if (playerUnit.IsGround == false)
            playerUnit.ChangeState(PlayerUnit.jumpState);
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    private void ComputeMoveDir(PlayerUnit playerUnit)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerUnit.transform.position + Vector3.up, Vector3.down, out hit, 2f, playerUnit.GrounLayer))
        {
            playerUnit.MoveDir = (Vector3.ProjectOnPlane(playerUnit.transform.forward, hit.normal)).normalized;
        }
        else
        {
            playerUnit.MoveDir = playerUnit.transform.forward;
        }
    }
}
