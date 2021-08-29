using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBehavior : StateMachineBehaviour
{
    private PlayerUnit _playerUnit;

    private void Awake()
    {
        _playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Respawn Enter");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Respawn Exit");
        animator.SetBool("Respawn", false);
        _playerUnit.ChangeState(PlayerUnit.defaultState);
    }
}
