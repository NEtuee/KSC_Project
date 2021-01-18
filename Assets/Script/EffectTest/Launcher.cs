using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField]private Vector3 startForce;
    [SerializeField]private Vector3 startToque;
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        AddForce(startForce);
        AddTorque(startToque);
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(transform.up.normalized * force.y);
        rb.AddForce(transform.right.normalized * force.x);
    }

    public void AddTorque(Vector3 torque)
    {
        rb.AddTorque(torque);
    }
}
