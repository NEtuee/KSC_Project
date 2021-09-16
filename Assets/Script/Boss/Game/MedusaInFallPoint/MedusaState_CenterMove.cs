using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaState_CenterMove : MedusaFallPointStateBase
{
    public override string stateIdentifier => "CenterMove";

    public Transform centerPosition;
    public Transform rayPoint;
    public float accelSpeed = 10f;
    public float maxSpeed = 5f;
    
    public LayerMask wallLayer;
    public float rayDist = 5f;
    public float centerDist = 6f;
    public float waitTime = 5f;

    private Vector3 _velocity;
    private Vector3 _direction;

    private bool _back = false;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("WaitTime",0f,waitTime);

        _velocity = Vector3.zero;
        _direction = MathEx.DeleteYPos(target.target.position - target.transform.position).normalized;

        if(Vector3.Distance(centerPosition.position,target.target.position) < centerDist)
        {
            _back = true;
        }

        target.AnimationChange(4);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("WaitTime",out var limit, deltaTime);
        if(limit)
        {
            StateChange("RushToTarget");
        }

        var look = MathEx.DeleteYPos(target.target.position - target.transform.position).normalized;

        if(_back)
        {
            _direction = MathEx.DeleteYPos(target.transform.position - target.target.position).normalized;
        }
        else
        {
            _direction = MathEx.DeleteYPos(centerPosition.position - target.transform.position).normalized;
        }
        
        _velocity += _direction * accelSpeed;
        if(MathEx.abs(_velocity.magnitude) >= maxSpeed)
        {
            _velocity = _velocity.normalized * maxSpeed;
        }
        
        if(!Physics.SphereCast(rayPoint.position,3f,_direction,out var hit,rayDist,wallLayer))
            target.Move(_velocity,1f,deltaTime);
        target.Turn(look,deltaTime);
    }
}
