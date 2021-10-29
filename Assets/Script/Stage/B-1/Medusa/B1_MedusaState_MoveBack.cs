using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MedusaState_MoveBack : B1_MedusaStateBase
{
    public override string stateIdentifier => "MoveBack";
    public float backSpeed = 8f;
    public float moveTime = 3f;

    public Transform backRayPoint;

    private RayEx _ray;
    private string _prev;

    public override void Assign()
    {
        base.Assign();

        _ray = new RayEx(new Ray(Vector3.zero,Vector3.down),10f,target.groundLayer);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        _prev = prevState.stateIdentifier;

        _timeCounter.InitTimer("moveTime",0f,moveTime);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);
        target.Move(-target.transform.forward,backSpeed,deltaTime);

        _timeCounter.IncreaseTimerSelf("moveTime",out var limit,deltaTime);

        if(!_ray.Cast(backRayPoint.position,out var hit) || limit)
        {
            StateChange("March");
        }
    }
}
