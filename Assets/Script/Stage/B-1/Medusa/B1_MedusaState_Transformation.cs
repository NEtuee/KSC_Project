using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MedusaState_Transformation : B1_MedusaStateBase
{
    public override string stateIdentifier => "Transformation";
    public float transformationTime = 5f;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        target.ChangeMainAnimation(0);

        _timeCounter.InitTimer("timer",0f,transformationTime);
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
