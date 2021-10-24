using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_SpiderState_AvoidTurn : B1_SpiderStateBase
{
    public override string stateIdentifier => "AvoidTurn";

    private RayEx _forwardRay;

    private bool _left = false;

    public override void Assign()
    {
        base.Assign();

        _forwardRay = new RayEx(new Ray(Vector3.zero,Vector3.zero),5f,target.wallLayer);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        var dir = (target.target.position - target.transform.position).normalized;
        _left = Vector3.Dot(target.transform.right,dir) > 0;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        target.Turn(_left,target.transform,target.rotationSpeed,deltaTime);

        _forwardRay.SetDirection(target.transform.forward);
        if(!_forwardRay.Cast(target.transform.position,out var wallHit))
        {
            StateChange("MoveForward");
        }
    }
}
