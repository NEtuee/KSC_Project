using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WankoState_Chase : WankoStateBase
{
    public override string stateIdentifier => "Chase";

    public LayerMask groundLayer;


    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        if(Physics.Raycast(target.transform.position + target.transform.forward * 2f,-target.transform.up,10f,groundLayer))
        {
            target.Move(transform.forward,target.moveSpeed * 1.8f,target.rotationSpeed * 1.5f,deltaTime);
        }

        var turnDir = MathEx.DeleteYPos(target.target.transform.position - target.transform.position).normalized;
        target.Turn(turnDir,deltaTime);
        
    }
}
