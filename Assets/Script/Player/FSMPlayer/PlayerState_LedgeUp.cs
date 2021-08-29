using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_LedgeUp : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        var p = playerUnit.Transform.position;
        p += animator.deltaPosition;
        playerUnit.Transform.position = p;
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "LedgeUp";
        playerUnit.HandIK.DisableHandIK();
        playerUnit.CapsuleCollider.isTrigger = true;
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
