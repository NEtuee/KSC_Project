using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_SpiderStateBase : StateBase
{
    protected B1_Spider target;

    public override void Assign()
    {
        base.Assign();
        target = (B1_Spider)targetObject;
    }
}
