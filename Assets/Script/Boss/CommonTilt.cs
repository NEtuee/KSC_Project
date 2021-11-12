using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonTilt : MonoBehaviour
{
    public Transform rotateTarget;
    public float tiltFactor;
    public float lerpFactor;

    public bool inverseX;
    public bool inverseZ;

    

    private Vector3 _prevPosition;
    private Vector3 _localPosition;

    private Vector3 _eulerAngles;
    private Vector3 _currentEuler;
    private float _sin;
    public void Start()
    {
        _prevPosition = transform.position;
        _localPosition = rotateTarget.localPosition;
        _eulerAngles = rotateTarget.localEulerAngles;
        _currentEuler = Vector3.zero;
    }

    public void FixedUpdate()
    {
        var factor = _prevPosition - transform.position;
        MathEx.Swap<float>(ref factor.x,ref factor.z);

        factor = transform.rotation * factor;

        factor.x *= inverseX ? -1f : 1f;
        factor.z *= inverseZ ? -1f : 1f;

        _currentEuler = Vector3.Lerp(_currentEuler, factor, lerpFactor * Time.fixedDeltaTime);

        rotateTarget.localEulerAngles = _eulerAngles + _currentEuler * tiltFactor * 1000f;
        _prevPosition = transform.position;
    }
}
