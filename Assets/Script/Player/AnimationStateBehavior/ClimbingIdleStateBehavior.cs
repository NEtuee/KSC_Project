using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingIdleStateBehavior : StateMachineBehaviour
{
    private PlayerCtrl_Ver2 player;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(player == null)
        {
            player = (PlayerCtrl_Ver2)GameManager.Instance.player;
        }

        player.SetClimbMove(false);
    }
}
