using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_FlySpiderStateBase : StateBase
{
    protected B1_FlySpider target;

    public override void Assign()
    {
        base.Assign();
        target = (B1_FlySpider)targetObject;
    }
}
