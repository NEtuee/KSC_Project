using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCtrl_Dummy : MonoBehaviour
{
    private PlayerCtrl_Ver2 owner;
    private Animator animator;
    private HandIKCtrl handIk;
    [SerializeField] private Vector3 rootPosition;

    private void Start()
    { 
        owner=GetComponent<PlayerCtrl_Ver2>();
        animator = GetComponent<Animator>();
        handIk = GetComponent<HandIKCtrl>();
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
        GameManager.Instance.soundManager.Play(16, Vector3.zero,transform);
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

    private void StartLeftHandClimbing()
    {
        //handIk.DisableLeftHandIk();
        //handIk.DisableRightHandIk();
    }

    private void StartrightHandClimbing()
    {
        //handIk.DisableLeftHandIk();
        //handIk.DisableRightHandIk();
    }

    private void TraceLeftHand()
    {
        handIk.EnableLeftTrace();
    }

    private void TraceRightHand()
    {
        handIk.EnableRightTrace();
    }

    private void TraceBothHand()
    {
        handIk.EnableLeftTrace();
        handIk.EnableRightTrace();
    }

    private void TraceOff()
    {
        handIk.DisableLeftTrace();
        handIk.DisableRightTrace();
    }

    private void JogFootStep(int left)
    {
        Vector3 footStepPosition;
        if (left == 0)
            footStepPosition = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        else
            footStepPosition = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

        GameManager.Instance.soundManager.Play(12, footStepPosition);
    }

    private void RunFootStep(int left)
    {
        Vector3 footStepPosition;
        if (left == 0)
            footStepPosition = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        else
            footStepPosition = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

        GameManager.Instance.soundManager.Play(13, footStepPosition);
    }

    private void StartJumpEnd()
    {
        //GameManager.Instance.soundManager.Play(18, transform.position);
    }

    private void MoveLeftHand()
    {
        //handIk.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.0f);
    }

    private void MoveRightHand()
    {
        //handIk.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.0f);
    }

    private void StartClimbingJump()
    {
        owner.ChangeState(PlayerCtrl_Ver2.PlayerState.ClimbingJump);
    }
}
