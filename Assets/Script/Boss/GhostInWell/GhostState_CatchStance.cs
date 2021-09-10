using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_CatchStance : GhostStateBase
{
    public override string stateIdentifier => "CatchStance";
    public Transform head;
    public float catchDist;

    public float passageTime = 20f;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        _timeCounter.InitTimer("PassageStand",0f,passageTime);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("PassageStand",out var timeout, deltaTime);
        if(timeout)
        {
            StateChange("HeadOut");
            return;
        }

        var dist = Vector3.Distance(head.position,target.target.transform.position);
        if(dist <= catchDist)
        {
            StateChange("Throw");
        }
    }
}
