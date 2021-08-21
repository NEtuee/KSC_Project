using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Default : PlayerState
{
    private Vector3 prevDir;

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Default";

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
        playerUnit.CurrentJumpPower = 0.0f;

        if (playerUnit.IsGround == false)
        {
            playerUnit.ChangeState(playerUnit.JumpState);
            return;
        }

        Vector3 moveDir;
        Vector3 lookDir;
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
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
        if (Physics.Raycast(playerUnit.Transform.position + Vector3.up, Vector3.down, out hit, 2f, playerUnit.GrounLayer))
        {
            moveDir = (Vector3.ProjectOnPlane(playerUnit.Transform.forward, hit.normal)).normalized;
        }

        Quaternion targetRotation = Quaternion.identity;
        if (playerUnit.CurrentSpeed != 0.0f && lookDir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation,
                Quaternion.LookRotation(lookDir, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(playerUnit.Transform.forward, Vector3.up);
            playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation,
                Quaternion.LookRotation(playerUnit.Transform.forward, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }

        moveDir *= playerUnit.CurrentSpeed;

        if (Physics.Raycast(playerUnit.Transform.position + playerUnit.CapsuleCollider.center, moveDir,
                    playerUnit.CapsuleCollider.radius + playerUnit.CurrentSpeed * Time.fixedDeltaTime, playerUnit.FrontCheckLayer) == false)
        {
            playerUnit.Move(moveDir,Time.fixedDeltaTime);
        }
    }

    public override void AnimatorMove(Animator animator)
    {
    }

}
