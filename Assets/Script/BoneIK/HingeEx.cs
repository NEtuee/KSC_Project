using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HingeEx : MonoBehaviour
{
    public Vector3 hingeAngle;

    public bool RotateLock = false;

    private Transform _transform;
    private float _centerAngle = 0f;

    private Vector3 _forwardOrigin;
    private Vector3 _rightOrigin;
    private Vector3 _upOrigin;
    private Vector3 _eulerOrigin;

    public void Start()
    {
        _transform = GetComponent<Transform>();
        _forwardOrigin = _transform.forward;
        _rightOrigin = _transform.right;
        _upOrigin = _transform.up;

        _eulerOrigin = _transform.rotation.eulerAngles;
    }

    public void Update()
    {
        if(CheckOutOfRange(out Vector3 outAngle))
        {
            var euler =_transform.rotation.eulerAngles;
            euler += outAngle;
            _transform.rotation = Quaternion.Euler(euler);
        }
    }

    public void DrawDebugAngle()
    {

    }

    public bool CheckOutOfRange(out Vector3 outAngle)
    {
        var euler = _transform.rotation.eulerAngles;
        var hinge = hingeAngle * .5f;
        outAngle.x = MathEx.tiltZero(Mathf.DeltaAngle(euler.x,_eulerOrigin.x),hinge.x);
        outAngle.y = MathEx.tiltZero(Mathf.DeltaAngle(euler.y,_eulerOrigin.y),hinge.y);
        outAngle.z = MathEx.tiltZero(Mathf.DeltaAngle(euler.z,_eulerOrigin.z),hinge.z);

        return outAngle.sqrMagnitude > 0f;
    }



    #if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        Handles.color = Color.white;
        var forward = transform.forward;
        var right = transform.right;
        var up = transform.up;
        var position = transform.position;
        
        var currForward = forward;
        var currRight = right;
        var currUp = up;

        if(Application.isPlaying)
        {
            forward = _forwardOrigin;
            right = _rightOrigin;
            up = _upOrigin;
        }

        Handles.color = Color.red;
        Handles.DrawLine(position,position + currRight);
        Handles.DrawLine(position,position + (Quaternion.Euler(hingeAngle.x * .5f,0f,0f) * forward));
        Handles.DrawLine(position,position + (Quaternion.Euler(-hingeAngle.x * .5f,0f,0f) * forward));

        Handles.color = Color.green;
        Handles.DrawLine(position,position + currUp);
        Handles.DrawLine(position,position + (Quaternion.Euler(0f,hingeAngle.y * .5f,0f) * right));
        Handles.DrawLine(position,position + (Quaternion.Euler(0f,-hingeAngle.y * .5f,0f) * right));

        Handles.color = Color.blue;
        Handles.DrawLine(position,position + currForward);
        Handles.DrawLine(position,position + (Quaternion.Euler(0f,0f,hingeAngle.z * .5f) * up));
        Handles.DrawLine(position,position + (Quaternion.Euler(0f,0f,-hingeAngle.z * .5f) * up));

    }
#endif

}