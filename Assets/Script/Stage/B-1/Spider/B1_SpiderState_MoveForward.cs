using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_SpiderState_MoveForward : B1_SpiderStateBase
{
    public override string stateIdentifier => "MoveForward";

    public Vector2 randomMaxDistance;
    public Transform rayPosition;

    private float _maxDistance;
    private Vector3 _startPosition;

    private SphereRayEx _downRay;

    public override void Assign()
    {
        base.Assign();

        _downRay = new SphereRayEx(new Ray(Vector3.zero,Vector3.down),5f,1f,target.groundLayer);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _maxDistance = Random.Range(randomMaxDistance.x,randomMaxDistance.y);
        _startPosition = transform.position;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        target.Move(target.transform.forward,target.moveSpeed, deltaTime);
        var distance = Vector3.Distance(transform.position, _startPosition);

        if(distance >= _maxDistance)
        {
            StateChange("Idle");
        }

        if(!_downRay.Cast(rayPosition.position,out var hit))
        {
            StateChange("Idle");
        }
    }

}
