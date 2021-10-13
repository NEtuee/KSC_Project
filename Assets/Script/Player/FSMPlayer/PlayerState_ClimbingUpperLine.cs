using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_ClimbingUpperLine : PlayerState
{
    private Transform _startPosition;
    private Transform _endPosition;

    private void Start()
    {
        CreatePoint();
    }

    public override void AnimatorMove(PlayerUnit playerUnit, Animator animator)
    {

    }

    public override void Enter(PlayerUnit playerUnit, Animator animator)
    {
        CreatePoint();

        playerUnit.currentStateName = "ClimbingUpperLine";

        Transform planInfo = playerUnit.CurrentFollowLine.GetPlaneInfo(playerUnit.leftPointNum, playerUnit.rightPointNum);

        //playerUnit.Transform.SetParent(null);
        _startPosition.SetParent(playerUnit.PrevFollowLine.transform);
        _startPosition.position = playerUnit.Transform.position;
        //Debug.Log(_startPosition);
        _endPosition.SetParent(playerUnit.CurrentFollowLine.transform);
        _endPosition.position = playerUnit.LineTracker.position + (planInfo.up * playerUnit.DetectionOffset.y) - (planInfo.forward * playerUnit.DetectionOffset.z);
        playerUnit.Transform.SetParent(playerUnit.LineTracker);
        animator.SetTrigger("ClimbingUpper");
    }

    public override void Exit(PlayerUnit playerUnit, Animator animator)
    {

    }

    public override void FixedUpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climb_ShortJump") == false)
        {
            playerUnit.Transform.position = _startPosition.position;
            return;
        }

        float normalizeTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        
        if (normalizeTime < 1.0f)
        {
            Vector3 startToEnd = _endPosition.position - _startPosition.position;
            float t = playerUnit.ClimbingUpperLineInterpolateCurve.Evaluate(normalizeTime);
            playerUnit.Transform.position = _startPosition.position + startToEnd * t;
        }
        else if(normalizeTime >= 1.0f)
        {
            animator.SetTrigger("EndClimbingUpperLine");
            playerUnit.ChangeState(PlayerUnit.readyGrabState);
        }
    }

    public override void UpdateState(PlayerUnit playerUnit, Animator animator)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climb_ShortJump") == false)
        {
            playerUnit.Transform.position = _startPosition.position;
            return;
        }
    }

    private void CreatePoint()
    {
        if(_startPosition == null)
        {
            _startPosition = new GameObject("sp").transform;
        }

        if(_endPosition == null)
        {
            _endPosition = new GameObject("ep").transform;
        }
    }
}
