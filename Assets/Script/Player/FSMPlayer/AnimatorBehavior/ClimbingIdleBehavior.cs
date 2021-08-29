using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingIdleBehavior : StateMachineBehaviour
{
    private PlayerUnit _playerUnit;

    private void Awake()
    {
        _playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerUnit.SetClimbMove(false);
    }
}
