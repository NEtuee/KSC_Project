using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaState_WallHit : MedusaFallPointStateBase
{
    public override string stateIdentifier => "WallHit";

    public Vector3 moveDirection;

    public float moveSpeed = 5f;
    public float moveTime = .5f;
    public float waitTime = 5f;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("WaitTime",0f,waitTime);
        _timeCounter.InitTimer("MoveTime",0f,moveTime);

        target.AnimationChange(3);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        var time = _timeCounter.IncreaseTimerSelf("MoveTime",out var limit, deltaTime);
        target.Move(moveDirection,Mathf.Lerp(moveSpeed,0f,time / moveTime),deltaTime);
        

        _timeCounter.IncreaseTimerSelf("WaitTime",out limit, deltaTime);
        if(limit)
        {
            var look = MathEx.DeleteYPos(target.target.position - target.transform.position).normalized;
            if(target.Turn(look,deltaTime))
            {
                StateChange("RushToTarget");
            }
        }
    }
}
