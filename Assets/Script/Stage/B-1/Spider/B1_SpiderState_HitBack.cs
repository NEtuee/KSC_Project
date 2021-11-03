using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_SpiderState_HitBack : B1_SpiderStateBase
{
    public override string stateIdentifier => "HitBack";

    public float backSpeed = 1f;
    public float backTime = 1f;
    public Transform rayPosition;

    private SphereRayEx _downRay;
    private Vector3 _direction;
    private float _speed;
    private float _up;

    public override void Assign()
    {
        base.Assign();

        _downRay = new SphereRayEx(new Ray(Vector3.zero,Vector3.down),5f,1f,target.groundLayer);

        _up = rayPosition.position.y;
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("timer",0f,backTime);
        _direction = target.backDirection;//Vector3.ProjectOnPlane((target.transform.position - target.target.position),Vector3.up).normalized;

        var pos = target.transform.position + _direction * 5f;
        pos.y = _up;
        rayPosition.position = pos;

        _speed = backSpeed;

        target.ChangeAnimation("Hit");

        if(target.shell != null)
        {
            target.shell.isKinematic = false;
            target.shell.AddForce(_direction * 10f,ForceMode.Impulse);
            target.shell.transform.SetParent(null);
            target.shellCollider.enabled = true;

            target.shell = null;
        }
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        float time = _timeCounter.IncreaseTimerSelf("timer",out var limit,deltaTime);
        if(limit)
        {
            StateChange("Idle");
        }
        else
        {
            target.Move(_direction,_speed,deltaTime);
            target.Turn(_direction,deltaTime);
            _speed = backSpeed * (backTime - (time / backTime));

            if(!_downRay.Cast(rayPosition.position,out var hit))
            {
                StateChange("Idle");
            }
        }
    }
}
