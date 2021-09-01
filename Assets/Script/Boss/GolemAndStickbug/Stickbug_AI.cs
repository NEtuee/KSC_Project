using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stickbug_AI : PathfollowObjectBase
{
    public string headPositionPath;
    public string mainPath;
    public Transform mainBody;
    public GraphAnimator graphAnimator;
    public LevelEdit_ExplosionPhysics explosionPhysics;

    private bool _dance;
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        graphAnimator.Play("Idle",mainBody,true);
        //SetPath(headPositionPath,false);
        Launch();
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);
        graphAnimator.Progress(deltaTime);

        if(!_dance)
            FollowPath(deltaTime);
        else
        {
            _timeCounter.IncreaseTimerSelf("HitTimer",out var limit,deltaTime);
            
            if(limit)
            {
                _dance = false;
                graphAnimator.Stop();
                graphAnimator.Play("Idle",mainBody,true);
            }
        }
    }

    public void Launch()
    {
        SetPath(mainPath,true);
        _timeCounter.InitTimer("HitTimer",0f);

        _dance = false;
    }

    public void WhenHit()
    {
        _dance = true;
        _timeCounter.InitTimer("HitTimer",0f);
        graphAnimator.Stop();
        graphAnimator.Play("Dance",mainBody,true);
    }

    public void WhenDestroy()
    {
        this.enabled = false;
        explosionPhysics.Launch();
    }
}
