using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_FlySpiderState_RushReady : B1_FlySpiderStateBase
{
    public override string stateIdentifier => "RushReady";

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
        _timeCounter.AddSequence("main",aimTime,Aimming,(x)=>{
            lineRenderer.enabled = true;
        });
        _timeCounter.AddSequence("main",waitLockTime,null,null);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _targetPosition = target.transform.position;
        lineRenderer.enabled = true;

        _lineEnableTime = 0f;

        _timeCounter.InitSequencer("main");

        MD.SoundPlayData soundData = MessageDataPooling.GetMessageData<MD.SoundPlayData>();
        soundData.id = 1529;
        if (Physics.Raycast(transform.position, target.direction, out var hit, 10000f, hitLayer))
            soundData.position = hit.point;
        else
            soundData.position = transform.position;
        soundData.returnValue = false;
        soundData.dontStop = false;
        target.SendMessageEx(MessageTitles.fmod_play, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);
        _deltaTime = deltaTime;

        if(_timeCounter.ProcessSequencer("main",deltaTime))
        {
            lineRenderer.enabled = false;
            StateChange("Rush");
        }

        lineRenderer.SetPosition(0,target.transform.position);

        if(Physics.Raycast(transform.position,target.direction,out var hit,10000f,hitLayer))
        {
            lineRenderer.SetPosition(1,hit.point);
        }
    }

    public void Aimming(float f)
    {
        _targetPosition = Vector3.Lerp(_targetPosition,target.target.position,_deltaTime * aimSpeed);
        target.direction = (_targetPosition - target.transform.position).normalized;

        float factor = f / aimTime;

        _lineEnableTime += (factor * 70f * _deltaTime);
        var enable = Mathf.Sin(_lineEnableTime) > 0;

        lineRenderer.enabled = enable;
    }
}
