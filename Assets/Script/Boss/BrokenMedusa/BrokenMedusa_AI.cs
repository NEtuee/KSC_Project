using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BrokenMedusa_AI : IKBossBase
{
    public enum AnimationType
    {
        KeyFrame,
        Graph
    };

    public enum State
    {
        Idle,
        TransformIdle,
        TransformOpen,
        TransformClose,
        Scanned,
        LockOnMove,
        LockOnLook,
        LockOnFrontWalk,
        SearchIdle,
        SearchRotate,
        SearchScan,
        CenterMove,
        Dead
    }

    public AnimationType animationType;

    public float headRotationLock = 90f;
    public float pushDistance = 3f;
    public float scanYLimit = 10f;
    public float lookDistance = 20f;
    public float pushUpDist = 3f;
    public float jumpPushDist = 2f;
    
    public Animator animatorControll;

    public LayerMask pushObstacleLayer;

    //public Animation animationControll;
    public GraphAnimator graphAnimator;
    public BossScan scanner;
    public GameObject shield;
    public FloorControl floorControl;


    public Transform head;
    public Transform body;
    public Transform shildTransform;
    public Transform shildGraphic;

    public State currentState;

    private Vector3 _moveLine;
    private Vector3 _perpendicularPoint;
    private Vector3 _searchDirection;
    private Vector3 _scannedPosition;

    private Quaternion _headRotation;

    private float _pointDistance;
    private float _direction;//1 = right, -1 = left
    private float _pushCooldown;

    private bool _scanCheck = false;
    private bool _armPushLerpBack = false;
    private bool _jumpPush = false;
    private bool _firstScan = false;

    public UnityEvent scannedEvent;
    public UnityEvent whenSearch;
    public UnityEvent whenSearchIdle;
    public UnityEvent deadEvent;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.player_EMPHit,(x)=>{
            BodyHitAnimation();
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        SetLegHitGroundSound(1512);
        
        if(animationType == AnimationType.Graph)
            graphAnimator.Play("UpDown",body);
        else if(animationType == AnimationType.KeyFrame)
        {

        }

        _timeCounter.InitTimer("transformTime");
        _timeCounter.InitTimer("firstScanTime",0f,0.5f);

        _timeCounter.InitTimer("FrontWalk");
        _timeCounter.InitTimer("FrontWalk_Init");
        _timeCounter.InitTimer("timer");
        _timeCounter.InitTimer("pushStand");
        _timeCounter.InitTimer("pushCooldown");
        
        SoundPlay(4005,transform,Vector3.zero);
        //GameManager.Instance.soundManager.Play(4005,Vector3.zero,transform);

        ChangeState(State.TransformIdle);
    }

    public override void FixedProgress(float deltaTime)
    {
        UpdateProcess(deltaTime);
    }

    // public override void AfterProgress(float deltaTime)
    // {
    //     head.rotation = _headRotation;
    //     Debug.Log(head.eulerAngles);
    // }

    // public void Update()
    // {
    //     if (GameManager.Instance.PAUSE == true || GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Update)
    //         return;

    //     UpdateProcess(Time.deltaTime);
    // }

    // public void FixedUpdate()
    // {
    //     if (GameManager.Instance.PAUSE == true || GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Fixed)
    //         return;
        
    //     UpdateProcess(Time.fixedDeltaTime);
    // }

    public void LateUpdate()
    {
        head.rotation = _headRotation;

    }

    private void UpdateProcess(float deltaTime)
    {
        if (_isTriggered)
        {
            _timeCounter.IncreaseTimerSelf("scanTime",out bool limit,deltaTime);
            if (limit)
            {
                _isTriggered = false;
            }
            
        }
        
        _timeCounter.IncreaseTimerSelf("timer",1f,out bool timeLimit,deltaTime);
        if(!timeLimit)
        {
            return;
        }

        UpdateDirection();
        UpdatePerpendicularPoint();
        Push(deltaTime);
        var upDist = MathEx.distance(transform.position.y, _target.position.y);
        _jumpPush = _jumpPush && jumpPushDist <= upDist;

        if (_targetDistance <= 3f && (currentState == State.SearchIdle || currentState == State.SearchRotate || currentState == State.SearchScan))
        {
            UpdateMoveLine();
            ChangeState(State.LockOnMove);
        }


        _timeCounter.IncreaseTimerSelf("pushStand",out bool pushLimit,deltaTime);
        if(pushLimit)
        {
            _armPushLerpBack = !(IsPlaying(2,"Anim_Medusa_Push") || IsPlaying(2,"Anim_Medusa_PushUp"));

            if(_armPushLerpBack)
            {
                var value = Mathf.Lerp(animatorControll.GetLayerWeight(2),0f,0.02f);
                animatorControll.SetLayerWeight(2,value);
            }
        }
        

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
            LookTarget_Head(_target.position, deltaTime);
            if(lookDistance - 2 > _targetDistance)
            {
                UpdateMoveLine();
                ChangeState(State.LockOnMove);
            }

            if(!IsOnGrounded())
            {
                SoundPlay(1514,transform,Vector3.zero);
                //GameManager.Instance.soundManager.Play(1514, Vector3.zero, transform);
                ChangeState(State.SearchIdle);
            }

            if(!_firstScan)
            {
                _timeCounter.IncreaseTimerSelf("firstScanTime",out var scan,deltaTime);
                if(scan)
                {
                    ScanForward();
                    scannedEvent?.Invoke();
                    _firstScan = true;
                }
                
            }

            FrontMoveProgress(deltaTime);
        }
        else if(currentState == State.LockOnMove)
        {
            LookLineForward_Body(deltaTime);
            LineMove(deltaTime);
            if(Vector3.Angle(transform.forward,MathEx.DeleteYPos(_target.position - transform.position)) >= 100f)
            {
                UpdateMoveLine();
            }
            
            if(lookDistance + 2f < _targetDistance)
                ChangeState(State.LockOnLook);
            
            if(!IsOnGrounded())
            {
                SoundPlay(1514,transform,Vector3.zero);
                //GameManager.Instance.soundManager.Play(1514, Vector3.zero, transform);
                ChangeState(State.SearchIdle);
            }

            if(!_firstScan)
            {
                _timeCounter.IncreaseTimerSelf("firstScanTime",out var scan,deltaTime);
                if(scan)
                {
                    ScanForward();
                    scannedEvent?.Invoke();
                    _firstScan = true;
                }
                
            }

            FrontMoveProgress(deltaTime);
        }
        else if(currentState == State.LockOnFrontWalk)
        {
            TargetFrontMove(deltaTime);
            if(Vector3.Angle(transform.forward,MathEx.DeleteYPos(_target.position - transform.position)) >= 100f)
            {
                UpdateMoveLine();
            }
        }
        else if(currentState == State.Scanned)
        {
            if(LookTarget_Head(_scannedPosition, deltaTime))
            {
                if(!scanner.scaning)
                {
                    if (_scanCheck)
                    {
                        ChangeState(State.SearchRotate);
                    }
                    else
                    {
                        ScanForward();
                        _scanCheck = true;
                    }
                    
                }
                
            }

            if(ScanCheck())
            {
                UpdateMoveLine();
                ChangeState(State.LockOnMove);

                if(!floorControl._launch)
                {
                    floorControl.SpecialLaunch();
                }
            }
        }
        else if(currentState == State.SearchIdle)
        {
            _timeCounter.IncreaseTimer("SearchIdle",1f,out bool limit);
            CenterMove(deltaTime);
            if(limit)
            {
                ChangeState(State.SearchScan);
            }
        }
        else if(currentState == State.SearchRotate)
        {
            BodyTurn(false,deltaTime);
            //BodyRotateForHead();

            var angle = Vector3.Angle(_searchDirection,GetHeadForward());
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
                        floorControl.SpecialLaunch();
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
            if(animationType == AnimationType.Graph)
            {
                graphAnimator.Stop();
                graphAnimator.Play("Dead",body);
            }
            else if(animationType == AnimationType.KeyFrame)
            {
                MainAnimationPlay(2);
            }

            this.enabled = false;
        }
        else if(currentState == State.TransformOpen)
        {
            _timeCounter.IncreaseTimerSelf("transformTime",out var limit, deltaTime);
            if(limit && !IsPlaying(0,"Anim_Medusa_Box_Open"))
            {
                SetIKActive(true);
                ChangeState(State.LockOnLook);

                animatorControll.SetLayerWeight(1,1f);
                animatorControll.SetLayerWeight(2,1f);
                MainAnimationPlay(3);

                SetPlayerRunningLock(false);
                SetAimLock(false);
                SetCameraDefault();

                shield.SetActive(true);
                //animationControll.Play("Anim_Medusa_Finding");
            }
            // if(!animationControll.isPlaying)
            // {
            //     SetIKActive(true);
            //     ChangeState(State.Idle);
            //     animationControll.Play("Anim_Medusa_Finding");
            // }
        }
        else if(currentState == State.CenterMove)
        {
            if(CenterMove(deltaTime))
            {
                SetPlayerRunningLock(false);
                SetAimLock(false);
                SetCameraDefault();

                ChangeState(State.LockOnLook);
            }
        }


        _headRotation = head.rotation;
    }

    public void ChangeState(State state)
    {
        State prevState = currentState;

        switch(prevState)
        {
            case State.LockOnLook:
                {
                    if(state != State.LockOnMove)
                    whenSearch?.Invoke();
                }
                break;
            case State.LockOnMove:
                {
                    if (state != State.LockOnLook)
                        whenSearch?.Invoke();
                }
                break;
        }

        switch(state)
        {
            case State.Scanned:
                {
                    _scanCheck = false;
                    _scannedPosition = _target.position;
                }
                break;
            case State.SearchRotate:
            case State.SearchIdle:
            case State.SearchScan:
            {
                if (prevState == State.LockOnMove || prevState == State.LockOnLook || prevState == State.LockOnFrontWalk)
                    whenSearchIdle?.Invoke();
            } 
                break;
            case State.Dead:
                {
                    deadEvent.Invoke();
                }
                break;
        }

        currentState = state;

        if(currentState == State.Idle)
        {
            if(!IsPlaying(0,"Anim_Medusa_Idle"))
            {
                MainAnimationPlay(5);
            }
        }
        else if(currentState == State.SearchIdle)
        {
            _moveLine = Vector3.zero;
            _timeCounter.InitTimer("SearchIdle");

            if(animationType == AnimationType.KeyFrame && !IsPlaying(0, "Anim_Medusa_Finding"))
            {
                MainAnimationPlay(3);
            }

            // if(animationType == AnimationType.KeyFrame && !animationControll.IsPlaying("Anim_Medusa_Finding"))
            // {
            //     animationControll.Play("Anim_Medusa_Finding");
            // }
        }
        else if(currentState == State.SearchRotate)
        {
            _searchDirection = GetHeadForward();
            _timeCounter.InitTimer("SearchRotate");

            if(animationType == AnimationType.KeyFrame && !IsPlaying(0, "Anim_Medusa_Finding"))
            {
                MainAnimationPlay(3);
            }

            // if(animationType == AnimationType.KeyFrame && !animationControll.IsPlaying("Anim_Medusa_Finding"))
            // {
            //     animationControll.Play("Anim_Medusa_Finding");
            // }
        }
        else if(currentState == State.SearchScan)
        {
            scanner.scaning = false;
            _timeCounter.InitTimer("SearchScan");

            // if(animationType == AnimationType.KeyFrame && !animationControll.IsPlaying("Anim_Medusa_Finding"))
            // {
            //     animationControll.Play("Anim_Medusa_Finding");
            // }
        }
        else if(currentState == State.LockOnLook)
        {
            if(!IsPlaying(0,"Anim_Medusa_Idle"))
            {
                MainAnimationPlay(5);
            }

            ArmStateAnimationPlay(1);
            //animationControll.PlayQueued("Anim_Medusa_Close");
        }
        else if(currentState == State.LockOnMove)
        {
            if(!IsPlaying(0,"Anim_Medusa_Idle"))
            {
                MainAnimationPlay(5);
            }

            ArmStateAnimationPlay(0);
            //animationControll.PlayQueued("Anim_Medusa_Open");
        }
        else if(currentState == State.TransformOpen)
        {
            SetIKActive(false);
            MainAnimationPlay(0);
            animatorControll.SetLayerWeight(1,0f);
            animatorControll.SetLayerWeight(2,0f);
            _timeCounter.InitTimer("transformTime");

            //_soundManager.Play(1519,transform.position);
            
            //animationControll.Play("Anim_Medusa_Box_Open");
        }
        else if(currentState == State.TransformClose)
        {
            SetIKActive(false);
            this.enabled = false;
            MainAnimationPlay(1);
            animatorControll.SetLayerWeight(1,0f);
            animatorControll.SetLayerWeight(2,0f);
            //animationControll.Play("Anim_Medusa_Box_Close");
        }
        else if(currentState == State.TransformIdle)
        {
            SetIKActive(false);
            animatorControll.SetLayerWeight(1,0f);
            animatorControll.SetLayerWeight(2,0f);
            //animationControll.Play("Anim_Medusa_Box_Close");
        }



    }

    public void BodyHitAnimation()
    {
        if(graphAnimator.IsPlaying("HitV") == null && (currentState != State.TransformIdle || currentState != State.TransformOpen))
            graphAnimator.Play("HitV",body,false);
    }

    public void OpenTrigger()
    {
        ChangeState(State.TransformOpen);
    }

    public void WhenPushFall()
    {
        if(currentState == State.TransformIdle || currentState == State.TransformOpen || currentState == State.TransformClose)
            return;

        ChangeState(State.CenterMove);
    }

    public override void Hit()
    {
        if(animationType == AnimationType.Graph)
            graphAnimator.Play("Hit",body);
        else if(animationType == AnimationType.KeyFrame)
        {
            MainAnimationPlay(4);
            //animationControll.Play("Anim_Medusa_Hit");
        }

        SoundPlay(1701,null,transform.position);
        //_soundManager.Play(1701,transform.position);
    }

    public void Dead()
    {
        MD.SetTimeScaleMsg data = MessageDataPooling.GetMessageData<MD.SetTimeScaleMsg>();
            data.timeScale = 0.2f;
            data.lerpTime = 4f;
            data.stopTime = 0f;
            data.startTime = 0f;
            SendMessageEx(MessageTitles.timemanager_settimescale, GetSavedNumber("TimeManager"), data);

        graphAnimator.Stop();
        graphAnimator.enabled = false;

        //SoundPlay(1510,null,transform.position);
        SoundPlay(1700,null,transform.position);
        //_soundManager.Play(1510,transform.position);
        //_soundManager.Play(1700,transform.position);
        SetIKActive(false);
        ChangeState(State.Dead);
    }

    public override void Scanned()
    {
        if((currentState == State.Idle || currentState == State.SearchIdle || currentState == State.SearchRotate || currentState == State.SearchScan))
        {
            Debug.Log("Check");
            ChangeState(State.Scanned);
            _isTriggered = true;

            _timeCounter.InitTimer("scanTime",0f,2f);
        }
        // else if(currentState == State.TransformIdle)
        // {
        //     ChangeState(State.TransformOpen);
        // }
    }

    public void FrontMoveProgress(float deltaTime)
    {
        _timeCounter.IncreaseTimer("FrontWalk_Init",4f,out bool limit);
        if(limit)
        {
            TargetFrontMove(deltaTime);
            var timelimit = 1.2f;
            timelimit = _targetDistance >= 20f ? 4f : timelimit;
            _timeCounter.IncreaseTimer("FrontWalk",timelimit,out limit);
            if(limit)
            {
                _timeCounter.InitTimer("FrontWalk");
                _timeCounter.InitTimer("FrontWalk_Init");
            }
        }
    }

    public bool IsOnGrounded()
    {
        return (_target.position.y - transform.position.y) < scanYLimit;
    }

    public void ScanForward()
    {
        SoundPlay(1511,null,transform.position);
        //_soundManager.Play(1511,transform.position);
        scanner.SetHeight(transform.position.y + scanYLimit);
        scanner.ScanSetup(transform.position,GetHeadForward());
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

        if (Mathf.Acos(Vector3.Dot(GetHeadForward(), (scanObjPosition - startPosition).normalized)) * Mathf.Rad2Deg < scanner.arc)
        {
            float mag = (scanObjPosition - startPosition).magnitude;
            if (mag <= scanner.range && mag >= scanner.range - 3f)
            {
                //scan
                //GameManager.Instance.soundManager.Play(1513, Vector3.zero, transform);
                SoundPlay(1513,transform,Vector3.zero);
                
                return true;
            }
        }

        return false;
    }

    public void Push(float deltaTime)
    {
        _timeCounter.IncreaseTimerSelf("pushCooldown",out var limit,deltaTime);
        if(!limit)
        {
            return;
        }

        if (IsPlaying(2, "Anim_Medusa_Push") || IsPlaying(2, "Anim_Medusa_PushUp"))
            return;
        
        if(pushDistance >= _targetDistance/* && graphAnimator.IsPlaying("ShildAttack") == null*/)
        {
            var dist = Vector3.Distance(transform.position, _target.position);
            if(dist >= 7f)
            {
                UpdateMoveLine();
                return;
            }

            var rayPos = transform.position;
            rayPos.y += 2f;
            var rayDir = (_target.position - rayPos).normalized;
            var rayDist = Vector3.Distance(_target.position,rayPos);
            if(Physics.Raycast(rayPos,rayDir,rayDist,pushObstacleLayer))
            {
                return;
            }


            var upDist = MathEx.distance(transform.position.y, _target.position.y);

            if(upDist >= pushUpDist && !_jumpPush)
            {
                _jumpPush = true;
                PushBackUp();
                SoundPlay(1015,null,_target.position);
                //GameManager.Instance.soundManager.Play(1015,_target.position);
                _timeCounter.InitTimer("pushCooldown");
            }
            else if(((upDist <= jumpPushDist) && _jumpPush) || !_jumpPush)
            {
                PushBack();
                _jumpPush = false;
                SoundPlay(1015,null,_target.position);
                //GameManager.Instance.soundManager.Play(1015,_target.position);
                _timeCounter.InitTimer("pushCooldown");
            }
        }
    }

    public void PushBack()
    {
        if(animationType == AnimationType.Graph)
            graphAnimator.Play("ShildAttack",shildGraphic);
        else if(animationType == AnimationType.KeyFrame)
        {
            ArmPushAnimationPlay(0);
           // animationControll.Play("Anim_Medusa_Push");
        }

        Collider[] playerColl = Physics.OverlapSphere(shildTransform.position, 10f,targetLayer);

        if(playerColl.Length != 0)
        {
            //foreach(Collider curr in playerColl)
            //{
            //    PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
                
            //    if (ragdoll != null)
            //    {
            //        _player.TakeDamage(5f);
            //        ragdoll.ExplosionRagdoll(250.0f, transform.forward);
            //            //Vector3.ProjectOnPlane((_target.position - _perpendicularPoint),Vector3.up).normalized);
                
            //        break;
            //    }

                
            //}
            _player.TakeDamage(5f, 250.0f, transform.forward);
        }
    }

    public void PushBackUp()
    {
        if(animationType == AnimationType.Graph)
            graphAnimator.Play("ShildAttack",shildGraphic);
        else if(animationType == AnimationType.KeyFrame)
        {
            ArmPushAnimationPlay(1);
         //   animationControll.Play("Anim_Medusa_PushUp");
        }

        var dir = (MathEx.DeleteYPos(_target.position) - MathEx.DeleteYPos(_perpendicularPoint)).normalized;

        if(_player.GetState == PlayerUnit.grabState || _player.GetState == PlayerUnit.hangLedgeState)
            _player.TakeDamage(5f, 250.0f, transform.forward);
        else
            _player.TakeDamage(5f);

        _player.SetJumpPower(20f);
        _player.SetVelocity(dir * 15f);
        
        // Collider[] playerColl = Physics.OverlapSphere(shildTransform.position, 10f,targetLayer);
        //
        // if(playerColl.Length != 0)
        // {
        //     foreach(Collider curr in playerColl)
        //     {
        //         PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
        //         if(ragdoll != null)
        //         {
        //             ragdoll.ExplosionRagdoll(300.0f,_perpendicularPoint,1000f);
        //         }
        //     }
        // }
    }

    public void SetCameraDefault()
    {
        //GameManager.Instance.cameraManager.ActivePlayerFollowCamera();
    }

    public void SetPlayerRunningLock(bool value)
    {
        _player.SetRunningLock(value);
    }

    public void SetAimLock(bool value)
    {
        _player.SetAimLock(value);
    }

    public void LineMove(float deltaTime)
    {
        if(_pointDistance >= 1f)
        {
            Move(_moveLine, (_pointDistance * 4f) * _direction, deltaTime);
        }
    }

    public bool LookTarget_Head(Vector3 pos, float deltaTime)
    {
        return HeadLook((pos - transform.position).normalized,deltaTime);
        //BodyLook((_target.position - transform.position));
    }

    public void LookLineForward_Body(float deltaTime)
    {
        var dir = Vector3.Cross(transform.transform.up,_moveLine).normalized;

        HeadLook(dir,deltaTime);
        BodyLook(dir,deltaTime);
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

    public override bool Move(Vector3 direction, float speed, float deltaTime,float legSpeed = 4f)
    {
        base.Move(direction,speed,deltaTime,legSpeed);
        //if(base.Move(direction,speed,deltaTime,legSpeed))
            //body.localRotation = body.localRotation * Quaternion.Euler(0f,0f,speed * deltaTime * 10f);
        
        return true;
    }

    public bool HeadLook(Vector3 direction,float deltaTime)
    {
        var plane = Vector3.ProjectOnPlane(direction,GetHeadUp()).normalized;
        var headAngle = Vector3.SignedAngle(GetHeadForward(),plane,GetHeadUp());
        bool look = false;

        //head.RotateAround(head.position,head.up,headAngle);
        if(MathEx.abs(headAngle) >= 1f)
        {
            HeadTurn(headAngle > 0,deltaTime);
        }
        else
        {
            look = true;
        }

        BodyRotateForHead(deltaTime);
        
        
        return look;
        
    }

    public void BodyLook(Vector3 direction, float deltaTime)
    {
        var plane = Vector3.ProjectOnPlane(direction,transform.up).normalized;
        var bodyAngle = Vector3.SignedAngle(transform.forward,plane,transform.up);

        if(MathEx.abs(bodyAngle) >= 1f)
        {
            BodyTurn(bodyAngle > 0,deltaTime);
        }
    }

    public void HeadTurn(bool isLeft, float deltaTime)
    {
        var forward = GetHeadForward();
        var rotation = head.rotation;
        
        //Turn(isLeft,head,deltaTime);
        Turn(isLeft,head,deltaTime,GetHeadUp());

        // var angle = Vector3.Angle(body.forward, forward);
        // if (angle > headRotationLock)
        // {
        //     Turn(head,(headRotationLock - angle));
        // }
        //head.RotateAround(head.position,head.up,rotationSpeed * Time.deltaTime * (isLeft ? 1f : -1f));
    }

    public bool BodyRotateForHead(float deltaTime)
    {
        //var angle = Vector3.Angle(transform.forward, head.forward);
        var angle = Vector3.SignedAngle(GetHeadForward(),transform.forward,GetHeadUp());
        if(MathEx.abs(angle) > headRotationLock)
        {
            BodyTurn(Vector3.SignedAngle(GetHeadForward(),transform.forward,GetHeadUp()) < 0, deltaTime);//_direction < 0);
            return true;
        }

        return false;
    }

    public void BodyTurn(bool isLeft, float deltaTime)
    {
        Turn(isLeft,transform,deltaTime);
        //transform.RotateAround(transform.position,transform.up,rotationSpeed * Time.deltaTime * (isLeft ? 1f : -1f));
    }

    public bool IsPlaying(int layer, string n)
    {
        return animatorControll.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1f && animatorControll.GetCurrentAnimatorStateInfo(layer).IsName(n);
        //return animatorControll.GetCurrentAnimatorStateInfo(layer).IsName(n);
    }

    public void MainAnimationPlay(int code)
    {
        animatorControll.SetInteger("MainState",code);
        animatorControll.SetTrigger("MainChange");
    }

    public void ArmStateAnimationPlay(int code)
    {
        animatorControll.SetInteger("ArmState",code);
        animatorControll.SetTrigger("ArmChange");
    }

    public void ArmPushAnimationPlay(int code)
    {
        animatorControll.SetInteger("ArmPush",code);
        animatorControll.SetTrigger("ArmPushChange");

        animatorControll.SetLayerWeight(2,1f);
        _timeCounter.InitTimer("pushStand");
    }

    public Vector3 GetHeadForward()
    {
        return head.up;
    }

    public Vector3 GetHeadUp()
    {
        return -head.right;
    }

    public Vector3 GetHeadRight()
    {
        return -head.forward;
    }
}
