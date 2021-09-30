using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieStateBase : StateBase
{
    protected Genie_Phase_AI target;

    public override void Assign()
    {
        base.Assign();
        target = (Genie_Phase_AI)targetObject;
    }

    public void HeadLookTarget(Transform rotateTarget,Vector3 position)
    {
        var dir = (position - rotateTarget.position).normalized;
        rotateTarget.rotation = Quaternion.LookRotation(dir,Vector3.up);

    }

    public void LookTarget(Transform rotateTarget,Vector3 position, float deltaTime)
    {
        var dir = position - rotateTarget.position;
        dir = Vector3.ProjectOnPlane(dir,Vector3.up).normalized;

        var speed = deltaTime * target.bodyRotateSpeed;
        var rot = Quaternion.LookRotation(dir, Vector3.up);
        rotateTarget.rotation = Quaternion.Lerp(rotateTarget.rotation, rot, speed);
    }
}
