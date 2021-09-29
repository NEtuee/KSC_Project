using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieState_Idle : GenieStateBase
{
    public override string stateIdentifier => "Idle";

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        LookTarget(target.body,target.targetTransform.position,deltaTime);
    }
}
