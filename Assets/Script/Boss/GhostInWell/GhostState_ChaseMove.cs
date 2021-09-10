using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_ChaseMove : GhostStateBase
{
    public float maxDistance = 4f;
    public float attackDistance = 8f;
    public float attackAngle = 30f;

    public override string stateIdentifier => "ChaseMove";

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        if(target.CheckTargetInArea(attackAngle,attackDistance))
        {
            StateChange("MeleeAttack");
            return;
        }

        var targetDist = target.target.transform.position.magnitude;

        var position = MathEx.DeleteYPos((target.target.transform.position).normalized) * 
                                        (targetDist > maxDistance ? maxDistance : targetDist);
        var dist = Vector3.Distance(MathEx.DeleteYPos(target.transform.position),position);
        var dir = (position - MathEx.DeleteYPos(target.transform.position)).normalized;

        if(target.CheckTargetInArea(attackAngle, 10000f) && target.distanceAccuracy < dist)
        {
            target.Move(dir,target.moveSpeed,target.rotationSpeed,deltaTime);
        }

        var targetDir = (MathEx.DeleteYPos((target.target.transform.position)) -
                        MathEx.DeleteYPos((target.transform.position))).normalized;

        target.Turn(targetDir,deltaTime);
    
    }

}
