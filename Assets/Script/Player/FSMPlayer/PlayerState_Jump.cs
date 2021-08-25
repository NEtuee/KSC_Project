using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Jump : PlayerState
{

    private Vector3 prevDir;

    public override void Enter(PlayerUnit playerUnit,Animator animator)
    {
        playerUnit.currentStateName = "Jump";
        playerUnit.HorizonWeight = 0.0f;
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
            playerUnit.ChangeState(PlayerUnit.defaultState);
        }

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

        if (playerUnit.InputVertical != 0.0f || playerUnit.InputHorizontal != 0.0f)
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

        Quaternion targetRotation = Quaternion.identity;
        if (playerUnit.CurrentSpeed != 0.0f && playerUnit.LookDir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(playerUnit.LookDir, Vector3.up);
            playerUnit.transform.rotation = Quaternion.Lerp(playerUnit.transform.rotation,
                Quaternion.LookRotation(playerUnit.LookDir, Vector3.up), Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }
        else
        {
            Vector3 targetDir = playerUnit.transform.forward;
            if (Vector3.Cross(playerUnit.transform.up, Vector3.up).normalized.x != 0.0f)
            {
                targetDir.Set(playerUnit.transform.up.x, 0.0f, playerUnit.transform.up.z);
                targetDir.Normalize();
            }

            targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            playerUnit.transform.rotation = Quaternion.Lerp(playerUnit.transform.rotation, targetRotation, Time.fixedDeltaTime * playerUnit.RotationSpeed);
        }

        playerUnit.MoveDir *= playerUnit.CurrentSpeed;

        if (Physics.Raycast(playerUnit.transform.position + playerUnit.CapsuleCollider.center, playerUnit.MoveDir,
                    playerUnit.CapsuleCollider.radius + playerUnit.CurrentSpeed * Time.fixedDeltaTime, playerUnit.FrontCheckLayer))
        {
            playerUnit.Move(Vector3.up * playerUnit.CurrentJumpPower,Time.fixedDeltaTime);
        }
        else
        {
            playerUnit.Move(playerUnit.MoveDir + (Vector3.up * playerUnit.CurrentJumpPower), Time.fixedDeltaTime);        
        }

        //playerUnit.PrevDir = playerUnit.MoveDir;
    }

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }
}
