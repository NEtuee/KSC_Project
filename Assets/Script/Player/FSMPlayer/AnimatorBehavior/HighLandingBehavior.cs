using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLandingBehavior : StateMachineBehaviour
{
    private PlayerUnit _playerUnit;

    private void Awake()
    {
        _playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("HighLanding", false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_playerUnit.GetState != PlayerUnit.ragdollState)
         _playerUnit.ChangeState(PlayerUnit.defaultState);
    }
}
