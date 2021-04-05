using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImmortalJirungE_AI : IKPathFollowBossBase
{
    public enum State
    {
        WallMove,
        FloorWhip,
        WallMoveExit,
        Stun,
        Recovery
    };
    public State currentState;
    public BossHead head;
    public EMPShield shield;
    public int whipPath = 0;

    public LayerMask obstacleLayer;

    public List<Rigidbody> bodys = new List<Rigidbody>();

    public UnityEvent whenReactiveshield;
    public UnityEvent whenRecover;

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
        if (GameManager.Instance.PAUSE == true)
            return;


        if (Input.GetKeyDown(KeyCode.J))
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
            Move(transform.forward,mainSpeed);
            _targetDistance = Vector3.Distance(GameManager.Instance.player.transform.position,transform.position);
            if(_targetDistance >= 10f)
            {
                Debug.Log("re");
                SetTarget(GameManager.Instance.player.transform.position);
            }

            Collider[] playerColl = Physics.OverlapSphere(transform.position, 2.5f,targetLayer);

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
                whenReactiveshield?.Invoke();

                if (Random.Range(0,2) == 0)
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

        whenRecover?.Invoke();
    }

    public void ChangeState(State state)
    {
        currentState = state;

        if(state == State.WallMove)
        {
            GetPath("WallMove");
            _timeCounter.InitTimer("wallMoveTime",0f,Random.Range(5f,12f));
            _pathLoop = true;
        }
        else if(state == State.FloorWhip)
        {
            if(GameManager.Instance.player.transform.position.y >= 10f)
            {
                GetPath("FloorWhip" + whipPath);
            }
            else
                SetTarget(GameManager.Instance.player.transform.position);
                
            _timeCounter.InitTimer("whipTime",0f,Random.Range(8f,12f));
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

}
