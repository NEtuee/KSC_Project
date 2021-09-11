using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAIBase : ObjectBase
{
    public bool directionRotation = false;
    public float speed;
    public float maxSpeed;

    protected Vector3 _velocity;
    protected Vector3 _direction;
    protected Vector3 _targetDirection;
    protected Vector3 _randomRotateDirectionFactor;
    protected Vector3 _targetOffset;

    protected float _directionUpdateTerm;
    protected float _targetDistance;

    protected PlayerUnit _player;
    protected Transform _target;
    protected TimeCounterEx _timeCounterEx = new TimeCounterEx();

    public override void Assign()
    {
        base.Assign();
        
        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            _target = _player.transform;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        _velocity = Vector3.zero;
        _direction = Vector3.zero;
        _randomRotateDirectionFactor = Vector3.zero;
        _directionUpdateTerm = 0f;
        _timeCounterEx.InitTimer("directionUpdate",0f,_directionUpdateTerm);

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl,GetSavedNumber("PlayerManager"),null);

        Debug.Log("?");
    }

    public override void FixedProgress(float deltaTime)
    {
        if(_target != null)
            UpdateTargetDirection(deltaTime);

        if(directionRotation)
        {
            DirectionRotation();
        }

        UpdateVelocity(deltaTime);
    }

    public void DirectionRotation()
    {
        _targetDirection = (GetTargetPosition() - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(_targetDirection,Vector3.up);
    }

    public void UpdateTargetDirection(float deltaTime)
    {
        if(_directionUpdateTerm == 0f)
        {
            UpdateTargetDirection();
        }
        else
        {
            _timeCounterEx.IncreaseTimerSelf("directionUpdate",out var limit, deltaTime);
            if(limit)
            {
                _timeCounterEx.InitTimer("directionUpdate",0f,_directionUpdateTerm);
                UpdateTargetDirection();
            }
        }
    }

    public void UpdateTargetDirection()
    {
        _targetDirection = (GetTargetPosition() - transform.position).normalized;
        _targetDistance = Vector3.Distance(GetTargetPosition(),transform.position);
        var dir = Quaternion.Euler(MathEx.RandomVector3(_randomRotateDirectionFactor)) * _targetDirection;
        ChangeDirection((dir).normalized);
    }

    public Vector3 GetTargetPosition()
    {
        return _target.position + _targetOffset;
    }

    public void SetTargetOffset(Vector3 offset)
    {
        _targetOffset = offset;
    }

    public void SetRandomRotate(Vector3 euler)
    {
        _randomRotateDirectionFactor = euler * 0.5f;
    }

    public void SetTargetDirectionUpdateTime(float value)
    {
        _directionUpdateTerm = value;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        UpdateTargetDirection();
    }

    public void ChangeDirection(Vector3 dir)
    {
        _direction = dir;
    }

    public void UpdateVelocity(float deltaTime)
    {
        AddForce(speed * _direction * deltaTime);
        CheckMaxSpeed();

        transform.position += _velocity * deltaTime;
    }

    public void InitVelocity()
    {
        _velocity = Vector3.zero;
    }

    public void AddForce(Vector3 force)
    {
        _velocity += force;
        CheckMaxSpeed();
    }

    public void CheckMaxSpeed()
    {
        if(_velocity.magnitude >= maxSpeed)
        {
            _velocity = _velocity.normalized * maxSpeed;
        }
    }
}
