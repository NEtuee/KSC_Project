using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_FlySpiderState_Idle : B1_FlySpiderStateBase
{
    public override string stateIdentifier => "Idle";
    
    public float idleTime = 3f;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("idle",0f,idleTime);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("idle",out var limit, deltaTime);
        if(limit)
        {
            StateChange("RushReady");
        }
    }
}
