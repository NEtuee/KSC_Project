using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCtrl_Dummy : MonoBehaviour
{
    private PlayerCtrl_Ver2 owner;
    private Animator animator;
    [SerializeField] private Vector3 rootPosition;

    private void Start()
    {
        owner=GetComponent<PlayerCtrl_Ver2>();
        animator = GetComponent<Animator>();
    }

    private void EndTurnBack()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }

    private void StartStop()
    {
        animator.applyRootMotion = true;
    }

    private void EndStop()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }

    private void JumpTiming()
    {
        owner.Jump();
    }

    private void StartLandingAdditive()
    {
        //animator.SetLayerWeight(1, 1f);
    }

    private void EndLandingAdditive()
    {
       //animator.SetLayerWeight(1, 0f);
    }

    private void EndClimbMove()
    {
        owner.SetClimbMove(false);
    }
    private void Left()
    {
        animator.SetBool("Left",false);
    }

    private void Right()
    {
        animator.SetBool("Left", true);
    }

    private void CanInput()
    {
    }

    private void EndLedgeUp()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }

    private void EndGetUp()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }
}
