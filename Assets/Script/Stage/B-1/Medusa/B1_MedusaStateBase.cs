using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MedusaStateBase : StateBase
{
    protected B1_Medusa target;

    public override void Assign()
    {
        base.Assign();
        target = (B1_Medusa)targetObject;
    }
}
