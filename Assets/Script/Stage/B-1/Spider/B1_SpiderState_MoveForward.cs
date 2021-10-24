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
    private RayEx _forwardRay;

    public override void Assign()
    {
        base.Assign();

        _downRay = new SphereRayEx(new Ray(Vector3.zero,Vector3.down),5f,1f,target.groundLayer);
        _forwardRay = new RayEx(new Ray(Vector3.zero,Vector3.zero),5f,target.wallLayer);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _maxDistance = Random.Range(randomMaxDistance.x,randomMaxDistance.y);

        if(target.GetTargetDistance() - target.explosionCheckRadius < _maxDistance)
        {
            _maxDistance = target.GetTargetDistance() - target.explosionCheckRadius + .5f;
        }
        _startPosition = transform.position;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _forwardRay.SetDirection(target.transform.forward);
        if(_forwardRay.Cast(target.transform.position,out var wallHit))
        {
            StateChange("AvoidTurn");
        }

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
