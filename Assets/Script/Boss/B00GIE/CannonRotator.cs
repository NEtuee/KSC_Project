using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CannonRotator : MonoBehaviour
{
    public Transform cannonVertical;
    public Transform cannonHorizontal;

    public Vector3 verticalAngleOffset;
    public Vector3 horizontalAngleOffset;

    public Transform target;

    public float lerpFactor;
    public float minDist;
    public float maxDist;
    
    public bool rotateLock = false;
    public bool targetInArea = false;

    private Quaternion _verticalTarget;
    private Quaternion _horizontalTarget;
    
    private Quaternion _verticalOrigin;
    private Quaternion _horizontalOrigin;

    public void Start()
    {
        _verticalOrigin = cannonVertical.rotation;
        _horizontalOrigin = cannonHorizontal.rotation;
    }

    public void Update()
    {
        RotateProgress(Time.deltaTime);
    }

    public void RotateProgress(float deltaTime)
    {
        var dist = Vector3.Distance(transform.position, target.position);
        if (dist < minDist || dist > maxDist)
        {
            targetInArea = false;
            SetTargetToOrigin();
        }
        else
        {
            targetInArea = true;
            
            var verticalDir = target.position - cannonVertical.position;
            var horizontalDir = target.position - cannonHorizontal.position;

            verticalDir = Vector3.ProjectOnPlane(verticalDir, transform.up).normalized;
            //horizontalDir = Vector3.ProjectOnPlane(horizontalDir, transform.up);

            _verticalTarget = Quaternion.LookRotation(verticalDir) * Quaternion.Euler(verticalAngleOffset);
            _horizontalTarget = Quaternion.LookRotation(horizontalDir) * Quaternion.Euler(horizontalAngleOffset);
            //_horizontalTarget = Quaternion.Euler(0f, 0f, Vector3.Angle(transform.forward, horizontalDir));
            // _horizontalTarget = Quaternion.Euler(0f,0f,Vector3.Angle(-cannonHorizontal.up, horizontalDir)) 
            //                     * Quaternion.Euler(horizontalAngleOffset);
        }
        
        if(!rotateLock)
            Rotate(deltaTime);
    }

    public Vector3 GetLookDirection()
    {
        return -cannonHorizontal.up;
    }
    
    public void SetTargetToOrigin()
    {
        var angle = transform.rotation.eulerAngles;
        var inverse = Quaternion.identity *
                             Quaternion.AngleAxis(angle.x, Vector3.forward) *
                             Quaternion.AngleAxis(angle.z, -Vector3.up) *
                             Quaternion.AngleAxis(angle.y, -Vector3.right);
        
        _verticalTarget = _verticalOrigin * inverse;
        _horizontalTarget = _horizontalOrigin * inverse;
    }
    

    public void Rotate(float deltaTime)
    {
        cannonVertical.rotation = Quaternion.Lerp(cannonVertical.rotation, _verticalTarget, lerpFactor * deltaTime);
        cannonHorizontal.rotation = Quaternion.Lerp(cannonHorizontal.rotation, _horizontalTarget, lerpFactor * deltaTime);
    }
}
