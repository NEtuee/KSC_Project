using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_MoveToPassage : GhostStateBase
{
    public override string stateIdentifier => "MoveToPassage";

    public AnimationCurve curve;
    public GhostPassage[] passage;

    private int _passageTarget;
    private bool _arrived = false;
    private bool _ikSet = false;
    private Vector3[] _ikOrigins;

    public void SetPassage(int target) {_passageTarget = target;}

    public override void Assign()
    {
        base.Assign();
        _ikOrigins = new Vector3[target.arms.Count];
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("Wait",0f,0.3f);
        _timeCounter.InitTimer("ArmMove");

        _arrived = false;
        _ikSet = false;
    }

    public override void StateChanged(StateBase targetState)
    {
        base.StateChanged(targetState);

        if(targetState.stateIdentifier == "CatchStance")
            return;

        target.EnableMovement();
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        target.SetTarget(passage[_passageTarget].bodyPoint.position);

        if(target.SyncTurn(passage[_passageTarget].bodyPoint,deltaTime))
        {
            transform.rotation = Quaternion.LookRotation(target.PlaneDirection(passage[_passageTarget].bodyPoint),Vector3.up);
            _arrived = true;
        }
        else
        {
            _arrived = false;
        }

        var dist = Vector3.Distance(transform.position,passage[_passageTarget].bodyPoint.position);
        if(dist <= target.distanceAccuracy)
        {
        }
        else
        {
            target.Move(target.targetDirection,target.moveSpeed,deltaTime);
            _arrived = false;
        }

        if(_arrived)
        {
            _timeCounter.IncreaseTimerSelf("Wait",out var limit, deltaTime);
            if(limit)
            {
                var time = _timeCounter.IncreaseTimerSelf("ArmMove",out limit, 1.5f * deltaTime);
                for(int i = 0; i < target.arms.Count; ++i)
                {
                    target.arms[i].DisableMovement();
                    target.arms[i].ik.position = Vector3.Lerp(_ikOrigins[i],i == 0 ? 
                            passage[_passageTarget].leftArmPoint.position : 
                            passage[_passageTarget].rightArmPoint.position, time) + 
                            new Vector3(0f,curve.Evaluate(time));
                }

                if(limit)
                {
                    if(target.passageCheck)
                    {
                        target.SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse, 
                                        UniqueNumberBase.GetSavedNumberStatic("CameraManager"), null);
                        target.AnimationChange(1);
                        StateChange("CatchStance");
                    }
                    else
                    {
                        StateChange("RandomMove");
                    }
                    
                }
            }
            else
            {
                for(int i = 0; i < target.arms.Count; ++i)
                {
                    _ikOrigins[i] = target.arms[i].ik.position;
                }
            }
            
        }
    }
}
