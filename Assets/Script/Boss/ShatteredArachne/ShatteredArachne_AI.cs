using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteredArachne_AI : IKPathFollowBossBase
{
    public enum State
    {
        Idle,
        Move,
        PickBombMove,
        PickBomb,
        GoUp,
        ThrowBomb,
        Hit,
        Fall,
        Flip,
    };

    public State currentState;
    public BombCollector bombCollector;
    public LayerMask bombLayer;
    public GraphAnimator animator;
    
    public Transform bombPoint;
    public Transform body;

    public IKFootPointRotator rotator;
    public Rigidbody rig;
    
    public List<Transform> handIKList = new List<Transform>();
    public List<Transform> handPosOrigin = new List<Transform>();
    public List<Transform> handFollowTarget = new List<Transform>();


    private EMPShield _targetBomb;

    private float _bombPickSpeed;

    public void Start()
    {
        ChangeState(State.Move);
        _timeCounter.InitTimer("PickTime",0f);
        _timeCounter.InitTimer("MoveTime",0f);
        _timeCounter.InitTimer("ThrowTime",0f);
        _timeCounter.InitTimer("PickMoveTime",0f,Random.Range(5f,15f));
        _timeCounter.InitTimer("Hit",0f,3);
        _timeCounter.InitTimer("IdleTime",0f);
    }

    void Update()
    {
        UpdateHandIK();

        if(currentState == State.Idle)
        {
            _timeCounter.IncreaseTimer("IdleTime",out var limit);
            if(limit)
            {
                ChangeState(State.Move);
            }
        }
        else if(currentState == State.Move)
        {
            FollowPath();

            _timeCounter.IncreaseTimer("PickMoveTime",out var limit);
            if(limit)
            {
                ChangeState(State.PickBombMove);
            }
        }
        else if(currentState == State.PickBombMove)
        {
            if(_targetBomb == null || _targetBomb.isOver)
            {
                _targetBomb = bombCollector.FindFarestBomb(transform.position);

                if(_targetBomb == null)
                {
                    ChangeState(State.Move);
                    return;
                }
            }

            var dist = Vector3.Distance(transform.position,_targetTransform.position);

            if(dist <= 6f + mainSpeed)
            {
                _bombPickSpeed = dist - 6f;;
                if(_bombPickSpeed <= 0.3f)
                {
                    _bombPickSpeed = 0f;
                    ChangeState(State.PickBomb);
                }
            }

            SetTarget(_targetTransform.position);
            Move(transform.forward,_bombPickSpeed);
        }
        else if(currentState == State.PickBomb)
        {
            if(_targetBomb == null || _targetBomb.isOver)
            {
                ChangeState(State.PickBombMove);
            }

            SetFollowTargetToTarget(_targetBomb.transform.position,1f);
            _timeCounter.IncreaseTimer("PickTime",out var limit);
            if(limit)
            {
                var pos = _targetBomb.transform.position;
                var targetPos = bombPoint.position;

                pos.y = Mathf.Lerp(pos.y, targetPos.y,0.07f);
                pos.x = Mathf.Lerp(pos.x, targetPos.x,0.01f);
                pos.z = Mathf.Lerp(pos.z, targetPos.z,0.01f);
                _targetBomb.transform.position = pos;

                _timeCounter.IncreaseTimer("MoveTime",out limit);
                if(limit)
                {
                    ChangeState(State.GoUp);
                }
            }

            
        }
        else if(currentState == State.GoUp)
        {
            if(_targetBomb == null || _targetBomb.isOver)
            {
                ChangeState(State.Move);
            }

            FollowPath();
            if(_pathArrived)
            {
                ChangeState(State.ThrowBomb);
            }
        }
        else if(currentState == State.ThrowBomb)
        {
            _timeCounter.IncreaseTimer("ThrowTime",out bool limit);
            if(limit)
            {
                SetFollowTargetToOrigin();
                _targetBomb.transform.SetParent(null);
                _targetBomb.GetComponent<Rigidbody>().isKinematic = false;

                ChangeState(State.Move);
            }
        }
        else if(currentState == State.Hit)
        {
            _timeCounter.IncreaseTimer("Hit",out var limit);
            if(limit)
            {
                ChangeState(State.Move);
            }
        }
        else if(currentState == State.Fall)
        {
            rig.AddForce(Vector3.down * 200f * Time.deltaTime,ForceMode.Acceleration);
        }
        else if(currentState == State.Flip)
        {
            transform.RotateAround(transform.position,transform.forward,180f * Time.deltaTime);
            if(GroundCheck(out var hit,10f))
            {
                rotator.enabled = true;
                rotator.IKUnHold();
                rig.isKinematic = true;

                ChangeState(State.Idle);
            }
            
        }
    }

    public void ChangeState(State state)
    {
        if(state == State.Idle)
        {
            _timeCounter.InitTimer("IdleTime",0f,3f);
        }
        else if(state == State.Move)
        {
            _timeCounter.InitTimer("PickMoveTime",0f,Random.Range(5f,15f));
            GetPath("GoEverywhere",false);
            SetFollowTargetToOrigin();

            _pathLoop = true;
        }
        else if(state == State.PickBombMove)
        {
            _targetBomb = bombCollector.FindFarestBomb(transform.position);

            if(_targetBomb == null || _targetBomb.isOver)
            {
                ChangeState(State.Move);
            }
            else
            {
                _targetTransform = _targetBomb.transform;
                _bombPickSpeed = mainSpeed;
            }

        }
        else if(state == State.PickBomb)
        {
            _timeCounter.InitTimer("PickTime",0f);
            _timeCounter.InitTimer("MoveTime",0f);
            _targetBomb.GetComponent<Rigidbody>().isKinematic = true;
            _targetBomb.transform.SetParent(transform);
        }
        else if(state == State.GoUp)
        {
            GetPath("GoUp_" + Random.Range(0,3),false);
            _pathLoop = false;
        }
        else if(state == State.ThrowBomb)
        {
            _timeCounter.InitTimer("ThrowTime",0f,3f);
        }
        else if(state == State.Hit)
        {
            _timeCounter.InitTimer("Hit",0f,3);
        }
        else if(state == State.Fall)
        {
            rotator.enabled = false;
            rig.isKinematic = false;
        }
        else if(state == State.Flip)
        {
            //rig.AddForce(Vector3.up * 4000f,ForceMode.Impulse);
        }

        currentState = state;
    }

    public void Dead()
    {

    }

    public void ShieldHit()
    {
        ChangeState(State.Flip);
    }

    public void Hit()
    {
        if(CeilingCheck(30f))
        {
            Debug.Log("?");
            rotator.IKHold();
            ChangeState(State.Fall);
        }
        else
        {
            animator.Play("Hit",body);
            animator.Play("HitRotate",body);
            ChangeState(State.Hit);
        }

    }

    public bool CeilingCheck(float angle)
    {
        GroundCheck(out var hit,10f);
        return Vector3.Angle(hit.normal,Vector3.down) < angle;
    }


    public void SetFollowTargetToTarget(Vector3 target,float dist)
    {
        for(int i = 0; i < handIKList.Count; ++i)
        {
            var dir = (handIKList[i].position - target).normalized;
            var pos = target + dir * dist;
            // Ray ray = new Ray(handIKList[i].position,dir);

            // if(Physics.Raycast(ray,out var hit,10f,bombLayer))
            // {
                handFollowTarget[i].position = pos;
                //handFollowTarget[i].SetParent(hit.transform);
            //}
        }
    }

    public void UpdateHandIK()
    {
        for(int i = 0; i < handIKList.Count; ++i)
        {
            handIKList[i].position = Vector3.Lerp(handIKList[i].position, handFollowTarget[i].position,0.1f);
        }
    }

    public void SetFollowTargetToOrigin()
    {
        for(int i = 0; i < handFollowTarget.Count; ++i)
        {
            handFollowTarget[i].position = handPosOrigin[i].position;
        }
    }

    
}
