using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenMedusa_AI : Scanable
{
    public enum State
    {
        Idle,
        Scanned,
        LockOnMove,
        LockOnLook,
        LockOnFrontWalk,
        SearchIdle,
        SearchRotate,
        SearchScan,
        Dead
    }

    public float rotationSpeed = 180f;
    public float groundCheckRadius = 5f;
    public float headRotationLock = 90f;
    public float pushDistance = 3f;
    public float scanYLimit = 10f;
    public float lookDistance = 20f;
    public float frontMoveSpeed = 5f;
    public float minimumTargetDistance = 20f;

    public Transform test;

    public GraphAnimator graphAnimator;
    public BossScan scanner;
    public FloorControl floorControl;

    public LayerMask groundLayer;
    public LayerMask targetLayer;
    public Transform head;
    public Transform body;
    public Transform shildTransform;
    public Transform shildGraphic;
    public Transform groundRayPoint;

    public List<IKLegMovement> legs = new List<IKLegMovement>();

    public State currentState;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private Transform _target;

    private Vector3 _moveLine;
    private Vector3 _perpendicularPoint;
    private Vector3 _searchDirection;

    private float _pointDistance;
    private float _targetDistance;
    private float _direction;//1 = right, -1 = left

    public void Start()
    {
        _target = GameManager.Instance.player.transform;
        graphAnimator.Play("UpDown",body);
    }

    public void Update()
    {
        UpdateDirection();
        UpdatePerpendicularPoint();
        Push(false);

        body.localRotation = Quaternion.Lerp(body.localRotation,Quaternion.identity,0.1f);

        if(currentState == State.Idle)
        {
            var dist = Vector3.Distance(_target.position,transform.position);
            if(dist < 7f)
            {
                ChangeState(State.Scanned);
            }
            else if(Vector3.Angle(transform.forward,MathEx.DeleteYPos(_target.position - transform.position)) >= 70f)
            {
                ChangeState(State.Scanned);
            }
        }
        else if(currentState == State.LockOnLook)
        {
            LookTarget_Head();
            if(lookDistance > _targetDistance)
            {
                UpdateMoveLine();
                ChangeState(State.LockOnMove);
            }

            if(!IsOnGrounded())
            {
                ChangeState(State.SearchIdle);
            }
        }
        else if(currentState == State.LockOnMove)
        {
            LookLineForward_Body();
            LineMove();

            if(lookDistance < _targetDistance)
                ChangeState(State.LockOnLook);

            if(!IsOnGrounded())
            {
                ChangeState(State.SearchIdle);
            }

            // if(_targetDistance >= minimumTargetDistance)
            // {
            //     ChangeState(State.LockOnFrontWalk);
            // }
        }
        else if(currentState == State.LockOnFrontWalk)
        {
            TargetFrontMove();
            if(_targetDistance <= minimumTargetDistance)
            {
                ChangeState(State.LockOnMove);
            }
        }
        else if(currentState == State.Scanned)
        {
            if(LookTarget_Head())
            {
                if(!scanner.scaning)
                {
                    ScanForward();
                }
                else
                {
                    if(ScanCheck())
                    {
                        UpdateMoveLine();
                        ChangeState(State.LockOnMove);

                        if(!floorControl._launch)
                        {
                            floorControl.Launch();
                        }
                    }
                }

                

            }
        }
        else if(currentState == State.SearchIdle)
        {
            _timeCounter.IncreaseTimer("SearchIdle",1f,out bool limit);
            if(limit)
            {
                ChangeState(State.SearchScan);
            }
        }
        else if(currentState == State.SearchRotate)
        {
            HeadTurn(false,true);
            BodyRotateForHead();

            var angle = Vector3.Angle(_searchDirection,head.forward);
            if(angle >= 90f)
            {
                ChangeState(State.SearchIdle);
            }
        }
        else if(currentState == State.SearchScan)
        {
            if(!scanner.scaning)
            {
                ScanForward();
            }
            else
            {
                if(ScanCheck())
                {
                    UpdateMoveLine();
                    ChangeState(State.LockOnMove);

                    if(!floorControl._launch)
                    {
                        floorControl.Launch();
                    }
                }
                else
                {
                    _timeCounter.IncreaseTimer("SearchScan",1f,out bool limit);
                    if(limit)
                    {
                        ChangeState(State.SearchRotate);
                    }
                }
            }
        }
        else if(currentState == State.Dead)
        {
            graphAnimator.Stop();
            graphAnimator.Play("Dead",body);

            this.enabled = false;
        }

    }

    public void ChangeState(State state)
    {
        currentState = state;

        if(currentState == State.SearchIdle)
        {
            _moveLine = Vector3.zero;
            _timeCounter.InitTimer("SearchIdle");
        }
        else if(currentState == State.SearchRotate)
        {
            _searchDirection = head.forward;
            _timeCounter.InitTimer("SearchRotate");
        }
        else if(currentState == State.SearchScan)
        {
            scanner.scaning = false;
            _timeCounter.InitTimer("SearchScan");
        }
    }

    public void Dead()
    {
        ChangeState(State.Dead);
    }

    public override void Scanned()
    {
        if((currentState == State.Idle || currentState == State.SearchIdle || currentState == State.SearchRotate || currentState == State.SearchScan)
         && IsOnGrounded())
        {
            ChangeState(State.Scanned);
        }
    }

    public bool IsOnGrounded()
    {
        return (_target.position.y - transform.position.y) < scanYLimit;
    }

    public void ScanForward()
    {
        scanner.ScanSetup(transform.position,head.forward);
    }

    public bool ScanCheck()
    {
        if(!scanner.scaning)
            return false;

        Vector3 scanObjPosition = _target.position;
        Vector3 startPosition = transform.position;
        
        if(scanObjPosition.y >= startPosition.y + scanYLimit)
        {
            return false;
        }

        scanObjPosition.y = startPosition.y = 0.0f;

        if (Mathf.Acos(Vector3.Dot(head.forward, (scanObjPosition - startPosition).normalized)) * Mathf.Rad2Deg < scanner.arc)
        {
            float mag = (scanObjPosition - startPosition).magnitude;
            if (mag <= scanner.range && mag >= scanner.range - 3f)
            {
                //scan
                Debug.Log("scanned");
                return true;
            }
        }

        return false;
    }

    public void Push(bool up)
    {
        if(pushDistance >= _targetDistance && graphAnimator.IsPlaying("ShildAttack") == null)
        {
            if(up)
            {
                PushBackUp();
            }
            else
            {
                PushBack();
            }
        }
    }

    public void PushBack()
    {
        graphAnimator.Play("ShildAttack",shildGraphic);

        Collider[] playerColl = Physics.OverlapSphere(shildTransform.position, 10f,targetLayer);

        if(playerColl.Length != 0)
        {
            foreach(Collider curr in playerColl)
            {
                PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
                if(ragdoll != null)
                {
                    ragdoll.ExplosionRagdoll(150.0f, 
                        Vector3.ProjectOnPlane((_target.position - _perpendicularPoint),Vector3.up).normalized);
                }
            }
        }
    }

    public void PushBackUp()
    {
        graphAnimator.Play("ShildAttack",shildGraphic);

        Collider[] playerColl = Physics.OverlapSphere(shildTransform.position, 10f,targetLayer);

        if(playerColl.Length != 0)
        {
            foreach(Collider curr in playerColl)
            {
                PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
                if(ragdoll != null)
                {
                    ragdoll.ExplosionRagdoll(220.0f,_perpendicularPoint,1000f);
                }
            }
        }
    }

    public void TargetFrontMove()
    {
        var dir = (_target.position - transform.position).normalized;
        if(_pointDistance >= 1f)
        {
            Move(dir, frontMoveSpeed);
        }
    }

    public void LineMove()
    {
        test.position = _perpendicularPoint;

        if(_pointDistance >= 1f)
        {
            Move(_moveLine, (_pointDistance * 4f) * _direction);
        }
    }

    public bool LookTarget_Head()
    {
        return HeadLook((_target.position - transform.position), true);
        //BodyLook((_target.position - transform.position));
    }

    public void LookLineForward_Body()
    {
        var dir = Vector3.Cross(transform.transform.up,_moveLine).normalized;

        HeadLook(dir, false);
        BodyLook(dir);
    }

    public void SetLegMovementSpeed(float speed)
    {
        foreach(var leg in legs)
        {
            leg.legSpeed = speed;
        }
    }

    public void UpdatePerpendicularPoint()
    {
        _perpendicularPoint = MathEx.PerpendicularPoint(transform.position + _moveLine * -100f,transform.position + _moveLine * 100f, _target.position);
        _pointDistance = Vector3.Distance(transform.position,_perpendicularPoint);

        var point = _perpendicularPoint;
        point.y = _target.position.y;
        _targetDistance = Vector3.Distance(_target.position,point);
    }

    public void UpdateDirection()
    {
        var cross = Vector3.Cross(transform.forward,_target.position - transform.position);
        _direction = Vector3.Dot(Vector3.up,cross) < 0 ? 1f : -1f;
    }

    public void UpdateMoveLine()
    {
        var direction = (_target.position - transform.position);
        direction = Vector3.ProjectOnPlane(direction,Vector3.up).normalized;

        _moveLine = Vector3.Cross(direction,Vector3.up);
    }

    public void Move(Vector3 direction, float speed)
    {
        if(!ThereIsGround((direction * (groundCheckRadius * 0.5f)) * MathEx.normalize(speed),10f))
        {
            return;
        }

        SetLegMovementSpeed(4f + (MathEx.clampOverZero(MathEx.abs(speed) - 4f) * 0.4f));

        transform.position += direction * speed * Time.deltaTime;

        body.localRotation = body.localRotation * Quaternion.Euler(0f,0f,speed * Time.deltaTime * 10f);
    }

    public bool HeadLook(Vector3 direction, bool bodyRotate)
    {
        var plane = Vector3.ProjectOnPlane(direction,head.up).normalized;
        var headAngle = Vector3.SignedAngle(head.forward,plane,head.up);
        bool look = false;
        //head.RotateAround(head.position,head.up,headAngle);
        if(MathEx.abs(headAngle) >= 1f)
        {
            HeadTurn(headAngle > 0,bodyRotate);
        }
        else
        {
            look = true;
        }

        BodyRotateForHead();

        return look;
        
    }

    public void BodyLook(Vector3 direction)
    {
        var plane = Vector3.ProjectOnPlane(direction,transform.up).normalized;
        var bodyAngle = Vector3.SignedAngle(transform.forward,plane,transform.up);

        if(MathEx.abs(bodyAngle) >= 1f)
        {
            BodyTurn(bodyAngle > 0);
        }
    }

    public void HeadTurn(bool isLeft, bool bodyRotate)
    {
        head.RotateAround(head.position,head.up,rotationSpeed * Time.deltaTime * (isLeft ? 1f : -1f));
    }

    public void BodyRotateForHead()
    {
        var angle = Vector3.Angle(head.forward, transform.forward);

        if(angle > headRotationLock)
        {
            BodyTurn(Vector3.SignedAngle(head.forward,transform.forward,head.up) < 0);//_direction < 0);
        }
    }

    public void BodyTurn(bool isLeft)
    {
        transform.RotateAround(transform.position,transform.up,rotationSpeed * Time.deltaTime * (isLeft ? 1f : -1f));
    }

    public bool ThereIsGround(Vector3 position, float dist)
    {
        Ray ray = new Ray();
        ray.origin = groundRayPoint.position + position;
        ray.direction = -transform.up;

        return Physics.Raycast(ray,dist,groundLayer);
    }
}
