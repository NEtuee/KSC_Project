using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEditor;
using UnityEngine;

public class DirectionCollisionEx
{
    private Transform _target;
    private float _collisionRadius;
    private LayerMask _collisionLayer;
    private RayEx _ray;

    public DirectionCollisionEx(Transform main,float radius,LayerMask layer)
    {
        _target = main;
        SetRadius(radius);
        _collisionLayer = layer;

        _ray = new RayEx(new Ray(Vector3.zero, Vector3.zero), 0f, layer);
    }

    public bool Cast(Vector3 start,Vector3 direction, float farDist, out float collisionDist, out Vector3 collisionCenter)
    {
        _ray.SetDirection(direction);
        //_ray.radius = _collisionRadius;
        _ray.Distance = farDist;

        if (_ray.Cast(start, out var hit))
        {
            collisionCenter = hit.point + hit.normal * 0.2f;// - dir * _collisionRadius;
            collisionDist = Vector3.Distance(start, collisionCenter);

            return true;
        }
        
        collisionCenter = Vector3.zero;
        collisionDist = 0f;
        return false;
    }

    public void SetRadius(float value)
    {
        _collisionRadius = value;
    }
    
}
