using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Dead : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Dead";
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        Debug.Log("Dead Exit");
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
