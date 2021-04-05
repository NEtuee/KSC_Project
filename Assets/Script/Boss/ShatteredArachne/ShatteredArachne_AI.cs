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
        Dead,
        FlipDown,
        FlipReady,
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


    private State _prevState;

    private EMPShield _targetBomb;


    private Vector3 _flipOrigin;
    private Vector3 _flipTarget;
    private Quaternion _flipOriginRotation;
    private Quaternion _flipTargetRotation;
    private float _bombPickSpeed;

    public void Start()
    {
        ChangeState(State.FlipReady);
        _timeCounter.InitTimer("PickTime",0f);
        _timeCounter.InitTimer("MoveTime",0f);
        _timeCounter.InitTimer("ThrowTime",0f);
        _timeCounter.InitTimer("FallTime",0f,5f);
        _timeCounter.InitTimer("FlipFallTime",0f);
        _timeCounter.InitTimer("PickMoveTime",0f,Random.Range(5f,15f));
        _timeCounter.InitTimer("Hit",0f,3);
        _timeCounter.InitTimer("IdleTime",0f);
    }

    void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

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
            Push(10f,250f);

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
                _bombPickSpeed = dist - 3f;
                if(dist <= 7f)
                {
                    _bombPickSpeed = 0f;
                    ChangeState(State.PickBomb);
                }
            }
            else
            {
                Push(10f,250f);
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

                pos.y = Mathf.Lerp(pos.y, targetPos.y,0.14f);
                pos.x = Mathf.Lerp(pos.x, targetPos.x,0.02f);
                pos.z = Mathf.Lerp(pos.z, targetPos.z,0.02f);
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
            Push(10f,250f);
            
            if(_targetBomb == null || _targetBomb.isOver)
            {
                Debug.Log("Check");
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
                rotator.enabled = true;

                if((_prevState == State.PickBomb || _prevState == State.PickBombMove) && _targetBomb != null)
                {
                    ChangeState(State.PickBomb);
                }
                else if(_targetBomb != null && !_targetBomb.isOver)
                {
                    ChangeState(State.GoUp);
                }
                else
                    ChangeState(State.Move);
            }
        }
        else if(currentState == State.Fall)
        {
            rig.AddForce(Vector3.down * 200f * Time.deltaTime,ForceMode.Acceleration);
            _timeCounter.IncreaseTimer("FallTime",out var limit);
            if(limit)
            {
                ChangeState(State.Flip);
            }
        }
        else if(currentState == State.Flip)
        {
            transform.RotateAround(transform.position,transform.forward,270f * Time.deltaTime);
            if(GroundCheck(out var hit,10f))
            {
                rotator.enabled = true;
                rotator.IKUnHold();
                rig.isKinematic = true;

                animator.Play("Recover",body);
                ChangeState(State.Idle);
            }
            
        }
        else if(currentState == State.FlipDown)
        {
            var time = _timeCounter.IncreaseTimer("FlipFallTime",out var limit);
            if(limit)
            {
                rotator.enabled = true;
                rotator.IKUnHold();
                animator.Play("Landing",body);

                Push(18f,250f);
                ChangeState(State.Idle);
            }

            transform.position = Vector3.Lerp(_flipOrigin,_flipTarget,time * .5f);
            transform.rotation = Quaternion.Lerp(_flipOriginRotation,_flipTargetRotation,time * .5f);
        }
        else if(currentState == State.FlipReady)
        {
            _timeCounter.IncreaseTimer("FlipReady",out var limit);
            if(limit)
            {
                ChangeState(State.FlipDown);
            }
        }
    }




    public void ChangeState(State state)
    {
        _prevState = currentState;
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
                return;
            }
            else
            {
                _targetTransform = _targetBomb.transform;
                _bombPickSpeed = mainSpeed;
            }

        }
        else if(state == State.PickBomb)
        {
            _timeCounter.InitTimer("PickTime",0f,0.5f);
            _timeCounter.InitTimer("MoveTime",0f,0.5f);
            _targetBomb.GetComponent<Rigidbody>().isKinematic = true;
            _targetBomb.transform.SetParent(transform);
        }
        else if(state == State.GoUp)
        {
            var up = "GoUp_" + Random.Range(0,3);
            GetPath(up,false);
            _pathLoop = false;
        }
        else if(state == State.ThrowBomb)
        {
            _timeCounter.InitTimer("ThrowTime",0f,2f);
        }
        else if(state == State.Hit)
        {
            rotator.enabled = false;
            _timeCounter.InitTimer("Hit",0f,3);
        }
        else if(state == State.Fall)
        {
            rotator.enabled = false;
            rig.isKinematic = false;

            _timeCounter.InitTimer("FallTime",0f,10f);
        }
        else if(state == State.Flip)
        {
            
            //rig.AddForce(Vector3.up * 4000f,ForceMode.Impulse);
        }
        else if(state == State.Dead)
        {
            Dead();
        }
        else if(state == State.FlipDown)
        {
            if(DownCheck())
            {
                rotator.IKHold();
                rotator.enabled = false;
            }
            else
            {
                ChangeState(State.Idle);
                return;
            }
            
        }
        else if(state == State.FlipReady)
        {
            _timeCounter.InitTimer("FlipReady",0f,10f);
        }

        currentState = state;
    }

    public void Push(float raidus, float power)
    {
        Collider[] playerColl = Physics.OverlapSphere(transform.position, raidus,targetLayer);


        if(playerColl.Length != 0)
        {
            foreach(Collider curr in playerColl)
            {
                PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
                if(ragdoll != null)
                {
                    ragdoll.ExplosionRagdoll(power, 
                        (Vector3.ProjectOnPlane(ragdoll.transform.position - transform.position,Vector3.up).normalized));
                }
            }
        }
    }
    
    public bool DownCheck()
    {
        Ray ray = new Ray(transform.position,transform.up);
        if(Physics.Raycast(ray,out var hit,100f,groundLayer))
        {
            _flipOrigin = transform.position;
            _flipOriginRotation = transform.rotation;
            _flipTarget = hit.point + hit.normal;
            _flipTargetRotation = transform.rotation * Quaternion.Euler(0f,0f,180f);
            _timeCounter.InitTimer("FlipFallTime",0f,2f);

            return true;
        }

        Debug.Log("????");
        return false;
    }

    public void Dead()
    {
        foreach(var leg in rotator.legs)
        {
            var legRig = leg.gameObject.AddComponent<Rigidbody>();
            var dir = transform.position - leg.transform.position;
            leg.transform.SetParent(null);
            leg.enabled = false;
            legRig.AddForce(dir * 10f,ForceMode.Impulse);
        }

        rig.isKinematic = false;
        rig.AddForce(Vector3.up * 4000f,ForceMode.Impulse);

        rotator.enabled = false;
        this.enabled = false;
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
