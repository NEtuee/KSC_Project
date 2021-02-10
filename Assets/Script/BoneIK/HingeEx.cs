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


    public void Start()
    {
        _transform = GetComponent<Transform>();
        _forwardOrigin = _transform.forward;
        _rightOrigin = _transform.right;
        _upOrigin = _transform.up;
    }

    public void LateUpdate()
    {
        if(CheckOutOfRange(out Vector3 outAngle))
        {
//            Debug.Log(outAngle.x + "," + outAngle.y + "," + outAngle.z);
        }
    }

    public void DrawDebugAngle()
    {

    }

    

    public bool CheckOutOfRange(out Vector3 outAngle)
    {
        var forward = transform.eulerAngles - 
        var right = MathEx.PlaneAngle(_transform.right,_rightOrigin,new Vector3(0f,1f,0f));
        var up = MathEx.PlaneAngle(_transform.up,_upOrigin,new Vector3(0f,0f,1f));
        
        outAngle = (new Vector3(forward,right,up) - (hingeAngle * 0.5f));
        Debug.Log(outAngle.x + "," + outAngle.y + "," + outAngle.z);

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