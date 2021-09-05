using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WankoStateBase : StateBase
{
    protected Wanko_AI target;

    public override void Assign()
    {
        base.Assign();
        target = (Wanko_AI)targetObject;
    }
}
