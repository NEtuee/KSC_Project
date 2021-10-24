using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MedusaState_MarchForward : B1_MedusaStateBase
{
    public override string stateIdentifier => "March";

    public Transform forwardRayPoint;
    public Transform backRayPoint;

    public Transform leftRayPosition;
    public Transform rightRayPosition;

    private Vector3 _moveLine;
    private Vector3 _perpendicularPoint;
    private Vector3 _searchDirection;
    private Vector3 _scannedPosition;

    private Quaternion _headRotation;

    private float _pointDistance;
    private float _direction;//1 = right, -1 = left
    private bool _targetIsBack = false;

    private RayEx _ray;


    public override void Assign()
    {
        base.Assign();

        _ray = new RayEx(new Ray(Vector3.zero,Vector3.down),10f,target.groundLayer);
        
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        UpdateMoveLine();

        _targetIsBack = false;

        _timeCounter.InitTimer("check");
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("check",out var limit, deltaTime);
        if(!limit)
            return;

        _targetIsBack = Vector3.Dot(target.transform.forward,(target.target.position - target.transform.position).normalized) < 0;

        UpdatePerpendicularPoint();
        UpdateDirection();

        if(_targetIsBack)
        {
            if(!_ray.Cast(backRayPoint.position,out var hit))
            {
                //StateChange("Idle");
            }
            else
            {
                target.Move(-target.transform.forward,target.moveSpeed * 3f,deltaTime);
                LineMove(deltaTime);
            }
        }
        else
        {
            if(!_ray.Cast(forwardRayPoint.position,out var hit))
            {
                //StateChange("Idle");
            }
            else
            {
                target.Move(target.transform.forward,target.moveSpeed,deltaTime);
                LineMove(deltaTime);
            }
        }

        foreach(var leg in target.legMovements)
        {
            leg.legSpeed = Mathf.Clamp(4f * _pointDistance * 0.9f,4f,8f);
        }
    }

    public void LineMove(float deltaTime)
    {
        if(_pointDistance >= 1f)
        {
            if(_direction < 0f)
            {
                if(!_ray.Cast(rightRayPosition.position,out var hit))
                    return;
            }
            else
            {
                if(!_ray.Cast(leftRayPosition.position,out var hit))
                    return;
            }

            var dist = Mathf.Clamp(target.GetTargetDistance() * .25f,1f,6f);

            target.transform.position += (_moveLine * (_pointDistance * 4f) * _direction * deltaTime) / dist;

            //target.Move(_moveLine, (_pointDistance * 4f) * _direction, deltaTime);
        }
    }

    public void UpdatePerpendicularPoint()
    {
        _perpendicularPoint = MathEx.PerpendicularPoint(target.transform.position + _moveLine * -100f,target.transform.position + _moveLine * 100f, target.target.position);
        _pointDistance = Vector3.Distance(MathEx.DeleteYPos(target.transform.position),MathEx.DeleteYPos(_perpendicularPoint));

        var point = _perpendicularPoint;
        point.y = target.target.position.y;
    }

    public void UpdateDirection()
    {
        var cross = Vector3.Cross(target.transform.forward,target.target.position - transform.position);
        _direction = Vector3.Dot(Vector3.up,cross) < 0 ? 1f : -1f;
    }

    public void UpdateMoveLine()
    {
        // var direction = (target.target.position - target.transform.position);
        // direction = Vector3.ProjectOnPlane(direction,Vector3.up).normalized;

        _moveLine = -target.transform.right;
    }
}
