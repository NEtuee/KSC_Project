using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickObstacle : ObjectBase
{
    public UnityEngine.Events.UnityEvent whenKicked = new UnityEngine.Events.UnityEvent();

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.object_kick, (msg) =>
        {
            whenKicked.Invoke();
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
    }
}
