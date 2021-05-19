using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImmortalJirungE_V2_AI : IKPathFollowBossBase
{
    public enum State
    {
        LaunchReady,
        LaunchReadyTimer,
        Launch,
        TransformOpen,
        TransformClose,
        TransformFoldLeg,
        TransformSpreadLeg,
        WallMove,
        FloorWhip,
        WallMoveExit,
        Hit,
        Stun,
        Recovery
    };

    public enum Whip
    {
        Sphere,
        Capsule
    }

    public State currentState;
    public Whip whipState;
    public BossHead head;
    public EMPShield shield;
    public Core core;
    public LevelEdit_PlayerShockEvent shockEvent;
    
    public Transform rollSphere;
    public Animator animatorControll;

    public Vector3 lastPosition;

    public int whipPath = 0;
    public bool canFloorWhip = false;
    public bool lowCheck = false;
    public bool isDead = false;

    public float whipSpeed = 12f;
    public float rollSpeed = 15f;

    public LayerMask obstacleLayer;

    public List<Rigidbody> bodys = new List<Rigidbody>();

    public UnityEvent whenReactiveshield;
    public UnityEvent whenRecover;
    public UnityEvent whenLaunch;

    private SphereRayEx _forwardRay;
    private SphereRayEx _sideRay;

    private State _nextState;
    private State _prevState;

    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;

    private bool _roll = false;
    private bool _shieldBroke = false;
    private bool _launched = false;

    private float _rolledAngle = 0f;

    public void Start()
    {
        _spawnPosition = transform.position;
        _spawnRotation = transform.rotation;
        
        ChangeState(currentState);
        _pathLoop = true;

        _forwardRay = new SphereRayEx(new Ray(Vector3.zero,Vector3.zero),11f,8f,obstacleLayer);
        _sideRay = new SphereRayEx(new Ray(Vector3.zero,Vector3.zero),8f,8f,obstacleLayer);

        GetSoundManager();
        SetLegHitGroundSound(1509);

        //ChangeState(State.TransformClose);
    }

    public void Update()
    {
        if (GameManager.Instance.PAUSE == true || GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Update)
            return;
        
        UpdateProcess(Time.deltaTime);
    }

    public void FixedUpdate()
    {
        if (GameManager.Instance.PAUSE == true || GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Fixed)
            return;
        
        UpdateProcess(Time.fixedDeltaTime);
    }

    public void UpdateProcess(float deltaTime)
    {
        shockEvent.progress = !_shieldBroke;
        if(currentState == State.WallMove)
        {
            FollowPath(deltaTime);

            _timeCounter.IncreaseTimer("wallMoveTime",out bool limit);
            if(limit)
            {
                canFloorWhip = true;
                //ChangeState(State.FloorWhip);
            }
        }
        else if(currentState == State.FloorWhip)
        {
            if(lowCheck)
            {
                var dir = whipState == Whip.Sphere ? transform.forward : transform.right;
                dir = _roll ? dir : transform.forward;

                var speed = _roll ? rollSpeed : whipSpeed;
                Move(dir,speed,deltaTime);
                _targetDistance = Vector3.Distance(GameManager.Instance.player.transform.position,transform.position);
                if(_targetDistance >= 20f)
                {
                    SetTarget(GameManager.Instance.player.transform.position);
                }
            }   
            else
            {
                FollowPath(deltaTime);
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
                if (!_roll)
                {
                    canFloorWhip = false;
                    ChangeState(State.WallMove);
                    SetSphereRotationIdentity();
                }
                else if(SphereUpCheck(_rolledAngle))
                {
                    canFloorWhip = false;
                    ChangeState(State.WallMove);
                    SetSphereRotationIdentity();
                }
                
            }
        }
        else if(currentState == State.WallMoveExit)
        {
            FollowPath(deltaTime);

            _timeCounter.IncreaseTimer("wallMoveTime",out bool limit);
            if(limit)
            {
                shield.Reactive();
                _shieldBroke = false;
                whenReactiveshield?.Invoke();

//                if (Random.Range(0,2) == 0)
                    ChangeState(State.WallMove);
                // else
                //     ChangeState(State.FloorWhip);
            }
        }
        else if(currentState == State.Stun)
        {
            // foreach(var body in bodys)
            // {
            //     body.AddForce(Vector3.down * (2000f * Time.deltaTime),ForceMode.Acceleration);
            // }

            _timeCounter.IncreaseTimer("stunTime",5,out bool limit);
            if(limit)
            {
                if(transform.up.y <= 0.3f)
                {
                    bodys[0].AddTorque(-transform.eulerAngles * deltaTime * 10f,ForceMode.Acceleration);
                }
                else
                {
                    ChangeState(State.Recovery);
                }
            }
        }
        else if(currentState == State.Recovery)
        {
            // _timeCounter.IncreaseTimer("recoverTime",1f,out bool limit);
            // if(limit)
            // {
                _nextState = _launched ? State.WallMoveExit : State.WallMove;
                _launched = true;
                ChangeState(State.TransformOpen);
            //}
        }
        else if(currentState == State.TransformOpen)
        {
            _timeCounter.IncreaseTimer("TransformTime",out bool limit);
            if(limit)
            {
                shield.gameObject.SetActive(true);
                core.gameObject.SetActive(true);
                ChangeState(_nextState);
            }
        }
        else if(currentState == State.TransformClose)
        {
            _timeCounter.IncreaseTimer("TransformTime",out bool limit);
            //if()
            {
                ChangeState(_nextState);
            }
        }
        else if(currentState == State.TransformFoldLeg)
        {
            _timeCounter.IncreaseTimer("TransformTime",out bool limit);
            //if()
            {
                ChangeState(_nextState);
            }
        }
        else if(currentState == State.TransformSpreadLeg)
        {
            _timeCounter.IncreaseTimer("TransformTime",out bool limit);
            if(limit)
            {
                shield.gameObject.SetActive(true);
                core.gameObject.SetActive(true);
                ChangeState(_nextState);
            }
        }
        else if(currentState == State.Launch)
        {
            _timeCounter.IncreaseTimer("stunTime",out bool limit);
            if(limit)
            {
                ChangeState(State.Recovery);
            }
        }
        else if (currentState == State.LaunchReadyTimer)
        {
            _timeCounter.IncreaseTimer("launchTime",out bool limit);
            if(limit)
            {
                ChangeState(State.Launch);
            }
        }
        else if (currentState == State.Hit)
        {
            _timeCounter.IncreaseTimer("hitTime",out bool limit);
            if(limit)
            {
                ChangeState(_prevState);
            }
        }

    }

    
    
    public void SetShieldBroke()
    {
        _shieldBroke = true;
    }

    public void HoldLegs()
    {
        foreach(var leg in legs)
        {
            leg.Hold(true,false);
        }
    }

    public void UnHoldLegs()
    {
        foreach(var leg in legs)
        {
            leg.Hold(false,false);
        }
    }

    public void Hit()
    {
        ChangeState(State.Hit);
    }
    
    public void Dead()
    {
        isDead = true;
        lastPosition = transform.position;
        transform.SetPositionAndRotation(_spawnPosition,_spawnRotation);
        rollSphere.localRotation = Quaternion.identity;
        ChangeState(State.LaunchReadyTimer);
    }
    

    public void Stun()
    {
        foreach(var rig in bodys)
        {
            rig.isKinematic = false;
            rig.AddForce(transform.forward * 100f,ForceMode.Acceleration);
            rig.AddForce(transform.up * 1500f,ForceMode.Acceleration);
            bodys[0].AddTorque(MathEx.RandomCircle(1f) * 400f,ForceMode.Acceleration);
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
        _prevState = currentState;
        currentState = state;

        if (state == State.Hit)
        {
            if (_prevState == State.TransformClose || _prevState == State.TransformOpen)
            {
                currentState = _prevState;
                return;
            }
            
            _timeCounter.InitTimer("hitTime", 0f, 2f);
        }
        else if(state == State.LaunchReadyTimer)
        {
            animatorControll.SetInteger("AnimationCode",2);
            animatorControll.SetTrigger("ChangeAnimation");

            _timeCounter.InitTimer("launchTime", 0f, 15f);
        }
        else if(state == State.LaunchReady)
        {
            animatorControll.SetInteger("AnimationCode",2);
            animatorControll.SetTrigger("ChangeAnimation");
        }
        else if(state == State.Launch)
        {
            shield.Reactive();
            core.Reactive();
            
            _timeCounter.InitTimer("stunTime",0f,6f);
            foreach(var rig in bodys)
            {
                rig.isKinematic = false;
                rig.AddForce(MathEx.DeleteYPos(transform.position).normalized * 700f,ForceMode.Acceleration);
                bodys[0].AddTorque(MathEx.RandomCircle(1f) * 400f,ForceMode.Acceleration);
            }
    
            foreach(var rotator in head.allParts)
            {
                rotator.enabled = false;
            }
    
            foreach(var leg in legs)
            {
                leg.Hold(true);
            }
            
            whenLaunch.Invoke();
    
            head.enabled = false;
            _launched = false;
            _shieldBroke = false;
            isDead = false;
            _roll = false;

            shield.gameObject.SetActive(false);
            core.gameObject.SetActive(false);

        }
        else if(state == State.WallMove)
        {
            if(_shieldBroke && _roll)
            {
                _nextState = State.WallMove;
                if(whipState == Whip.Sphere)
                    ChangeState(State.TransformOpen);
                else
                    ChangeState(State.TransformSpreadLeg);
                return;
            }

            GetPath("WallMove");
            _timeCounter.InitTimer("wallMoveTime",0f,Random.Range(5f,12f));
            _pathLoop = true;
        }
        else if(state == State.FloorWhip)
        {
            if(_shieldBroke && !_roll)
            {
                _nextState = State.FloorWhip;

                if(whipState == Whip.Sphere)
                {
                    ChangeState(State.TransformClose);
                }
                else if(whipState == Whip.Capsule)
                {
                    ChangeState(State.TransformFoldLeg);
                }
                return;
            }

            if(GameManager.Instance.player.transform.position.y >= 10f)
            {
                GetPath("FloorWhip" + whipPath);
                lowCheck = false;
            }
            else
            {
                SetTarget(GameManager.Instance.player.transform.position);
                lowCheck = true;
            }
                
            _timeCounter.InitTimer("whipTime",0f,Random.Range(8f,12f));
            _pathLoop = true;
        }
        else if(state == State.WallMoveExit)
        {
            GetPath("WallMoveExit");
            _timeCounter.InitTimer("wallMoveTime",0f,Random.Range(21f,28f));
            _pathLoop = true;
        }
        else if(state == State.Stun)
        {
            if(_nextState != State.Stun)
            {   
                Stun();
                _timeCounter.InitTimer("stunTime");
                _nextState = State.Stun;
                ChangeState(State.TransformClose);
            }
            else
            {
                
            }
            
            _roll = false;
            // if(!_roll)
            // {
                
            // }
        }
        else if(state == State.Recovery)
        {
            if(transform.up.y <= 0.3f)
            {
                var rand = MathEx.RandomCircle(1f);
                rand.z = 0f;
                rand.Normalize();
                bodys[0].AddTorque(rand * 400f,ForceMode.Acceleration);
                currentState = State.Stun;
                _timeCounter.InitTimer("stunTime");
                return;
            }
            // if(!GroundCheck(out var hit, 3f))
            // {
            //     // bodys[0].AddForce(Vector3.up * 1500f,ForceMode.Acceleration);
            //     // bodys[0].AddTorque(new Vector3(0f,0f,1f) * 400f,ForceMode.Acceleration);
            //     currentState = State.Stun;
            //     _timeCounter.InitTimer("stunTime");
            // }
            // else
            // {
                 Recover();
            //     _timeCounter.InitTimer("recoverTime");
            // }
            
        }
        else if(state == State.TransformOpen)
        {
            _timeCounter.InitTimer("TransformTime",0f,4f);
            animatorControll.SetInteger("AnimationCode",0);
            animatorControll.SetTrigger("ChangeAnimation");

            _roll = false;
        }
        else if(state == State.TransformClose)
        {
            _timeCounter.InitTimer("TransformTime",0f,4f);
            animatorControll.SetInteger("AnimationCode",1);
            animatorControll.SetTrigger("ChangeAnimation");

            shield.gameObject.SetActive(false);
            core.gameObject.SetActive(false);
    
            _roll = true;
        }
        else if(state == State.TransformFoldLeg)
        {
            _timeCounter.InitTimer("TransformTime",0f,4f);
            animatorControll.SetInteger("AnimationCode",3);
            animatorControll.SetTrigger("ChangeAnimation");

            shield.gameObject.SetActive(false);
            //core.gameObject.SetActive(false);
    
            _roll = true;
        }
        else if(state == State.TransformSpreadLeg)
        {
            _timeCounter.InitTimer("TransformTime",0f,4f);
            animatorControll.SetInteger("AnimationCode",4);
            animatorControll.SetTrigger("ChangeAnimation");
    
            _roll = false;
        }
    }

    public override bool Move(Vector3 direction, float speed, float deltaTime,float legMovementSpeed = 4f)
    {
        //transform.position += transform.forward * _movementSpeed * Time.deltaTime;

        var dir = direction;
        
        //var dir = whipState == Whip.Sphere ? transform.forward : transform.right;
        //dir = _roll ? dir : transform.forward;

        if(ForwardRayCheck(out var hit))
        {
            // var dir = hit.point - transform.position;
            // dir = Vector3.ProjectOnPlane(dir,transform.up).normalized;
            // var angle = Vector3.Angle(transform.forward,dir);

            //Turn(angle < 0,this.transform);
            Turn(false,this.transform, deltaTime);
            speed *= 0.5f;
        }
        else if(LeftRayCheck(out hit))
        {
            Turn(true,this.transform,deltaTime);
        }
        else if(RightRayCheck(out hit))
        {
            Turn(false,this.transform,deltaTime);
        }
        else
        {
            var angle = Vector3.SignedAngle(dir,_targetDirection,transform.up);

            if(MathEx.abs(angle) > _turnAccuracy)
            {
                if(angle > 0)
                    Turn(true,this.transform,deltaTime);
                else
                    Turn(false,this.transform,deltaTime);
            }
        }

        var moveDist = (speed * Time.deltaTime);
        transform.position += direction * moveDist;
        if(_roll)
        {
            _rolledAngle = moveDist * (360f / (3f * Mathf.PI));
            var quat = whipState == Whip.Sphere ? Quaternion.Euler(_rolledAngle,0f,0f) : Quaternion.Euler(0f,0f,_rolledAngle);
            rollSphere.rotation = rollSphere.rotation * quat;
        }
        

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

    public void SetSphereRotationIdentity()
    {
        rollSphere.localRotation = Quaternion.identity;
    }

    public bool SphereUpCheck(float movedAngle)
    {
        return MathEx.abs(rollSphere.localRotation.eulerAngles.x) < movedAngle;
    }

    public bool IsPlaying(int layer, string n)
    {
        return animatorControll.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1f && animatorControll.GetCurrentAnimatorStateInfo(layer).IsName(n);
        //return animatorControll.GetCurrentAnimatorStateInfo(layer).IsName(n);
    }

    public void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.name == "EventBridge")
        {
            coll.gameObject.SetActive(false);
        }
    }
}
