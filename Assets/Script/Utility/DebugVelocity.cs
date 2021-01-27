using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVelocity : MonoBehaviour
{
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(this.gameObject.name+" : "+rigidbody.velocity);
        Debug.Log(rigidbody.centerOfMass);
    }
}
