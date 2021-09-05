using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WankoState_HitTarget : WankoStateBase
{
    public override string stateIdentifier => "HitTarget";

    public float waitTime = 5f;

    private string prevStateId = "";

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("Wait",0f,waitTime);
        target.graphAnimator.Play("HitTarget",target.model);

        prevStateId = prevState.stateIdentifier;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        
        _timeCounter.IncreaseTimerSelf("Wait",out var limit,deltaTime);
        if(limit)
        {
            StateChange(prevStateId);
        }
    }
}
