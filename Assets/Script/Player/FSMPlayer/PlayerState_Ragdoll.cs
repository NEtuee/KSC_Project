using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Ragdoll : PlayerState
{
    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "Ragdoll";

        //if (prevState == PlayerState.Gesture)
        //    drone.CompleteRespawn();

        if (playerUnit.LookDir == Vector3.zero)
        {
            playerUnit.Transform.rotation = Quaternion.LookRotation(playerUnit.Transform.forward, Vector3.up);
        }
        else
        {
            playerUnit.Transform.rotation = Quaternion.LookRotation(playerUnit.LookDir, Vector3.up);
        }

        playerUnit.Transform.SetParent(null);
        playerUnit.HandIK.DisableHandIK();
        playerUnit.AirTime = 0.0f;
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
    }
}
