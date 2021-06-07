using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureStateBehavior : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameManager.Instance == null)
            return;

        if(((PlayerCtrl_Ver2)GameManager.Instance.player).GetState() != PlayerCtrl_Ver2.PlayerState.Respawn)
        ((PlayerCtrl_Ver2)GameManager.Instance.player).ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }
}
