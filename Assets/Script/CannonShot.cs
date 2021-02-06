using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShot : MonoBehaviour
{
    public float shotAngle = 180f;
    public float currentAngle = 0f;
    public Transform target;
    public GameObject cannonBall;

    public bool _canShot = false;

    public void CanShot(Transform t)
    {
        target = t;

        var one = (target.position - transform.position).normalized;
        var two = transform.forward;

        one.y = 0f;
        two.y = 0f;

        currentAngle = Vector3.Angle(one,two);
        _canShot = currentAngle <= shotAngle;
    }

    public void Shot()
    {
        if(!_canShot)
            return;

        var obj = Instantiate(cannonBall,transform.position,Quaternion.identity).GetComponent<CannonBall>();
        obj.Shot(transform.position,target.position);
    }
}
