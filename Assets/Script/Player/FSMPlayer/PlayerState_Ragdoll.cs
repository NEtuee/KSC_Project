using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Ragdoll : PlayerState
{
    private bool _isCanUseQuickStand = true;
    private float _quickStandCoolTime = 15.0f;

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

    private IEnumerator UpdateCoolTime()
    {
        _isCanUseQuickStand = false;
        float time = 0.0f;
        while(time < _quickStandCoolTime)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _isCanUseQuickStand = true;
    }

    public override void OnQuickStand(InputAction.CallbackContext value, PlayerUnit playerUnit, Animator animator)
    {
        if(playerUnit.Ragdoll.GetRagdollState() == PlayerRagdoll.RagdollState.Ragdoll && playerUnit.Ragdoll.PelvisGrounded == true && _isCanUseQuickStand == true)
        {
            playerUnit.Ragdoll.ReturnAnimated();
            StartCoroutine(UpdateCoolTime());
        }
    }
}
