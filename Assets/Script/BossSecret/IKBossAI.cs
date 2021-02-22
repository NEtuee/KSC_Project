using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBossAI : MonoBehaviour
{
    public enum Phase
    {
        ZERO,
        ONE,
        TWO,
        THREE,
    }

    public LevelEdit_Controll controll;
    public BossHead bossHead;
    public Transform player;
    public LayerMask wallLayer;
    public Phase currentPhase = Phase.ONE;

    public float distanceAccuracy = 10f;

    public Vector3 slowSpeed;
    public Vector3 fastSpeed;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private LevelEdit_PointManager.PathClass _currentPath;
    private SphereRayEx _frontSideCheck;

    private Transform _targetTransform;
    private int _targetPoint;
    private int _currentPoint = 0;

    private float _movementSpeed;
    private float _turnAccuracy = 3f;
    private float _turnAngle;

    private bool _pathLoop = true;
    private bool _pathArrived = false;
    private bool _rushToPlayer = false;
    private bool _isStunned = false;
    private bool _fastRun = false;

    private Vector3 _targetDirection;
    private Vector3 _rushStartPoint;

    public void Start()
    {
        PhaseInitialize();
        _timeCounter.InitTimer("rushReady",0f);
        _timeCounter.InitTimer("stun",0f);
        _timeCounter.InitTimer("randomTimer",0f);

        _frontSideCheck = new SphereRayEx(new Ray(Vector3.zero,Vector3.zero),1f,3f,wallLayer);
    }

    public void Update()
    {
        PhaseProgress();

        if(Input.GetKeyDown(KeyCode.T))
        {
            ChangePhase(Phase.ONE);
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            ChangePhase(Phase.TWO);
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            ChangePhase(Phase.THREE);
        }
    }


    public void ChangePhase(Phase phase)
    {
        currentPhase = phase;
        PhaseInitialize();
    }

    public void PhaseProgress()
    {
        if(currentPhase == Phase.ONE)
        {
            Phase_One_Progress();
        }
        else if(currentPhase == Phase.TWO)
        {
            Phase_Two_Progress();
        }
        else if(currentPhase == Phase.THREE)
        {
            Phase_Three_Progress();
        }
        else if(currentPhase == Phase.ZERO)
        {
            Phase_Zero_Progress();
        }
    }

    public void PhaseInitialize()
    {
        if(currentPhase == Phase.ONE)
        {
            Phase_One_Init();
        }
        else if(currentPhase == Phase.TWO)
        {
            Phase_Two_Init();
        }
        else if(currentPhase == Phase.THREE)
        {
            Phase_Three_Init();
        }
        else if(currentPhase == Phase.ZERO)
        {
            Phase_Zero_Init();
        }
    }

    public void Phase_Zero_Init()
    {
        _pathLoop = false;
        _pathArrived = false;

        SetSlowMovement();
        GetPath("PhaseThree");
    }

    public void Phase_Zero_Progress()
    {
        FollowPath();
    }

    public void Phase_Three_Init()
    {
        _pathLoop = false;
        _pathArrived = false;

        SetSlowMovement();
        GetPath("PhaseThree");
    }

    public void Phase_Three_Progress()
    {
        FollowPath();
    }

    public void Phase_Two_Init()
    {
        _pathLoop = true;

        SetSlowMovement();
        GetPath("PhaseTwo_0");
        _timeCounter.InitTimer("randomTimer",Random.Range(3f,15f));
    }

    public void Phase_Two_Progress()
    {
        FollowPath();
        _timeCounter.DecreaseTimer("randomTimer",0f,out bool limit);

        if(limit)
        {
            _timeCounter.InitTimer("randomTimer",Random.Range(8f,17f));
            if(_currentPoint == 2)
                _fastRun = !_fastRun;
            _currentPoint = _currentPoint == 2 ? (_fastRun ? 1 : 0) : 2;
            

            GetPath("PhaseTwo_" + _currentPoint.ToString());

            if(_currentPoint == 1)
            {
                SetFastMovement();
            }
            else
            {
                SetSlowMovement();
            }
        }
    }

    public void Phase_One_Init()
    {
        _pathLoop = true;
        _pathArrived = false;

        SetSlowMovement();
        GetPath("PhaseOne");
    }

    public void Phase_One_Progress()
    {
        if(_isStunned)
        {
            _timeCounter.IncreaseTimer("stun",15f,out bool end);
            if(end)
            {
                _isStunned = false;
            }
            return;
        }

        if(_rushToPlayer)
        {
            Rush();
            WallCheck();
        }
        else
        {
            FollowPath();
            RushCheck();
        }
        
    }

    public void WallCheck()
    {
        _frontSideCheck.SetDirection(transform.forward);
        if(_frontSideCheck.Cast(transform.position,out RaycastHit hit))
        {
            _isStunned = true;
            _timeCounter.InitTimer("stun");
            bossHead.StartTwist();
            RushEnd();
            bossHead.SetHeightInOrder(3f);
        }
    }

    public void RushCheck()
    {
        _timeCounter.IncreaseTimer("rushTimer",1f,out bool check);
        if(check)
        {
            float dist = Vector3.Distance(player.transform.position,transform.position);
            if(dist >= 90f)
            {
                InitRush();
            }
            
        }
    }

    public void RushEnd()
    {
        _rushToPlayer = false;

        bossHead.SetHeightInOrder(6f);

        //SetNearestPointToTarget();
        SetNearestPointInArc(50f);
        SetSlowMovement();
    }

    public void InitRush()
    {
        _rushStartPoint = transform.position;
        _rushToPlayer = true;
        _timeCounter.InitTimer("rushTimer");
        _targetTransform = player;

        bossHead.SetHeightInOrder(3f);

        SetFastMovement();
    }

    public void Rush()
    {
        SetTarget(_targetTransform.position);
        float moveDist = Vector3.Distance(_rushStartPoint,transform.position);
        bool turn = false;

        if(moveDist <= 70f)
        {
            turn = true;
        }
        else if(moveDist >= 120f)
        {
            RushEnd();
        }

        Move(turn);

    }

    public bool FollowPath()
    {
        if(_pathArrived)
            return true;

        SetTarget(_targetTransform.position);
        Move(true);
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
            Debug.Log("Fucn");
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
        _currentPath = controll.GetPath(path);
        SetNearestPointInArc(50f);
        //SetNearestPointToTarget();
    }

    public bool IsArrivedTarget()
    {
        return Vector3.Distance(transform.position,_targetTransform.position) <= distanceAccuracy;
    }

    public LevelEdit_MovePoint GetNextPoint(out bool isEnd)
    {
        return _currentPath.GetNextPoint(ref _targetPoint, out isEnd);
    }

    public void Move(bool turn)
    {
        transform.position += transform.forward * _movementSpeed * Time.deltaTime;

        var angle = Vector3.SignedAngle(transform.forward,_targetDirection,transform.up);

        if(MathEx.abs(angle) > _turnAccuracy && turn)
        {
            if(angle > 0)
                Turn(true);
            else
                Turn(false);
        }
    }

    public void Turn(bool isLeft)
    {
        transform.RotateAround(transform.position,transform.up,_turnAngle * Time.deltaTime * (isLeft ? 1f : -1f));
    }

    public float GetTargetDistance()
    {
        return Vector3.Distance(transform.position,_targetTransform.position);
    }

    public void SetTarget(Vector3 target)
    {
        var direction = target - transform.position;
        _targetDirection = Vector3.ProjectOnPlane(direction,transform.up).normalized;
    }

    public void SetFastMovement()
    {
        SetMovementSpeed(fastSpeed.x);
        SetTurnAngle(fastSpeed.y);
        SetTurnAccuracy(fastSpeed.z);
    }

    public void SetSlowMovement()
    {
        SetMovementSpeed(slowSpeed.x);
        SetTurnAngle(slowSpeed.y);
        SetTurnAccuracy(fastSpeed.z);
    }

    public void SetMovementSpeed(float speed) {_movementSpeed = speed;}
    public void SetTurnAngle(float speed) {_turnAngle = speed;}
    public void SetTurnAccuracy(float accur) {_turnAccuracy = accur;}
}
