using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeExV2 : MonoBehaviour
{
    public LayerMask collisionLayer;
    public List<RopeJoint> joints;
    public int jointCount = 10;
    public float ropeLength = 100f;
    public float ropeThickness = 1f;
    public float defaultMass = 1f;
    public float gravityScale = 1f;
    public float springiness = 1f;

    private LineRenderer lineRenderer;
    private Transform _transform;
    private float _jointDistance;
    private float _newJointDistance;

    private bool _jointDistModified = false;

    private Vector3 _gravity = new Vector3(0f,-9.8f,0f);

    public void Awake()
    {
        _transform = transform;
        CreateRopeJoints(ropeLength, jointCount);

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = jointCount;

        UpdateLineRenderer();
        SetRopeThickness(ropeThickness);
    }

    public void Update()
    {
        UpdateGravity();
        UpdatePhysics();
        UpdatePosition();
    }

    public void LateUpdate()
    {
        UpdateLineRenderer();
    }

    public void CreateRopeJoints(float len, int count)
    {
        jointCount = count;
        SetRopeLength(len);

        for(int i = 0; i < jointCount; ++i)
        {
            var joint = new RopeJoint();
            joint.thickness = ropeThickness;
            joint.collisionLayer = collisionLayer;
            joint.SetDefault(defaultMass,_jointDistance);

            joints.Add(joint);
        }

        for(int i = 0; i < jointCount; ++i)
        {
            if(i - 1 > 0)
                joints[i].connectionJoint1 = joints[i - 1];
            if(i + 1 < jointCount)
                joints[i].connectionJoint2 = joints[i + 1];
        }

    }

    public void UpdateGravity()
    {
        for(int i = 1; i < joints.Count; ++i)
        {
            joints[i].AddForce(_gravity * gravityScale * Time.deltaTime);
        }
    }

    public void UpdatePhysics()
    {
        for(int i = 0; i < joints.Count; ++i)
        {
            joints[i].UpdateTension();
        }

        
    }

    public Vector3 ChangeDirection(Vector3 velocity, Vector3 dir)
	{
		Vector3 vel = velocity;
		Vector3 normal = vel.normalized;

		vel.x = normal.x != 0 ? dir.x * (vel.x / normal.x) : dir.x;
		vel.y = normal.y != 0 ? dir.y * (vel.y / normal.y) : dir.y;
        vel.z = normal.z != 0 ? dir.z * (vel.z / normal.z) : dir.z;

		return vel;
	}

    public void UpdatePosition()
    {
        joints[0].position = _transform.position;
        for(int i = 1; i < joints.Count; ++i)
        {
            //joints[i].position += joints[i].velocity;

            var dist = joints[i].CalcDistance(joints[i - 1]);

            if(_jointDistModified)
            {
                joints[i].SetMaxDistance(_newJointDistance);
            }

            var max = joints[i].GetMaxDistance();
            
            if(dist > max)
            {
                var dir = joints[i - 1].position - joints[i].position;
                //dir = dir.normalized;

                joints[i].velocity = dir * 0.1f;
                joints[i].position += dir.normalized * (dist - max);

                // var velocity = joints[i].velocity.normalized;
                // var dirPoint = joints[i - 1].position + velocity * _jointDistance;

                // dir = dirPoint - joints[i].position;

                // joints[i].velocity = ChangeDirection(joints[i].velocity,dir.normalized);
            }

            joints[i].UpdateCollision();
        }

        _jointDistModified = false;
    }

    public void SetRopeLength(float len)
    {
        ropeLength = len;
        SetJointDistance(ropeLength / (float)(jointCount - 1));
    }

    public void SetJointDistance(float value)
    {
        _jointDistance = _newJointDistance = value;
        _jointDistModified = true;
    }

    public void UpdateLineRenderer()
    {
        for(int i = 0; i < jointCount; ++i)
        {
            lineRenderer.SetPosition(i,joints[i].position);
        }
    }

    public void SetRopeThickness(float t)
    {
        ropeThickness = t;
        lineRenderer.startWidth = ropeThickness;
        lineRenderer.endWidth = ropeThickness;
    }
}
