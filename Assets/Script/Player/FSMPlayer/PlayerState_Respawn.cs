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
        animator.SetFloat("Speed", 0.0f);
        playerUnit.InitVelocity();

        if (playerUnit.GetPrevState == PlayerUnit.ragdollState)
            playerUnit.Ragdoll.ResetRagdoll();

        ActionData action = MessageDataPooling.GetMessageData<ActionData>();
        action.value = () =>
        {
            animator.SetBool("Respawn", true);
            playerUnit.Drone.Respawn(transform);
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
