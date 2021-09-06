using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShotBehavior : StateMachineBehaviour
{

    private PlayerUnit _playerUnit;
    private void Awake()
    {
        _playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerUnit.CanCharge = true;
    }
}
