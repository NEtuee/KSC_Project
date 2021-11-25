using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using MD;

public class Drone : PathfollowObjectBase
{
    public enum DroneState { Default, Approach, Collect, Return , AimHelp ,Help,Scan}
    public enum DroneFollowState{Left,Right,Forward,Backward}

    [SerializeField] private bool visible;
    public bool Visible
    {
        get { return visible; }
        set
        {
            if (value == visible)
                return;
            visible = value;
            if(visible)          
                droneVisual.gameObject.SetActive(true);          
            else        
                droneVisual.gameObject.SetActive(false);           
        }
    }

    [SerializeField] public LayerMask droneCollisionLayer;
    [SerializeField] private Transform target;
    [SerializeField] private Transform scanPosition;
    [SerializeField] public List<Transform> dronePoints = new List<Transform>();
    [SerializeField] public Transform droneAimPoint;
    [SerializeField] private DroneState state;
    //[SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Vector3 defaultFollowOffset;
    // [SerializeField] private float defaultFollowSpeed = 10.0f;
    // [SerializeField] private float finalTargetLerpSpeed = 5.0f;
    // private Vector3 _currentVelocity;
    // [SerializeField] private Vector3 aimHelpOffset;
    // [SerializeField] private Vector3 helpOffset;
    // [SerializeField] private Vector3 helpGrabStateOffset;
    [SerializeField] private Vector3 respawnOffset;
    // [SerializeField] private float collectRequiredTime = 1f;
    // [SerializeField] private FloatingMove floatingMove;
    [SerializeField] private bool help = false;
    [SerializeField] private float scanCoolTime = 3f;
    public float ScanCoolTime => scanCoolTime;
    private float _scanLeftTime = 0.0f;
    public FloatReactiveProperty scanLeftCoolTime;
    private Vector3 _targetPosition;
    private Vector3 _finalTargetPosition;
    private Vector3 _prevTargetPosition;
    [SerializeField]private DroneFollowState _followState = DroneFollowState.Forward;
    [SerializeField]private DroneFollowState _prevFollowState = DroneFollowState.Right;

    [Header("DroneVisual")]
    [SerializeField] private Transform droneVisual;
    [SerializeField] private Transform droneBody;
    public Animator mainAnimator;
    private Vector3 droneBodyOriginPos;
    //private Animator _droneAnim;
    private FloatingMove _floatingMoveComponent;
    private bool _respawn = false;
    public bool canMove = true;


    [Header("DroneEffects")]
    public List<GameObject> disapearTargets = new List<GameObject>();
    public Material cooltimeMat;
    public Material dissolveMat;
    public Material jetMat;
    public Transform backEffectPos;
    public float dissolveTime = 1f;
    public float dissolveStartTime = 0.2f;
    private bool _dissolve = false;



    private float collectStartTime;
    
    private int _droneSide = 0;

    private Transform approachTarget;
    private Stack<Transform> orderList = new Stack<Transform>();
    private Transform mainCam;

    private Transform playerHead;
    private PlayerUnit player;

    private DroneHelperRoot droneHelperRoot;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private Vector3 _droneMovePosition;
    private bool _frontHit = false;
    private bool _scanning = false;

    //?�캔
    private Quaternion _scanTargetRotation;
    private float _rotationSpeed = 800.0f;
    private DroneScaner _droneScaner;
    public delegate void WhenAimHelp();
    public WhenAimHelp whenAimHelp;
    public delegate void WhenHelp();
    public WhenAimHelp whenHelp;
    public delegate void WhenCompleteRespawn();
    public WhenCompleteRespawn whenCompleteRespawn;

    public override void Assign()
    {
        base.Assign();

        SaveMyNumber("Drone");

        AddAction(MessageTitles.set_setplayer, (msg) =>
        {
            player = (PlayerUnit)msg.data;
        });

        AddAction(MessageTitles.scan_registerScanObject, (msg) =>
        {
            _droneScaner.AddScanMessageObject((MessageReceiver)msg.data);
        });

        _timeCounter.CreateSequencer("ScanProcess");
        _timeCounter.AddSequence("ScanProcess",0.3f,null,Scan);
        _timeCounter.AddSequence("ScanProcess",0.4f,null,null);

        _timeCounter.CreateSequencer("DissolvProcess");
        _timeCounter.AddSequence("DissolvProcess",dissolveStartTime + 0.3f,null,(x)=>{
            foreach(var item in disapearTargets)
            {
                item.SetActive(true);
            }

            ChangeAnimation(1);
        });
        _timeCounter.AddSequence("DissolvProcess",dissolveTime,UpdateDissolve,null);

        _timeCounter.CreateSequencer("RandomMove");
        _timeCounter.AddSequence("RandomMove", 15f, null, (x) =>
        {
            //ChangeAnimation(Random.Range(1, 3));
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("PlayerManager"));

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        if(player == null)
        {
            Debug.LogError("Not Set PlayerCtrl");
        }

        mainCam = Camera.main.transform;
        playerHead = player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        //target = player.transform;
        droneHelperRoot = GetComponent<DroneHelperRoot>();
        _droneScaner = GetComponent<DroneScaner>();

        //GameManager.Instance.soundManager.Play(1300, Vector3.zero, transform);
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1300; soundData.localPosition = Vector3.zero; soundData.parent = transform; soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);

        if (droneVisual == null)
        {
            Debug.LogWarning("Not Set DroneVisual");
            return;
        }

        //_droneAnim = droneVisual.GetComponent<Animator>();
        _floatingMoveComponent = droneVisual.GetComponent<FloatingMove>();
        droneBodyOriginPos = droneBody.localPosition;

        if (visible)
            droneVisual.gameObject.SetActive(true);
        else
            droneVisual.gameObject.SetActive(false);

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        _targetPosition = (camForward * defaultFollowOffset.z + camRight * defaultFollowOffset.x + Vector3.up * defaultFollowOffset.y) + target.position;
        _finalTargetPosition = _targetPosition;

        _droneMovePosition = transform.position;

        _timeCounter.InitSequencer("ScanProcess");
        _timeCounter.InitSequencer("DissolvProcess");
        _timeCounter.InitSequencer("RandomMove");
    }

    // Update is called once per frame
    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if (visible == false || _respawn)
            return;

        if(scanLeftCoolTime.Value > 0.0f)
        {
            scanLeftCoolTime.Value -= deltaTime;
            scanLeftCoolTime.Value = Mathf.Clamp(scanLeftCoolTime.Value, 0.0f, 10.0f);
            if(scanLeftCoolTime.Value <= 0f)
            {
                EffectActiveData effectData = MessageDataPooling.GetMessageData<EffectActiveData>();
                effectData.key = "BirdyScanReady";
                effectData.position = backEffectPos.position;
                effectData.rotation = transform.rotation;
                effectData.parent = transform;
                SendMessageEx(MessageTitles.effectmanager_activeeffectsetparent,GetSavedNumber("EffectManager"),effectData);
            }
        }

        var coolGague = (scanCoolTime - scanLeftCoolTime.Value) / scanCoolTime;
        cooltimeMat.SetFloat("Cooltime_gauge",coolGague);

        if(_scanning)
        {
            _scanning = !_timeCounter.ProcessSequencer("ScanProcess",deltaTime);
        }

        if(canMove)
            DroneMovement(deltaTime);

    }

    public void DroneMovement(float deltaTime)
    {
        UpdateDroneSide();

        if(player.IsMoving() || player.IsJump)
        {
            _droneMovePosition = GetDronePoint();
            if (_timeCounter.ProcessSequencer("RandomMove", deltaTime))
            {
                _timeCounter.InitSequencer("RandomMove");
            }

        }
        
        if(player.IsClimbing())
        {
            _droneMovePosition = dronePoints[_droneSide * 2 + 1].position;
        }

        if(player.IsAiming())
        {
            _droneMovePosition = droneAimPoint.position;
        }

        if(_scanning)
        {
            _droneMovePosition = scanPosition.position;
        }

        if(_dissolve)
        {
            _dissolve = !_timeCounter.ProcessSequencer("DissolvProcess",deltaTime);
        }

        var dist = Vector3.Distance(transform.position,_droneMovePosition);
        jetMat.SetFloat("Power",Mathf.Clamp(dist * 0.7f,0.3f,1f));

        if(dist >= 10f)
        {
            _droneMovePosition = GetDronePoint();
            StartDissolveEffect();
        }

        transform.position = Vector3.Lerp(transform.position,_droneMovePosition,deltaTime * 13f);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, deltaTime * 13f);
    }

    public void ChangeAnimation(int code)
    {
        mainAnimator.SetTrigger("Change");
        mainAnimator.SetInteger("Target",code);

        _timeCounter.InitSequencer("RandomMove");
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void UpdateDissolve(float t)
    {
        //Debug.Log(t);
        dissolveMat.SetFloat("Dissvole", 1f - (t / dissolveTime));
    }

    public void UpdateDroneSide()
    {
        var left = Physics.Raycast(transform.position,-transform.right,out var leftHit,1f,droneCollisionLayer);
        var right = Physics.Raycast(transform.position,transform.right,out var rightHit,1f,droneCollisionLayer);

        if(left || right)
        {
            _droneSide = left && right ? (leftHit.distance < rightHit.distance ? 0 : 1) : (left ? 1 : 0);

            StartDissolveEffect();
        }
    }

    public void StartDissolveEffect()
    {
        dissolveMat.SetFloat("Dissvole",1f);
        _timeCounter.InitSequencer("DissolvProcess");
        _dissolve = true;

        foreach(var item in disapearTargets)
        {
            item.SetActive(false);
        }
    }

    public Vector3 GetDronePoint()
    {
        var cam = MathEx.DeleteYPos(Camera.main.transform.forward).normalized;
        var targetDir = MathEx.DeleteYPos(target.forward).normalized;
        float angle = Vector3.Angle(cam,targetDir);
        float factor = Mathf.Clamp01(angle / 90f);

        var forward = Physics.Raycast(transform.position,transform.forward,out var hit,_frontHit ? 4f : 2f,droneCollisionLayer);
        _frontHit = forward;
        factor = forward ? 0f : factor;
        var position = Vector3.Lerp(dronePoints[_droneSide * 2 + (forward ? 1 : 0)].position,
                                dronePoints[_droneSide * 2 + (forward ? 0 : 1)].position,factor);

        return position;
    }

    private void LateUpdate()
    {   
        //if (InputManager.Instance.GetInput(KeybindingActions.Scan) && _scanLeftTime <= 0.0f)
        //{
        //    Scan();
        //}

        //if (player.updateMethod != UpdateMethod.Update)
        //    return;

        //UpdateDrone(Time.deltaTime);

      
    }

    private void UpdateDrone(float deltaTime)
    {
        if (visible == false || _respawn)
            return;

        if(scanLeftCoolTime.Value > 0.0f)
        {
            scanLeftCoolTime.Value -= deltaTime;
            scanLeftCoolTime.Value = Mathf.Clamp(scanLeftCoolTime.Value, 0.0f, 10.0f);
        }

        // switch(state)
        // {
        //     case DroneState.Default:
        //         {

        //             Vector3 camForward = Camera.main.transform.forward;
        //             Vector3 camRight = Camera.main.transform.right;
        //             camForward.y = 0;
        //             camRight.y = 0;

                    
                    
        //             if (player.CurrentSpeed > 0.0f)
        //             {
        //                 if (Vector3.Dot(Vector3.Cross(camForward.normalized, target.forward), Vector3.up) > 0.9f )
        //                 {
        //                     if (_followState != DroneFollowState.Right)
        //                     {
        //                         _prevFollowState = _followState;
        //                         _followState = DroneFollowState.Right;
        //                     }
        //                 }
        //                 else if (Vector3.Dot(Vector3.Cross(camForward.normalized, target.forward), Vector3.up) < -0.9f )
        //                 {
        //                     if (_followState != DroneFollowState.Left)
        //                     {
        //                         _prevFollowState = _followState;
        //                         _followState = DroneFollowState.Left;
        //                     }
        //                 }
        //                 else if(Vector3.Dot(Vector3.Cross(camRight.normalized, target.forward), Vector3.up) < 0.0f)
        //                 {
        //                     if (_followState != DroneFollowState.Forward)
        //                     {
        //                         _prevFollowState = _followState;
        //                         _followState = DroneFollowState.Forward;
        //                     }
        //                 }
        //                 else
        //                 {
        //                     if (_followState != DroneFollowState.Backward)
        //                     {
        //                         _prevFollowState = _followState;
        //                         _followState = DroneFollowState.Backward;
        //                     }
        //                 }
        //                 //_targetPosition = (camForward * defaultFollowOffset.z + camRight * defaultFollowOffset.x + Vector3.up * defaultFollowOffset.y) + target.position;
        //                 //_targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;

        //                 if (_followState == DroneFollowState.Forward )
        //                 {
        //                     if (_prevFollowState == DroneFollowState.Left)
        //                     {
        //                         if (player.HorizonWeight < 0.0f)
        //                             _prevFollowState = DroneFollowState.Right;
        //                         _targetPosition =
        //                             (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x +
        //                              target.up * defaultFollowOffset.y) + target.position;
        //                     }
        //                     else
        //                     {
        //                         if (player.HorizonWeight > 0.0f)
        //                             _prevFollowState = DroneFollowState.Left;
        //                         _targetPosition =
        //                             (target.forward * defaultFollowOffset.z + target.right * -defaultFollowOffset.x +
        //                              target.up * defaultFollowOffset.y) + target.position;
        //                     }
        //                 }
        //                 else if (_followState == DroneFollowState.Backward)
        //                 {
        //                     if (_prevFollowState == DroneFollowState.Left)
        //                         _targetPosition =
        //                             (target.forward * defaultFollowOffset.z + target.right * -defaultFollowOffset.x +
        //                              target.up * defaultFollowOffset.y) + target.position;
        //                     else
        //                         _targetPosition =
        //                             (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x +
        //                              target.up * defaultFollowOffset.y) + target.position;
        //                 }
        //                 else if(_followState == DroneFollowState.Left)
        //                 {
        //                     if(_prevFollowState == DroneFollowState.Backward)
        //                         _targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
        //                     else 
        //                         _targetPosition = (target.forward * defaultFollowOffset.z + target.right * -defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
        //                 }
        //                 else if (_followState == DroneFollowState.Right)
        //                 {
        //                     if(_prevFollowState == DroneFollowState.Backward)
        //                         _targetPosition = (target.forward * defaultFollowOffset.z + target.right * -defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
        //                     else
        //                         _targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
        //                 }
        //             }

        //             if(player.IsNowClimbingBehavior() == true)
        //             {
        //                 _targetPosition = (target.forward * helpGrabStateOffset.z + target.right * helpGrabStateOffset.x + Vector3.up * helpGrabStateOffset.y) + target.position;
        //             }

        //             if (Vector3.Distance(_targetPosition, transform.position) == 0.0f)
        //                 return;

        //             //Vector3 targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
        //             //Vector3 lookDir = targetPosition - transform.position;
        //             //Vector3 lookDir = target.position+Vector3.up*1.5f - transform.position;
        //             Vector3 lookDir = camForward;
        //             //lookDir.y = 0.0f;
        //             Quaternion targetRot;
        //             if (lookDir != Vector3.zero)
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
        //             else
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

        //             _finalTargetPosition = Vector3.Lerp(_finalTargetPosition, _targetPosition, finalTargetLerpSpeed * deltaTime);
        //             transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, _finalTargetPosition,ref _currentVelocity, deltaTime * defaultFollowSpeed), targetRot);
        //             //transform.SetPositionAndRotation(_targetPosition, targetRot);

        //         }
        //         break;
        //     case DroneState.AimHelp:
        //         {
        //             Vector3 targetPosition = (target.forward * aimHelpOffset.z + target.right * aimHelpOffset.x + target.up * aimHelpOffset.y) + target.position;
        //             targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 12f);
        //             Vector3 lookDir = playerHead.position - transform.position;
        //             Quaternion targetRot;
        //             if (lookDir != Vector3.zero)
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
        //             else
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

        //             transform.SetPositionAndRotation(targetPosition, targetRot);
        //         }
        //         break;
        //     case DroneState.Help:
        //         {
        //             Vector3 camForward = Camera.main.transform.forward;
        //             Vector3 camRight = Camera.main.transform.right;
        //             camForward.y = 0;
        //             camRight.y = 0;


        //             Vector3 targetPosition; 
        //             if(player.IsNowClimbingBehavior() == false)
        //                 targetPosition= (camForward * helpOffset.z + camRight * helpOffset.x + Vector3.up * helpOffset.y) + target.position;
        //             else
        //                 targetPosition = (target.forward * helpGrabStateOffset.z + target.right * helpGrabStateOffset.x + Vector3.up * helpGrabStateOffset.y) + target.position;

        //             targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 5f);
        //             Vector3 lookDir = playerHead.position - transform.position;
        //             Quaternion targetRot;
        //             if (lookDir != Vector3.zero)
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
        //             else
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

        //             transform.SetPositionAndRotation(targetPosition, targetRot);
        //         }
        //         break;
        //     case DroneState.Approach:
        //         {
        //             Vector3 targetPosition = Vector3.MoveTowards(transform.position, approachTarget.position, moveSpeed * deltaTime);
        //             Vector3 lookDir = approachTarget.position - transform.position;
        //             Quaternion targetRot;
        //             if (lookDir != Vector3.zero)
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
        //             else
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

        //             transform.SetPositionAndRotation(targetPosition, targetRot);

        //             if ((transform.position - approachTarget.position).sqrMagnitude < 1f)
        //             {
        //                 state = DroneState.Collect;
        //                 collectStartTime = Time.time;
        //             }
        //         }
        //         break;
        //     case DroneState.Collect:
        //         {
        //             if(Time.time - collectStartTime > collectRequiredTime)
        //             {
        //                 if(orderList.Count == 0)
        //                 {
        //                     state = DroneState.Return;
        //                 }
        //                 else
        //                 {
        //                     state = DroneState.Approach;
        //                     approachTarget = orderList.Pop();
        //                 }
        //             }
        //         }
        //         break;
        //     case DroneState.Return:
        //         {
        //             Vector3 destination = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
        //             Vector3 targetPosition = Vector3.MoveTowards(transform.position, destination, deltaTime * moveSpeed);
        //             Vector3 lookDir = targetPosition - transform.position;
        //             Quaternion targetRot;
        //             if (lookDir != Vector3.zero)
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
        //             else
        //                 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

        //             transform.SetPositionAndRotation(targetPosition, targetRot);

        //             if ((transform.position - destination).sqrMagnitude <=2.0f)
        //             {
        //                 _targetPosition = transform.position;
        //                 state = DroneState.Default;
        //             }
        //         }
        //         break;
        //     case DroneState.Scan:
        //         {
        //             Vector3 targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
        //             targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 5f);

        //             Quaternion targetRot;
        //             targetRot = Quaternion.RotateTowards(transform.rotation,_scanTargetRotation,_rotationSpeed * deltaTime);

        //             transform.SetPositionAndRotation(targetPosition, targetRot);

        //             if (targetRot == _scanTargetRotation)
        //             {
        //                 _droneScaner.Scanning();
        //                 state = DroneState.Default;
        //                 _targetPosition = transform.position;
        //             }
        //         }
        //         break;
        // }
    }

    public void Scan(float t = 0f)
    {
        scanLeftCoolTime.Value = scanCoolTime;
        Vector3 targetDir = Camera.main.transform.forward;
        targetDir.y = 0;
        targetDir.Normalize();

        BoolData activeData = MessageDataPooling.GetMessageData<BoolData>();
        activeData.value = true;
        SendMessageEx(MessageTitles.uimanager_visibleScanCoolTimeUi, GetSavedNumber("UIManager"), activeData);

        _scanTargetRotation = Quaternion.LookRotation(targetDir,Vector3.up);
        _droneScaner.Scanning();
        ChangeAnimation(3);
    }

    public void OrderApproch(Transform target)
    {
        if (state == DroneState.Default || state == DroneState.Return)
        {
            state = DroneState.Approach;
            approachTarget = target;
        }
        else
        {
            orderList.Push(target);
        }
    }

    public void OrderAimHelp(bool value)
    {
        if (value == true)
        {
            state = DroneState.AimHelp;
            _floatingMoveComponent.SetRangeRatio(0.2f);
            whenAimHelp?.Invoke();
        }
        else
        {
            if(help == true)
            {
                state = DroneState.Help;
                _floatingMoveComponent.SetRangeRatio(1.0f);
                whenHelp?.Invoke();
            }
            else
            {
                state = DroneState.Default;
                _floatingMoveComponent.SetRangeRatio(1.0f);
            }
        }
    }

    public void OrderDefault()
    {
        help = false;
        if (state != DroneState.AimHelp)
        {
            _targetPosition = transform.position;
            _finalTargetPosition = _targetPosition;
            state = DroneState.Default;
            _floatingMoveComponent.SetRangeRatio(1.0f);
        }
    }

    public void InitFollowPosition()
    {
        _targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x +
                                     target.up * defaultFollowOffset.y) + target.position;
        _finalTargetPosition = _targetPosition;

        transform.position = _finalTargetPosition;
    }

    public void OrderHelp()
    {
        help = true;

        if(state != DroneState.AimHelp)
        {
            state = DroneState.Help;
            //GameManager.Instance.soundManager.Play(1302, Vector3.zero, transform);
            AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundData.id = 1302; soundData.localPosition = Vector3.zero; soundData.parent = transform; soundData.returnValue = false;
            SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
            whenHelp?.Invoke();
        }
    }

    public DroneState GetState() { return state; }

    public void DroneHelpCall(string key)
    {
        if(ReferenceEquals(droneHelperRoot,null) == false)
        {
            droneHelperRoot.HelpEvent(key);
        }
    }

    public void DroneHelpCall(string key, float duration)
    {
        if (ReferenceEquals(droneHelperRoot, null) == false)
        {
            droneHelperRoot.HelpEvent(key,duration);
        }
    }

    public void DeactiveDialog()
    {
        droneHelperRoot.DeactiveDialog();
    }

    public void SetDialogName(string name)
    {
        droneHelperRoot.NameText = name;
    }

    public void DroneTextCall(string text)
    {
        if(ReferenceEquals(droneHelperRoot,null) == false)
        {
            droneHelperRoot.ShowText(text);
        }
    }

    public void ResetEffect()
    {
        _droneScaner.ResetEffect();
    }

    public void Respawn(Transform playerTransform)
    {
        _respawn = true;
        transform.SetParent(playerTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        droneVisual.localPosition = Vector3.zero;
        droneVisual.localRotation = Quaternion.identity;

        //_droneAnim.enabled = true;
        _floatingMoveComponent.enabled = false;
        
        //_droneAnim.SetTrigger("Respawn");
    }

    public void Gesture(Transform playerTransform, int num)
    {
        _respawn = true;
        transform.SetParent(playerTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        droneVisual.localPosition = Vector3.zero;
        droneVisual.localRotation = Quaternion.identity;

        //_droneAnim.enabled = true;
        //_floatingMoveComponent.enabled = false;

        //if(num == 1)
        //   _droneAnim.SetTrigger("Gesture1");
        //else
        //   _droneAnim.SetTrigger("Gesture2");
    }

    public void CompleteRespawn()
    {
        //_droneAnim.SetTrigger("Init");

        transform.SetParent(null);
        droneBody.localPosition = droneBodyOriginPos;
        _respawn = false;
        _targetPosition = (target.forward * respawnOffset.z + target.right * respawnOffset.x + Vector3.up * respawnOffset.y) + target.position;
        _finalTargetPosition = _targetPosition;
        transform.position = _finalTargetPosition;

        whenCompleteRespawn?.Invoke();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        _targetPosition = position;
        _finalTargetPosition = position;
    }

    public void OnScan(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        if (scanLeftCoolTime.Value <= 0.0f)
        {
            _timeCounter.InitSequencer("ScanProcess");
            _scanning = true;
        }
    }
}
