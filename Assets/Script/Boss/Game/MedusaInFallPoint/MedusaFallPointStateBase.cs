using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaFallPointStateBase : StateBase
{
    protected MedusaInFallPoint_AI target;

    public override void Assign()
    {
        base.Assign();
        target = (MedusaInFallPoint_AI)targetObject;
    }
}
