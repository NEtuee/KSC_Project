using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGravity : MonoBehaviour
{
    public Vector3 gravityFactor;

    private Rigidbody rig;

    public void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        rig.AddForce(gravityFactor);
    }
}
