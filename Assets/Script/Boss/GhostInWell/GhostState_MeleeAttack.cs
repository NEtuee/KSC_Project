using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_MeleeAttack : GhostStateBase
{
    public LayerMask groundLayer;
    public AnimationCurve attackCurve;

    public float attackSpeed = 1f;

    public override string stateIdentifier => "MeleeAttack";

    private Vector3 _armOrigin;
    private int _armTarget = 0;
    private bool _attack = false;


    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        // float near = 0f;
        // for(int i = 0; i < arms.Count; ++i)
        // {
        //     var dist = Vector3.Distance(target.transform.position,arms[i].ik.position);
        //     if(i == 0 || dist <= near)
        //     {
        //         _armTarget = i;
        //         near = dist;
        //     }
        // }

        var dir = (target.target.transform.position - target.transform.position).normalized;
        var side = Vector3.Dot(target.transform.up,Vector3.Cross(target.transform.forward,dir));
        _armTarget = side > 0 ? 1 : 0;

        _armOrigin = target.arms[_armTarget].ik.position;
        target.arms[_armTarget].DisableMovement();

        _timeCounter.InitTimer("Attack");
        _timeCounter.InitTimer("Wait");

        _attack = false;
    }

    public override void StateChanged(StateBase targetState)
    {
        base.StateChanged(targetState);
        target.arms[_armTarget].enabled = true;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        var time = _timeCounter.IncreaseTimerSelf("Attack",out var limit,attackSpeed * deltaTime);

        if(limit)
        {
            if(!_attack)
            {
                _attack = true;
                var dir = MathEx.DeleteYPos(target.target.transform.position - target.transform.position).normalized;
                target.target.Ragdoll.ExplosionRagdoll(200f,dir);
            }

            _timeCounter.IncreaseTimerSelf("Wait",out limit,deltaTime);
            if(limit)
            {
                StateChange("ChaseMove");
            }
        }
        else
        {
            target.arms[_armTarget].ik.transform.position = Vector3.Lerp(_armOrigin,target.target.transform.position,time) + 
                                                new Vector3(0f,attackCurve.Evaluate(time),0f);
        }
    }
}
