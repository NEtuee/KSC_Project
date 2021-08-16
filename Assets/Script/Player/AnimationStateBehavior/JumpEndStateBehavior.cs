using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEndStateBehavior : StateMachineBehaviour
{
    private PlayerCtrl_Ver2 player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl_Ver2>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        animator.ResetTrigger("FastStop");

        if(player.GetState() != PlayerCtrl_Ver2.PlayerState.Respawn)
           player.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }
}
