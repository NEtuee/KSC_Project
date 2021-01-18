using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereForceExplosion : MonoBehaviour
{
    [SerializeField]private float maxPower = 1000f;
    [SerializeField]private float minPower = 500f;
    [SerializeField]private float distance = 100f;
    [SerializeField]private float gravityFactor = 2f;

    [SerializeField]private Vector3 torqueMin;
    [SerializeField]private Vector3 torqueMax;

    [SerializeField]private List<Rigidbody> targetList = new List<Rigidbody>();

    public void Start()
    {
        Launch();
    }

    public void Launch()
    {
        foreach(var target in targetList)
        {
            var direction = (target.transform.position - transform.position).normalized;
            var dist = Vector3.Distance(target.transform.position, transform.position);
            var power = dist <= distance ? minPower : Mathf.Lerp(minPower,maxPower,((distance - dist) / distance));
            target.AddForce(direction * power);
            target.AddTorque(MathEx.RandomVector3(torqueMin,torqueMax),ForceMode.Acceleration);
        }
    }

    public void FixedUpdate()
    {
        foreach(var target in targetList)
        {
            if(!target.IsSleeping())
                target.AddForce(Vector3.down * gravityFactor);
        }
    }
}
