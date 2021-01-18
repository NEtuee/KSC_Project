using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_LookPlayer : MonoBehaviour
{
    public Transform forward;
    public Transform targetTransform;

    private bool trigger = false;

    private Vector3 startEuler;

    public void Start()
    {
    }

    public void LateUpdate()
    {
        float y = Quaternion.LookRotation(targetTransform.position - transform.position).eulerAngles.y;
        Vector3 angleVector = transform.eulerAngles;

        var dist = Vector3.Distance(transform.position,targetTransform.position);
        var angle = Vector2.Angle(MathEx.Vector3ToVector2(forward.forward), 
                    MathEx.Vector3ToVector2((targetTransform.position - transform.position).normalized));

        if(dist >= 20f && angle <= 50)
        {
            if(!trigger)
            {
                trigger = true;

                startEuler = transform.eulerAngles;
            }
            
            angleVector.y = (y - 90f);
            var euler = transform.eulerAngles;
        }
        else
        {
            trigger = false;
        }

        startEuler = MathEx.LerpAngle(startEuler,angleVector,2f * Time.deltaTime);
        transform.eulerAngles = startEuler;
        
    }

}
