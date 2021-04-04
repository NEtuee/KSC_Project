using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKPathFollowBossBase : IKBossBase
{
    public LevelEdit_Controll controll;

    public float distanceAccuracy = 5f;
    public float mainSpeed = 10f;

    protected LevelEdit_PointManager.PathClass _currentPath;
    protected Transform _targetTransform;
    protected int _targetPoint;

    protected Vector3 _targetDirection;
    protected float _turnAccuracy = 1f;
    protected float _accelSpeed = 0f;

    protected bool _pathArrived = false;
    protected bool _pathLoop = false;

    public bool FollowPath()
    {
        if(_pathArrived)
            return true;

        SetTarget(_targetTransform.position);
        Move(transform.forward,mainSpeed);
        if(IsArrivedTarget(distanceAccuracy))
        {
            var target = GetNextPoint(out bool isEnd).transform;

            _targetTransform = target;
            if(isEnd && !_pathLoop)
            {
                _pathArrived = true;
            }
            
            return isEnd;
        }

        return false;
    }

    public override bool Move(Vector3 direction, float speed, float legMovementSpeed = 4f)
    {
        _accelSpeed = Mathf.Lerp(_accelSpeed,speed,0.2f);
        var angle = Vector3.SignedAngle(transform.forward,_targetDirection,transform.up);

        if(MathEx.abs(angle) > _turnAccuracy)
        {
            if(angle > 0)
                Turn(true,this.transform);
            else
                Turn(false,this.transform);
        }
        


        transform.position += direction * _accelSpeed * Time.deltaTime;
        

        return true;
    }

    public bool IsArrivedTarget(float dist)
    {
        return Vector3.Distance(transform.position,_targetTransform.position) <= dist;
    }

    public LevelEdit_MovePoint GetNextPoint(out bool isEnd)
    {
        return _currentPath.GetNextPoint(ref _targetPoint, out isEnd);
    }

    public void SetTarget(Vector3 target)
    {
        var direction = target - transform.position;
        _targetDirection = Vector3.ProjectOnPlane(direction,transform.up).normalized;
    }

    public void SetNearestPointToTarget()
    {
        _targetTransform = _currentPath.FindNearestPoint(transform.position,out _targetPoint).transform;
    }

    public void SetNearestPointInArc(float angle)
    {
        _targetTransform = FindNearestPointInArc(angle, out _targetPoint);
    }

    public Transform FindNearestPointInArc(float angle,out int target)
    {
        var points = _currentPath.movePoints;
        
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
            return _currentPath.FindNearestPoint(transform.position,out target).transform;
        }
        else
        {
            target = point;
            return points[point].transform;
        }
    }

    public void GetPath(string path, bool arc = true)
    {
        _pathArrived = false;
        _currentPath = controll.GetPath(path);
        if(arc)
            SetNearestPointInArc(30f);
        else
            SetNearestPointToTarget();
    }
}
