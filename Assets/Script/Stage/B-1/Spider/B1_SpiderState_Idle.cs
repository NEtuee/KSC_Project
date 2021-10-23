using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_SpiderState_Idle : B1_SpiderStateBase
{
    public override string stateIdentifier => "Idle";

    public float idleTime = 3f;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("timer",0f,idleTime);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("timer",out var limit, deltaTime);
        if(limit)
        {
            StateChange("Turn");
        }
    }
}
