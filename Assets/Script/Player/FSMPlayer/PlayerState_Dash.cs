using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Dash : PlayerState
{
    private float _dashTime = 0.3f;
    private float _dashSpeed = 25f;
    private float _originAnimatorSpeed;

    private float _lateTime;

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Dash";
        _originAnimatorSpeed = animator.speed;
        animator.speed = 0.0f;

        //StartCoroutine(playerUnit.StartDashCoolTime());
        playerUnit.UseDash();
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        _lateTime = 0.0f;
        animator.speed = _originAnimatorSpeed;
        playerUnit.InitVelocity();
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        //playerUnit.Move(playerUnit.Transform.forward * playerUnit.DashSpeed, Time.fixedDeltaTime);
        //playerUnit.Rigidbody.MovePosition(playerUnit.Transform.position + (playerUnit.Transform.forward * playerUnit.DashSpeed * Time.fixedDeltaTime));
        playerUnit.Rigidbody.velocity = playerUnit.Transform.forward * playerUnit.DashSpeed;
        _lateTime += Time.fixedDeltaTime;

        if (_lateTime >= playerUnit.DashTime)
            playerUnit.ChangeState(playerUnit.GetPrevState);
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
