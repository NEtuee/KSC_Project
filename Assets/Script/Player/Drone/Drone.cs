using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MD;

public class Drone : UnTransfromObjectBase
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

    [SerializeField] private Transform target;
    [SerializeField] private DroneState state;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Vector3 defaultFollowOffset;
    [SerializeField] private float defaultFollowSpeed = 10.0f;
    [SerializeField] private float finalTargetLerpSpeed = 5.0f;
    private Vector3 _currentVelocity;
    [SerializeField] private Vector3 aimHelpOffset;
    [SerializeField] private Vector3 helpOffset;
    [SerializeField] private Vector3 helpGrabStateOffset;
    [SerializeField] private Vector3 respawnOffset;
    [SerializeField] private float collectRequiredTime = 1f;
    [SerializeField] private FloatingMove floatingMove;
    [SerializeField] private bool help = false;
    [SerializeField] private float scanCoolTime = 3f;
    private float _scanLeftTime = 0.0f;
    private Vector3 _targetPosition;
    private Vector3 _finalTargetPosition;
    private Vector3 _prevTargetPosition;
    [SerializeField]private DroneFollowState _followState = DroneFollowState.Forward;
    [SerializeField]private DroneFollowState _prevFollowState = DroneFollowState.Right;

    [Header("DroneVisual")]
    [SerializeField] private Transform droneVisual;
    [SerializeField] private Transform droneBody;
    private Vector3 droneBodyOriginPos;
    private Animator _droneAnim;
    private FloatingMove _floatingMoveComponent;
    private bool _respawn = false;

    private float collectStartTime;
    
    private Transform approachTarget;
    private Stack<Transform> orderList = new Stack<Transform>();
    private Transform mainCam;

    private Transform playerHead;
    private PlayerCtrl_Ver2 player;

    private DroneHelperRoot droneHelperRoot;
    
    //스캔
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

        AddAction(MessageTitles.set_setplayer, (msg) =>
        {
            player = (PlayerCtrl_Ver2)msg.data;
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
        target = player.transform;
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

        _droneAnim = droneVisual.GetComponent<Animator>();
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player.updateMethod != UpdateMethod.FixedUpdate)
            return;

        UpdateDrone(Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {

        //if (InputManager.Instance.GetInput(KeybindingActions.Scan) && _scanLeftTime <= 0.0f)
        //{
        //    Scan();
        //}

        if (player.updateMethod != UpdateMethod.Update)
            return;

        UpdateDrone(Time.deltaTime);
    }

    private void UpdateDrone(float deltaTime)
    {
        if (visible == false || _respawn)
            return;

        if(_scanLeftTime > 0.0f)
        {
            _scanLeftTime -= deltaTime;
            _scanLeftTime = Mathf.Clamp(_scanLeftTime, 0.0f, 10.0f);
        }

        switch(state)
        {
            case DroneState.Default:
                {

                    Vector3 camForward = Camera.main.transform.forward;
                    Vector3 camRight = Camera.main.transform.right;
                    camForward.y = 0;
                    camRight.y = 0;

                    
                    
                    if (player.IsMove)
                    {
                        if (Vector3.Dot(Vector3.Cross(camForward.normalized, target.forward), Vector3.up) > 0.9f )
                        {
                            if (_followState != DroneFollowState.Right)
                            {
                                _prevFollowState = _followState;
                                _followState = DroneFollowState.Right;
                            }
                        }
                        else if (Vector3.Dot(Vector3.Cross(camForward.normalized, target.forward), Vector3.up) < -0.9f )
                        {
                            if (_followState != DroneFollowState.Left)
                            {
                                _prevFollowState = _followState;
                                _followState = DroneFollowState.Left;
                            }
                        }
                        else if(Vector3.Dot(Vector3.Cross(camRight.normalized, target.forward), Vector3.up) < 0.0f)
                        {
                            if (_followState != DroneFollowState.Forward)
                            {
                                _prevFollowState = _followState;
                                _followState = DroneFollowState.Forward;
                            }
                        }
                        else
                        {
                            if (_followState != DroneFollowState.Backward)
                            {
                                _prevFollowState = _followState;
                                _followState = DroneFollowState.Backward;
                            }
                        }
                        //_targetPosition = (camForward * defaultFollowOffset.z + camRight * defaultFollowOffset.x + Vector3.up * defaultFollowOffset.y) + target.position;
                        //_targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;

                        if (_followState == DroneFollowState.Forward )
                        {
                            if (_prevFollowState == DroneFollowState.Left)
                            {
                                if (player.HorizonWeight < 0.0f)
                                    _prevFollowState = DroneFollowState.Right;
                                _targetPosition =
                                    (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x +
                                     target.up * defaultFollowOffset.y) + target.position;
                            }
                            else
                            {
                                if (player.HorizonWeight > 0.0f)
                                    _prevFollowState = DroneFollowState.Left;
                                _targetPosition =
                                    (target.forward * defaultFollowOffset.z + target.right * -defaultFollowOffset.x +
                                     target.up * defaultFollowOffset.y) + target.position;
                            }
                        }
                        else if (_followState == DroneFollowState.Backward)
                        {
                            if (_prevFollowState == DroneFollowState.Left)
                                _targetPosition =
                                    (target.forward * defaultFollowOffset.z + target.right * -defaultFollowOffset.x +
                                     target.up * defaultFollowOffset.y) + target.position;
                            else
                                _targetPosition =
                                    (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x +
                                     target.up * defaultFollowOffset.y) + target.position;
                        }
                        else if(_followState == DroneFollowState.Left)
                        {
                            if(_prevFollowState == DroneFollowState.Backward)
                                _targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
                            else 
                                _targetPosition = (target.forward * defaultFollowOffset.z + target.right * -defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
                        }
                        else if (_followState == DroneFollowState.Right)
                        {
                            if(_prevFollowState == DroneFollowState.Backward)
                                _targetPosition = (target.forward * defaultFollowOffset.z + target.right * -defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
                            else
                                _targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
                        }
                    }

                    if(player.IsNowClimbingBehavior() == true)
                    {
                        _targetPosition = (target.forward * helpGrabStateOffset.z + target.right * helpGrabStateOffset.x + Vector3.up * helpGrabStateOffset.y) + target.position;
                    }

                    if (Vector3.Distance(_targetPosition, transform.position) == 0.0f)
                        return;

                    //Vector3 targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
                    //Vector3 lookDir = targetPosition - transform.position;
                    //Vector3 lookDir = target.position+Vector3.up*1.5f - transform.position;
                    Vector3 lookDir = camForward;
                    //lookDir.y = 0.0f;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    _finalTargetPosition = Vector3.Lerp(_finalTargetPosition, _targetPosition, finalTargetLerpSpeed * deltaTime);
                    transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, _finalTargetPosition,ref _currentVelocity, deltaTime * defaultFollowSpeed), targetRot);
                    //transform.SetPositionAndRotation(_targetPosition, targetRot);

                }
                break;
            case DroneState.AimHelp:
                {
                    Vector3 targetPosition = (target.forward * aimHelpOffset.z + target.right * aimHelpOffset.x + target.up * aimHelpOffset.y) + target.position;
                    targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 12f);
                    Vector3 lookDir = playerHead.position - transform.position;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);
                }
                break;
            case DroneState.Help:
                {
                    Vector3 camForward = Camera.main.transform.forward;
                    Vector3 camRight = Camera.main.transform.right;
                    camForward.y = 0;
                    camRight.y = 0;


                    Vector3 targetPosition; 
                    if(player.IsNowClimbingBehavior() == false)
                        targetPosition= (camForward * helpOffset.z + camRight * helpOffset.x + Vector3.up * helpOffset.y) + target.position;
                    else
                        targetPosition = (target.forward * helpGrabStateOffset.z + target.right * helpGrabStateOffset.x + Vector3.up * helpGrabStateOffset.y) + target.position;

                    targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 5f);
                    Vector3 lookDir = playerHead.position - transform.position;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);
                }
                break;
            case DroneState.Approach:
                {
                    Vector3 targetPosition = Vector3.MoveTowards(transform.position, approachTarget.position, moveSpeed * deltaTime);
                    Vector3 lookDir = approachTarget.position - transform.position;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);

                    if ((transform.position - approachTarget.position).sqrMagnitude < 1f)
                    {
                        state = DroneState.Collect;
                        collectStartTime = Time.time;
                    }
                }
                break;
            case DroneState.Collect:
                {
                    if(Time.time - collectStartTime > collectRequiredTime)
                    {
                        if(orderList.Count == 0)
                        {
                            state = DroneState.Return;
                        }
                        else
                        {
                            state = DroneState.Approach;
                            approachTarget = orderList.Pop();
                        }
                    }
                }
                break;
            case DroneState.Return:
                {
                    Vector3 destination = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
                    Vector3 targetPosition = Vector3.MoveTowards(transform.position, destination, deltaTime * moveSpeed);
                    Vector3 lookDir = targetPosition - transform.position;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);

                    if ((transform.position - destination).sqrMagnitude <=2.0f)
                    {
                        _targetPosition = transform.position;
                        state = DroneState.Default;
                    }
                }
                break;
            case DroneState.Scan:
                {
                    Vector3 targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
                    targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 5f);

                    Quaternion targetRot;
                    targetRot = Quaternion.RotateTowards(transform.rotation,_scanTargetRotation,_rotationSpeed * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);

                    if (targetRot == _scanTargetRotation)
                    {
                        _droneScaner.Scanning();
                        state = DroneState.Default;
                        _targetPosition = transform.position;
                    }
                }
                break;
        }
    }

    public void Scan()
    {
        _scanLeftTime = scanCoolTime;
        Vector3 targetDir = Camera.main.transform.forward;
        targetDir.y = 0;
        targetDir.Normalize();

        _scanTargetRotation = Quaternion.LookRotation(targetDir,Vector3.up);
        state = DroneState.Scan;
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

    public void Respawn(Transform playerTransform)
    {
        _respawn = true;
        transform.SetParent(playerTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        droneVisual.localPosition = Vector3.zero;
        droneVisual.localRotation = Quaternion.identity;

        _droneAnim.enabled = true;
        _floatingMoveComponent.enabled = false;
        
        _droneAnim.SetTrigger("Respawn");
    }

    public void Gesture(Transform playerTransform, int num)
    {
        _respawn = true;
        transform.SetParent(playerTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        droneVisual.localPosition = Vector3.zero;
        droneVisual.localRotation = Quaternion.identity;

        _droneAnim.enabled = true;
        _floatingMoveComponent.enabled = false;

        if(num == 1)
           _droneAnim.SetTrigger("Gesture1");
        else
           _droneAnim.SetTrigger("Gesture2");
    }

    public void CompleteRespawn()
    {
        _droneAnim.SetTrigger("Init");

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

        if (_scanLeftTime <= 0.0f)
            Scan();
    }
}
