using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_FlySpiderState_RushReadyQuick : B1_FlySpiderStateBase
{
    public override string stateIdentifier => "RushQuick";

    public LineRenderer lineRenderer;
    public LayerMask hitLayer;

    public float aimTime = 3f;
    public float waitLockTime = 1f;
    public float aimSpeed = 10f;

    private Vector3 _targetPosition;

    private float _deltaTime;
    private float _lineEnableTime;

    public override void Assign()
    {
        base.Assign();

        _timeCounter.CreateSequencer("main");
        _timeCounter.AddSequence("main", waitLockTime, null, null);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _targetPosition = target.transform.position;
        lineRenderer.enabled = true;

        _timeCounter.InitSequencer("main");

        if (Physics.Raycast(transform.position, target.direction, out var hit, 10000f, hitLayer))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + target.direction * 1000f);
        }
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);
        _deltaTime = deltaTime;

        if (_timeCounter.ProcessSequencer("main", deltaTime))
        {
            lineRenderer.enabled = false;
            StateChange("Rush");
        }

        lineRenderer.SetPosition(0, target.transform.position);

        
    }
}
