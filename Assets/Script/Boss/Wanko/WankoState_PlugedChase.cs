using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WankoState_PlugedChase : WankoStateBase
{
    public override string stateIdentifier => "PlugedChase";

    public WankoCable cable;

    public float assultDist = 3f;

    [Header("Gague")]
    //public float firstGague = 5f;
    public float secondGague = 5f;
    public float thirdGague = 5f;

    [Header("Distance")]

    public float firstDist = 3f;
    public float secondDist = 6f;
    public float thirdDist = 8f;

    private float _addDist = 0f;
    private float _dist = 0f;
    private bool _edgeCut = false;
    private bool _stop = false;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        UpdateDistance();

        _timeCounter.InitTimer("Wait");

        _edgeCut = false;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        if(_stop)
        {
            _timeCounter.IncreaseTimerSelf("Wait",out var limit, deltaTime);
            if(limit)
                _stop = false;
            else
                return;
        }

        UpdateDistance();

        var targetPosition = MathEx.DeleteYPos(target.target.transform.position).normalized * (_dist + _addDist);
        var targetDist = Vector3.Distance(targetPosition,MathEx.DeleteYPos(target.transform.position));
        var targetDir = MathEx.DeleteYPos(targetPosition - target.transform.position).normalized;

        if(targetDist > target.distanceAccuracy)
        {
            target.Move(targetDir,target.moveSpeed * (_addDist == 0f ? 1f : 1.5f),target.rotationSpeed,deltaTime);
        }
        else if(_dist == thirdDist && !_edgeCut)
        {
            target.graphAnimator.Play("EdgeCut",target.model);
            cable.DeleteFlag();
            _edgeCut = true;
            _stop = true;

            _timeCounter.InitTimer("Wait");
        }

        var turnDir = MathEx.DeleteYPos(target.target.transform.position - target.transform.position).normalized;
        target.Turn(turnDir,deltaTime);



        var playerDist = Vector3.Distance(target.transform.position,target.target.transform.position);
        if(playerDist <= assultDist && _dist != thirdDist && _addDist == 0f)
        {
            _addDist = assultDist;

            _timeCounter.InitTimer("Assult",0f,3f);
        }

        if(_addDist != 0f)
        {
            _timeCounter.IncreaseTimerSelf("Assult",out var limit, deltaTime);
            if(limit)
            {
                _addDist = 0f;
            }
        }
        
    }

    public void UpdateDistance()
    {
        var gague = target.angryGague;
        var prev = _dist;
        _dist = gague < secondGague ? firstDist : (gague < thirdDist ? secondDist : thirdDist);

        if(prev != _dist)
        {
            _edgeCut = false;
            _stop = false;
        }
    }
}
