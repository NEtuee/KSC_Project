using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBossBase : Scanable
{
    public float rotationSpeed = 180f;
    public float groundCheckRadius = 5f;
    public float frontMoveSpeed = 5f;

    public LayerMask groundLayer;
    public LayerMask targetLayer;
    
    public Transform groundRayPoint;

    public List<IKLegMovement> legs = new List<IKLegMovement>();

    protected TimeCounterEx _timeCounter = new TimeCounterEx();
    protected Transform _target;

    protected Vector3 _centerPosition;
    protected float _targetDistance;

    public virtual void Initialize()
    {
        _target = GameManager.Instance.player.transform;
        _centerPosition = transform.position;
    }

    public override void Scanned()
    {
        
    }

    public void TargetFrontMove()
    {
        var dir = (_target.position - transform.position);
        dir = Vector3.ProjectOnPlane(dir,Vector3.up).normalized;
        if(_targetDistance >= 2f)
        {
            Move(dir, frontMoveSpeed);
        }
    }

    public void CenterMove()
    {
        var dir = (_centerPosition - transform.position).normalized;
        var centerDist = Vector3.Distance(_centerPosition,transform.position);
        if(centerDist >= 1f)
        {
            Move(dir, frontMoveSpeed);
        }
    }

    public void SetLegMovementSpeed(float speed)
    {
        foreach(var leg in legs)
        {
            leg.legSpeed = speed;
        }
    }

    public virtual bool Move(Vector3 direction, float speed, float legMovementSpeed = 4f)
    {
        if(!ThereIsGround((direction * (groundCheckRadius * 0.5f)) * MathEx.normalize(speed),10f))
        {
            return false;
        }

        SetLegMovementSpeed(legMovementSpeed + (MathEx.clampOverZero(MathEx.abs(speed) - legMovementSpeed) * 0.4f));

        transform.position += direction * speed * Time.deltaTime;

        return true;
    }

    public void Turn(bool isLeft, Transform target)
    {
        target.RotateAround(target.position,target.up,rotationSpeed * Time.deltaTime * (isLeft ? 1f : -1f));
    }

    public bool GroundCheck(out RaycastHit hit,float dist)
    {
        Ray ray = new Ray();
        ray.origin = groundRayPoint.position;
        ray.direction = -transform.up;

        return Physics.Raycast(ray,out hit, dist, groundLayer);
    }

    public bool ThereIsGround(Vector3 position, float dist)
    {
        Ray ray = new Ray();
        ray.origin = groundRayPoint.position + position;
        ray.direction = -transform.up;

        return Physics.Raycast(ray,dist,groundLayer);
    }
}
