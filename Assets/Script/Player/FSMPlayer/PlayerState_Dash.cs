using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Dash : PlayerState
{
    private float _originAnimatorSpeed;

    private float _lateTime;

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Dash";
        animator.SetBool("Dash", true);
        animator.ResetTrigger("Jump");

        //StartCoroutine(playerUnit.StartDashCoolTime());
        playerUnit.UseDash();
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        animator.SetBool("Dash", false);
        _lateTime = 0.0f;
        playerUnit.InitVelocity();

        playerUnit.RunTime = 0.0f;
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        //playerUnit.Move(playerUnit.Transform.forward * playerUnit.DashSpeed, Time.fixedDeltaTime);
        //playerUnit.Rigidbody.MovePosition(playerUnit.Transform.position + (playerUnit.Transform.forward * playerUnit.DashSpeed * Time.fixedDeltaTime));
        playerUnit.Rigidbody.velocity = playerUnit.Transform.forward * playerUnit.DashSpeed;
        _lateTime += Time.fixedDeltaTime;

        if (_lateTime >= playerUnit.DashTime)
        {
            if(playerUnit.IsGround)
            {
                playerUnit.ChangeState(PlayerUnit.dashEndState);
            }
            else
            {
                playerUnit.ChangeState(playerUnit.GetPrevState);
            }
        }
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
