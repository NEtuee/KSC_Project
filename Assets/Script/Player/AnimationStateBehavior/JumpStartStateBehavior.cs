using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpStartStateBehavior : StateMachineBehaviour
{
    private PlayerCtrl_Ver2 player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl_Ver2>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.pressJump = false;
    }
}
