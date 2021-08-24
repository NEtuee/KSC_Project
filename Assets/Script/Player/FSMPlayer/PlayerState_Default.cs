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
            playerUnit.ChangeState(PlayerUnit.jumpState);
            return;
        }

        Vector3 lookDir;
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

        if(playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
        {
            playerUnit.MoveDir = (camForward * playerUnit.InputVertical) + (camRight * playerUnit.InputHorizontal);
            playerUnit.MoveDir.Normalize();
            playerUnit.LookDir = playerUnit.MoveDir;
        }
        else
        {
            playerUnit.MoveDir = prevDir;
            playerUnit.MoveDir.Normalize();
            playerUnit.LookDir = playerUnit.MoveDir;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerUnit.transform.position + Vector3.up, Vector3.down, out hit, 2f, playerUnit.GrounLayer))
        {
            playerUnit.MoveDir = (Vector3.ProjectOnPlane(playerUnit.transform.forward, hit.normal)).normalized;
        }

        Quaternion targetRotation = Quaternion.identity;
        if (playerUnit.CurrentSpeed != 0.0f && playerUnit.LookDir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(playerUnit.LookDir, Vector3.up);
            playerUnit.transform.rotation = Quaternion.Lerp(playerUnit.transform.rotation,
                Quaternion.LookRotation(playerUnit.LookDir, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(playerUnit.transform.forward, Vector3.up);
            playerUnit.transform.rotation = Quaternion.Lerp(playerUnit.transform.rotation,
                Quaternion.LookRotation(playerUnit.transform.forward, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }

        playerUnit.MoveDir *= playerUnit.CurrentSpeed;

        if (Physics.Raycast(playerUnit.transform.position + playerUnit.CapsuleCollider.center, playerUnit.MoveDir,
                    playerUnit.CapsuleCollider.radius + playerUnit.CurrentSpeed * Time.fixedDeltaTime, playerUnit.FrontCheckLayer) == false)
        {
            playerUnit.Move(playerUnit.MoveDir, Time.fixedDeltaTime);
        }

        //playerUnit.PrevDir = playerUnit.MoveDir;
    }

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void OnJump(PlayerUnit playerUnit, Animator animator)
    {
        if(playerUnit.JumpStart == false)
        {
            playerUnit.JumpStart = true;
            animator.SetTrigger("Jump");
        }
    }
}
