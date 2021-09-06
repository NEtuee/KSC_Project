using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WankoState_HitMove : WankoStateBase
{
    public override string stateIdentifier => "HitMove";
    public LayerMask groundLayer;
    public float moveTime = 1f;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("Move",0f,moveTime);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("Move",out var limit, deltaTime);
        if(!limit)
        {
            if(Physics.Raycast(target.transform.position - target.transform.forward * 2f,-target.transform.up,10f,groundLayer))
            {
                target.Move(-transform.forward,target.moveSpeed,target.rotationSpeed,deltaTime);
            }
        }
        else
        {
            StateChange(target.pluged ? "PlugedChase" : "Chase");
        }
    }

}
