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

    public EMPGun gun;

    private void Start()
    {
        owner = GetComponent<PlayerCtrl_Ver2>();
        animator = GetComponent<Animator>();
        handIk = GetComponent<HandIKCtrl>();

        if (animator != null)
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
        GameManager.Instance.soundManager.Play(1003, Vector3.zero, transform);
    }

    private void EndClimbMove()
    {
        owner.SetClimbMove(false);
    }
    private void Left()
    {
        animator.SetBool("Left", false);
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

    private void GetupSound()
    {
        GameManager.Instance.soundManager.Play(1017, Vector3.up, transform);
    }

    private void JogFootStep(int left)
    {
        Vector3 footStepPosition;
        if (left == 0)
            footStepPosition = leftFootTransform.position;
        else
            footStepPosition = rightFootTransform.position;

        GameManager.Instance.soundManager.Play(1000, footStepPosition);
    }

    private void RunFootStep(int left)
    {
        Vector3 footStepPosition;
        if (left == 0)
            footStepPosition = leftFootTransform.position;
        else
            footStepPosition = rightFootTransform.position;

        GameManager.Instance.soundManager.Play(1001, footStepPosition);
    }

    private void StartClimbingJump()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.ClimbingJump);
    }

    private void EndShot()
    {
    }

    private void EndGrabShake()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Grab);
    }

    private void EndReadyGrab()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.Grab);
    }

    private void CanCancelReadyClimbing()
    {
        owner.isCanReadyClimbingCancel = true;
    }

    private void CanClimbingCancel()
    {
        owner.SetCanClimbingCancel(true);
    }

    private void LandingSound()
    {
        GameManager.Instance.soundManager.Play(1004, Vector3.up, transform);
    }
}
