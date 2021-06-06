using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLandingStateBehavior : StateMachineBehaviour
{
    private PlayerCtrl_Ver2 player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl_Ver2>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("HighLanding",false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }
}
