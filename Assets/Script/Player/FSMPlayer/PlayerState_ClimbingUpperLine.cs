using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_ClimbingUpperLine : PlayerState
{
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {

    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        playerUnit.currentStateName = "ClimbingUpperLine";

        Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);

        //playerUnit.Transform.SetParent(null);
        _startPosition = playerUnit.Transform.position;
        //Debug.Log(_startPosition);
        _endPosition = playerUnit.LineTracker.position + (planInfo.up * playerUnit.DetectionOffset.y) - (planInfo.forward * playerUnit.DetectionOffset.z);

        animator.SetTrigger("ClimbingUpper");
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {

    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climb_ShortJump") == false)
        {
            playerUnit.Transform.position = _startPosition;
            return;
        }

        float normalizeTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        
        if (normalizeTime < 1.0f)
        {
            Vector3 startToEnd = _endPosition - _startPosition;
            playerUnit.Transform.position = _startPosition + startToEnd * normalizeTime;
        }
        else if(normalizeTime >= 1.0f)
        {
            playerUnit.Transform.SetParent(playerUnit.LineTracker);
            animator.SetTrigger("EndClimbingUpperLine");
            playerUnit.ChangeState(PlayerUnit.readyGrabState);
        }
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climb_ShortJump") == false)
        {
            playerUnit.Transform.position = _startPosition;
            return;
        }
    }
}
