using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

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

    public Animator animatorControll;

    //public Animation animationControll;
    public GraphAnimator graphAnimator;
    public BossScan scanner;
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

    private bool _scanCheck = false;
    private bool _armPushLerpBack = false;

    public UnityEvent scannedEvent;
    public UnityEvent whenSearch;
    public UnityEvent deadEvent;

    public void Start()
    {
        Initialize();
        GetSoundManager();
        SetLegHitGroundSound(1512);
        
        if(animationType == AnimationType.Graph)
            graphAnimator.Play("UpDown",body);
        else if(animationType == AnimationType.KeyFrame)
        {

        }

        _timeCounter.InitTimer("FrontWalk");
        _timeCounter.InitTimer("FrontWalk_Init");
        _timeCounter.InitTimer("timer");
        _timeCounter.InitTimer("pushStand");

        scannedEvent.AddListener(() => { GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneHelperRoot>().HelpEvent("Scanned"); });
        
        GameManager.Instance.soundManager.Play(4004,Vector3.zero,transform);

        ChangeState(State.TransformOpen);
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

    public void LateUpdate()
    {
        head.rotation = _headRotation;

    }

    private void UpdateProcess(float deltaTime)
    {
        if (_isTriggered)
        {
            _timeCounter.IncreaseTimer("scanTime",out bool limit);
            if (limit)
            {
                _isTriggered = false;
            }
            
        }
        
        _timeCounter.IncreaseTimer("timer",1f,out bool timeLimit);
        if(!timeLimit)
        {
            return;
        }

        UpdateDirection();
        UpdatePerpendicularPoint();
        Push(false);

        if (Input.GetKeyDown(KeyCode.L))
        {
            //animationControll.Play("Anim_Medusa_Hit");
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            Dead();
        }


        _timeCounter.IncreaseTimer("pushStand",out bool pushLimit);
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
                ChangeState(State.SearchIdle);
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
                ChangeState(State.SearchIdle);
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
                    floorControl.Launch();
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
            if(!IsPlaying(0,"Anim_Medusa_Box_Open"))
            {
                SetIKActive(true);
                ChangeState(State.SearchScan);

                animatorControll.SetLayerWeight(1,1f);
                animatorControll.SetLayerWeight(2,1f);
                MainAnimationPlay(3);
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
                    scannedEvent.Invoke();
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



    }

    public void WhenPushFall()
    {
        if(currentState == State.TransformOpen || currentState == State.TransformClose)
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
        _soundManager.Play(1701,transform.position);
    }

    public void Dead()
    {
        _soundManager.Play(1510,transform.position);
        _soundManager.Play(1501,transform.position);
        SetIKActive(false);
        ChangeState(State.Dead);
    }

    public override void Scanned()
    {
        if((currentState == State.Idle || currentState == State.SearchIdle || currentState == State.SearchRotate || currentState == State.SearchScan))
        {
            ChangeState(State.Scanned);
            _isTriggered = true;

            _timeCounter.InitTimer("scanTime",0f,2f);
        }
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
        _soundManager.Play(1511,transform.position);
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
                Debug.Log("scanned");
                return true;
            }
        }

        return false;
    }

    public void Push(bool up)
    {
        if(pushDistance >= _targetDistance/* && graphAnimator.IsPlaying("ShildAttack") == null*/)
        {
            var dist = Vector3.Distance(transform.position, _target.position);
            if(dist >= 7f)
            {
                UpdateMoveLine();
                return;
            }
            
            GameManager.Instance.soundManager.Play(1015,_target.position);

            var upDist = MathEx.distance(transform.position.y, _target.position.y);

            if(upDist >= 3f)
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
            foreach(Collider curr in playerColl)
            {
                PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
                if(ragdoll != null)
                {
                    ragdoll.ExplosionRagdoll(200.0f, 
                        Vector3.ProjectOnPlane((_target.position - _perpendicularPoint),Vector3.up).normalized);
                }
            }
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

        Collider[] playerColl = Physics.OverlapSphere(shildTransform.position, 10f,targetLayer);

        if(playerColl.Length != 0)
        {
            foreach(Collider curr in playerColl)
            {
                PlayerRagdoll ragdoll = curr.GetComponent<PlayerRagdoll>();
                if(ragdoll != null)
                {
                    ragdoll.ExplosionRagdoll(300.0f,_perpendicularPoint,1000f);
                }
            }
        }
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
        if(base.Move(direction,speed,deltaTime,legSpeed))
            body.localRotation = body.localRotation * Quaternion.Euler(0f,0f,speed * deltaTime * 10f);
        
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
