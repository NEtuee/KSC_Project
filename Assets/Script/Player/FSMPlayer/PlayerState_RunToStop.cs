using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_RunToStop : PlayerState
{

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
        if (Physics.Raycast(playerUnit.transform.position + Vector3.up + playerUnit.MoveDir.normalized * 0.5f, Vector3.down, 1.5f, playerUnit.GrounLayer))
        {
            playerUnit.transform.position += playerUnit.MoveDir.normalized * animator.deltaPosition.magnitude;
        }
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "RunToStop";

        animator.SetTrigger("RunToStop");
        animator.SetFloat("HorizonWeight", 0.0f);

        playerUnit.CurrentSpeed = 0.0f;
        playerUnit.HorizonWeight = 0.0f;

        ComputeMoveDir(playerUnit);
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
