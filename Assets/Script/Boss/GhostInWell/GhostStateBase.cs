using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostStateBase : StateBase
{
    protected GhostInWell_AI target;

    public override void Assign()
    {
        base.Assign();
        target = (GhostInWell_AI)targetObject;
    }
}
