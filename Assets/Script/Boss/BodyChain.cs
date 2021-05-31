using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyChain : MonoBehaviour
{
    public Transform lookTarget;
    public float distance;

    // public void Update()
    // {
    //     if (GameManager.Instance.GAMEUPDATE == GameManager.GameUpdate.Fixed)
    //         return;

    //     Progress(Time.deltaTime);
    // }

    public void FixedUpdate()
    {
        // if (GameManager.Instance.GAMEUPDATE == GameManager.GameUpdate.Update)
        //     return;

        Progress(Time.fixedDeltaTime);
    }

    public void Progress(float deltaTime)
    {
        var dist = Vector3.Distance(lookTarget.position,transform.position);
        var dir = (lookTarget.position - transform.position).normalized;
        if(dist > distance)
        {
            transform.position = lookTarget.position - dir * distance;
        }

        var look = Quaternion.LookRotation(dir).eulerAngles;
        var angle = transform.eulerAngles;
        float currVelocity = 0f;
        angle.x = look.x;
        angle.y = look.y;
        angle.z = Mathf.SmoothDampAngle(angle.z,lookTarget.eulerAngles.z,ref currVelocity,.05f);
        transform.eulerAngles = angle;
    }

}
