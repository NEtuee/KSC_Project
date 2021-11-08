using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_DroneTilt : MonoBehaviour
{
    public float tiltDirection = 1f;
    public float floatingSpeed = 5f;
    public float floatingFactor = 0.5f;

    public Transform target;
    private Vector3 _prevPosition;
    private Vector3 _localPosition;

    private Vector3 _eulerAngles;
    private float _sin;
    public void Start()
    {
        _prevPosition = transform.position;
        _localPosition = target.localPosition;
        _eulerAngles = transform.localEulerAngles;
    }

    public void FixedUpdate()
    {
        var factor = _prevPosition - transform.position;
        transform.localEulerAngles = _eulerAngles + Vector3.right * factor.magnitude * 1000f;

        if(target.parent != null)
        {
            _sin += Time.fixedDeltaTime * floatingSpeed;
            target.localPosition = _localPosition + Vector3.forward * Mathf.Sin(_sin) * floatingFactor;
        }
        

        _prevPosition = transform.position;
    }
}
