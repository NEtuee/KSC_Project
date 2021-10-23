using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MedusaState_HitTarget : B1_MedusaStateBase
{
    public override string stateIdentifier => "HitTarget";

    public float force = 10f;
    public float waitTime = 1f;


    private string _prev;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        target.ChangeArmPushAnimation(0);
        target.Explosion(target.transform.forward,force);
        
        _timeCounter.InitTimer("timer",0f,waitTime);

        _prev = prevState.stateIdentifier;

        StateChange("March");
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        // _timeCounter.IncreaseTimerSelf("timer",out var limit,deltaTime);
        // if(limit)
        // {
        //     StateChange(_prev);
        // }
    }
}
