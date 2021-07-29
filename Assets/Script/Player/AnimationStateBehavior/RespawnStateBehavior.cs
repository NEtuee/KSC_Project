using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnStateBehavior : StateMachineBehaviour
{
    private PlayerCtrl_Ver2 player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl_Ver2>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Respawn", false);
        player.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }
}
