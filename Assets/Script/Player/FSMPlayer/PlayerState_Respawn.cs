using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class PlayerState_Respawn : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Respawn";

        animator.SetFloat("Speed", 0.0f);
        playerUnit.InitVelocity();

        if (playerUnit.GetPrevState == PlayerUnit.ragdollState)
            playerUnit.Ragdoll.ResetRagdoll();

        BoolData data = MessageDataPooling.GetMessageData<BoolData>();
        data.value = true;
        playerUnit.SendMessageEx(MessageTitles.timemanager_timestop, UniqueNumberBase.GetSavedNumberStatic("TimeManager"), data);

        ActionData action = MessageDataPooling.GetMessageData<ActionData>();
        action.value = () =>
        {
            //animator.SetBool("Respawn", true);
            //playerUnit.Drone.Respawn(transform);
            BoolData data = MessageDataPooling.GetMessageData<BoolData>();
            data.value = false;
            playerUnit.SendMessageEx(MessageTitles.timemanager_timestop, UniqueNumberBase.GetSavedNumberStatic("TimeManager"), data);
            playerUnit.ChangeState(PlayerUnit.defaultState);
        };
        playerUnit.SendMessageEx(MessageTitles.uimanager_fadeinout, UniqueNumberBase.GetSavedNumberStatic("UIManager"), action);
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
