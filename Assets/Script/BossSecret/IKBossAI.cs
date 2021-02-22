using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBossAI : MonoBehaviour
{
    public enum Phase
    {
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

    private float _movementSpeed;
    private float _turnAccuracy = 3f;
    private float _turnAngle;

    private bool _pathLoop = true;
    private bool _rushToPlayer = false;
    private bool _isStunned = false;

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
            ChangePhase(Phase.TWO);
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
    }

    public void Phase_Three_Init()
    {
        _pathLoop = false;

        SetFastMovement();
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
            int path = MathEx.RandomInt(0,2);
            GetPath("PhaseTwo_" + path.ToString());

            if(path == 1)
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

        SetNearestPointToTarget();
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
        SetTarget(_targetTransform.position);
        Move(true);
        if(IsArrivedTarget())
        {
            var target = GetNextPoint(out bool isEnd).transform;
            _targetTransform = target;

            return isEnd;
        }

        return false;
    }

    public void SetNearestPointToTarget()
    {
        _targetTransform = _currentPath.FindNearestPoint(transform.position,out _targetPoint).transform;
    }

    public void GetPath(string path)
    {
        _currentPath = controll.GetPath(path);
        SetNearestPointToTarget();
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
