using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using MD;

public enum UpdateMethod
{
    FixedUpdate,
    Update
}

public class PlayerCtrl_Ver2 : PlayerCtrl
{
    public enum PlayerState
    {
        Default,
        TurnBack,
        RunToStop,
        Jump,
        Grab,
        ReadyGrab,
        HangLedge,
        HangEdge,
        ClimbingLedge,
        Ragdoll,
        LedgeUp,
        HangRagdoll,
        Aiming,
        ClimbingJump,
        ReadyClimbingJump,
        HangShake,
        Respawn,
        HighLanding,
        Gesture,
        Gesture2
    }

    public enum ClimbingJumpDirection
    {
        Up = 0,
        Left,
        Right,
        UpLeft,
        UpRight,
        Falling
    }

    public enum EMPLaunchType
    {
        ButtonDiff,
        Switching,
        Load
    }

    public UpdateMethod updateMethod;
    public GameObject hitMaker;
    public bool playerDebug;

    [SerializeField] private List<MeshRenderer> legBlur = new List<MeshRenderer>();
    [SerializeField] private Transform steamPosition;

    [Header("State")] [SerializeField] private bool isWalk = false;
    public bool IsWalk { get=> isWalk; set => isWalk = value; }
    [SerializeField] private PlayerState state;
    private PlayerState prevState;
    [SerializeField] private bool isClimbingMove = false;
    [SerializeField] private bool isLedge = false;
    [SerializeField] private Transform headTransform;
    [SerializeField] public bool isCanReadyClimbingCancel = false;
    [SerializeField] private bool isCanClimbingCancel = false;
    [SerializeField] private bool isClimbingGround = false;
    private bool _pressedJumpInput = false;
    public bool IsMove{ get { return movement.Speed == 0.0f ? false : true; } }

    [Header("Movement Speed Value")] [SerializeField]
    private float walkSpeed = 15.0f;

    [SerializeField] private float runSpeed = 25.0f;
    [SerializeField] private float aimingWalkSpeed = 5.5f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float prevSpeed;
    [SerializeField] private float rotateSpeed = 6.0f;
    [Range(0, 5)] [SerializeField] private float fallingControlSensitive = 1f;
    [SerializeField] private float horizonWeight = 0.0f;
    public float HorizonWeight => horizonWeight;
    [SerializeField] private float rotAngle = 0.0f;
    [SerializeField] private float airRotateSpeed = 2.5f;
    private float _runToStopTime = 0.0f;

    [Header("Jump Value")] 
    public bool pressJump = false;
    [SerializeField] private float currentJumpPower = 0f;
    [SerializeField] private float minJumpPower = -10.0f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float currentClimbingJumpPower = 0f;
    [SerializeField] private float climbingHorizonJumpPower = 5.0f;
    [SerializeField] private float climbingUpJumpPower = 8.0f;
    [SerializeField] private float keepClimbingJumpTime = 0.8f;
    [SerializeField] private float airTime = 0.0f;
    [SerializeField] private float landingFactor = 2.0f;
    [SerializeField] private AnimationCurve climbingHorizonJumpSpeedCurve;
    private float climbingJumpStartTime;
    private ClimbingJumpDirection climbingJumpDirection;


    [Header("LayerMask")] [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask climbingLayer;
    [SerializeField] private LayerMask climbingPaintLayer;
    [SerializeField] private LayerMask ledgeAbleLayer;
    [SerializeField] private LayerMask adjustAbleLayer;
    [SerializeField] private LayerMask frontCheckLayer;
    [SerializeField] private Vector3 detectionOffset;

    [Header("Input Record")] [SerializeField]
    private float currentVerticalValue = 0.0f;

    [SerializeField] private float currentHorizontalValue = 0.0f;
    [SerializeField] private float inputVertical;
    [SerializeField] private float inputHorizontal;
    [SerializeField] private float fixedVertical;
    [SerializeField] private float fixedHorizontal;
    [SerializeField] private float _gunPoseVerticalValue = 0.0f;
    [SerializeField] private float _gunPoseHorizonValue = 0.0f;
    [SerializeField] private AnimationCurve bandCurve;

    [Header("Move Direction")] [SerializeField]
    private Vector3 moveDir;
    private Vector3 lookDir;
    private Vector3 prevDir;
    private Vector3 camForward;
    private Vector3 camRight;

    [Header("Detection")] 
    [SerializeField] private LedgeChecker ledgeChecker;
    [SerializeField] private SpaceChecker spaceChecker;
    [SerializeField] private Vector3 wallUnderCheckOffset;

    [Header("Edge")] [SerializeField] private float hangAbleEdgeDist = 2f;

    [Header("EMP Lunacher")] 
    [SerializeField] private EMPGun empGun;

    [SerializeField] private float restoreValuePerSecond = 10f;
    [SerializeField] private float costValue = 25f;
    [SerializeField] private float chargeNecessaryTime = 1f;
    public FloatReactiveProperty chargeTime = new FloatReactiveProperty(0.0f);
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] public IntReactiveProperty loadCount = new IntReactiveProperty(0);
    [SerializeField] private float loadTerm = 2f;
    [SerializeField] private float loadTime = 0f;
    [SerializeField] private bool loading = false;
    [SerializeField] private float chargeDelayTime = 1f;
    private TimeCounterEx _chargeDelayTimer = new TimeCounterEx();
    [SerializeField] private GameObject pelvisGunObject;
    [SerializeField] private float dechargingDuration = 2.5f;
    [SerializeField] private float dechargingRatio = 0.5f;
    private float _emissionTargetValue = 10f;
    private Color _originalEmissionColor;
    [SerializeField] private bool decharging = false;
    [SerializeField] private TMPro.TextMeshProUGUI chargingCountText;
    private Color _chargingCountTextColor;
    private List<Material> pelvisGunMaterial = new List<Material>();
    private IEnumerator _dechargingCoroutine;
    [SerializeField] private Drone drone;
    [SerializeField] private AnimationCurve reloadWeightCurve;
    [SerializeField] private Animator gunAnim;

    [Header("EnergyRestore")] 
    [SerializeField] private float walkRestoreEnergyValue;

    [SerializeField] private float runRestoreEnergyValue;
    [SerializeField] private float aimRestoreEnergyValue;
    [SerializeField] private float climbingRestoreEnergyValue;
    [SerializeField] private float hitEnergyRestoreValue = 0.0f;
    [SerializeField] private float jumpEnergyRestoreValue = 5.0f;
    [SerializeField] private float climbingJumpEnergyRestoreValue;

    [Header("Spine")] 
    private Transform spine;
    [SerializeField] private Vector3 relativeVec;
    [SerializeField] private Transform lookAtAim;
    private Quaternion storeSpineRotation;

    [Header("HpPack")] public IntReactiveProperty hpPackCount = new IntReactiveProperty(0);
    [SerializeField] private float hpPackRestoreValue = 6.0f;
    [SerializeField] private float _hpPackRestoreDuration = 10.0f;
    [SerializeField] private bool isHpRestore = false;
    private IEnumerator restoreHpPackCoroutine;


    [Header("Stamina")] 
    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private float idleConsumeValue = 1f;
    [SerializeField] private float climbingMoveConsumeValue = 2f;
    [SerializeField] private float climbingJumpConsumeValue = 5f;
    [SerializeField] private float wallJumpConsumeValue = 5f;
    [SerializeField] private float staminaRestoreValue = 2f;
    [SerializeField] private float staminaRestoreDelayTime = 2f;
    private TimeCounterEx _staminaTimer;

    private float _turnOverTime = 0.0f;

    public delegate void ActiveAimEvent();

    public ActiveAimEvent activeAimEvent;

    public delegate void ReleaseAimEvent();

    public ReleaseAimEvent releaseAimEvent;

    public delegate void WhenTakeDamage();
    public WhenTakeDamage whenTakeDamage;


    private Rigidbody rigidbody;
    private CapsuleCollider collider;
    private Transform mainCameraTrasform;
    private Animator animator;
    private PlayerMovement movement;
    private PlayerRagdoll ragdoll;
    private IKCtrl footIK;
    private HandIKCtrl handIK;
    private RigCtrl rigCtrl;

    private RaycastHit wallHit;

    [SerializeField]private FMODUnity.StudioEventEmitter _charge;

    private int _transformCount = 0;
    private bool _runLock = false;
    private bool _aimLock = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        movement = GetComponent<PlayerMovement>();
        ragdoll = GetComponent<PlayerRagdoll>();
        footIK = GetComponent<IKCtrl>();
        handIK = GetComponent<HandIKCtrl>();

        if (animator != null)
        {
            headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
        }

        if (empGun != null)
        {
            empGun.Active(false);
        }

        moveDir = Vector3.zero;
        currentSpeed = 0.0f;
        mainCameraTrasform = Camera.main.transform;

        chargeTime.Value = 0.0f;

        // if(updateMethod == UpdateMethod.FixedUpdate)
        // {
        //     animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        // }

        spine = animator.GetBoneTransform(HumanBodyBones.Spine);

        _staminaTimer = new TimeCounterEx();
        _staminaTimer.InitTimer("Stamina", 0.0f, staminaRestoreDelayTime);

        _chargeDelayTimer.InitTimer("ChargeDelay", chargeDelayTime, chargeDelayTime);

        foreach(var renderer in pelvisGunObject.GetComponentsInChildren<Renderer>())
        {
            pelvisGunMaterial.Add(renderer.material);
            _originalEmissionColor = renderer.material.GetColor("_EmissionColor");
        }

        //pelvisGunMaterial = pelvisGunObject.GetComponent<Renderer>().material;
        //_originalEmissionColor = pelvisGunMaterial.GetColor("_EmissionColor");

        if (chargingCountText != null)
            _chargingCountTextColor = chargingCountText.color;

        //StartCoroutine(StopCheck());

    }

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("Player");

        AddAction(MessageTitles.player_initalizemove, (msg) => 
        {
            InitializeMove();
        });

        AddAction(MessageTitles.player_initVelocity, (msg) =>
        {
            InitVelocity();
        });

        AddAction(MessageTitles.player_visibledrone, (msg) =>
         {
             bool visible = (bool)msg.data;
             drone.Visible = visible;
         });

        AddAction(MessageTitles.fmod_soundEmitter, (msg) =>
         {
             _charge = (FMODUnity.StudioEventEmitter)msg.data;
         });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("PlayerManager"));
    }

    int ground = 0;
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Y))
        //{
        //    ground = ++ground >= 3 ? 0 : ground;
        //    GameManager.Instance.soundManager.SetGlobalParam(5,ground);
        //}

        //if (GameManager.Instance.optionMenuCtrl.CurrentTutorial == false && 
        //    state != PlayerState.Respawn &&
        //    InputManager.Instance.GetInput(KeybindingActions.Option) && 
        //    GameManager.Instance.optionMenuCtrl.sceneLoadUi.Loading == false &&
        //    dead == false)
        //{
        //    GameManager.Instance.optionMenuCtrl.InputEsc();
        //}

        if (isPause == true)
        {
            return;
        }

        // if (Input.GetKeyDown(KeyCode.E))
        //     animator.SetTrigger("Shot");

        //if (Input.GetKeyDown(KeyCode.Q))
        //    energy.Value = 100.0f;

        //InputUpdate();

        //if(Keyboard.current.jKey.wasPressedThisFrame == true)
        //{
        //    Action action;
        //    action = () =>
        //    {
        //        Debug.Log("Call Action");
        //    };
        //    SendMessageEx(MessageTitles.uimanager_fadeinout, GetSavedNumber("UIManager"), action);
        //}

        camForward = mainCameraTrasform.forward;
        camRight = mainCameraTrasform.right;
        camForward.y = 0;
        camRight.y = 0;

        UpdateInputValue(inputVertical, inputHorizontal);


        if (updateMethod == UpdateMethod.Update)
        {
            ProcessUpdate(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.PAUSE == true)
            return;


        UpdateStamina(Time.fixedDeltaTime);

        CheckRunToStop(Time.fixedDeltaTime);

        if (updateMethod == UpdateMethod.FixedUpdate)
        {
            ProcessUpdate(Time.fixedDeltaTime);
        }

        ProcessFixedUpdate();
    }

    //private void InputUpdate()
    //{
    //    camForward = mainCameraTrasform.forward;
    //    camRight = mainCameraTrasform.right;
    //    camForward.y = 0;
    //    camRight.y = 0;

    //    inputVertical = InputManager.Instance.GetMoveAxisVertical();
    //    inputHorizontal = InputManager.Instance.GetMoveAxisHorizontal();

    //    UpdateInputValue(inputVertical, inputHorizontal);

    //    //if (InputManager.Instance.GetInput(KeybindingActions.RunToggle))
    //    //{
    //    //    isRun = true;
    //    //}
    //    //else
    //    //{
    //    //    isRun = false;
    //    //}
    //    InputUseHpPack();
    //    InputRun();

    //    //if (Input.GetKeyDown(KeyCode.U))
    //    //    drone.Visible = true;

    //    if (InputManager.Instance.GetAdditionalKey(AdditionalType.First) && currentSpeed == 0.0f && state == PlayerState.Default && movement.isGrounded == true)
    //    {
    //        ChangeState(PlayerState.Gesture);
    //        GameManager.Instance.soundManager.Play(1026, Vector3.up, transform);
    //        return;
    //    }
    //    if (InputManager.Instance.GetAdditionalKey(AdditionalType.Second) && currentSpeed == 0.0f && state == PlayerState.Default && movement.isGrounded == true)
    //    {
    //        ChangeState(PlayerState.Gesture2);
    //        return;
    //    }

    //    switch (state)
    //    {
    //        case PlayerState.Default:
    //            {
    //                if (InputManager.Instance.GetInput(KeybindingActions.Jump) && pressJump == false)
    //                {
    //                    pressJump = true;
    //                    animator.SetTrigger("Jump");
    //                    return;
    //                }

    //                if (InputTryGrab())
    //                    return;

    //                if (InputAiming())
    //                    return;
    //            }
    //            break;
    //        case PlayerState.Jump:
    //            {
    //                if (InputAiming())
    //                    return;

    //                if (InputTryGrab())
    //                    return;
    //            }
    //            break;
    //        case PlayerState.Grab:
    //            {
    //                if (InputManager.Instance.GetInput(KeybindingActions.Jump))
    //                {
    //                    animator.SetTrigger("ClimbingCancel");
    //                    handIK.DisableHandIK();
    //                    return;
    //                }

    //                if (InputClimbingJump())
    //                    return;

    //                if (InputReleaseGrab())
    //                    return;

    //                if (isCanClimbingCancel == true)
    //                {
    //                    if (currentVerticalValue != 0.0f || currentHorizontalValue != 0.0f)
    //                    {
    //                        animator.SetTrigger("ClimbingCancel");
    //                        isCanClimbingCancel = false;
    //                    }
    //                }
    //            }
    //            break;
    //        case PlayerState.ReadyGrab:
    //            {
    //                if (InputReleaseGrab())
    //                    return;

    //                if (isCanReadyClimbingCancel == true && (inputVertical != 0 || inputHorizontal != 0))
    //                {
    //                    animator.SetTrigger("ReadyClimbCancel");
    //                    ChangeState(PlayerState.Grab);
    //                }
    //            }
    //            break;
    //        case PlayerState.HangEdge:
    //        case PlayerState.HangLedge:
    //            {
    //                if (InputReleaseGrab())
    //                    return;
    //                if (InputLedgeUp())
    //                    return;
    //            }
    //            break;
    //        case PlayerState.HangRagdoll:
    //        case PlayerState.HangShake:
    //            {
    //                if (InputReleaseGrab())
    //                    return;
    //            }
    //            break;
    //        case PlayerState.RunToStop:
    //            {
    //                if (InputTryGrab())
    //                    return;
    //            }
    //            break;
    //        case PlayerState.Aiming:
    //            {
    //                // if(loading == false && impactLoading == false && loadCount.Value < 3 && Input.GetKeyDown(KeyCode.LeftControl))
    //                // {
    //                //     loading = true;
    //                //     loadTime = 0f;
    //                //      empGun.GunLoad();
    //                // }
    //                _chargeDelayTimer.IncreaseTimerSelf("ChargeDelay", out bool limit, Time.deltaTime);
    //                if (limit)
    //                {
    //                    if (_charge == null)
    //                        _charge = GameManager.Instance.soundManager.Play(1013, Vector3.up, transform);

    //                    chargeTime.Value += Time.deltaTime * (decharging ? dechargingRatio : 1f);
    //                    chargeTime.Value = Mathf.Clamp(chargeTime.Value, 0.0f, Mathf.Abs(energy.Value / costValue));
    //                    chargeTime.Value = Mathf.Clamp(chargeTime.Value, 0.0f, 3.0f);

    //                    GameManager.Instance.soundManager.SetParam(1013, 10131, (chargeTime.Value) * 100f);

    //                    gunAnim.SetFloat("Energy", chargeTime.Value * 100.0f);

    //                    if (_transformCount < (int)chargeTime.Value)
    //                    {
    //                        GameManager.Instance.soundManager.Play(1019 + _transformCount, Vector3.up, transform);
    //                        _transformCount = (int)chargeTime.Value;

    //                    }
    //                }

    //                InputChargeShot();

    //                if (InputReleaseAiming())
    //                    return;
    //            }
    //            break;
    //        case PlayerState.ClimbingJump:
    //            {
    //                if (InputTryGrab())
    //                    return;
    //            }
    //            break;
    //    }
    //}

    private void ProcessUpdate(float deltaTime)
    {
        // if (rigidbody.velocity != Vector3.zero)
        // {
        //     rigidbody.velocity = Vector3.zero;
        // }

        if (state != PlayerState.Grab &&
            state != PlayerState.HangLedge &&
            state != PlayerState.HangEdge &&
            state != PlayerState.ClimbingJump &&
            movement.isGrounded == false &&
            state != PlayerState.ReadyGrab)
        {
            currentJumpPower -= gravity * deltaTime;
            currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, 50f);
        }
        else
        {
            currentJumpPower = 0.0f;
            InitVelocity();
        }

        RestoreEnergy(deltaTime);

        switch (state)
        {
            case PlayerState.Default:
            {
                if (movement.isGrounded == false && pressJump == false)
                {
                    ChangeState(PlayerState.Jump);
                    return;
                }

                if (inputVertical != 0.0f || inputHorizontal != 0.0f)
                {
                    moveDir = (camForward * inputVertical) + (camRight * inputHorizontal);
                    moveDir.Normalize();
                    lookDir = moveDir;
                    //prevDir = moveDir;
                }
                else
                {
                    moveDir = prevDir;
                    moveDir.Normalize();
                    lookDir = moveDir;
                }

                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, groundLayer))
                {
                    moveDir = (Vector3.ProjectOnPlane(transform.forward, hit.normal)).normalized;
                }

                Quaternion targetRotation = Quaternion.identity;
                if (currentSpeed != 0.0f && lookDir != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * rotateSpeed);
                }
                else
                {
                    targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(transform.forward, Vector3.up), deltaTime * rotateSpeed);
                }

                if (currentSpeed > 5.5f && targetRotation != Quaternion.identity)
                {
                    rotAngle = (int) Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up));
                    if (Vector3.Dot(Vector3.Cross(transform.forward, lookDir), transform.up) < 0)
                    {
                        rotAngle = -rotAngle;
                    }

                    horizonWeight = Mathf.Lerp(horizonWeight, rotAngle, deltaTime * rotateSpeed);
                }
                else
                {
                    horizonWeight = Mathf.Lerp(horizonWeight, 0.0f, deltaTime * 12f);
                }

                moveDir *= currentSpeed;

                animator.SetFloat("Speed", currentSpeed);
                animator.SetFloat("HorizonWeight", horizonWeight);

                if (Physics.Raycast(transform.position + collider.center, moveDir,
                    collider.radius + currentSpeed * deltaTime, frontCheckLayer) == false)
                {
                    movement.Move(moveDir);
                }

                    //if(movement.GroundDistance > movement.groundMinDistance)
                    //{
                    //    movement.Move(Vector3.down);
                    //}
                }
                break;
            case PlayerState.TurnBack:
            case PlayerState.RunToStop:
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, groundLayer))
                {
                    moveDir = (Vector3.ProjectOnPlane(transform.forward, hit.normal)).normalized;
                }

                if (movement.isGrounded == false)
                {
                    ChangeState(PlayerState.Jump);
                }
            }
                break;
            case PlayerState.Jump:
            {
                airTime += deltaTime;

                if (movement.isGrounded == true)
                {
                        if(airTime >= landingFactor)
                        {
                            ChangeState(PlayerState.HighLanding);
                            return;
                        }
                        else
                        {
                            ChangeState(PlayerState.Default);
                            return;
                        }
                }

                // Vector3 plusDir = ((camForward * inputVertical) + (camRight * inputHorizontal));
                // movement.Move(plusDir * fallingControlSenstive);

                if (inputVertical != 0.0f || inputHorizontal != 0.0f)
                {
                    moveDir = (camForward * inputVertical) + (camRight * inputHorizontal);
                    moveDir.Normalize();
                    lookDir = moveDir;
                    //prevDir = moveDir;
                }
                else
                {
                    moveDir = prevDir;
                    moveDir.Normalize();
                    lookDir = moveDir;
                }

                //lookDir = ((camForward * inputVertical) + (camRight * inputHorizontal)).normalized;
                // if (lookDir != Vector3.zero)
                // {
                //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * airRotateSpeed);
                // }
                // else
                // {
                //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), deltaTime * airRotateSpeed);
                // }

                Quaternion targetRotation = Quaternion.identity;
                if (currentSpeed != 0.0f && lookDir != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * rotateSpeed);
                }
                else
                {
                    Vector3 targetDir = transform.forward;
                    if (Vector3.Cross(transform.up, Vector3.up).normalized.x != 0.0f)
                    {
                        targetDir.Set(transform.up.x, 0.0f, transform.up.z);
                        targetDir.Normalize();
                    }

                    targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, deltaTime * rotateSpeed);
                }

                moveDir *= currentSpeed;

                if (rigidbody.velocity != Vector3.zero && moveDir != Vector3.zero)
                {
                    if (Vector3.Angle(moveDir.normalized, rigidbody.velocity.normalized) >= 90f)
                    {
                        AddVelocity((-rigidbody.velocity) * deltaTime);
                    }
                }

                if (Physics.Raycast(transform.position + collider.center, moveDir,
                    collider.radius + currentSpeed * deltaTime, frontCheckLayer))
                {
                    movement.Move(Vector3.up * currentJumpPower);
                }
                else
                {
                    movement.Move(moveDir + (Vector3.up * currentJumpPower));
                }
            }
                break;
            case PlayerState.Grab:
            {
                if (isCanClimbingCancel == true)
                {
                    if (currentVerticalValue != 0.0f || currentHorizontalValue != 0.0f)
                    {
                        animator.SetTrigger("ClimbingCancel");
                        isCanClimbingCancel = false;
                    }
                }

                if (stamina.Value <= 0.0f)
                {
                    isClimbingMove = false;
                    isLedge = false;

                    Vector3 currentRot = transform.rotation.eulerAngles;
                    currentRot.x = 0.0f;
                    currentRot.z = 0.0f;
                    transform.rotation = Quaternion.Euler(currentRot);

                    climbingJumpDirection = ClimbingJumpDirection.Falling;

                    movement.Detach();

                    ChangeState(PlayerState.Default);
                    return;
                }

                CheckLedge();
            }
                break;
            case PlayerState.ReadyGrab:
            {
                if (ledgeChecker.IsDetectedLedge() == true)
                {
                    ChangeState(PlayerState.Grab);
                    ChangeState(PlayerState.HangLedge);
                }

                if (isCanReadyClimbingCancel == true && (inputVertical != 0 || inputHorizontal != 0))
                {
                    animator.SetTrigger("ReadyClimbCancel");
                    ChangeState(PlayerState.Grab);
                }
            }
                break;
            case PlayerState.HangRagdoll:
            {
            }
                break;
            case PlayerState.HangLedge:
            {
            }
                break;
            case PlayerState.Aiming:
            {
                if (loading == true)
                {
                    loadTime += deltaTime;
                    if (loadTime > loadTerm)
                    {
                        loadCount.Value++;
                        loadTime = 0;
                        loading = false;
                    }
                }

                RaycastHit hit;

                if (movement.isGrounded == true)
                {
                    if (inputVertical != 0.0f || inputHorizontal != 0.0f)
                    {
                        moveDir = (camForward * inputVertical) + (camRight * inputHorizontal);
                        moveDir.Normalize();
                    }
                    else
                    {
                        moveDir = prevDir;
                        moveDir.Normalize();
                    }

                    Vector3 aimDir = camForward;
                    lookDir = aimDir;

                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(aimDir, Vector3.up), 30.0f * deltaTime);

                    if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, groundLayer))
                    {
                        moveDir = (Vector3.ProjectOnPlane(moveDir, hit.normal)).normalized;
                    }

                    moveDir *= currentSpeed;

                    animator.SetFloat("Speed", currentSpeed);
                    movement.Move(moveDir);
                }
                else
                {
                    moveDir = transform.forward * currentSpeed;

                    Vector3 plusDir = ((camForward * inputVertical) + (camRight * inputHorizontal));
                    movement.Move(plusDir * fallingControlSensitive);

                    lookDir = ((camForward * inputVertical) + (camRight * inputHorizontal)).normalized;
                    if (lookDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation,
                            Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * 30.0f);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation,
                            Quaternion.LookRotation(transform.forward, Vector3.up), deltaTime * 30.0f);
                    }

                    movement.Move(moveDir + (Vector3.up * currentJumpPower));
                }

                _chargeDelayTimer.IncreaseTimerSelf("ChargeDelay", out bool limit, Time.deltaTime);
                if (limit)
                {
                        //if(_charge == null)
                        //_charge = GameManager.Instance.soundManager.Play(1013, Vector3.up, transform);

                        if (_charge == null)
                        {
                            AttachSoundPlayData chargeSoundPlayData = MessageDataPooling.GetMessageData<AttachSoundPlayData>(); ;
                            chargeSoundPlayData.id = 1013; chargeSoundPlayData.localPosition = Vector3.up; chargeSoundPlayData.parent = transform; chargeSoundPlayData.returnValue = true;
                            SendMessageQuick(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), chargeSoundPlayData);
                        }

                        chargeTime.Value += Time.deltaTime * (decharging ? dechargingRatio : 1f);
                        chargeTime.Value = Mathf.Clamp(chargeTime.Value, 0.0f, Mathf.Abs(energy.Value / costValue));
                        chargeTime.Value = Mathf.Clamp(chargeTime.Value, 0.0f, 3.0f);

                        //GameManager.Instance.soundManager.SetParam(1013,10131,(chargeTime.Value) * 100f);
                        SetParameterData setParameterData = MessageDataPooling.GetMessageData<SetParameterData>();
                        setParameterData.soundId = 1013; setParameterData.paramId = 10131; setParameterData.value = (chargeTime.Value) * 100f;
                        SendMessageEx(MessageTitles.fmod_setParam, GetSavedNumber("FMODManager"), setParameterData);

                        gunAnim.SetFloat("Energy", chargeTime.Value * 100.0f);

                    if(_transformCount < (int)chargeTime.Value)
                    {
                            //GameManager.Instance.soundManager.Play(1019 + _transformCount, Vector3.up, transform);
                        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                        soundData.id = 1019 + _transformCount; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData); 
                        _transformCount = (int)chargeTime.Value;
                    }
                }
            }
                break;
            case PlayerState.ClimbingJump:
            {
                if (movement.isGrounded == true)
                {
                    ChangeState(PlayerState.Default);
                    return;
                }

                Vector3 climbingJumpDir = Vector3.zero;
                Vector3 upDirect = Vector3.zero;
                switch (climbingJumpDirection)
                {
                    case ClimbingJumpDirection.Up:
                        currentClimbingJumpPower -= gravity * deltaTime;
                        currentClimbingJumpPower = Mathf.Clamp(currentClimbingJumpPower, minJumpPower, 50f);
                        moveDir = transform.up;
                        break;
                    case ClimbingJumpDirection.UpLeft:
                        currentClimbingJumpPower -= gravity * deltaTime;
                        currentClimbingJumpPower = Mathf.Clamp(currentClimbingJumpPower, minJumpPower, 50f);
                        if (currentClimbingJumpPower > 0)
                        {
                            moveDir = (transform.up + -transform.right).normalized;
                        }
                        else
                        {
                            moveDir = transform.up;
                        }

                        break;
                    case ClimbingJumpDirection.UpRight:
                        currentClimbingJumpPower -= gravity * deltaTime;
                        currentClimbingJumpPower = Mathf.Clamp(currentClimbingJumpPower, minJumpPower, 50f);
                        if (currentClimbingJumpPower > 0)
                        {
                            moveDir = (transform.up + transform.right).normalized;
                        }
                        else
                        {
                            moveDir = transform.up;
                        }

                        break;
                    case ClimbingJumpDirection.Left:
                    {
                        moveDir = -transform.right;
                        float normalizeTime = (Time.time - climbingJumpStartTime) / keepClimbingJumpTime;
                        if (normalizeTime < 0.5f)
                        {
                            upDirect = transform.up * 3f;
                        }
                        else
                        {
                            upDirect = -transform.up * 3f;
                        }

                        currentClimbingJumpPower = climbingHorizonJumpPower *
                                                   climbingHorizonJumpSpeedCurve.Evaluate(normalizeTime);
                    }
                        break;
                    case ClimbingJumpDirection.Right:
                    {
                        moveDir = transform.right;
                        float normalizeTime = (Time.time - climbingJumpStartTime) / keepClimbingJumpTime;
                        if (normalizeTime < 0.5f)
                        {
                            upDirect = transform.up * 3f;
                        }
                        else
                        {
                            upDirect = -transform.up * 3f;
                        }

                        currentClimbingJumpPower = climbingHorizonJumpPower *
                                                   climbingHorizonJumpSpeedCurve.Evaluate(normalizeTime);
                    }
                        break;
                }

                moveDir *= currentClimbingJumpPower;
                Vector3 finalDir = moveDir + upDirect;
                movement.Move(finalDir);

                if (Time.time - climbingJumpStartTime >= keepClimbingJumpTime)
                {
                    moveDir = moveDir.normalized * finalDir.magnitude;
                    movement.ClimbingJump();
                    ChangeState(PlayerState.Jump);
                    if (climbingJumpDirection != ClimbingJumpDirection.Left &&
                        climbingJumpDirection != ClimbingJumpDirection.Right)
                        currentJumpPower = currentClimbingJumpPower;
                    TryGrab();
                }
            }
                break;
        }

        UpdateCurrentSpeed(deltaTime);

        prevDir = state == PlayerState.Aiming ? moveDir : lookDir;
    }

    private void ProcessFixedUpdate()
    {
          animator.SetBool("IsGround", pressJump == false ? movement.isGrounded : false);
           
          bool isNearGround =Physics.Raycast(transform.position, -transform.up, 1.0f, groundLayer);
          animator.SetBool("IsNearGround",isNearGround);

          switch (state)
        {
            case PlayerState.Grab:
            {
                UpdateGrab();

                //if (_turnOverTime > 0.5f)
                //{
                //    _turnOverTime = 0.0f;
                //    ChangeState(PlayerState.HangShake);
                //    return;
                //}

                float climbingPlaneAngle = Vector3.Dot(Vector3.Cross(transform.up, Vector3.right), Vector3.forward);

                //Debug.Log(climbingPlaneAngle*Mathf.Rad2Deg);
                isClimbingGround = climbingPlaneAngle > -15f * Mathf.Deg2Rad;

                if (climbingPlaneAngle >= 0.3f)
                {
                    _turnOverTime += Time.fixedDeltaTime;
                }
                else
                {
                    _turnOverTime = 0.0f;
                }
            }
                break;
            case PlayerState.HangEdge:
            case PlayerState.HangLedge:
            case PlayerState.ReadyGrab:
            {
                UpdateGrab();
            }
                break;
        }
    }

    private void LateUpdate()
    {
        switch (state)
        {
            case PlayerState.Aiming:
            {
                if (spine != null)
                {
                    Vector3 dir = (spine.position - lookAtAim.position).normalized;
                    Quaternion originalRot = spine.rotation;
                    var spineRotation = spine.rotation;
                    spineRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(relativeVec);
                    spineRotation *= Quaternion.Inverse(transform.rotation);
                    spineRotation *= originalRot;
                    spine.rotation = spineRotation;
                    //spine.rotation = spineRotation;
                    // spine.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(relativeVec);
                    // spine.rotation *= Quaternion.Inverse(transform.rotation);
                    // spine.rotation *= originalRot;
                }
            }
                break;
        }
    }

    private void UpdateInputValue(float vertical, float horizontal)
    {
        animator.SetFloat("InputVertical", Mathf.Abs(inputVertical));
        animator.SetFloat("InputHorizon", Mathf.Abs(inputHorizontal));
        animator.SetFloat("InputHorizonNoAbs", inputHorizontal);
        animator.SetFloat("InputVerticalValue", _gunPoseVerticalValue);
        animator.SetFloat("InputHorizonValue", _gunPoseHorizonValue);

        if (state == PlayerState.Default)
        {
            if (vertical == 1.0f)
            {
                currentVerticalValue = 1.0f;
            }
            else if (vertical == -1.0f)
            {
                currentVerticalValue = -1.0f;
            }
            else
            {
                currentVerticalValue = 0.0f;
            }

            if (horizontal == 1.0f)
            {
                currentHorizontalValue = 1.0f;
            }
            else if (horizontal == -1.0f)
            {
                currentHorizontalValue = -1.0f;
            }
            else
            {
                currentHorizontalValue = 0.0f;
            }

            return;
        }

        if ((state == PlayerState.Grab || state == PlayerState.HangLedge || state == PlayerState.HangEdge) &&
            isClimbingMove == false)
        {
            //if (_pressedJumpInput == true)
            //{
            //    return;
            //}

            if (vertical == 0.0f)
            {
                currentVerticalValue = 0.0f;
            }
            else if (vertical >= 0.0f)
            {
                currentVerticalValue = 1.0f;
            }
            else
            {
                if (movement.isGrounded == false)
                {
                    currentVerticalValue = -1.0f;
                }
                else
                {
                    currentVerticalValue = 0.0f;
                }
            }

            if (horizontal > 0.0f)
            {
                currentHorizontalValue = Mathf.Ceil(horizontal);
            }
            else
            {
                currentHorizontalValue = Mathf.Floor(horizontal);
            }

            if (isLedge == false)
            {
                if (currentVerticalValue != 0.0f)
                {
                    if (currentVerticalValue > 0.0f)
                    {
                        if (currentHorizontalValue == 0.0f)
                        {
                            if (UpDetection() == false)
                            {
                                animator.SetTrigger("UpClimbing");
                                isClimbingMove = true;
                            }

                            //animator.SetTrigger("UpClimbing");
                            // isClimbingMove = true;
                        }
                        else
                        {
                            if (currentHorizontalValue > 0.0f)
                            {
                                animator.SetTrigger("UpRightClimbing");
                                isClimbingMove = true;
                            }
                            else
                            {
                                animator.SetTrigger("UpLeftClimbing");
                                isClimbingMove = true;
                            }
                        }
                    }
                    else
                    {
                        if (currentHorizontalValue == 0.0f)
                        {
                            if (DownDetection() == true)
                            {
                                animator.SetTrigger("DownClimbing");
                                isClimbingMove = true;
                            }
                        }
                        else
                        {
                            if (currentHorizontalValue > 0.0f)
                            {
                                animator.SetTrigger("DownRightClimbing");
                                isClimbingMove = true;
                            }
                            else
                            {
                                animator.SetTrigger("DownLeftClimbing");
                                isClimbingMove = true;
                            }
                        }
                    }
                }
                else
                {
                    if (currentHorizontalValue > 0.0f)
                    {
                        animator.SetTrigger("RightClimbing");
                        isClimbingMove = true;
                    }
                    else if (currentHorizontalValue < 0.0f)
                    {
                        animator.SetTrigger("LeftClimbing");
                        isClimbingMove = true;
                    }
                }
            }
            else
            {
                if (currentVerticalValue == -1.0f)
                {
                    if (state != PlayerState.HangEdge)
                    {
                        ChangeState(PlayerState.Grab);
                        animator.SetBool("IsLedge", false);
                        animator.SetTrigger("DownClimbing");

                        isLedge = false;
                        isClimbingMove = true;
                    }
                }
                else if (currentVerticalValue == 0.0f && currentHorizontalValue != 0.0f)
                {
                    if (currentHorizontalValue == 1.0f)
                    {
                        animator.SetTrigger("RightClimbing");
                        isClimbingMove = true;
                    }
                    else
                    {
                        animator.SetTrigger("LeftClimbing");
                        isClimbingMove = true;
                    }
                }
            }
        }
    }

    private void UpdateCurrentSpeed(float deltaTime)
    {
        Vector3 moveForward = lookDir;
        Vector3 prevForward = prevDir;
        moveForward.y = prevForward.y = 0.0f;
        moveForward.Normalize();
        prevForward.Normalize();

        //if(currentSpeed > 0f)
        float speedFactor = (currentSpeed / runSpeed) * 0.002f;
        float dot = Vector3.Dot(transform.forward, camRight);
        float dotY = Vector3.Dot(transform.forward, camForward);
        dotY *= 0.6f;
        //dot = Mathf.Clamp(MathEx.abs(dot),0.2f,1f);
        bool active = false; //currentSpeed > 7f && movement.isGrounded;

        foreach (var leg in legBlur)
        {
            leg.gameObject.SetActive(active);
            leg.material.SetFloat("_XOffset", speedFactor * dot);
            leg.material.SetFloat("_YOffset", speedFactor * dotY);
        }

        if (state == PlayerState.Grab || state == PlayerState.RunToStop || state == PlayerState.HighLanding)
        {
            return;
        }

        if (state == PlayerState.Default && currentSpeed > 0.0f && Vector3.Dot(moveForward, prevForward) < -0.5f)
        {
            if (currentSpeed > walkSpeed)
            {
                ChangeState(PlayerState.TurnBack);
            }

            return;
        }

        if (inputVertical != 0 || inputHorizontal != 0)
        {
            if (state != PlayerState.Aiming)
            {
                if (isWalk == true || _runLock)
                {
                    currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, deltaTime * 20.0f);
                }
                else
                {
                    currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, deltaTime * 20.0f);
                }
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, aimingWalkSpeed, deltaTime * 20.0f);
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, deltaTime * 40.0f);
        }

        if (inputVertical != 0)
        {
            _gunPoseVerticalValue =
                Mathf.MoveTowards(_gunPoseVerticalValue, inputVertical < 0 ? -1f : 1f, deltaTime * 5.0f);
        }
        else
        {
            _gunPoseVerticalValue = Mathf.MoveTowards(_gunPoseVerticalValue, 0.0f, deltaTime * 5.0f);
        }

        if (inputHorizontal != 0)
        {
            _gunPoseHorizonValue =
                Mathf.MoveTowards(_gunPoseHorizonValue, inputHorizontal < 0 ? -1f : 1f, deltaTime * 5.0f);
        }
        else
        {
            _gunPoseHorizonValue = Mathf.MoveTowards(_gunPoseHorizonValue, 0.0f, deltaTime * 5.0f);
        }
    }

    public void AddJumpPower(float value)
    {
        currentJumpPower += value;
    }

    public void SetJumpPower(float value)
    {
        if (dead)
            return;
        currentJumpPower = value;
    }

    public void Jump()
    {
        currentJumpPower = jumpPower;
        movement.Jump();
        prevSpeed = currentSpeed;
        climbingJumpDirection = ClimbingJumpDirection.Up;
        AddEnergyValue(jumpEnergyRestoreValue);
        ChangeState(PlayerState.Jump);
    }

    public void ClimbingSound()
    {
        //GameManager.Instance.soundManager.Play(1006, Vector3.up, transform);
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>(); ;
        soundData.id = 1006; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
    }

    public void ChangeState(PlayerState changeState)
    {
        prevState = state;
        state = changeState;

        if(changeState != PlayerState.Aiming)
        {
            if(_charge != null)
            {
                _charge.Stop();
                _charge = null;
            }
        }

        switch (prevState)
        {
            case PlayerState.Aiming:
            {
                    ActiveAim(false);

                    //if(changeState == PlayerState.Default)
                    //{
                    //    Debug.Log("!!!!!!!");
                    //}

                    if (!_aimLock)
                        SendMessageEx(MessageTitles.cameramanager_activeplayerfollocamera, GetSavedNumber("CameraManager"), null);
                    //GameManager.Instance.cameraManager.ActivePlayerFollowCamera();
                drone.OrderAimHelp(false);
                    //releaseAimEvent?.Invoke();
                    BoolData data = MessageDataPooling.GetMessageData<BoolData>();
                    data.value = false;
                    SendMessageEx(MessageTitles.uimanager_activegunui, GetSavedNumber("UIManager"), data);
                }
                break;
            case PlayerState.Jump:
            {
                if (changeState == PlayerState.Default)
                {
                        //GameManager.Instance.soundManager.Play(1004, Vector3.up, transform);
                        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                        soundData.id = 1004; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
                }
            }
                break;
            case PlayerState.HangEdge:
            case PlayerState.HangLedge:
            {
                animator.SetBool("IsLedge", false);
                isLedge = false;
            }
                break;
            case PlayerState.Respawn:
                {
                    if (state == PlayerState.Default)
                        Debug.Log("change default");
                }
                break;
        }

        switch (state)
        {
            case PlayerState.Default:
            {
                collider.isTrigger = false;
                animator.applyRootMotion = false;
                animator.SetBool("IsGrab", false);
                animator.SetBool("IsLedge", false);
                animator.SetTrigger("Landing");
                footIK.EnableFeetIk();
                handIK.DisableHandIK();
                //GameManager.Instance.stateManager.Visible(false);

                //임시
                collider.height = 1.898009f;
                collider.center = new Vector3(0.0f, 0.95622f, 0.0f);

                    //GameManager.Instance.cameraManager.SetFollowCameraDistance("Default");
                    StringData data = MessageDataPooling.GetMessageData<StringData>();
                    data.value = "Default";
                    SendMessageEx(MessageTitles.cameramanager_setfollowcameradistance, GetSavedNumber("CameraManager"), data);

                    airTime = 0.0f;
                // else
                //     GameManager.Instance.cameraManager.SetFollowCameraDistance("ExistParent");
            }
                break;
            case PlayerState.Grab:
            {
                animator.SetBool("IsGrab", true);
                currentJumpPower = 0.0f;
                currentSpeed = 0.0f;

                if (prevState != PlayerState.HangLedge)
                {
                    currentVerticalValue = 0.0f;
                    currentHorizontalValue = 0.0f;
                }

                //임시
                collider.height = 1f;
                collider.center = new Vector3(0.0f, 0.5f, 0.0f);

                handIK.ActiveHandIK(true);
                handIK.ActiveLedgeIK(false);
                footIK.DisableFeetIk();
                isClimbingMove = false;
                movement.SetGrab();
                    //GameManager.Instance.cameraManager.SetFollowCameraDistance("Grab");
                    StringData data = MessageDataPooling.GetMessageData<StringData>();
                    data.value = "Grab";
                SendMessageEx(MessageTitles.cameramanager_setfollowcameradistance, GetSavedNumber("CameraManager"), data);

                }
                break;
            case PlayerState.ReadyGrab:
            {
                animator.SetBool("IsGrab", true);
                animator.SetInteger("ReadyClimbNum", (int) climbingJumpDirection);
                currentJumpPower = 0.0f;
                currentSpeed = 0.0f;

                //handIK.ActiveHandIK(true);
                handIK.ActiveLedgeIK(false);
                footIK.DisableFeetIk();
                isCanReadyClimbingCancel = false;

                currentVerticalValue = 0.0f;
                currentHorizontalValue = 0.0f;
                isClimbingMove = false;
                movement.SetGrab();

                    animator.ResetTrigger("RightClimbing");
                    animator.ResetTrigger("LeftClimbing");
                    animator.ResetTrigger("UpClimbing");
                    animator.ResetTrigger("DownClimbing");
                    animator.ResetTrigger("UpLeftClimbing");
                    animator.ResetTrigger("UpRightClimbing");
                    animator.ResetTrigger("DownLeftClimbing");
                    animator.ResetTrigger("DownRightClimbing");

                    airTime = 0.0f;
                }
                break;
            case PlayerState.Jump:
            {
                if (prevState != PlayerState.ClimbingJump)
                    moveDir = transform.forward * currentSpeed;
                horizonWeight = 0.0f;
                animator.SetFloat("HorizonWeight", horizonWeight);
            }
                break;
            case PlayerState.TurnBack:
            {
                animator.applyRootMotion = true;
                    //GameManager.Instance.soundManager.Play(1018, Vector3.up, transform);
                    AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                    soundData.id = 1018; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                    SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
                animator.SetTrigger("TurnBack");
            }
                break;
            case PlayerState.RunToStop:
            {
                animator.applyRootMotion = true;

                    //AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                    //soundData.id = 1002; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                    //SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
                    //GameManager.Instance.soundManager.Play(1002, Vector3.up, transform);
                if (currentSpeed > walkSpeed)
                {
                    animator.SetTrigger("FastStop");
                }
                else
                {
                    animator.SetTrigger("SlowStop");
                }

                currentSpeed = 0.0f;
                animator.SetFloat("Speed", 0.0f);
                horizonWeight = 0.0f;
                animator.SetFloat("HorizonWeight", horizonWeight);
            }
                break;
            case PlayerState.Ragdoll:
            {
                    if (prevState == PlayerState.Gesture)
                        drone.CompleteRespawn();

                if (lookDir == Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
                }

                movement.SetParent(null);
                handIK.DisableHandIK();

                    airTime = 0.0f;
                }
                break;
            case PlayerState.HangRagdoll:
            {
                handIK.DisableHandIK();
                ragdoll.ActiveLeftHandFixRagdoll();

                    airTime = 0.0f;
                }
                break;
            case PlayerState.Aiming:
            {
                    //GameManager.Instance.cameraManager.ActiveAimCamera();
                    //GameManager.Instance.stateManager.Visible(false);
                SendMessageEx(MessageTitles.cameramanager_activeaimcamera, GetSavedNumber("CameraManager"), null);
                    BoolData visibleDisable = MessageDataPooling.GetMessageData<BoolData>();
                    visibleDisable.value = false;
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), visibleDisable);

                footIK.DisableFeetIk();
                drone.OrderAimHelp(true);
                activeAimEvent?.Invoke();

                    BoolData data = MessageDataPooling.GetMessageData<BoolData>();
                    data.value = true;
                SendMessageEx(MessageTitles.uimanager_activegunui, GetSavedNumber("UIManager"), data);
                _transformCount = 0;
            }
                break;
            case PlayerState.HangEdge:
            case PlayerState.HangLedge:
                {
                    isLedge = true;
                    isClimbingMove = false;
                    animator.SetBool("IsLedge", true);
                    handIK.ActiveLedgeIK(true);
                    AdjustLedgeOffset();
                }
                break;
            case PlayerState.LedgeUp:
            {
                handIK.DisableHandIK();
                collider.isTrigger = true;
            }
                break;
            case PlayerState.ClimbingJump:
            {
                climbingJumpStartTime = Time.time;

                    AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                    soundData.id = 1007; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                    SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
                    //GameManager.Instance.soundManager.Play(1007, Vector3.up, transform);

                if (climbingJumpDirection == ClimbingJumpDirection.Left ||
                    climbingJumpDirection == ClimbingJumpDirection.Right)
                    currentClimbingJumpPower = climbingHorizonJumpPower;
                else
                    currentClimbingJumpPower = climbingUpJumpPower;

                stamina.Value -= climbingJumpConsumeValue;
                stamina.Value = Mathf.Clamp(stamina.Value, 0.0f, maxStamina);
                    AddEnergyValue(climbingJumpEnergyRestoreValue);
            }
                break;
            case PlayerState.ReadyClimbingJump:
            {
                if (inputVertical >= 0.5f)
                {
                    if (inputHorizontal == 0.0f)
                        climbingJumpDirection = ClimbingJumpDirection.Up;
                    else if (inputHorizontal > 0.0f)
                        climbingJumpDirection = ClimbingJumpDirection.UpRight;
                    else if (inputHorizontal < 0.0f)
                        climbingJumpDirection = ClimbingJumpDirection.UpLeft;
                }
                else
                {
                    if (inputHorizontal >= 0.5f)
                    {
                        climbingJumpDirection = ClimbingJumpDirection.Right;
                    }
                    else if (inputHorizontal <= -0.5f)
                    {
                        climbingJumpDirection = ClimbingJumpDirection.Left;
                    }
                    else
                    {
                        Vector3 backVector = -transform.forward;
                        backVector.y = 0f;
                        transform.rotation = Quaternion.LookRotation(backVector);

                        moveDir = transform.forward;
                        moveDir.Normalize();
                        currentSpeed = runSpeed;
                        moveDir *= currentSpeed;
                        currentJumpPower = jumpPower * 0.5f;
                        transform.position = transform.position +
                                             (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;

                        animator.SetBool("IsGrab", false);

                        stamina.Value -= climbingJumpConsumeValue;
                        stamina.Value = Mathf.Clamp(stamina.Value, 0.0f, maxStamina);

                        SetVelocity(moveDir);

                        handIK.DisableHandIK();
                        Jump();
                        ChangeState(PlayerState.Jump);

                        return;
                    }
                }

                handIK.DisableHandIK();
                animator.SetBool("IsGrab", false);
                animator.SetTrigger("ClimbingJump");
            }
                break;
            case PlayerState.HangShake:
            {
                ragdoll.ActiveHangShake();
            }
                break;
            case PlayerState.Respawn:
            {
                loadTime = 0.0f;
                animator.SetFloat("Speed", 0.0f);
                InitVelocity();

                if (prevState == PlayerState.Ragdoll)
                {
                    ragdoll.ResetRagdoll();
                }
                //GameManager.Instance.optionMenuCtrl.respawnFadeCtrl.FadeInOut(() =>
                //{ 
                //    animator.SetBool("Respawn",true);
                //    drone.Respawn(transform);
                //});
                    ActionData action = MessageDataPooling.GetMessageData<ActionData>();
                    action.value = () =>
                    {
                        animator.SetBool("Respawn", true);
                        drone.Respawn(transform);
                    };
                    SendMessageEx(MessageTitles.uimanager_fadeinout, GetSavedNumber("UIManager"), action);
            }
                break;
            case PlayerState.HighLanding:
                {
                    currentSpeed = 0.0f;
                    animator.SetFloat("Speed", 0.0f);
                    animator.SetBool("HighLanding",true);
                    AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                    soundData.id = 1004; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                    SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
                    //GameManager.Instance.soundManager.Play(1004, Vector3.up, transform);
                    //GameManager.Instance.cameraManager.GenerateRecoilImpulse();
                    SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse, GetSavedNumber("CameraManager"), null);
                }
                break;
            case PlayerState.Gesture:
                {
                    animator.SetTrigger("Gesture1");
                    drone.Gesture(transform , 1);
                }
                break;
            case PlayerState.Gesture2:
                {
                    animator.SetTrigger("Gesture2");
                    drone.Gesture(transform, 2);
                }
                break;
        }
    }

    IEnumerator StopCheck()
    {
        float time = 0.0f;

        while (true)
        {
            Debug.Log(time);
            if (time >= 0.05f && currentSpeed > walkSpeed && inputVertical == 0.0f && inputHorizontal == 0.0f)
            {
                ChangeState(PlayerState.RunToStop);
                time = 0.0f;
            }

            if (state == PlayerState.Default && currentSpeed > 0.0f && inputVertical == 0.0f && inputHorizontal == 0.0f)
            {
                time += Time.deltaTime;
            }
            else
            {
                time = 0.0f;
            }

            yield return null;
        }
    }

    private void CheckRunToStop(float deltaTime)
    {
        if (_runToStopTime >= 0.05f && currentSpeed > walkSpeed && inputVertical == 0.0f && inputHorizontal == 0.0f)
        {
            ChangeState(PlayerState.RunToStop);
            _runToStopTime = 0.0f;
        }

        if (state == PlayerState.Default && currentSpeed > 0.0f && inputVertical == 0.0f && inputHorizontal == 0.0f)
        {
            _runToStopTime += deltaTime;
        }
        else
        {
            _runToStopTime = 0.0f;
        }
    }

    void OnAnimatorMove()
    {
        switch (state)
        {
            case PlayerState.Default:
            {
            }
                break;
            case PlayerState.TurnBack:
            {
                transform.position += moveDir.normalized * animator.deltaPosition.magnitude;
                var r = transform.rotation;
                r *= animator.deltaRotation;
                transform.rotation = r;
            }
                break;
            case PlayerState.RunToStop:
            {
                if (Physics.Raycast(transform.position + Vector3.up + moveDir.normalized * 0.5f, Vector3.down, 1.5f,
                    groundLayer))
                {
                    transform.position += moveDir.normalized * animator.deltaPosition.magnitude;
                }
            }
                break;
            case PlayerState.HangEdge:
            case PlayerState.HangLedge:
            {
                if (isClimbingMove == true)
                {
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-wallHit.normal, Vector3.up), 5f * Time.deltaTime);
                }

                var p = transform.position;
                p += animator.deltaPosition;
                transform.position = p;
            }
                break;
            case PlayerState.Grab:
            {
                if (isClimbingMove == true)
                {
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-wallHit.normal, Vector3.up), 5f * Time.deltaTime);
                }

                if (CheckCanClimbingMoveByVertexColor() == false)
                    break;

                var p = transform.position;
                p += animator.deltaPosition;
                //transform.position = p;

                if (isClimbingGround == false)
                {
                    transform.position = p;
                    break;
                }

                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                bool detect = false;
                if (stateInfo.IsName("Climbing.Up_LtoR") || stateInfo.IsName("Climbing.Up_RtoL"))
                {
                    if (Physics.Raycast(p + transform.up * collider.height, transform.forward, 3f))
                        detect = true;
                }
                else if (stateInfo.IsName("Climbing.Down_LtoR") || stateInfo.IsName("Climbing.Down_RtoL"))
                {
                    if (Physics.Raycast(p, transform.forward, 3f))
                        detect = true;
                }
                else if (stateInfo.IsName("Climb_Left") || stateInfo.IsName("Climb_Right"))
                {
                    if (Physics.Raycast(p + transform.up * collider.height * 0.5f, transform.forward, 3f))
                        detect = true;
                }

                if (detect == true)
                    transform.position = p;

                break;
            }
            case PlayerState.LedgeUp:
            case PlayerState.ReadyGrab:
            {
                var p = transform.position;
                p += animator.deltaPosition;
                transform.position = p;
            }
                break;
        }
    }

    #region Detection

    private bool UpDetection()
    {
        if (ledgeChecker.IsDetectedLedge() == true)
            return true;

        if (DetectionCanClimbingAreaByVertexColor(transform.position + transform.up * collider.height * 0.5f,
            transform.forward) == true)
        {
            return true;
        }

        if (DetectionCanClimbingAreaByVertexColor(transform.position + transform.up * collider.height * 0.75f,
            transform.forward) == true)
        {
            return true;
        }

        return false;
    }

    private bool DetectionCanClimbingAreaByVertexColor(Vector3 startPoint, Vector3 dir, float dist = 2f)
    {
        RaycastHit hit;
        if (Physics.SphereCast(startPoint, collider.radius, dir, out hit, dist, climbingPaintLayer))
        {
            MeshFilter wallMesh = hit.collider.GetComponent<MeshFilter>();
            int[] triangles = wallMesh.mesh.triangles;
            Color[] vertexColors = wallMesh.mesh.colors;

            if (vertexColors[triangles[hit.triangleIndex * 3 + 0]] == Color.red
                || vertexColors[triangles[hit.triangleIndex * 3 + 1]] == Color.red
                || vertexColors[triangles[hit.triangleIndex * 3 + 2]] == Color.red)
            {
                return true;
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    private bool DetectLedgeCanHangLedgeByVertexColor()
    {
        Vector3 start = transform.position + transform.up * collider.height * 2;
        RaycastHit hit;
        if (Physics.SphereCast(start, collider.radius * 2f, -transform.up, out hit, collider.height * 2,
            climbingPaintLayer))
        {
            MeshFilter wallMesh = hit.collider.GetComponent<MeshFilter>();
            int[] triangles = wallMesh.mesh.triangles;
            Color[] vertexColors = wallMesh.mesh.colors;

            if (vertexColors[triangles[hit.triangleIndex * 3 + 0]] == Color.red
                || vertexColors[triangles[hit.triangleIndex * 3 + 1]] == Color.red
                || vertexColors[triangles[hit.triangleIndex * 3 + 2]] == Color.red)
            {
                return true;
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    private bool CheckCanClimbingMoveByVertexColor()
    {
        if (isClimbingMove == false)
            return true;

        Vector3 start = Vector3.zero;

        if (currentVerticalValue != 0)
        {
            start = currentVerticalValue == 1
                ? transform.position + transform.up * collider.height * 0.7f
                : transform.position;
        }
        else
        {
            start = currentHorizontalValue == 1
                ? transform.position + transform.up * collider.height * 0.5f + transform.right * collider.radius
                : transform.position + transform.up * collider.height * 0.5f + transform.right * -collider.radius;
        }

        RaycastHit hit;
        if (Physics.SphereCast(start, collider.radius, transform.forward, out hit, 2f, climbingPaintLayer))
        {
            MeshFilter wallMesh = hit.collider.GetComponent<MeshFilter>();
            int[] triangles = wallMesh.mesh.triangles;
            Color[] vertexColors = wallMesh.mesh.colors;

            //Debug.Log(vertexColors[triangles[hit.triangleIndex * 3 + 0]].ToString() + vertexColors[triangles[hit.triangleIndex * 3 + 1]].ToString() + vertexColors[triangles[hit.triangleIndex * 3 + 2]].ToString());
            if (vertexColors[triangles[hit.triangleIndex * 3 + 0]] == Color.red
                || vertexColors[triangles[hit.triangleIndex * 3 + 1]] == Color.red
                || vertexColors[triangles[hit.triangleIndex * 3 + 2]] == Color.red)
            {
                handIK.TraceCenter();
                return false;
            }
        }
        else
        {
            return true;
        }

        return true;
    }

    private bool DownDetection()
    {
        RaycastHit hit;
        Vector3 point1 = transform.position + transform.up * -0.3f;
        if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 2f, detectionLayer))
        {
            return true;
        }

        return false;
    }

    private bool LedgeDetection()
    {
        RaycastHit hit;
        Vector3 point1 = headTransform.position;
        if (Physics.Raycast(point1, transform.forward, out hit, 2f, detectionLayer))
        {
            return true;
        }

        return false;
    }

    #endregion

    #region 인풋

    private bool InputTryGrab()
    {
        Vector3 point1;
        RaycastHit hit;
        //if (InputManager.Instance.GetAction(KeybindingActions.Grab))
        if (InputManager.Instance.GetInput(KeybindingActions.Grab) && stamina.Value >= 0.0f)
        {
            point1 = transform.position + collider.center - transform.forward;
            //Physics.CapsuleCast(point1, point2, collider.radius, transform.forward, out hit, 1f, detectionLayer)
            if (Physics.SphereCast(point1, collider.radius * 1.5f, transform.forward, out hit, 3f, detectionLayer))
            {
                // Vector3 surfaceNormal = hit.normal;
                // //surfaceNormal.z = 0;
                // surfaceNormal.x = 0;
                // float angle = Vector3.Angle(Vector3.forward, surfaceNormal);
                // surfaceNormal.z = 0;
                // //Debug.Log(Vector3.Angle(Vector3.forward, surfaceNormal));
                // if (surfaceNormal.normalized.y < 0 && angle > 45f)
                //     return false;
                
                if (DetectionCanClimbingAreaByVertexColor(point1, transform.forward, 3f) == true)
                {
                    return false;
                }

                movement.SetParent(hit.collider.transform);
                movement.Attach();

                if (ledgeChecker.IsDetectedLedge() == false)
                {
                    //ChangeState(PlayerState.Grab);
                    ChangeState(PlayerState.ReadyGrab);
                }
                else
                {
                    ChangeState(PlayerState.Grab);
                    ChangeState(PlayerState.HangLedge);
                }

                transform.rotation = Quaternion.LookRotation(-hit.normal);
                transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * 0.05f;

                prevSpeed = currentSpeed;
                moveDir = Vector3.zero;

                // movement.SetParent(hit.collider.transform);
                // movement.Attach();

                //Debug.Log("default");

                return true;
            }
            else
            {
                point1 = transform.position + Vector3.up;
                if (Physics.Raycast(point1, -transform.up, out hit, 1.5f, detectionLayer))
                {
                    point1 += transform.forward;
                    if (Physics.Raycast(point1, -transform.up, 1.5f, detectionLayer) == false)
                        return false;

                    transform.rotation = Quaternion.LookRotation(-hit.normal, transform.forward);
                    transform.position = (hit.point) + (hit.normal) * collider.radius;

                    //ChangeState(PlayerState.Grab);
                    ChangeState(PlayerState.ReadyGrab);

                    movement.SetParent(hit.collider.transform);
                    movement.Attach();
                    moveDir = Vector3.zero;

                    //Debug.Log("groundgrab");

                    return true;
                }
            }

            point1 = transform.position + transform.up * collider.height * 0.5f - transform.forward;
            if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 5f, ledgeAbleLayer))
            {
                RaycastHit ledgePointHit;
                point1 = transform.position + transform.up * collider.height * 2;
                if (Physics.SphereCast(point1, collider.radius * 2f, -transform.up, out ledgePointHit,
                    collider.height * 2, adjustAbleLayer))
                {
                    if (Vector3.Distance(ledgePointHit.point, transform.position) > hangAbleEdgeDist)
                    {
                        return false;
                    }

                    transform.rotation = Quaternion.LookRotation(-hit.normal);
                    transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * 0.05f;

                    ChangeState(PlayerState.Grab);
                    ChangeState(PlayerState.HangEdge);

                    InitVelocity();
                    prevSpeed = currentSpeed;
                    moveDir = Vector3.zero;

                    movement.SetParent(hit.collider.transform);
                    movement.Attach();

                    //Debug.Log("ledgegrab");

                    return true;
                }
            }
        }

        return false;
    }

    private bool InputReleaseGrab()
    {
        //if (InputManager.Instance.GetAction(KeybindingActions.ReleaseGrab))
        if (InputManager.Instance.GetRelease(KeybindingActions.Grab))
        {
            switch (state)
            {
                case PlayerState.Grab:
                case PlayerState.HangEdge:
                case PlayerState.HangLedge:
                case PlayerState.ReadyGrab:
                {
                    isClimbingMove = false;
                    isLedge = false;

                    Vector3 currentRot = transform.rotation.eulerAngles;
                    currentRot.x = 0.0f;
                    currentRot.z = 0.0f;
                    transform.rotation = Quaternion.Euler(currentRot);

                    climbingJumpDirection = ClimbingJumpDirection.Falling;

                    ChangeState(PlayerState.Default);

                    movement.Detach();
                    return true;
                }
                case PlayerState.HangRagdoll:
                {
                    ragdoll.ReleaseHangRagdoll();
                    return true;
                }
                case PlayerState.HangShake:
                {
                    ragdoll.ReleaseHangShake();
                    return true;
                }
            }
        }

        return false;
    }


    private void InputRun()
    {
        if(isWalk)
        {
            if (InputManager.Instance.GetRelease(KeybindingActions.RunToggle) || _runLock) 
                isWalk = false;
        }
        else if(!_runLock)
        {
            if (InputManager.Instance.GetInput(KeybindingActions.RunToggle))
                isWalk = true;
        }

        //if (InputManager.Instance.GetInput(KeybindingActions.RunToggle))
        //{
        //    isRun = true;
        //}

        //if (InputManager.Instance.GetRelease(KeybindingActions.RunToggle))
        //{
        //    isRun = false;
        //}
    }

    //private bool InputAiming()
    //{
    //    //if (InputManager.Instance.GetAction(KeybindingActions.EMPAim))
    //    if (InputManager.Instance.GetInput(KeybindingActions.EMPAim) && !_aimLock)
    //    {
    //        GameManager.Instance.soundManager.Play(1008, Vector3.up, transform);
    //        _charge = GameManager.Instance.soundManager.Play(1013, Vector3.up, transform);
    //        ChangeState(PlayerState.Aiming);
    //        ActiveAim(true);
    //        playerPelvisGunObject.SetActive(false);
    //        return true;
    //    }

    //    return false;
    //}

    //private bool InputReleaseAiming()
    //{
    //    //if (InputManager.Instance.GetAction(KeybindingActions.EMPAimRelease))
    //    if (InputManager.Instance.GetRelease(KeybindingActions.EMPAim))
    //    {
    //        GameManager.Instance.soundManager.Play(1009, Vector3.up, transform);

    //        ReleaseAiming();
    //        return true;
    //    }

    //    return false;
    //}

    public void ReleaseAiming()
    {
        if (_charge != null)
            _charge.Stop();

        int loadCount = (int)(chargeTime.Value);
        if (loadCount == 3)
        {
            if (decharging == true)
                StopCoroutine(_dechargingCoroutine);

            _dechargingCoroutine = DechargingCoroutine();
            StartCoroutine(_dechargingCoroutine);

            //GameManager.Instance.effectManager
            //    .Active("Decharging", steamPosition.position, Quaternion.LookRotation(-steamPosition.up)).transform
            //    .SetParent(steamPosition);
            EffectActiveData data = MessageDataPooling.GetMessageData<EffectActiveData>();
            data.key = "Decharging";
            data.position = steamPosition.position;
            data.rotation = Quaternion.LookRotation(-steamPosition.up);
            data.parent = steamPosition;
            SendMessageEx(MessageTitles.effectmanager_activeeffectsetparent, GetSavedNumber("EffectManager"), data);

            AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundData.id = 1025; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
            SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
            //GameManager.Instance.soundManager.Play(1025,Vector3.up,transform);
        }

        ChangeState(PlayerState.Default);
        ActiveAim(false);
        chargeTime.Value = 0.0f;
        //playerPelvisGunObject.SetActive(true);
        pelvisGunObject.SetActive(true);
    }

    //private void InputChargeShot()
    //{
    //    //if (InputManager.Instance.GetAction(KeybindingActions.Shot) && chargeTime.Value >= 1.0f)
    //    if (InputManager.Instance.GetInput(KeybindingActions.Shot) && chargeTime.Value >= 1.0f)
    //    {
    //        if(_charge != null)
    //        {
    //            _charge.Stop();
    //            _charge = null;

    //        }

    //        int loadCount = (int) (chargeTime.Value);
    //        loadCount = loadCount > 3 ? 3 : loadCount;
    //        _transformCount = 0;

    //        empGun.LaunchLaser(loadCount * 40.0f);
    //        chargeTime.Value = 0.0f;
    //        AddEnergyValue(-loadCount * costValue);
    //        GameManager.Instance.cameraManager.SetAimCameraDistance(0.333f * (float) loadCount);
    //        _chargeDelayTimer.InitTimer("ChargeDelay", 0.0f, chargeDelayTime);

    //        GameManager.Instance.soundManager.Play(1009 + loadCount, Vector3.up, transform);

    //        if (loadCount >= 2)
    //        {
    //            TimeManager.instance.SetTimeScale(0f, .4f, 0.2f, 0.02f);
    //        }

    //        if (loadCount == 3)
    //        {
    //            GameManager.Instance.cameraManager.SetRadialBlur(1f, 0.2f, 0.4f);
    //        }
    //    }
    //}

    private bool InputLedgeUp()
    {
        //if (InputManager.Instance.GetAction(KeybindingActions.Jump))
        if (InputManager.Instance.GetRelease(KeybindingActions.Jump))
        {
            if (currentVerticalValue == 1.0f)
            {
                InputClimbingJump();
                return true;
            }

            if (DetectLedgeCanHangLedgeByVertexColor() == true)
                return true;

            if (isLedge == true && isClimbingMove == false && spaceChecker.Overlapped() == false)
            {
                isLedge = false;
                animator.SetTrigger("LedgeUp");
                animator.SetBool("IsLedge", false);

                Vector3 currentRot = transform.rotation.eulerAngles;
                currentRot.x = 0.0f;
                currentRot.z = 0.0f;
                transform.rotation = Quaternion.Euler(currentRot);

                ChangeState(PlayerState.LedgeUp);

                return true;
            }
        }

        return false;
    }

    private bool InputClimbingJump()
    {
        if (InputManager.Instance.GetRelease(KeybindingActions.Jump))
        {
            ChangeState(PlayerState.ReadyClimbingJump);
            return true;
        }

        return false;
    }

    private void InputUseHpPack()
    {
        if (InputManager.Instance.GetInput(KeybindingActions.UseHpPack) && hp.Value < 100.0f && hpPackCount.Value > 0 &&
            isHpRestore == false)
        {
            hpPackCount.Value--;

            restoreHpPackCoroutine = HpRestore();
            StartCoroutine(restoreHpPackCoroutine);
        }
    }

    #endregion

    #region 겟터

    public bool CheckAimLock()
    {
        return _aimLock;
    }

    public PlayerState GetState()
    {
        return state;
    }

    public void BackPrevState()
    {
        ChangeState(prevState);
    }

    public Vector3 GetPlayerCenter()
    {
        return collider.bounds.center;
    }

    public bool IsNowClimbingBehavior()
    {
        switch (state)
        {
            case PlayerState.Default:
            case PlayerState.Jump:
            case PlayerState.RunToStop:
            case PlayerState.TurnBack:
            case PlayerState.Aiming:
            case PlayerState.Ragdoll:
            case PlayerState.HangRagdoll:
            case PlayerState.Respawn:
            case PlayerState.HighLanding:
            {
                return false;
            }
            default:
                return true;
        }
    }
    #endregion

    #region 셋터

    public void SetAimLock(bool value)
    {
        _aimLock = value;
    }

    public void SetRunningLock(bool value)
    {
        _runLock = value;
    }

    public void SetClimbMove(bool move)
    {
        isClimbingMove = move;
        if (move == false)
        {
            isCanClimbingCancel = false;
        }
    }

    public void SetCanClimbingCancel(bool result)
    {
        isCanClimbingCancel = result;
        isClimbingMove = false;
    }

    #endregion

    public void ActiveAim(bool active)
    {
        if (active == true)
        {
            loadCount.Value = 1;
            //if (rigCtrl != null)
            empGun.Active(true);
            animator.SetLayerWeight(2, 1.0f);
            animator.SetLayerWeight(4, 1.0f);
        }
        else
        {
            loadCount.Value = 0;
            chargeTime.Value = 0.0f;
            empGun.Active(false);
            animator.SetLayerWeight(2, 0.0f);
            animator.SetLayerWeight(3, 0.0f);
            animator.SetLayerWeight(4, 0.0f);
        }
    }

    private void UpdateGrab()
    {
        Vector3 startPos = transform.position + transform.up * (collider.height * 0.5f) + (-transform.forward * 1f);

        if (Physics.SphereCast(startPos, collider.radius, transform.forward, out wallHit, 3.0f, adjustAbleLayer))
        {
            float distToWall = (wallHit.point - (transform.position + transform.up * (collider.height * 0.5f)))
                .magnitude;
            if (distToWall > 0.6f || distToWall < 0.35f)
            {
                transform.position = (wallHit.point - transform.up * (collider.height * 0.5f)) + wallHit.normal * 0.35f;
            }

            if (isClimbingMove == true)
            {
                transform.rotation = Quaternion.LookRotation(-wallHit.normal, transform.up);
            }

            if (wallHit.collider.transform != transform.parent)
            {
                movement.SetParent(wallHit.collider.transform);
            }
        }
        else
        {
            ChangeState(PlayerState.Default);
        }
    }

    private void CheckLedge()
    {
        // if (isClimbingMove == false)
        //     return;

        // bool dectect = false;
        // RaycastHit hit;
        // Vector3 point1 = headTransfrom.position + transform.up * 0.2f;
        // Vector3 point2 = point1 + transform.forward * 1f;
        // if (Physics.Raycast(point1, transform.forward, out hit, 2f, detectionLayer))
        // {
        //     dectect = true;
        // }

        if (currentVerticalValue.Equals(-1.0f))
            return;

        if (ledgeChecker.IsDetectedLedge() == true && (currentVerticalValue.Equals(-1.0f) == false) &&
            isClimbingGround == false)
        {
            if (DetectLedgeCanHangLedgeByVertexColor() == true)
            {
                return;
            }

            isClimbingMove = false;
            isLedge = true;
            ChangeState(PlayerState.HangLedge);
        }
    }

    private void RestoreEnergy(float deltaTime)
    {
        // if ((state == PlayerState.Aiming &&  currentSpeed == 0.0f)
        //     || (state == PlayerState.Default && currentSpeed == 0.0f)
        //     || (state == PlayerState.Grab && isClimbingMove == false)
        //     || state == PlayerState.Jump)
        //     return;

        float restoreValue;
        switch (state)
        {
            case PlayerState.Default:
            {
                if (currentSpeed >= runSpeed)
                    restoreValue = runRestoreEnergyValue;
                else if (currentSpeed >= walkSpeed)
                    restoreValue = walkRestoreEnergyValue;
                else
                    restoreValue = 0.0f;
            }
                break;
            case PlayerState.Grab:
            {
                restoreValue = isClimbingMove == true ? climbingRestoreEnergyValue : 0.0f;
            }
                break;
            case PlayerState.Aiming:
            {
                restoreValue = currentSpeed == 0.0f ? 0.0f : aimRestoreEnergyValue;
            }
                break;
            default:
                restoreValue = 0.0f;
                break;
        }

        AddEnergyValue(restoreValue * deltaTime);
    }

    private void UpdateStamina(float deltaTime)
    {
        if (IsNowClimbingBehavior() == true)
        {
            _staminaTimer.InitTimer("Stamina", 0.0f, staminaRestoreDelayTime);

            if (isClimbingMove == false)
            {
                stamina.Value -= idleConsumeValue * deltaTime;
            }
            else
            {
                stamina.Value -= climbingMoveConsumeValue * deltaTime;
            }

            stamina.Value = Mathf.Clamp(stamina.Value, 0.0f, maxStamina);
        }
        else
        {
            _staminaTimer.IncreaseTimerSelf("Stamina", out bool limit, deltaTime);
            if (limit && stamina.Value < maxStamina)
            {
                stamina.Value += staminaRestoreValue * deltaTime;
                stamina.Value = Mathf.Clamp(stamina.Value, 0.0f, maxStamina);
            }
        }
    }

    private void AdjustLedgeOffset()
    {
        Vector3 start = transform.position + transform.up * (collider.height * 2);
        RaycastHit upHit;
        Vector3 finalPosition;
        if (Physics.SphereCast(start, collider.radius * 2f, -transform.up, out upHit, collider.height * 2,
            adjustAbleLayer))
        {
            finalPosition = upHit.point + (transform.up * detectionOffset.y);
            finalPosition += transform.forward * detectionOffset.z;
            transform.position = finalPosition;

            StartCoroutine(ForceSnap(0.5f, finalPosition, transform.localPosition));
            //Debug.Log("AdjustLedgeOffset");
        }
    }

    IEnumerator ForceSnap(float snapTime, Vector3 snapPosition, Vector3 localPostiion)
    {
        float time = 0.0f;
        while (time < snapTime)
        {
            if (state != PlayerState.HangLedge)
                break;

            //Debug.Log("Adjusting");
            //transform.position = snapPosition;

            if (transform.parent == null)
            {
                transform.position = snapPosition;
            }
            else
            {
                transform.localPosition = localPostiion;
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    public override void TakeDamage(float damage, bool restoreEnergy = true)
    {
        if (state == PlayerState.Respawn && state == PlayerState.Ragdoll)
            return;

        hp.Value -= damage;
        if (restoreEnergy == true)
        {
            AddEnergyValue(hitEnergyRestoreValue);
        }

        if (isHpRestore == true)
        {
            isHpRestore = false;
            StopCoroutine(restoreHpPackCoroutine);
        }

        if (hp.Value <= 0.0f)
        {
            //ChangeState(PlayerState.Dead);
            PlayerDead();
        }

        SendMessageEx(MessageTitles.uimanager_damageEffect, GetSavedNumber("UIManager"), null);
    }

    public void PlayerDead()
    {
        dead = true;
        //whenPlayerDead?.Invoke();
        SendMessageEx(MessageTitles.uimanager_activeGameOverUi, GetSavedNumber("UIManager"), null);
    }

    private IEnumerator HpRestore()
    {
        float time = 0.0f;
        isHpRestore = true;
        while (time < _hpPackRestoreDuration && hp.Value < 100.0f)
        {
            time += Time.fixedDeltaTime;
            hp.Value += hpPackRestoreValue * Time.fixedDeltaTime;
            hp.Value = Mathf.Clamp(hp.Value, 0.0f, 100.0f);
            
            yield return new WaitForFixedUpdate();
        }

        isHpRestore = false;
    }

    public override void InitStatus()
    {
        Debug.Log("Player InitStatus");
        base.InitStatus();
        //hpPackCount.Value = 0;
        ragdoll.ResetRagdoll();
        rigidbody.velocity = Vector3.zero;
        footIK.InitPelvisHeight();
        dead = false;
        airTime = 0.0f;
        ChangeState(PlayerState.Default);
    }

    private IEnumerator DechargingCoroutine()
    {
        decharging = true;
        if (chargingCountText != null)
            chargingCountText.color = Color.red;

        float time = 0;
        while (time < dechargingDuration)
        {
            time += Time.deltaTime;
            float intencity = (dechargingDuration - time) / dechargingDuration * _emissionTargetValue;
            //pelvisGunMaterial.SetColor("_EmissionColor", _originalEmissionColor * intencity);
            foreach(var mat in pelvisGunMaterial)
            {
                mat.SetColor("_EmissionColor", _originalEmissionColor * intencity);
            }

            yield return null;
        }

        decharging = false;
        if (chargingCountText != null)
            chargingCountText.color = _chargingCountTextColor;
    }

    public PlayerRagdoll GetPlayerRagdoll() {return ragdoll;}

    public Drone GetDrone()
    {
        return drone;
    }

    public void DropHpPack()
    {
        hpPackCount.Value++;
        hpPackCount.Value = Mathf.Clamp(hpPackCount.Value, 0, 3);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("HpPack"))
        {
            DropHpPack();
            Destroy(collision.gameObject);
        }
    }

    public void AddForce(Vector3 force, ForceMode mode = ForceMode.Acceleration)
    {
        rigidbody.AddForce(force, mode);
    }

    public void SetVelocity(Vector3 velocity)
    {
        rigidbody.velocity = velocity;
    }

    public void AddVelocity(Vector3 velocity)
    {
        rigidbody.velocity += velocity;
    }

    public void InitVelocity()
    {
        SetVelocity(Vector3.zero);
    }

    public void AddEnergyValue(float value)
    {
        if (value > 0.0f ? energy.Value >= 100.0f : energy.Value <= 0.0f)
            return;

        energy.Value += value;
        energy.Value = Mathf.Clamp(energy.Value, 0.0f, 100.0f);
    }

    public void InitializeMove()
    {
        animator.SetFloat("Speed", 0.0f);
        if (state == PlayerState.Aiming)
        {
            ReleaseAiming();
            ChangeState(PlayerState.Default);
        }
    }

    private bool TryGrab()
    {
        if (state == PlayerState.Default ||
               state == PlayerState.Jump ||
               state == PlayerState.RunToStop ||
               state == PlayerState.ClimbingJump)
        {
            Vector3 point1;
            RaycastHit hit;
            //if (InputManager.Instance.GetAction(KeybindingActions.Grab))
            if (stamina.Value >= 0.0f)
            {
                point1 = transform.position + collider.center - transform.forward;
                //Physics.CapsuleCast(point1, point2, collider.radius, transform.forward, out hit, 1f, detectionLayer)
                if (Physics.SphereCast(point1, collider.radius * 1.5f, transform.forward, out hit, 3f, detectionLayer))
                {
                    // Vector3 surfaceNormal = hit.normal;
                    // //surfaceNormal.z = 0;
                    // surfaceNormal.x = 0;
                    // float angle = Vector3.Angle(Vector3.forward, surfaceNormal);
                    // surfaceNormal.z = 0;
                    // //Debug.Log(Vector3.Angle(Vector3.forward, surfaceNormal));
                    // if (surfaceNormal.normalized.y < 0 && angle > 45f)
                    //     return false;

                    if (DetectionCanClimbingAreaByVertexColor(point1, transform.forward, 3f) == true)
                    {
                        return false;
                    }

                    if (Physics.Raycast(transform.position + transform.TransformDirection(wallUnderCheckOffset), transform.forward, 3f, detectionLayer) == false)
                    {
                        return false;
                    }

                    movement.SetParent(hit.collider.transform);
                    movement.Attach();

                    if (ledgeChecker.IsDetectedLedge() == false)
                    {
                        //ChangeState(PlayerState.Grab);
                        ChangeState(PlayerState.ReadyGrab);
                    }
                    else
                    {
                        ChangeState(PlayerState.Grab);
                        ChangeState(PlayerState.HangLedge);
                    }

                    transform.rotation = Quaternion.LookRotation(-hit.normal);
                    transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * 0.05f;

                    prevSpeed = currentSpeed;
                    moveDir = Vector3.zero;

                    // movement.SetParent(hit.collider.transform);
                    // movement.Attach();

                    //Debug.Log("default");

                    return true;
                }
                else
                {
                    point1 = transform.position + Vector3.up;
                    if (Physics.Raycast(point1, -transform.up, out hit, 1.5f, detectionLayer))
                    {
                        point1 += transform.forward;
                        if (Physics.Raycast(point1, -transform.up, 1.5f, detectionLayer) == false)
                            return false;

                        transform.rotation = Quaternion.LookRotation(-hit.normal, transform.forward);
                        transform.position = (hit.point) + (hit.normal) * collider.radius;

                        //ChangeState(PlayerState.Grab);
                        ChangeState(PlayerState.ReadyGrab);

                        movement.SetParent(hit.collider.transform);
                        movement.Attach();
                        moveDir = Vector3.zero;

                        //Debug.Log("groundgrab");

                        return true;
                    }
                }

                point1 = transform.position + transform.up * collider.height * 0.5f - transform.forward;
                if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 5f, ledgeAbleLayer))
                {
                    RaycastHit ledgePointHit;
                    point1 = transform.position + transform.up * collider.height * 2;
                    if (Physics.SphereCast(point1, collider.radius * 2f, -transform.up, out ledgePointHit,
                        collider.height * 2, adjustAbleLayer))
                    {
                        if (Vector3.Distance(ledgePointHit.point, transform.position) > hangAbleEdgeDist)
                        {
                            return false;
                        }

                        transform.rotation = Quaternion.LookRotation(-hit.normal);
                        transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * 0.05f;

                        ChangeState(PlayerState.Grab);
                        ChangeState(PlayerState.HangEdge);

                        InitVelocity();
                        prevSpeed = currentSpeed;
                        moveDir = Vector3.zero;

                        movement.SetParent(hit.collider.transform);
                        movement.Attach();

                        //Debug.Log("ledgegrab");

                        return true;
                    }
                }
            }
        }
        return false;
    }

    #region New Input System

    public void OnMove(InputAction.CallbackContext value)
    {
        Vector2 inputVector = value.ReadValue<Vector2>();
        inputVertical = inputVector.y;
        inputHorizontal = inputVector.x;
    }

    public void OnRun(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        if(isWalk)
        {
            isWalk = false;
        }
        else
        {
            isWalk = true;
        }
    }

    public void OnShot(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        if (state != PlayerState.Aiming)
            return;

        if(chargeTime.Value >= 1.0f)
        {
            if (_charge != null)
            {
                _charge.Stop();
                _charge = null;

            }

            int loadCount = (int)(chargeTime.Value);
            loadCount = loadCount > 3 ? 3 : loadCount;
            _transformCount = 0;

            chargeTime.Value = 0.0f;
            empGun.LaunchLaser(loadCount * 40.0f);
            AddEnergyValue(-loadCount * costValue);
            FloatData camDist = MessageDataPooling.GetMessageData<FloatData>();
            camDist.value = 0.333f * (float)loadCount;
            SendMessageEx(MessageTitles.cameramanager_setaimcameradistance, GetSavedNumber("CameraManager"), camDist);

            _chargeDelayTimer.InitTimer("ChargeDelay", 0.0f, chargeDelayTime);

            AttachSoundPlayData soundPlayData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
            soundPlayData.id = 1009 + loadCount;
            soundPlayData.localPosition = Vector3.up;
            soundPlayData.parent = transform;
            soundPlayData.returnValue = false;
            SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundPlayData);

            if (loadCount >= 2)
            {
                SetTimeScaleMsg data = MessageDataPooling.GetMessageData<SetTimeScaleMsg>();
                data.timeScale = 0.0f;
                data.lerpTime = 0.4f;
                data.stopTime = 0.2f;
                data.startTime = 0.02f;
                SendMessageEx(MessageTitles.timemanager_settimescale, GetSavedNumber("TimeManager"), data);
            }
            if (loadCount == 3)
            {
                SetRadialBlurData data = MessageDataPooling.GetMessageData<SetRadialBlurData>();
                data.factor = 1.0f;
                data.radius = 0.2f;
                data.time = 0.8f;
                SendMessageEx(MessageTitles.cameramanager_setradialblur, GetSavedNumber("CameraManager"), data);
            }
        }
    }

    public void OnGrab(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;


        if (TryGrab() == true)
            return;

       
            if (state == PlayerState.Grab ||
            state == PlayerState.ReadyGrab ||
            state == PlayerState.HangEdge ||
            state == PlayerState.HangLedge ||
            state == PlayerState.HangRagdoll ||
            state == PlayerState.HangShake)
            {
                switch (state)
                {
                    case PlayerState.Grab:
                    case PlayerState.HangEdge:
                    case PlayerState.HangLedge:
                    case PlayerState.ReadyGrab:
                        {
                            isClimbingMove = false;
                            isLedge = false;

                            Vector3 currentRot = transform.rotation.eulerAngles;
                            currentRot.x = 0.0f;
                            currentRot.z = 0.0f;
                            transform.rotation = Quaternion.Euler(currentRot);

                            climbingJumpDirection = ClimbingJumpDirection.Falling;

                            ChangeState(PlayerState.Default);

                            movement.Detach();
                            return;
                        }
                    case PlayerState.HangRagdoll:
                        {
                            ragdoll.ReleaseHangRagdoll();
                            return;
                        }
                    case PlayerState.HangShake:
                        {
                            ragdoll.ReleaseHangShake();
                            return;
                        }
                }
            }
        

        return;
    }

    public void OnEmpAim(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        if (value.action.IsPressed())
        {
            if (state == PlayerState.Default ||
                state == PlayerState.Jump)
            {
                if (_aimLock == false)
                {
                    //GameManager.Instance.soundManager.Play(1008, Vector3.up, transform);
                    AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                    soundData.id = 1008; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                    SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);



                    AttachSoundPlayData chargeSoundPlayData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                    chargeSoundPlayData.id = 1013; chargeSoundPlayData.localPosition = Vector3.up; chargeSoundPlayData.parent = transform; chargeSoundPlayData.returnValue = true;
                    SendMessageQuick(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), chargeSoundPlayData);

                    //_charge = GameManager.Instance.soundManager.Play(1013, Vector3.up, transform);
                    ChangeState(PlayerState.Aiming);
                    ActiveAim(true);
                    //playerPelvisGunObject.SetActive(false);
                    pelvisGunObject.SetActive(false);
                    return;
                }
            }
        }

        if (value.action.WasReleasedThisFrame())
        {
            if (state == PlayerState.Aiming)
            {
                AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
                soundData.id = 1009; soundData.localPosition = Vector3.up; soundData.parent = transform; soundData.returnValue = false;
                SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);
                //GameManager.Instance.soundManager.Play(1009, Vector3.up, transform);
                ReleaseAiming();
                return;
            }
        }
    }

    public void OnUseHpPack(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        if(hp.Value < 100.0f && hpPackCount.Value > 0 && isHpRestore == false)
        {
            hpPackCount.Value--;
            restoreHpPackCoroutine = HpRestore();
            StartCoroutine(restoreHpPackCoroutine);
        }
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        //if (value.action.IsPressed())
        //{
        //    _pressedJumpInput = true;
        //    switch (state)
        //    {
        //        case PlayerState.Default:
        //            {
        //                if (pressJump == false)
        //                {
        //                    pressJump = true;
        //                    animator.SetTrigger("Jump");
        //                }
        //            }
        //            break;
        //        case PlayerState.Grab:
        //            {
        //                animator.SetTrigger("ClimbingCancel");
        //                handIK.DisableHandIK();
        //            }
        //            break;
        //    }
        //}

        switch(state)
        {
            case PlayerState.Default:
                {
                    if (pressJump == false)
                    {
                        pressJump = true;
                        animator.SetTrigger("Jump");
                    }
                }
                break;
            //case PlayerState.Grab:
            //    {
            //        animator.SetTrigger("ClimbingCancel");
            //        handIK.DisableHandIK();
            //    }
            //    break;
            case PlayerState.HangEdge:
            case PlayerState.HangLedge:
                {
                    if (currentVerticalValue == 1.0f)
                    {
                        ChangeState(PlayerState.ReadyClimbingJump);
                        return;
                    }

                    if (DetectLedgeCanHangLedgeByVertexColor() == true)
                        return;

                    if (isLedge == true && isClimbingMove == false && spaceChecker.Overlapped() == false)
                    {
                        isLedge = false;
                        animator.SetTrigger("LedgeUp");
                        animator.SetBool("IsLedge", false);

                        Vector3 currentRot = transform.rotation.eulerAngles;
                        currentRot.x = 0.0f;
                        currentRot.z = 0.0f;
                        transform.rotation = Quaternion.Euler(currentRot);

                        ChangeState(PlayerState.LedgeUp);

                        return;
                    }
                }
                break;
            case PlayerState.Grab:
                {
                    ChangeState(PlayerState.ReadyClimbingJump);
                }
                break;
        }

       
        //if(value.action.WasReleasedThisFrame())
        //{
        //    _pressedJumpInput = false;
        //    switch (state)
        //    {
        //        case PlayerState.HangEdge:
        //        case PlayerState.HangLedge:
        //            {
        //                if (currentVerticalValue == 1.0f)
        //                {
        //                    ChangeState(PlayerState.ReadyClimbingJump);
        //                    return;
        //                }

        //                if (DetectLedgeCanHangLedgeByVertexColor() == true)
        //                    return;

        //                if (isLedge == true && isClimbingMove == false && spaceChecker.Overlapped() == false)
        //                {
        //                    isLedge = false;
        //                    animator.SetTrigger("LedgeUp");
        //                    animator.SetBool("IsLedge", false);

        //                    Vector3 currentRot = transform.rotation.eulerAngles;
        //                    currentRot.x = 0.0f;
        //                    currentRot.z = 0.0f;
        //                    transform.rotation = Quaternion.Euler(currentRot);

        //                    ChangeState(PlayerState.LedgeUp);

        //                    return;
        //                }
        //            }
        //            break;
        //        case PlayerState.Grab:
        //            {
        //                ChangeState(PlayerState.ReadyClimbingJump);
        //            }
        //            break;
        //    }
        //}
    }

    #endregion

        #region 디버그

        private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + transform.TransformDirection(wallUnderCheckOffset) - transform.forward, transform.forward * 3f);

        if(Application.isPlaying)
           Gizmos.DrawRay(transform.position + collider.center - transform.forward, transform.forward * 3f);

        DebugDraw();
    }

    private void DebugDraw()
    {
        if (collider == null)
            return;

        RaycastHit hit;
        Vector3 start = transform.position + transform.up * collider.height * 2;
        bool isHit = Physics.SphereCast(start, collider.radius * 2f, -transform.up, out hit, collider.height * 2,
            climbingLayer);
        Gizmos.color = Color.red;
        if (isHit)
        {
            Gizmos.DrawRay(start, -transform.up * hit.distance);
            Gizmos.DrawWireSphere(start + -transform.up * hit.distance, collider.radius * 2f);
        }
        else
        {
            Gizmos.DrawRay(start, -transform.up * collider.height * 2);
        }

        Vector3 point1 = headTransform.position;
        DebugCastDetection.Instance.DebugSphereCastDetection(point1, collider.radius, transform.forward, 2f,
            climbingPaintLayer, Color.blue, Color.red);

        //Vector3 wallCheckPoint = transform.position + transform.TransformDirection(wallCheckOffset) - transform.forward;
        //DebugCastDetection.Instance.DebugSphereCastDetection(wallCheckPoint, collider.radius * 1.5f, transform.forward, 3f, detectionLayer, Color.green, Color.red);
        //start = transform.position + transform.up * collider.height * 2;
        //RaycastHit upHit;
        //RaycastHit forwardHit;
        //DebugCastDetection.Instance.DebugSphereCastDetection(start, collider.radius * 2f, -transform.up, collider.height * 2, ledgeAbleLayer,Color.white, Color.blue);

        //start = transform.position + transform.up * collider.height * 2;
        //Vector3 extents = new Vector3(2.0f, 2.0f, 2.0f);
        //DebugCastDetection.Instance.DebugBoxCastDetection(start, extents, -transform.up, transform.rotation, collider.height * 2, ledgeAbleLayer, Color.white, Color.red);
    }

    #endregion

    private void OnGUI()
    {
        if (playerDebug == true)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(10f, 100f, 100, 20), "State : " + state.ToString(), style);
            GUI.Label(new Rect(200f, 100f, 100, 20), "Parent : " + transform.parent, style);
            GUI.Label(new Rect(1650f, 100f, 100, 20), "Position : " + transform.position.ToString(), style);
            GUI.Label(new Rect(10f, 120f, 100, 20), "PrevState : " + prevState.ToString(), style);
            GUI.Label(new Rect(10f, 140f, 100, 20), "IsLedge : " + isLedge.ToString(), style);
            GUI.Label(new Rect(10f, 160f, 100, 20), "IsClimbingMove : " + isClimbingMove.ToString(), style);
            GUI.Label(new Rect(10f, 180f, 100, 20), "CurrentSpeed : " + currentSpeed.ToString(), style);
            GUI.Label(new Rect(10f, 200f, 100, 20), "CurrentJumpPower : " + currentJumpPower.ToString(), style);
            GUI.Label(new Rect(10f, 220f, 100, 20), "HorizonWeight : " + horizonWeight.ToString(), style);
            GUI.Label(new Rect(10f, 240f, 100, 20), "ChargeTime : " + chargeTime.ToString(), style);
        }
    }
}