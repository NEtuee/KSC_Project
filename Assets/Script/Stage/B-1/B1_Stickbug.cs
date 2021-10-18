using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_Stickbug : PathfollowObjectBase
{
    public Core core;
    public EMPShield shield;
    public SinglePathObject pathObject;

    public override void Assign()
    {
        base.Assign();

        InitPath();
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        FollowPath(deltaTime);
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
