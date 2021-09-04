using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_CatchStance : GhostStateBase
{
    public override string stateIdentifier => "CatchStance";
    public Transform head;
    public float catchDist;

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        var dist = Vector3.Distance(head.position,target.target.position);
        if(dist <= catchDist)
        {
            StateChange("Throw");
        }
    }
}
