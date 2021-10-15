using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MedusaState_Dead : B1_MedusaStateBase
{
    public override string stateIdentifier => "Dead";

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        target.SetIK(false);
        target.ChangeMainAnimation(2);
        target.animator.SetLayerWeight(2,0f);
        target.enabled = false;
    }
}
