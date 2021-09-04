using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_RandomMove : GhostStateBase
{
    public float maxDistance = 4f;
    public float recogDistance = 6f;
    public float recogAngle = 30f;

    public LayerMask obstacleLayer;

    public override string stateIdentifier => "RandomMove";

    private bool _translate = false;
    private bool _left = false;
    private bool _moving = false;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("Wait");
        _timeCounter.InitTimer("Move");

        _translate = false;
        _moving = false;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        if(target.CheckTargetInArea(recogAngle,recogDistance))
        {
            var dist = Vector3.Distance(MathEx.DeleteYPos(target.transform.position),
                                        MathEx.DeleteYPos(target.target.position));
            var dir = MathEx.DeleteYPos(target.target.position - target.recognizeStartPoint.position).normalized;
            if(!Physics.Raycast(target.recognizeStartPoint.position,dir,dist,obstacleLayer))
            {
                StateChange("ChaseMove");
            }
            
            return;
        }

        _timeCounter.IncreaseTimerSelf("Wait",out var wait,deltaTime);

        if(!wait)
            return;

        if(wait && !_moving)
        {
            _translate = Random.Range(0,2) == 0;
            _left = Random.Range(0,2) == 0;

            _moving = true;
        }




        _timeCounter.IncreaseTimerSelf("Move",out var move,deltaTime);

        if(move)
        {
            _timeCounter.InitTimer("Wait");
            _timeCounter.InitTimer("Move",0f,Random.Range(0.6f,1.2f));

            _translate = false;
            _moving = false;

            return;
        }

        if(_translate)
        {
            var targetDist = MathEx.DeleteYPos(target.transform.position + target.transform.forward).magnitude;

            if(targetDist <= maxDistance)
            {
                target.Move(target.transform.forward,target.moveSpeed,deltaTime);
            }
        }
        else
        {
            target.Turn(_left,target.transform,target.rotationSpeed,deltaTime);
        }

    
    }
}
