using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmortalJirungE_AI : IKBossBase
{
    public enum State
    {
        WallMove,
        FloorWhip,
        WallMoveExit,
        Stun,
        Recovery
    };

    public LevelEdit_Controll controll;
    public State currentState;
    public BossHead head;
    public EMPShield shield;

    public LayerMask obstacleLayer;

    public List<Rigidbody> bodys = new List<Rigidbody>();

    public float distanceAccuracy = 5f;
    public float mainSpeed = 10f;

    private LevelEdit_PointManager.PathClass _currentPath;
    private Transform _targetTransform;
    private int _targetPoint;

    private Vector3 _targetDirection;
    private float _turnAccuracy = 1f;

    private bool _pathArrived = false;
    private bool _pathLoop = false;

    private SphereRayEx _forwardRay;
    private SphereRayEx _sideRay;

    public void Start()
    {
        ChangeState(State.WallMove);
        _pathLoop = true;

        _forwardRay = new SphereRayEx(new Ray(Vector3.zero,Vector3.zero),11f,8f,obstacleLayer);
        _sideRay = new SphereRayEx(new Ray(Vector3.zero,Vector3.zero),8f,8f,obstacleLayer);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            ChangeState(State.Stun);
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            ChangeState(State.Recovery);
        }

        if(currentState == State.WallMove)
        {
            FollowPath();

            _timeCounter.IncreaseTimer("wallMoveTime",out bool limit);
            if(limit)
            {
                ChangeState(State.FloorWhip);
            }
        }
        else if(currentState == State.FloorWhip)
        {
            FollowPath();

            Collider[] playerColl = Physics.OverlapSphere(transform.position, 2f,targetLayer);

            if(playerColl.Length != 0)
            {
                foreach(Collider curr in playerColl)
                {
                    PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
                    if(ragdoll != null)
                    {
                        ragdoll.ExplosionRagdoll(300.0f, transform.forward);
                    }
                }
            }

            _timeCounter.IncreaseTimer("whipTime",out bool limit);
            if(limit)
            {
                ChangeState(State.WallMove);
            }
        }
        else if(currentState == State.WallMoveExit)
        {
            FollowPath();

            _timeCounter.IncreaseTimer("wallMoveTime",out bool limit);
            if(limit)
            {
                shield.Reactive();

                if(Random.Range(0,2) == 0)
                    ChangeState(State.WallMove);
                else
                    ChangeState(State.FloorWhip);
            }
        }
        else if(currentState == State.Stun)
        {
            foreach(var body in bodys)
            {
                body.AddForce(Vector3.down * 2000f * Time.deltaTime,ForceMode.Acceleration);
            }

            _timeCounter.IncreaseTimer("stunTime",5,out bool limit);
            if(limit)
            {
                ChangeState(State.Recovery);
            }
        }
        else if(currentState == State.Recovery)
        {
            _timeCounter.IncreaseTimer("recoverTime",1f,out bool limit);
            if(limit)
            {
                ChangeState(State.WallMoveExit);
            }
        }

        
    }

    public void Stun()
    {
        foreach(var rig in bodys)
        {
            rig.isKinematic = false;
            rig.AddForce(transform.forward * 3000f,ForceMode.Acceleration);
            //rig.AddForce(transform.up * 1000f,ForceMode.Acceleration);
        }

        foreach(var rotator in head.allParts)
        {
            rotator.enabled = false;
        }

        foreach(var leg in legs)
        {
            leg.Hold(true);
        }

        head.enabled = false;
    }

    public void Recover()
    {
        foreach(var rig in bodys)
        {
            rig.isKinematic = true;
        }

        foreach(var rotator in head.allParts)
        {
            rotator.enabled = true;
        }

        foreach(var leg in legs)
        {
            leg.Hold(false);
        }

        head.enabled = true;
    }

    public void ChangeState(State state)
    {
        currentState = state;

        if(state == State.WallMove)
        {
            GetPath("WallMove");
            _timeCounter.InitTimer("wallMoveTime",0f,Random.Range(8f,20f));
            _pathLoop = true;
        }
        else if(state == State.FloorWhip)
        {
            GetPath("FloorWhip" + Random.Range(0,4));
            _timeCounter.InitTimer("whipTime",0f,Random.Range(5f,8f));
            _pathLoop = true;
        }
        else if(state == State.WallMoveExit)
        {
            GetPath("WallMoveExit");
            _timeCounter.InitTimer("wallMoveTime",0f,Random.Range(13f,20f));
            _pathLoop = true;
        }
        else if(state == State.Stun)
        {
            Stun();
            _timeCounter.InitTimer("stunTime");
        }
        else if(state == State.Recovery)
        {
            if(!GroundCheck(out var hit, 3f))
            {
                bodys[0].AddForce(Vector3.up * 1500f,ForceMode.Acceleration);
                bodys[0].AddTorque(new Vector3(0f,0f,1f) * 400f,ForceMode.Acceleration);
                currentState = State.Stun;
                _timeCounter.InitTimer("stunTime");
            }
            else
            {
                Recover();
                _timeCounter.InitTimer("recoverTime");
            }
            
        }
    }

    public bool FollowPath()
    {
        if(_pathArrived)
            return true;

        SetTarget(_targetTransform.position);
        Move(transform.forward,mainSpeed);
        if(IsArrivedTarget())
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
        //transform.position += transform.forward * _movementSpeed * Time.deltaTime;

        if(ForwardRayCheck(out var hit))
        {
            // var dir = hit.point - transform.position;
            // dir = Vector3.ProjectOnPlane(dir,transform.up).normalized;
            // var angle = Vector3.Angle(transform.forward,dir);

            //Turn(angle < 0,this.transform);
            Turn(false,this.transform);
            speed *= 0.5f;
        }
        else if(LeftRayCheck(out hit))
        {
            Turn(true,this.transform);
        }
        else if(RightRayCheck(out hit))
        {
            Turn(false,this.transform);
        }
        else
        {
            var angle = Vector3.SignedAngle(transform.forward,_targetDirection,transform.up);

            if(MathEx.abs(angle) > _turnAccuracy)
            {
                if(angle > 0)
                    Turn(true,this.transform);
                else
                    Turn(false,this.transform);
            }
        }


        transform.position += direction * speed * Time.deltaTime;
        

        return true;
    }

    public bool IsArrivedTarget()
    {
        return Vector3.Distance(transform.position,_targetTransform.position) <= distanceAccuracy;
    }

    public bool LeftRayCheck(out RaycastHit hit)
    {
        _sideRay.SetDirection(-transform.right);
        return _sideRay.Cast(transform.position + transform.right * 3f,out hit);
    }

    public bool RightRayCheck(out RaycastHit hit)
    {
        _sideRay.SetDirection(transform.right);
        return _sideRay.Cast(transform.position - transform.right * 3f,out hit);
    }

    public bool ForwardRayCheck(out RaycastHit hit)
    {
        _forwardRay.SetDirection(transform.forward);
        return _forwardRay.Cast(transform.position - transform.forward * 3f,out hit);
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

    public void GetPath(string path)
    {
        _pathArrived = false;
        _currentPath = controll.GetPath(path);
        SetNearestPointInArc(30f);
        //SetNearestPointToTarget();
    }
}
