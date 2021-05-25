using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRespawnStateBehavior : StateMachineBehaviour
{
    public FloatingMove _floatingMove;
    private Drone _drone;

    private void Awake()
    {
        _drone = GameObject.FindGameObjectWithTag("Drone").GetComponent<Drone>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _floatingMove = animator.GetComponent<FloatingMove>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _floatingMove.enabled = true;
        animator.enabled = false;
        _drone.CompleteRespawn();
    }
}
