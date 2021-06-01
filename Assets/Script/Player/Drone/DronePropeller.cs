using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePropeller : MonoBehaviour
{
    private Transform transform;
    [SerializeField] private Transform propellerSphereJoint;
    [SerializeField] private float speed;

    private void Start()
    {
        transform = GetComponent<Transform>();
    }

    public void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, speed * Time.fixedDeltaTime);
        if(propellerSphereJoint != null)
            propellerSphereJoint.rotation = Quaternion.Lerp(propellerSphereJoint.rotation,Quaternion.LookRotation(Vector3.up),10.0f*Time.fixedDeltaTime );
    }
}
