using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RopeJoint
{
    public RopeJoint connectionJoint1;
    public RopeJoint connectionJoint2;

    public LayerMask collisionLayer;

    public Vector3 position;
    public Vector3 velocity;

    public float thickness;
    public float tension;
    public float mass;

    private float _massOrigin;
    private float _maxDistance;

    public float GetMaxDistance() {return _maxDistance;}

    public void UpdateCollision()
    {
        var colliders = Physics.OverlapSphere(position,thickness,collisionLayer);
        if(colliders.Length > 0)
        {
            var point = colliders[0].ClosestPoint(position);
            var len = Vector3.Distance(position,point);

            position += (position - point).normalized * (thickness - len);
        }
    }

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }
    
    public void UpdateTension()
    {
        float t = 0f;
        float count = 0;
        if(connectionJoint1 != null)
        {
            t += CalcDistance(connectionJoint1);
            ++count;
        }
        if(connectionJoint2 != null)
        {
            t += CalcDistance(connectionJoint2);
            ++count;
        }
        
        SetTension(t / count);
    }

    public float CalcDistance(RopeJoint joint)
    {
        if(joint == null)
            return 0;
        
        return Vector3.Distance(position,joint.position);
    }

    public float CalcTension(RopeJoint joint)
    {
        float tension = CalcDistance(joint) - _maxDistance;
        tension = tension < 0 ? 0f : tension;

        return tension;
    }

    public void SetTension(float value)
    {
        tension = value;
    }

    public void SetMaxDistance(float maxDistance)
    {
        _maxDistance = maxDistance;
    }

    public void SetDefault(float jointMass, float maxDistance)
    {
        _massOrigin = mass = jointMass;
        SetMaxDistance(maxDistance);
    }
}
