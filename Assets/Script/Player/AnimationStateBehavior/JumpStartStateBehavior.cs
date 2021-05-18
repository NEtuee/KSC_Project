using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpStartStateBehavior : StateMachineBehaviour
{
    private PlayerCtrl_Ver2 player;
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(player == null)
        {
            player = (PlayerCtrl_Ver2)GameManager.Instance.player;
        }

        player.pressJump = false;
    }
}
