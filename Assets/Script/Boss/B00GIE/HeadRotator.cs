using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HeadRotator : MonoBehaviour
{
    public Transform target;
    public Transform ik;

    public Transform ikHolder;

    public float widthAngle;
    public float heightAngle;

    public float maxHeadDist;

    public float maxDist;
    public float minDist;

    public float headMoveFactor = 20f;

    public bool ikHold = false;

    private Vector3 _targetPosition;
    private Vector3 _upOrigin;
    private Quaternion _ikQuaternionOrigin;

    public void Start()
    {
        if(ikHolder == null)
        {
            ikHolder = new GameObject("IKHolder").transform;
            ikHolder.position = ik.position;
        }

        _upOrigin = ik.up;
        _ikQuaternionOrigin = ik.rotation;
    }

    public void Update()
    {
        UpdateHead(Time.deltaTime);
    }

    public void UpdateHead(float deltaTime)
    {
        if(!TargetIsInArea() || ikHold)
        {
            SetTargetPosition(ikHolder.position);
            ReturnOriginRotate(headMoveFactor,deltaTime);

//            Debug.Log("?");
        }
        else
        {
            SetTargetPosition(target.position);
            UpdateIKRotation(headMoveFactor,deltaTime);

            Debug.Log("??");
        }

        UpdateTargetToHeadDist();
        UpdateIKPosition(headMoveFactor,deltaTime);
    }

    public void ReturnOriginRotate(float factor, float deltaTime)
    {
        ik.localRotation = Quaternion.Lerp(ik.localRotation,_ikQuaternionOrigin,factor * deltaTime);
    }

    public void UpdateIKRotation(float factor, float deltaTime)
    {
        var dir = (_targetPosition - transform.position).normalized;
        ik.rotation = Quaternion.Lerp(ik.rotation, _ikQuaternionOrigin * Quaternion.LookRotation(dir,_upOrigin),factor * deltaTime);
    }

    public void UpdateIKPosition(float factor, float deltaTime)
    {
        ik.position = Vector3.Lerp(ik.position,_targetPosition,factor * deltaTime);
    }

    public void UpdateTargetToHeadDist()
    {
        var dir = (_targetPosition - transform.position).normalized;
        var dist = Vector3.Distance(transform.position,_targetPosition);
        dist = dist >= maxHeadDist ? maxHeadDist : dist;
        _targetPosition = transform.position + dir * dist;
    }

    public void SetTargetPosition(Vector3 pos)
    {
        _targetPosition = pos;
    }

    public bool TargetIsInArea()
    {
        if(target == null)
            return false;

        var forward = transform.forward;
        var dist = Vector3.Distance(transform.position,target.position);

        if(dist < minDist || dist > maxDist)
            return false;

        var dir = (target.position - transform.position).normalized;
        var w = Vector3.Angle(Vector3.ProjectOnPlane(forward,transform.up),Vector3.ProjectOnPlane(dir,transform.up));

        if(w > widthAngle * .5f)
            return false;

        var h = Vector3.Angle(Vector3.ProjectOnPlane(forward,transform.right),Vector3.ProjectOnPlane(dir,transform.right));

        if(h > heightAngle * .5f)
            return false;

        return true;

    }

    void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var forward = transform.forward;
            var forwardMax = transform.position + forward * maxDist;
            Handles.color = Color.green;
            Handles.DrawLine(transform.position + forward * minDist, forwardMax);

            Handles.color = Color.red;
            var r00 = transform.rotation * (Quaternion.Euler(heightAngle * .5f,0f,0f) * Vector3.forward);
            var r01 = transform.rotation * (Quaternion.Euler(-heightAngle * .5f,0f,0f) * Vector3.forward);
            Handles.DrawLine(transform.position + r00 * minDist,transform.position + r00 * maxDist);
            Handles.DrawLine(transform.position + r01 * minDist,transform.position + r01 * maxDist);

            Handles.color = Color.blue;
            var r10 = transform.rotation * (Quaternion.Euler(0f,widthAngle * .5f,0f) * Vector3.forward);
            var r11 = transform.rotation * (Quaternion.Euler(0f,-widthAngle * .5f,0f) * Vector3.forward);
            Handles.DrawLine(transform.position + r10 * minDist,transform.position + r10 * maxDist);
            Handles.DrawLine(transform.position + r11 * minDist,transform.position + r11 * maxDist);

            Handles.color = Color.green;

            Handles.DrawLine(transform.position + r00 * minDist,transform.position + r10 * minDist);
            Handles.DrawLine(transform.position + r10 * minDist,transform.position + r01 * minDist);
            Handles.DrawLine(transform.position + r01 * minDist,transform.position + r11 * minDist);
            Handles.DrawLine(transform.position + r11 * minDist,transform.position + r00 * minDist);

            Handles.DrawLine(transform.position + r00 * maxDist,transform.position + r10 * maxDist);
            Handles.DrawLine(transform.position + r10 * maxDist,transform.position + r01 * maxDist);
            Handles.DrawLine(transform.position + r01 * maxDist,transform.position + r11 * maxDist);
            Handles.DrawLine(transform.position + r11 * maxDist,transform.position + r00 * maxDist);

            Handles.DrawLine(transform.position + r00 * maxDist,forwardMax);
            Handles.DrawLine(transform.position + r10 * maxDist,forwardMax);
            Handles.DrawLine(transform.position + r01 * maxDist,forwardMax);
            Handles.DrawLine(transform.position + r11 * maxDist,forwardMax);
#endif

        }
}
