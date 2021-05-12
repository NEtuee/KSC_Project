using System.Collections;
using System.Collections.Generic;
using AmplifyShaderEditor;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public enum UpdateMethod
{
    FixedUpdate, Update
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
        Dead
    }

    public enum ClimbingJumpDirection
    {
        Up=0,Left,Right,UpLeft,UpRight,Falling
    }

    public enum EMPLaunchType
    {
        ButtonDiff,Switching,Load
    }

    public UpdateMethod updateMethod;
    public GameObject hitMaker;
    public bool playerDebug;

    [SerializeField] private MeshRenderer legBlur;

    [Header("State")]
    [SerializeField] private bool isRun = false;
    [SerializeField] private PlayerState state;
    private PlayerState prevState;
    [SerializeField] private bool isClimbingMove = false;
    [SerializeField] private bool isLedge = false;
    [SerializeField] private Transform headTransfrom;
    [SerializeField] public bool isCanReadyClimbingCancel = false;
    [SerializeField] private bool isCanClimbingCancel = false;
    [SerializeField] private bool isClimbingGround = false;

    [Header("Movement Speed Value")]
    [SerializeField] private float walkSpeed = 15.0f;
    [SerializeField] private float runSpeed = 25.0f;
    [SerializeField] private float rollingSpeed = 10.0f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float prevSpeed;
    [SerializeField] private float rotateSpeed = 6.0f;
    [Range(0, 5)] [SerializeField] private float fallingControlSensitive = 1f;
    [SerializeField] private float horizonWeight = 0.0f;
    [SerializeField] private float rotAngle = 0.0f;
    [SerializeField] private float airRotateSpeed = 2.5f; 

    [Header("Jump Value")]
    [SerializeField] private float currentJumpPower = 0f;
    [SerializeField] private float minJumpPower = -10.0f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float currentClimbingJumpPower = 0f;
    [SerializeField] private float climbingHorizonJumpPower = 5.0f;
    [SerializeField] private float climbingUpJumpPower = 8.0f;
    [SerializeField] private float keepClimbingJumpTime = 0.8f;
    [SerializeField] private AnimationCurve climbingHorizonJumpSpeedCurve;
    private float climbingJumpStartTime;
    private ClimbingJumpDirection climbingJumpDirection;


    [Header("LayerMask")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask climbingLayer;
    [SerializeField] private LayerMask climbingPaintLayer;
    [SerializeField] private LayerMask ledgeAbleLayer;
    [SerializeField] private LayerMask adjustAbleLayer;
    [SerializeField] private Vector3 dectionOffset;

    [Header("Input Record")]
    [SerializeField] private float currentVerticalValue = 0.0f;
    [SerializeField] private float currentHorizontalValue = 0.0f;
    [SerializeField] private float inputVertical;
    [SerializeField] private float inputHorizontal;
    [SerializeField] private float fixedVertical;
    [SerializeField] private float fixedHorizontal;
    [SerializeField] private float _gunPoseVerticalValue = 0.0f;
    [SerializeField] private float _gunPoseHorizonValue = 0.0f;
    [SerializeField] private AnimationCurve bandCurve;

    [Header("Move Direction")]
    [SerializeField] private Vector3 moveDir;
    private Vector3 lookDir;
    private Vector3 prevDir;
    private Vector3 camForward;
    private Vector3 camRight;

    [Header("Detection")]
    [SerializeField] private LedgeChecker ledgeChecker;
    [SerializeField] private SpaceChecker spaceChecker;

    [Header("Edge")]
    [SerializeField] private float hangAbleEdgeDist = 2f;

    [Header("EMP Lunacher")]
    [SerializeField] private EMPGun empGun;
    [SerializeField] private float restoreValuePerSecond = 10f;
    [SerializeField] private float costValue = 25f;
    [SerializeField] private float chargeNecessaryTime = 1f;
    public FloatReactiveProperty chargeTime = new FloatReactiveProperty(0.0f);
    [SerializeField] private float hitEnergyRestoreValue = 0.0f;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] public IntReactiveProperty loadCount = new IntReactiveProperty(0);
    [SerializeField] private float loadTerm = 2f;
    [SerializeField] private float loadTime = 0f;
    [SerializeField] private bool loading = false;
    [SerializeField] private float chargeDelayTime = 1f;
    private TimeCounterEx _chargeDelayTimer = new TimeCounterEx();
    [SerializeField] private Drone drone;
    [SerializeField] private AnimationCurve reloadWeightCurve;
    [SerializeField] private Animator gunAnim;
    
    [Header("Spine")]
    private Transform spine;
    [SerializeField] private Vector3 relativeVec;
    [SerializeField] private Transform lookAtAim;
    private Quaternion storeSpineRotation;

    [Header("HpPack")] 
    public IntReactiveProperty hpPackCount = new IntReactiveProperty(0);
    [SerializeField] private float hpPackRestoreValue = 6.0f;
    private float _hpPackRestoreDuration = 10.0f;
    [SerializeField]private bool isHpRestore = false;
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

    private FMODUnity.StudioEventEmitter _charge;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        movement = GetComponent<PlayerMovement>();
        ragdoll = GetComponent<PlayerRagdoll>();
        footIK = GetComponent<IKCtrl>();
        handIK = GetComponent<HandIKCtrl>();
     
        if(animator != null)
        {
            headTransfrom = animator.GetBoneTransform(HumanBodyBones.Head);
        }

        if(empGun != null)
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
        _staminaTimer.InitTimer("Stamina",0.0f, staminaRestoreDelayTime);

        _chargeDelayTimer.InitTimer("ChargeDelay", chargeDelayTime, chargeDelayTime);

        restoreHpPackCoroutine = HpRestore();

        StartCoroutine(StopCheck());
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.L))
        //{
        //    AddJumpPower(10f);
        //}
        if (InputManager.Instance.GetInput(KeybindingActions.Option) && state != PlayerState.Dead)
        {
            GameManager.Instance.optionMenuCtrl.InputEsc();
        }

        if (GameManager.Instance.PAUSE == true)
            return;

        if (isPause == true)
        {
            return;
        }

        // if (Input.GetKeyDown(KeyCode.E))
        //     animator.SetTrigger("Shot");
        
        if (Input.GetKeyDown(KeyCode.Q))
            energy.Value = 100.0f;

        InputUpdate();

        if (updateMethod == UpdateMethod.Update)
        {
            ProcessUpdate(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (isPause == true)
        {
            return;
        }

        UpdateStamina(Time.fixedDeltaTime);

        if (updateMethod == UpdateMethod.FixedUpdate)
        {
            ProcessUpdate(Time.fixedDeltaTime);
        }

        ProcessFixedUpdate();
    }

    private void InputUpdate()
    {
        camForward = mainCameraTrasform.forward;
        camRight = mainCameraTrasform.right;
        camForward.y = 0;
        camRight.y = 0;

        inputVertical = InputManager.Instance.GetMoveAxisVertical();
        inputHorizontal = InputManager.Instance.GetMoveAxisHorizontal();

        UpdateInputValue(inputVertical, inputHorizontal);

        //if (InputManager.Instance.GetInput(KeybindingActions.RunToggle))
        //{
        //    isRun = true;
        //}
        //else
        //{
        //    isRun = false;
        //}
        InputUseHpPack();
        InputRun();

        switch (state)
        {
            case PlayerState.Default:
                {
                    if (InputManager.Instance.GetInput(KeybindingActions.Jump))
                    {
                        animator.SetTrigger("Jump");
                        return;
                    }

                    if (InputTryGrab())
                        return;

                    if (InputAiming())
                        return;
                }
                break;
            case PlayerState.Jump:
                {
                    if (InputTryGrab())
                        return;
                }
                break;
            case PlayerState.Grab:
                {
                    if (InputClimbingJump())
                        return;

                    if (InputReleaseGrab())
                        return;

                    if (isCanClimbingCancel == true)
                    {
                        if (currentVerticalValue != 0.0f || currentHorizontalValue != 0.0f)
                        {
                            animator.SetTrigger("ClimbingCancel");
                            isCanClimbingCancel = false;
                        }
                    }
                }
                break;
            case PlayerState.ReadyGrab:
                {
                    if (InputReleaseGrab())
                       return;
                
                    if (isCanReadyClimbingCancel == true && (inputVertical != 0 || inputHorizontal != 0))
                    {
                        animator.SetTrigger("ReadyClimbCancel");
                        ChangeState(PlayerState.Grab);
                    }
                }
                break;
            case PlayerState.HangEdge:
            case PlayerState.HangLedge:
                {
                    if (InputReleaseGrab())
                        return;
                    if (InputLedgeUp())
                        return;
                }
                break;
            case PlayerState.HangRagdoll:
            case PlayerState.HangShake:
                {
                    if (InputReleaseGrab())
                        return;
                }
                break;
            case PlayerState.RunToStop:
                {
                    if (InputTryGrab())
                        return;
                }
                break;
            case PlayerState.Aiming:
                {
                   // if(loading == false && impactLoading == false && loadCount.Value < 3 && Input.GetKeyDown(KeyCode.LeftControl))
                   // {
                   //     loading = true;
                   //     loadTime = 0f;
                   //      empGun.GunLoad();
                   // }
                   _chargeDelayTimer.IncreaseTimerSelf("ChargeDelay", out bool limit, Time.deltaTime);
                   if (limit)
                   {
                       chargeTime.Value += Time.deltaTime;
                       chargeTime.Value = Mathf.Clamp(chargeTime.Value, 0.0f, Mathf.Abs(energy.Value / costValue));
                       chargeTime.Value = Mathf.Clamp(chargeTime.Value, 0.0f, 3.0f);

                       gunAnim.SetFloat("Energy", chargeTime.Value * 100.0f);
                   }

                   InputChargeShot();

                    if (InputAimingRelease())
                        return;
                }
                break;
            case PlayerState.ClimbingJump:
                {
                    if (InputTryGrab())
                        return;
                }
                break;         
        }
    }

    private void ProcessUpdate(float deltaTime)
    {        
        if (rigidbody.velocity != Vector3.zero)
        {
            rigidbody.velocity = Vector3.zero;
        }

        animator.SetBool("IsGround", movement.isGrounded);

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
        }

        RestoreEnergy(deltaTime);

        switch (state)
        {
            case PlayerState.Default:
                {
                    if(movement.isGrounded == false)
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
                    if (currentSpeed != 0.0f &&lookDir != Vector3.zero)
                    {
                        targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * rotateSpeed);
                    }
                    else
                    {
                        targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), deltaTime * rotateSpeed);
                    }

                    if (currentSpeed > 5.5f && targetRotation != Quaternion.identity)
                    {
                        rotAngle = (int)Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up));
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

                    if (Physics.Raycast(transform.position + collider.center, moveDir, collider.radius + currentSpeed * deltaTime) == false)
                    {
                        movement.Move(moveDir);
                    }
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
                    if(movement.isGrounded == true)
                    {
                        ChangeState(PlayerState.Default);
                        return;
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
                    if (currentSpeed != 0.0f &&lookDir != Vector3.zero)
                    {
                        targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * rotateSpeed);
                    }
                    else
                    {
                        targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), deltaTime * rotateSpeed);
                    }
                    
                    moveDir *= currentSpeed;

                    if (Physics.Raycast(transform.position + collider.center, moveDir, collider.radius + currentSpeed * deltaTime))
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
                    if(loadTime > loadTerm) 
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

                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(aimDir, Vector3.up), 30.0f * deltaTime);
                        
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
                            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * 1.0f);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), deltaTime * 1.0f);
                        }

                        movement.Move(moveDir + (Vector3.up * currentJumpPower));
                    }
                }
                break;
            case PlayerState.ClimbingJump:
                {
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
                                currentClimbingJumpPower = climbingHorizonJumpPower * climbingHorizonJumpSpeedCurve.Evaluate(normalizeTime);
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
                                currentClimbingJumpPower = climbingHorizonJumpPower*climbingHorizonJumpSpeedCurve.Evaluate(normalizeTime);
                            }
                            break;
                    }

                    moveDir *= currentClimbingJumpPower;
                    Vector3 finalDir = moveDir + upDirect;
                    movement.Move(finalDir);

                    if (Time.time - climbingJumpStartTime >= keepClimbingJumpTime)
                    {
                        moveDir = moveDir.normalized * finalDir.magnitude;
                        movement.Jump();
                        ChangeState(PlayerState.Jump);
                        if (climbingJumpDirection != ClimbingJumpDirection.Left && climbingJumpDirection != ClimbingJumpDirection.Right)
                            currentJumpPower = currentClimbingJumpPower;
                    }
                }
                break;
        }

        UpdateCurrentSpeed(deltaTime);

        prevDir = state == PlayerState.Aiming ? moveDir : lookDir;
    }

    private void ProcessFixedUpdate()
    {
        switch(state)
        {
            case PlayerState.Grab:
                {
                    UpdateGrab();

                    if(_turnOverTime > 0.5f)
                    {
                        _turnOverTime = 0.0f;
                        ChangeState(PlayerState.HangShake);
                        return;
                    }

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
        switch(state)
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
        animator.SetFloat("InputVerticalValue",_gunPoseVerticalValue);
        animator.SetFloat("InputHorizonValue",_gunPoseHorizonValue);

        if (state == PlayerState.Default)
        {
            if (vertical > 0.0f)
            {
                currentVerticalValue = 1.0f;
            }
            else if (vertical < 0.0f)
            {
                currentVerticalValue = -1.0f;
            }
            else
            {
                currentVerticalValue = 0.0f;
            }

            if (horizontal > 0.0f)
            {
                currentHorizontalValue = 1.0f;
            }
            else if (horizontal < 0.0f)
            {
                currentHorizontalValue = -1.0f;
            }
            else
            {
                currentHorizontalValue = 0.0f;
            }
            return;
        }

        if((state == PlayerState.Grab || state == PlayerState.HangLedge || state == PlayerState.HangEdge) && isClimbingMove == false)
        {
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
                        if(currentHorizontalValue == 0.0f)
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
                            if(currentHorizontalValue > 0.0f)
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
        float dot = Vector3.Dot(transform.forward,camRight);
        float dotY = Vector3.Dot(transform.forward,camForward);
        dotY *= 0.6f;
        //dot = Mathf.Clamp(MathEx.abs(dot),0.2f,1f);

        legBlur.gameObject.SetActive(currentSpeed > 7f);

        legBlur.material.SetFloat("_XOffset",speedFactor * dot);
        legBlur.material.SetFloat("_YOffset",speedFactor * dotY);

        if(state == PlayerState.Grab || state == PlayerState.RunToStop)
        {
            return;
        }

        if (state == PlayerState.Default && currentSpeed > 0.0f && Vector3.Dot(moveForward, prevForward) < -0.8f)
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
                if (isRun == true)
                {
                    currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, deltaTime * 20.0f);
                }
                else
                {
                    currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, deltaTime * 20.0f);
                }
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, deltaTime * 20.0f);
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, deltaTime * 40.0f);
        }
        
        if (inputVertical != 0)
        {
            _gunPoseVerticalValue = Mathf.MoveTowards(_gunPoseVerticalValue, inputVertical < 0? -1f:1f, deltaTime * 5.0f);
        }
        else
        {
            _gunPoseVerticalValue = Mathf.MoveTowards(_gunPoseVerticalValue, 0.0f, deltaTime * 5.0f);
        }
        
        if (inputHorizontal != 0)
        {
            _gunPoseHorizonValue = Mathf.MoveTowards(_gunPoseHorizonValue, inputHorizontal < 0? -1f:1f, deltaTime * 5.0f);
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
        currentJumpPower = value;
    }

    public void Jump()
    {
        currentJumpPower = jumpPower;
        movement.Jump();
        prevSpeed = currentSpeed;
        climbingJumpDirection = ClimbingJumpDirection.Up;
        ChangeState(PlayerState.Jump);
    }

    public void ChangeState(PlayerState changeState)
    {
        prevState = state;
        state = changeState;

        switch (prevState)
        {
            case PlayerState.Aiming:
            {
                ActiveAim(false);
                    GameManager.Instance.cameraManager.ActivePlayerFollowCamera();
                    drone.OrderAimHelp(false);
                    releaseAimEvent?.Invoke();
                }
                break;
            case PlayerState.Jump:
                {
                    if(changeState == PlayerState.Default)
                    {
                        GameManager.Instance.soundManager.Play(1000, Vector3.zero, transform);
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
                    GameManager.Instance.stateManager.Visible(false);

                    if (transform.parent == null)
                        GameManager.Instance.cameraManager.SetFollowCameraDistance("Default");
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

                    handIK.ActiveHandIK(true);
                    handIK.ActiveLedgeIK(false);
                    footIK.DisableFeetIk();
                    isClimbingMove = false;
                    movement.SetGrab();
                    GameManager.Instance.cameraManager.SetFollowCameraDistance("Grab");
                }
                break;
            case PlayerState.ReadyGrab:
                {
                    animator.SetBool("IsGrab", true);
                    animator.SetInteger("ReadyClimbNum",(int)climbingJumpDirection);
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
                }
                break;
            case PlayerState.Jump:
                {
                    if(prevState != PlayerState.ClimbingJump)
                       moveDir = transform.forward * currentSpeed;
                    horizonWeight = 0.0f;
                    animator.SetFloat("HorizonWeight", horizonWeight);
                }
                break;
            case PlayerState.TurnBack:
                {
                    animator.applyRootMotion = true;
                    animator.SetTrigger("TurnBack");
                }
                break;
            case PlayerState.RunToStop:
                {
                    animator.applyRootMotion = true;
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
                }
                break;
            case PlayerState.HangRagdoll:
                {
                    handIK.DisableHandIK();
                    ragdoll.ActiveLeftHandFixRagdoll();
                }
                break;
            case PlayerState.Aiming:
                {
                    GameManager.Instance.cameraManager.ActiveAimCamera();
                    GameManager.Instance.stateManager.Visible(true);
                    footIK.DisableFeetIk();
                    drone.OrderAimHelp(true);
                    activeAimEvent?.Invoke();
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

                    if (climbingJumpDirection == ClimbingJumpDirection.Left || climbingJumpDirection == ClimbingJumpDirection.Right)
                        currentClimbingJumpPower = climbingHorizonJumpPower;
                    else
                        currentClimbingJumpPower = climbingUpJumpPower;

                    stamina.Value -= climbingJumpConsumeValue;
                    stamina.Value = Mathf.Clamp(stamina.Value, 0.0f, maxStamina);
                }
                break;
            case PlayerState.ReadyClimbingJump:
                {
                    if (inputVertical >= 0.5f)
                    {
                        if(inputHorizontal == 0.0f)
                            climbingJumpDirection = ClimbingJumpDirection.Up;
                        else if(inputHorizontal > 0.0f)
                            climbingJumpDirection = ClimbingJumpDirection.UpRight;
                        else if(inputHorizontal < 0.0f)
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
                            transform.position = transform.position + (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;

                            animator.SetBool("IsGrab", false);

                            stamina.Value -= climbingJumpConsumeValue;
                            stamina.Value = Mathf.Clamp(stamina.Value, 0.0f, maxStamina);

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
            case PlayerState.Dead:
                {
                    whenPlayerDead?.Invoke();
                }
                break;
        }
    }

    IEnumerator StopCheck()
    {
        float time = 0.0f;

        while(true)
        {
            if(time >= 0.05f && currentSpeed > walkSpeed)
            {
                ChangeState(PlayerState.RunToStop);
                time = 0.0f;
            }

            if (state == PlayerState.Default &&currentSpeed>0.0f && inputVertical == 0.0f && inputHorizontal == 0.0f)
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
                    if (Physics.Raycast(transform.position + Vector3.up + moveDir.normalized*0.5f, Vector3.down, 1.5f, groundLayer))
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
                        if (Physics.Raycast(p + transform.up * collider.height*0.5f, transform.forward, 3f))
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
  
        if(DetectionCanClimbingAreaByVertexColor(transform.position+transform.up*collider.height*0.5f,transform.forward) == true)
        {
            return true;
        }

        if(DetectionCanClimbingAreaByVertexColor(transform.position + transform.up * collider.height * 0.75f, transform.forward) == true)
        {
            return true;
        }

        return false;
    }

    private bool DetectionCanClimbingAreaByVertexColor(Vector3 startPoint, Vector3 dir ,float dist = 2f)
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
        if (Physics.SphereCast(start, collider.radius * 2f, -transform.up, out hit, collider.height * 2, climbingPaintLayer))
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

        if(currentVerticalValue != 0)
        {
            start = currentVerticalValue == 1 ? transform.position + transform.up * collider.height *0.7f : transform.position;
        }
        else
        {
            start = currentHorizontalValue == 1 ? transform.position+transform.up*collider.height*0.5f+transform.right*collider.radius : transform.position + transform.up * collider.height * 0.5f + transform.right * -collider.radius;
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
        Vector3 point1 = headTransfrom.position;
        if (Physics.Raycast(point1,transform.forward, out hit, 2f, detectionLayer))
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
        if(InputManager.Instance.GetInput(KeybindingActions.Grab))
        {
            point1 = transform.position + collider.center - transform.forward;
            //Physics.CapsuleCast(point1, point2, collider.radius, transform.forward, out hit, 1f, detectionLayer)
            if (Physics.SphereCast(point1, collider.radius * 1.5f, transform.forward, out hit, 3f, detectionLayer))
            {
                if(DetectionCanClimbingAreaByVertexColor(point1,transform.forward,3f) == true)
                {
                    return false;
                }

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

                rigidbody.velocity = Vector3.zero;
                prevSpeed = currentSpeed;
                moveDir = Vector3.zero;

                movement.SetParent(hit.collider.transform);
                movement.Attach();

                //Debug.Log("default");

                return true;
            }
            else
            {
                point1 = transform.position + Vector3.up;
                if(Physics.Raycast(point1, -transform.up, out hit, 1.5f, detectionLayer))
                {
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

            point1 = transform.position + transform.up*collider.height*0.5f - transform.forward;
            if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 5f, ledgeAbleLayer))
            {
                RaycastHit ledgePointHit;
                point1 = transform.position + transform.up * collider.height * 2;
                if (Physics.SphereCast(point1, collider.radius * 2f, -transform.up, out ledgePointHit, collider.height * 2, adjustAbleLayer))
                {
                    if (Vector3.Distance(ledgePointHit.point, transform.position) > hangAbleEdgeDist)
                    {
                        return false;
                    }

                    transform.rotation = Quaternion.LookRotation(-hit.normal);
                    transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * 0.05f;

                    ChangeState(PlayerState.Grab);
                    ChangeState(PlayerState.HangEdge);

                    rigidbody.velocity = Vector3.zero;
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
        if(InputManager.Instance.GetRelease(KeybindingActions.Grab))
        {
            switch(state)
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
        if(InputManager.Instance.GetInput(KeybindingActions.RunToggle))
        {
            isRun = true;
        }

        if(InputManager.Instance.GetRelease(KeybindingActions.RunToggle))
        {
            isRun = false;
        }
    }

    private bool InputAiming()
    {
        //if (InputManager.Instance.GetAction(KeybindingActions.EMPAim))
        if(InputManager.Instance.GetInput(KeybindingActions.EMPAim))
        {
            GameManager.Instance.soundManager.Play(1008, Vector3.zero, transform);
            _charge = GameManager.Instance.soundManager.Play(1013,Vector3.zero,transform);
            ChangeState(PlayerState.Aiming);
            ActiveAim(true);
            return true;
        }

        return false;
    }

    private bool InputAimingRelease()
    {
        //if (InputManager.Instance.GetAction(KeybindingActions.EMPAimRelease))
        if(InputManager.Instance.GetRelease(KeybindingActions.EMPAim))
        {
            GameManager.Instance.soundManager.Play(1009,Vector3.zero,transform);
            _charge.Stop();
            
            int loadCount = (int)(chargeTime.Value);
            if (loadCount == 3)
            {
                
            }
            
            ChangeState(PlayerState.Default);
            ActiveAim(false);
            chargeTime.Value = 0.0f;
            return true;
        }

        return false;
    }

    private void InputChargeShot()
    {
        //if (InputManager.Instance.GetAction(KeybindingActions.Shot) && chargeTime.Value >= 1.0f)
        if(InputManager.Instance.GetInput(KeybindingActions.Shot) && chargeTime.Value >= 1.0f)
        {
            GameManager.Instance.soundManager.Play(1010,Vector3.zero,transform);
            GameManager.Instance.soundManager.Play(1011,Vector3.zero,transform);
            
            int loadCount = (int)(chargeTime.Value);
            loadCount = loadCount > 3 ? 3 : loadCount;
            
            empGun.LaunchLaser(loadCount * 40.0f);
            chargeTime.Value = 0.0f;
            energy.Value -= loadCount * costValue;

            _chargeDelayTimer.InitTimer("ChargeDelay", 0.0f, chargeDelayTime);

            if(loadCount >= 2)
            {
                GameManager.Instance.cameraManager.AddAimCameraDistance(-.5f);
                TimeManager.instance.SetTimeScale(0f,.5f,0f,0.2f);
            }
        }
    }

    private bool InputLedgeUp()
    {
        //if (InputManager.Instance.GetAction(KeybindingActions.Jump))
        if(InputManager.Instance.GetInput(KeybindingActions.Jump))
        {
            if(currentVerticalValue == 1.0f)
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
        if (InputManager.Instance.GetInput(KeybindingActions.Jump))
        {
            ChangeState(PlayerState.ReadyClimbingJump);
            return true;
        }

        return false;
    }

    private void InputUseHpPack()
    {
        if (Input.GetKeyDown(KeyCode.E) && hpPackCount.Value > 0 && isHpRestore == false)
        {
            hpPackCount.Value--;
            StartCoroutine(restoreHpPackCoroutine);
        }
    }

    #endregion

    #region 겟터
    public PlayerState GetState()
    {
        return state;
    }
    public void BackPrevState() { ChangeState(prevState); }

    public Vector3 GetPlayerCenter() { return collider.bounds.center; }

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
            {
                return false;
            }
            default:
                return true;
        }
    }

    #endregion

    #region 셋터

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
        if(active == true)
        {
            loadCount.Value = 1;
            //if (rigCtrl != null)
            empGun.Active(true);
            animator.SetLayerWeight(2, 1.0f);
            animator.SetLayerWeight(3, 1.0f);
            
        }
        else
        {
            loadCount.Value = 0;
            
            empGun.Active(false);
            animator.SetLayerWeight(2, 0.0f);
            animator.SetLayerWeight(3, 0.0f);
            
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
        
        if (ledgeChecker.IsDetectedLedge() == true && (currentVerticalValue.Equals(-1.0f)== false) && isClimbingGround == false)
        {
            if(DetectLedgeCanHangLedgeByVertexColor() == true)
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
        if (state == PlayerState.Aiming 
            || (state == PlayerState.Default && currentSpeed == 0.0f)
            || (state == PlayerState.Grab && isClimbingMove == false))
            return;
        //if (IsNowClimbingBehavior() == true)
        //{
        //    if (isClimbingMove == false)
        //        return;
        //}
        //else
        //{
        //    if (state == PlayerState.Ragdoll || state == PlayerState.HangRagdoll || currentSpeed == 0.0f)
        //        return;
        //}
        
        energy.Value += restoreValuePerSecond * deltaTime;
        energy.Value = Mathf.Clamp(energy.Value, 0.0f, 100.0f);
    }

    private void UpdateStamina(float deltaTime)
    {
        if(IsNowClimbingBehavior() == true)
        {
            _staminaTimer.InitTimer("Stamina",0.0f, staminaRestoreDelayTime);

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
            if(limit && stamina.Value < maxStamina)
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
        if (Physics.SphereCast(start, collider.radius * 2f, -transform.up, out upHit, collider.height * 2, adjustAbleLayer))
        {
            finalPosition = upHit.point + (transform.up * dectionOffset.y);
            finalPosition += transform.forward * dectionOffset.z;
            transform.position = finalPosition;
           
            StartCoroutine(ForceSnap(0.5f, finalPosition, transform.localPosition));
            //Debug.Log("AdjustLedgeOffset");
        }
    }

    IEnumerator ForceSnap(float snapTime,Vector3 snapPosition,Vector3 localPostiion)
    {
        float time = 0.0f;
        while(time<snapTime)
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

    public override void TakeDamage(float damage)
    {
        hp.Value -= damage;
        energy.Value += hitEnergyRestoreValue;
        energy.Value = Mathf.Clamp(energy.Value, 0.0f, 100.0f);

        if (isHpRestore == true)
        {
            isHpRestore = false;
            StopCoroutine(restoreHpPackCoroutine);
        }

        if (hp.Value == 0.0f)
        {
            ChangeState(PlayerState.Dead);
        }
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
        base.InitStatus();
        hpPackCount.Value = 0;
        ChangeState(PlayerState.Default);
    }


    #region 디버그
    private void OnDrawGizmos()
    {
        DebugDraw();
    }

    private void DebugDraw()
    {
        if (collider == null)
            return;

        RaycastHit hit;
        Vector3 start = transform.position + transform.up * collider.height * 2;
        bool isHit = Physics.SphereCast(start, collider.radius * 2f, -transform.up, out hit, collider.height * 2, climbingLayer);
        Gizmos.color = Color.red;
        if(isHit)
        {
            Gizmos.DrawRay(start, -transform.up * hit.distance);
            Gizmos.DrawWireSphere(start + -transform.up * hit.distance, collider.radius * 2f);
        }
        else
        {
            Gizmos.DrawRay(start, -transform.up * collider.height * 2);
        }

        Vector3 point1 = headTransfrom.position;
        DebugCastDetection.Instance.DebugSphereCastDetection(point1, collider.radius, transform.forward, 2f, climbingPaintLayer, Color.blue, Color.red);

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
