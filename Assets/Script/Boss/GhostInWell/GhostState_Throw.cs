using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_Throw : GhostStateBase
{
    public override string stateIdentifier => "Throw";

    public float throwPower = 300f;

    public Transform playerPosition;

    private bool _throw = false;
    private Transform _hipParent;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        //fix ragdoll
        target.target.Ragdoll.ExplosionRagdoll(100f,target.transform.forward);
        target.target.Ragdoll.SetPlayerShock(7f);

        _hipParent = target.target.Ragdoll.GetHipTransform().parent;
        target.target.Ragdoll.GetHipTransform().GetComponent<Rigidbody>().isKinematic = true;
        target.target.Ragdoll.GetHipTransform().SetParent(playerPosition);
        target.target.Ragdoll.GetHipTransform().localPosition = Vector3.zero;

        target.AnimationChange(4);

        _timeCounter.InitTimer("Wait",0f,8f);
        _timeCounter.InitTimer("Throw",0f,5.216f);
        _throw = false;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        if(!_throw)
        {
            _timeCounter.IncreaseTimerSelf("Throw",out var th,deltaTime);
            if(th)
            {
                _throw = true;
                ThrowRagdoll();
            }
        }
        

        _timeCounter.IncreaseTimerSelf("Wait",out var limit,deltaTime);
        if(limit)
        {
            target.EnableMovement();
            StateChange("RandomMove");
        }
    }

    public void ThrowRagdoll()
    {
        target.target.Ragdoll.GetHipTransform().SetParent(_hipParent);
        target.target.Ragdoll.GetHipTransform().GetComponent<Rigidbody>().isKinematic = false;
        target.target.Ragdoll.GetHipTransform().GetComponent<Rigidbody>().AddForce(-target.transform.forward * throwPower);
        Debug.Log("Throw");
    }
}
