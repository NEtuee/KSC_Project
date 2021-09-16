using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaState_Dead : MedusaFallPointStateBase
{
    public override string stateIdentifier => "Dead";


    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        //target.SetIKMovement(false);
        target.AnimationChange(6);
        target.enabled = false;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        
    }
}
