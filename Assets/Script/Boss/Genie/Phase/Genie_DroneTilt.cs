using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_DroneTilt : MonoBehaviour
{
    private Vector3 _prevPosition;

    private Vector3 _eulerAngles;
    public void Start()
    {
        _prevPosition = transform.position;
        _eulerAngles = transform.localEulerAngles;
    }

    public void FixedUpdate()
    {
        var factor = _prevPosition - transform.position;
        transform.localEulerAngles = _eulerAngles + Vector3.right * factor.magnitude * 0.2f;

        _prevPosition = transform.position;
    }
}
