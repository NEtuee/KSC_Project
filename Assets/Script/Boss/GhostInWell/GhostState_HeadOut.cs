using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_HeadOut : GhostStateBase
{
    public override string stateIdentifier => "HeadOut";

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        target.EnableMovement();
        _timeCounter.InitTimer("Wait",0f,4f);
        target.AnimationChange(2);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("Wait",out var limit, deltaTime);
        if(limit)
        {
            StateChange("RandomMove");
        }
    }
}
