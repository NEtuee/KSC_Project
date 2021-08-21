using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Jump : PlayerState
{

    private Vector3 prevDir;

    public override void Enter(PlayerUnit playerUnit,Animator animator)
    {
        playerUnit.currentStateName = "Jump";
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.CurrentJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
        playerUnit.CurrentJumpPower = Mathf.Clamp(playerUnit.CurrentJumpPower, playerUnit.MinJumpPower, 50.0f);

        if(playerUnit.IsGround == true)
        {
            playerUnit.ChangeState(playerUnit.DefaultState);
        }
        Vector3 moveDir;
        Vector3 lookDir;
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

        if (playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
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

        Quaternion targetRotation = Quaternion.identity;
        if (playerUnit.CurrentSpeed != 0.0f && lookDir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation,
                Quaternion.LookRotation(lookDir, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }
        else
        {
            Vector3 targetDir = playerUnit.Transform.forward;
            if (Vector3.Cross(playerUnit.Transform.up, Vector3.up).normalized.x != 0.0f)
            {
                targetDir.Set(playerUnit.Transform.up.x, 0.0f, playerUnit.Transform.up.z);
                targetDir.Normalize();
            }

            targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            playerUnit.Transform.rotation = Quaternion.Lerp(playerUnit.Transform.rotation, targetRotation, Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }

        moveDir *= playerUnit.CurrentSpeed;

        if (Physics.Raycast(playerUnit.Transform.position + playerUnit.CapsuleCollider.center, moveDir,
                    playerUnit.CapsuleCollider.radius + playerUnit.CurrentSpeed * Time.fixedDeltaTime, playerUnit.FrontCheckLayer))
        {
            playerUnit.Move(Vector3.up * playerUnit.CurrentJumpPower,Time.fixedDeltaTime);
        }
        else
        {
            playerUnit.Move(moveDir+(Vector3.up * playerUnit.CurrentJumpPower), Time.fixedDeltaTime);        
        }
    }

    public override void AnimatorMove(Animator animator)
    {
    }
}
