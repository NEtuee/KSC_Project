using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingIdleStateBehavior : StateMachineBehaviour
{
    private PlayerCtrl_Ver2 player;

    private void Awake()
    { 
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl_Ver2>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.SetClimbMove(false);
    }
}
