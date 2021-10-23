using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_Stickbug : PathfollowObjectBase
{
    public Core core;
    public EMPShield shield;
    public SinglePathObject pathObject;
    public GraphAnimator animator;
    public Transform body;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    public override void Assign()
    {
        base.Assign();

        InitPath();
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        _timeCounter.InitTimer("dance",0f,1f);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        _timeCounter.IncreaseTimerSelf("dance",out var limit, deltaTime);
        if(limit)
        {
            animator.Stop();
            FollowPath(deltaTime);    
        }
        else
        {
            animator.Progress(deltaTime);
        }
        
    }

    public void Dance()
    {
        animator.Play("Dance",body);
        _timeCounter.InitTimer("dance",0f,1f);
    }

    public void InitPath()
    {
        currentPath = pathObject.path;
        pathLoop = true;
        SetPathTargetNear(false);
    }

    public void Respawn()
    {
        core.Reactive();
        shield.Reactive();

        gameObject.SetActive(true);
    }
}
