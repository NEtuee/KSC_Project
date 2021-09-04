using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_Throw : GhostStateBase
{
    public override string stateIdentifier => "Throw";

    private bool _throw = false;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        //fix ragdoll

        target.AnimationChange(4);

        _timeCounter.InitTimer("Wait",0f,8f);
        _timeCounter.InitTimer("Throw",0f,5.216f);
        _throw = false;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        if(!_throw)
        {
            _timeCounter.IncreaseTimerSelf("Throw",out var th,deltaTime);
            if(th)
            {
                _throw = true;
                ThrowRagdoll();
            }
        }
        

        _timeCounter.IncreaseTimerSelf("Wait",out var limit,deltaTime);
        if(limit)
        {
            target.EnableMovement();
            StateChange("RandomMove");
        }
    }

    public void ThrowRagdoll()
    {
        Debug.Log("Throw");
    }
}
