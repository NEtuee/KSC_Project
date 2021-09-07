using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageStateBase : StateBase
{
    protected PassageState target;

    public override void Assign()
    {
        base.Assign();
        target = (PassageState)targetObject;
    }
}
