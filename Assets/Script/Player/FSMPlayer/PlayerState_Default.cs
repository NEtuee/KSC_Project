using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Default : PlayerState
{
    private Vector3 prevDir;

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.CapsuleCollider.isTrigger = false;
        animator.applyRootMotion = false;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if(playerUnit.IsGround == false)
        {
            playerUnit.ChangeState(playerUnit.JumpStata);
            return;
        }

        Vector3 moveDir;
        Vector3 lookDir;
        Vector3 camForward = transform.forward;
        camForward.y = 0;
        Vector3 camRight = transform.right;
        camRight.y = 0;

        if(playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
        {
            moveDir = (camForward * playerUnit.InputVertical) + (camRight * playerUnit.InputHorizontal);
            moveDir.Normalize();
            lookDir = moveDir;
        }
        else
        {
            moveDir = prevDir;
            moveDir.Normalize();
            lookDir = moveDir;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, playerUnit.GrounLayer))
        {
            moveDir = (Vector3.ProjectOnPlane(transform.forward, hit.normal)).normalized;
        }

        Quaternion targetRotation = Quaternion.identity;
        if (playerUnit.CurrentSpeed != 0.0f && lookDir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(lookDir, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(transform.forward, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }

        moveDir *= playerUnit.CurrentSpeed;

        if (Physics.Raycast(transform.position + playerUnit.CapsuleCollider.center, moveDir,
                    playerUnit.CapsuleCollider.radius + playerUnit.CurrentSpeed * Time.fixedDeltaTime, playerUnit.FrontCheckLayer) == false)
        {
            playerUnit.Move(moveDir);
        }
    }

    public override void AnimatorMove(Animator animator)
    {
    }

}
