using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaState_Active : MedusaFallPointStateBase
{
    public override string stateIdentifier => "Active";

    public float activeTime = 5f;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("timer",0f,activeTime);
        target.AnimationChange(7);

    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("timer",out var limit, deltaTime);
        if(limit)
        {
            StateChange("CenterMove");
            target.SetIKMovement(true);
        }
    }
}
