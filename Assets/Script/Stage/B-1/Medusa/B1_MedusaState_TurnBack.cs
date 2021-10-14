using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MedusaState_TurnBack : B1_MedusaStateBase
{
    public override string stateIdentifier => "Turn";

    private float _dot;
    private Vector3 _forward;
    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        var dir = Vector3.ProjectOnPlane(target.target.position - target.transform.position,Vector3.up).normalized;
        _dot = Vector3.Dot(target.transform.forward, dir);
        _forward = target.transform.forward;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);
        if(_dot < 0)
        {
            if(target.Turn(-_forward,deltaTime))
            {
                StateChange("March");
            }
        }
        else
        {
            StateChange("March");
        }
    }
}
