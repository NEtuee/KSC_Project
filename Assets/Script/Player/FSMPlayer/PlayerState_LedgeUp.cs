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
        //playerUnit.HandIK.DisableHandIK();
        playerUnit.CapsuleCollider.isTrigger = true;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.LedgeUpAdjust = false;
        animator.SetBool("IsGround", true);
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        //playerUnit.CurrentJumpPower -= playerUnit.Gravity * Time.fixedDeltaTime;
        //playerUnit.CurrentJumpPower = Mathf.Clamp(playerUnit.CurrentJumpPower, playerUnit.MinJumpPower, 50.0f);

        if (playerUnit.LedgeUpAdjust == false)
            return;

        RaycastHit groundHit;
        if(Physics.Raycast(playerUnit.Transform.position, -playerUnit.Transform.up, out groundHit, 1f, playerUnit.GrounLayer))
        {
            Vector3 position = playerUnit.Transform.position;
            //float height = position.y;
            //height = Mathf.Lerp(height, groundHit.point.y, 20f * Time.fixedDeltaTime);
            position.y = groundHit.point.y;
            playerUnit.Transform.position = position;
        }
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
