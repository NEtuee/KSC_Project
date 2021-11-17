using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaState_RushTarget : MedusaFallPointStateBase
{
    public override string stateIdentifier => "RushToTarget";
    public float accelSpeed = 10f;
    public float maxSpeed = 5f;
    public float hitForce = 350f;

    public Transform centerPosition;
    public Transform rayPoint;
    public LayerMask wallLayer;
    public float rayDist = 4f;

    public MedusaState_WallHit wallHit;
    

    private Vector3 _velocity;
    private Vector3 _direction;



    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _velocity = Vector3.zero;
        _timeCounter.InitTimer("UpdateDirection",0f,1f);

        _direction = MathEx.DeleteYPos(target.target.position - target.transform.position).normalized;

        //target.AnimationChange(2);

        if(Physics.Raycast(transform.position + Vector3.up,target.transform.forward,5f,wallLayer))
        {
            var dir = (target.target.position - target.transform.position).normalized;
            var side = -MathEx.normalize(Vector3.Dot(target.transform.right,dir));

            _velocity = target.transform.right * side * maxSpeed;
        }

        // var dist = Vector3.Distance(target.target.transform.position,centerPosition.position);
        // if(dist >= 12f)
        // {
        //     _direction = MathEx.DeleteYPos(centerPosition.position - target.transform.position).normalized;
        // }
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        // var look = MathEx.DeleteYPos(target.target.position - target.transform.position).normalized;

        // _timeCounter.IncreaseTimerSelf("UpdateDirection",out var limit, deltaTime);
        // if(limit)
        // {
        //     _direction = look;
        //     _timeCounter.InitTimer("UpdateDirection",0f,1f);
        // }
        
        _velocity += _direction * accelSpeed;
        if(MathEx.abs(_velocity.magnitude) >= maxSpeed)
        {
            _velocity = _velocity.normalized * maxSpeed;
        }
        

        target.Move(_velocity,1f,deltaTime);
        //target.Turn(look,deltaTime);

        RayCheck();
    }

    public void HitTarget()
    {
        if(target.stateProcessor.currentState == stateIdentifier)
        {
            target.player.Ragdoll.ExplosionRagdoll(hitForce, _direction);
            target.player.TakeDamage(target.damage);
            wallHit.moveDirection = -_direction;
            StateChange("WallHit");

            HitEffect();
        }
    }

    public void RayCheck()
    {
        for(float i = 1; i <= 18f; ++i)
        {
            var rad = (20f * i) * Mathf.Deg2Rad;
            var dir = new Vector3(Mathf.Cos(rad),0f,Mathf.Sin(rad));

            if(Physics.Raycast(rayPoint.position,dir,out var hit,rayDist,wallLayer))
            {
                wallHit.moveDirection = hit.normal;
                StateChange("WallHit");

                HitEffect();
            }
        }
    }

    public void HitEffect()
    {
        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();

        data.key = "MedusaHit";
        //data.parent = target.hitPosition;
        data.position = target.hitPosition.position;
        data.rotation = Quaternion.LookRotation(-target.transform.up, Vector3.up);

        target.SendMessageEx(MessageTitles.effectmanager_activeeffectwithrotation,
                    UniqueNumberBase.GetSavedNumberStatic("EffectManager"), data);
    }
}
