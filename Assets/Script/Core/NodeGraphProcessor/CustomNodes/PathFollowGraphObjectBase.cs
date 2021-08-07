using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowGraphObjectBase : GraphObjectBase
{
    public float distanceAccuracy = 5f;
    public float turnAccuracy = 1f;

    [HideInInspector]public PathManagerEx.PathClass currentPath;
    [HideInInspector]public Transform targetTransform;
    [HideInInspector]public int targetPoint;

    [HideInInspector]public Vector3 targetDirection;
    [HideInInspector]public float accelSpeed = 0f;

    [HideInInspector]public bool pathArrived = false;
    [HideInInspector]public bool pathLoop = false;

    protected override void Awake()
    {
        base.Awake();

        _graph.GetExposedParameter("PathFollow Object").value = this;
    }

    public bool FollowPath(float mainSpeed, float rotationSpeed, float deltaTime)
    {
        if(pathArrived)
            return true;

        SetTarget(targetTransform.position);
        Move(transform.forward,mainSpeed,rotationSpeed, deltaTime);
        if(IsArrivedTarget(distanceAccuracy))
        {
            var target = GetNextPoint(out bool isEnd).transform;

            targetTransform = target;
            if(isEnd && !pathLoop)
            {
                pathArrived = true;
            }
            
            return isEnd;
        }

        return false;
    }

    public bool Move(Vector3 direction, float speed, float rotationSpeed, float deltaTime)
    {
        accelSpeed = Mathf.Lerp(accelSpeed,speed,0.2f);
        var angle = Vector3.SignedAngle(transform.forward,targetDirection,transform.up);

        if(Mathf.Abs(angle) > turnAccuracy)
        {
            if(angle > 0)
                Turn(true,this.transform,rotationSpeed,deltaTime);
            else
                Turn(false,this.transform,rotationSpeed,deltaTime);
        }
        
        transform.position += direction * (accelSpeed * deltaTime);
        
        return true;
    }

    public bool IsArrivedTarget(float dist)
    {
        return Vector3.Distance(transform.position,targetTransform.position) <= dist;
    }

    public bool GetPathArrived()
    {
        return pathArrived;
        
    }

    public MovePointEx GetNextPoint(out bool isEnd)
    {
        return currentPath.GetNextPoint(ref targetPoint, out isEnd);
    }

    public void SetTarget(Vector3 target)
    {
        var direction = target - transform.position;
        targetDirection = Vector3.ProjectOnPlane(direction,transform.up).normalized;
    }

    public void SetStartPointToTarget()
    {
        targetTransform = currentPath.GetPoint(0).transform;
    }

    public void SetNearestPointToTarget()
    {
        targetTransform = currentPath.FindNearestPoint(transform.position,out targetPoint).transform;
    }

    public void SetNearestPointInArc(float angle)
    {
        targetTransform = FindNearestPointInArc(angle, out targetPoint);
    }

    public Transform FindNearestPointInArc(float angle,out int target)
    {
        var points = currentPath.movePoints;
        
        int point = -1;
        float near = 0f;

        for(int i = 0; i < points.Count; ++i)
        {
            var pointProject = Vector3.ProjectOnPlane(points[i].GetPoint(),Vector3.up);
            var transformProject = Vector3.ProjectOnPlane(transform.position,Vector3.up);
            var directionProject = Vector3.ProjectOnPlane(transform.forward,Vector3.up);
            var pointDirection = (pointProject - transformProject).normalized;

            if(Vector3.Angle(directionProject,pointDirection) > angle)
            {
                var dist = Vector3.Distance(points[i].GetPoint(),transform.position);
                if(point == -1 || near > dist)
                {
                    point = i;
                    near = dist;
                }
            }
        }

        if(point == -1)
        {
            return currentPath.FindNearestPoint(transform.position,out target).transform;
        }
        else
        {
            target = point;
            return points[point].transform;
        }
    }

    public void SetPath(Message msg, bool loop)
    {
        pathArrived = false;
        currentPath = (PathManagerEx.PathClass)msg.data;
        
        pathLoop = loop;
    }

    public void SetPathTargetZero()
    {
        targetTransform = currentPath.GetPoint(0).transform;
    }

    public void SetPathTargetNear(bool arc = true)
    {
        if(arc)
            SetNearestPointInArc(30f);
        else
            SetNearestPointToTarget();
    }
}
