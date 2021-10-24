using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_SpiderState_Turn : B1_SpiderStateBase
{
    public override string stateIdentifier => "Turn";

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        var dir = (target.target.position - target.transform.position).normalized;
        dir = Vector3.ProjectOnPlane(dir,Vector3.up).normalized;
        if(target.Turn(dir,deltaTime))
        {
            StateChange("MoveForward");
        }
    }
}
