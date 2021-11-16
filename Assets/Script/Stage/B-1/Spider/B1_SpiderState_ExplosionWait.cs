using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_SpiderState_ExplosionWait : B1_SpiderStateBase
{
    public override string stateIdentifier => "ExplosionWait";

    public float explosionTime = 3f;
    public float explosionPower = 150f;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        //target.ChangeAnimation("Explosion");
        target.SetIdle(false);
        _timeCounter.InitTimer("Explosion",0f,explosionTime);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("Explosion",out var limit,deltaTime);
        if(limit)
        {
            target.Explosion(target.GetTargetDirection(),explosionPower);
            target.gameObject.SetActive(false);
            
        }
    }
}
