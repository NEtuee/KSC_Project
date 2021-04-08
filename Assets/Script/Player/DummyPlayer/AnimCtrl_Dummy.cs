using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCtrl_Dummy : MonoBehaviour
{
    private PlayerCtrl_Ver2 owner;
    private Animator animator;
    private HandIKCtrl handIk;

    private Transform leftFootTransform;
    private Transform rightFootTransform;

    private void Start()
    { 
        owner=GetComponent<PlayerCtrl_Ver2>();
        animator = GetComponent<Animator>();
        handIk = GetComponent<HandIKCtrl>();

        if(animator != null)
        {
            leftFootTransform = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            rightFootTransform = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        }
    }

    private void EndTurnBack()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }

    private void EndStop()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }

    private void JumpTiming()
    {
        owner.Jump();
        GameManager.Instance.soundManager.Play(16, Vector3.zero,transform);
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

    private void StartLedgeUp()
    {
        handIk.DisableLeftHandIk();
        handIk.DisableRightHandIk();
    }

    private void EndLedgeUp()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }

    private void EndGetUp()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Default);
    }

    private void JogFootStep(int left)
    {
        Vector3 footStepPosition;
        if (left == 0)
            footStepPosition = leftFootTransform.position;
        else
            footStepPosition = rightFootTransform.position;

        GameManager.Instance.soundManager.Play(12, footStepPosition);
    }

    private void RunFootStep(int left)
    {
        Vector3 footStepPosition;
        if (left == 0)
            footStepPosition = leftFootTransform.position;
        else
            footStepPosition = rightFootTransform.position;

        GameManager.Instance.soundManager.Play(13, footStepPosition);
    }

    private void StartClimbingJump()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.ClimbingJump);
    }
}
