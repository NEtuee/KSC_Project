using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnStateBehavior : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameManager.Instance == null)
            return;

        ((PlayerCtrl_Ver2)GameManager.Instance.player).ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }
}
