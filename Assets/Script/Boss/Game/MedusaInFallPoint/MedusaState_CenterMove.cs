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
    private bool _lock = false;

    public override void Assign()
    {
        base.Assign();

        _timeCounter.CreateSequencer("ready");
        _timeCounter.AddSequence("ready",waitTime - 1f,null,(x)=>{
            target.AnimationChange(2);

            MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
            data.key = "MedusaEyeLight";
            data.parent = target.eyeLightPosition;
            data.position = target.eyeLightPosition.position;
            data.rotation = Quaternion.LookRotation(target.transform.forward, Vector3.up);

            target.SendMessageEx(MessageTitles.effectmanager_activeeffectsetparent,
                        UniqueNumberBase.GetSavedNumberStatic("EffectManager"), data);
        });
        _timeCounter.AddSequence("ready",1f,null,(x)=>{StateChange("RushToTarget");});
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("WaitTime",0f,waitTime);

        _velocity = Vector3.zero;
        _direction = MathEx.DeleteYPos(target.target.position - target.transform.position).normalized;

        // if(Vector3.Distance(centerPosition.position,target.target.position) < centerDist)
        // {
        //     _back = true;
        // }

        target.AnimationChange(4);
        _timeCounter.InitSequencer("ready");

        _lock = false;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        var look = MathEx.DeleteYPos(target.target.position - target.transform.position).normalized;

        if(target.Turn(look,deltaTime))
        {
            if(!_lock)
            {
                

                _lock = true;
            }
            
        }

        if(_lock)
        {
            _timeCounter.ProcessSequencer("ready",deltaTime);
        }

        

        // if(_back)
        // {
        //     _direction = MathEx.DeleteYPos(target.transform.position - target.target.position).normalized;
        // }
        // else
        // {
        //     _direction = MathEx.DeleteYPos(centerPosition.position - target.transform.position).normalized;
        // }
        
        // _velocity += _direction * accelSpeed;
        // if(MathEx.abs(_velocity.magnitude) >= maxSpeed)
        // {
        //     _velocity = _velocity.normalized * maxSpeed;
        // }
        
        // if(!Physics.SphereCast(rayPoint.position,3f,_direction,out var hit,rayDist,wallLayer))
        //     target.Move(_velocity,1f,deltaTime);
        // target.Turn(look,deltaTime);
    }
}
