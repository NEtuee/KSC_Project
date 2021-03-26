using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
        HangLedge,
        ClimbingLedge,
        Ragdoll,
        LedgeUp,
        HangRagdoll,
        Aiming,
        ClimbingJump,
        ReadyClimbingJump
    }

    public enum ClimbingJumpDirection
    {
        Up,Left,Right,UpLeft,UpRight
    }

    public enum EMPLaunchType
    {
        ButtonDiff,Switching,Load
    }

    public UpdateMethod updateMethod;

    [Header("State")]
    [SerializeField] private bool isRun = false;
    [SerializeField] private PlayerState state;
    private PlayerState prevState;
    [SerializeField] private bool isClimbingMove = false;
    [SerializeField] private bool isLedge = false;
    [SerializeField] private Transform headTransfrom;

    [Header("Movement Speed Value")]
    [SerializeField] private float walkSpeed = 15.0f;
    [SerializeField] private float runSpeed = 25.0f;
    [SerializeField] private float rollingSpeed = 10.0f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float prevSpeed;
    [SerializeField] private float rotateSpeed = 6.0f;
    [Range(0, 5)] [SerializeField] private float fallingControlSenstive = 1f;
    [SerializeField] private float horizonWeight = 0.0f;
    [SerializeField] private float rotAngle = 0.0f;
    //[SerializeField] private AnimationCurve walkCurve;
    //[SerializeField] private AnimationCurve runCurve;

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
    [SerializeField] private Vector3 dectionOffset;

    [Header("Input Record")]
    [SerializeField] private float currentVerticalValue = 0.0f;
    [SerializeField] private float currentHorizontalValue = 0.0f;
    [SerializeField] private float inputVertical;
    [SerializeField] private float inputHorizontal;
    [SerializeField] private float fixedVertical;
    [SerializeField] private float fixedHorizontal;
    [SerializeField] private AnimationCurve bandCurve;
    private float horizonInputTime = 0.0f;

    [Header("Move Direction")]
    [SerializeField] private Vector3 moveDir;
    private Vector3 lookDir;
    private Vector3 prevDir;
    private Vector3 camForward;
    private Vector3 camRight;

    [Header("Detection")]
    [SerializeField] private LedgeChecker ledgeChecker;

    [Header("EMP Lunacher")]
    [SerializeField]private float restoreValuePerSecond = 10f;
    [SerializeField] private float costValue = 25f;
    [SerializeField] private float chargeNecessryTime = 1f;
    private float chargeTime = 0.0f;
    [SerializeField] private Transform launchPos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float impectPower = 50.0f;
    [SerializeField] private ParticleSystem impectEffect;
    [SerializeField] private GameObject destroyEffect;
    public IntReactiveProperty launcherMode = new IntReactiveProperty(1);
    [SerializeField] private EMPLaunchType type;
    [SerializeField] private bool isLayser;
    [SerializeField] private LayserRender line;
    [SerializeField] public IntReactiveProperty loadCount = new IntReactiveProperty(0);
    [SerializeField] private float loadTerm = 2f;
    [SerializeField] private float impactTerm = 2.5f;
    [SerializeField] private float loadTime = 0f;
    [SerializeField] private bool loading = false;
    private bool impactLoading = false;
    [SerializeField] private GameObject gunObject;
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Drone drone;
    [SerializeField] private AnimationCurve reloadWeightCurve;
    [SerializeField] private ParticleSystem layserParticle;

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

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        movement = GetComponent<PlayerMovement>();
        ragdoll = GetComponent<PlayerRagdoll>();
        footIK = GetComponent<IKCtrl>();
        handIK = GetComponent<HandIKCtrl>();
        if (launchPos == null)
        {
            launchPos = transform.Find("LunchPos");
        }
        rigCtrl = GetComponent<RigCtrl>();
        if(gunObject != null)
        {
            gunObject.SetActive(false);
        }

        moveDir = Vector3.zero;
        currentSpeed = 0.0f;
        mainCameraTrasform = Camera.main.transform;

        launcherMode.Value = 1;

        line = GetComponent<LayserRender>();

        StartCoroutine(StopCheck());
    }

    void Update()
    {
        if (isPause == true)
        {
            return;
        }

        InputUpdate();

        if (updateMethod == UpdateMethod.Update)
        {
            ProcessUpdate(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (isPause == true)
        {
            return;
        }

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

        if (InputManager.Instance.GetAction(KeybindingActions.RunToggle))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }

        switch (state)
        {
            case PlayerState.Default:
                {
                    if (InputManager.Instance.GetAction(KeybindingActions.Jump))
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
                }
                break;
            case PlayerState.HangLedge:
                {
                    if (InputReleaseGrab())
                        return;
                    if (InputLedgeUp())
                        return;
                }
                break;
            case PlayerState.HangRagdoll:
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
                    if(type == EMPLaunchType.ButtonDiff)
                    {
                        if(loading == false && impactLoading == false && loadCount.Value < 3 && Input.GetKeyDown(KeyCode.LeftControl))
                        {
                            loading = true;
                            loadTime = 0f;
                            gunAnim.SetTrigger("Next");
                        }
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
        InputChangeLauncherMode();
    }

    private void ProcessUpdate(float deltaTime)
    {        
        if (rigidbody.velocity != Vector3.zero)
        {
            rigidbody.velocity = Vector3.zero;
        }

        animator.SetBool("IsGround", movement.isGrounded);

        if (state != PlayerState.Grab && state != PlayerState.HangLedge && state != PlayerState.ClimbingJump &&movement.isGrounded == false)
        {
            currentJumpPower -= gravity * deltaTime;
            currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, 50f);
        }
        else
        {
            currentJumpPower = 0.0f;
        }

        switch (state)
        {
            case PlayerState.Default:
                {
                    RestoreEnergy(deltaTime);

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

                    if(currentSpeed > 5.5f&&targetRotation != Quaternion.identity)
                    {
                        rotAngle = (int)Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up));
                        if (Vector3.Dot(Vector3.Cross(transform.forward, lookDir), transform.up) < 0)
                        {
                            rotAngle = -rotAngle;
                        }

                        horizonWeight = Mathf.MoveTowards(horizonWeight, rotAngle, 25.0f * deltaTime);
                    }
                    else
                    {
                        horizonWeight = Mathf.MoveTowards(horizonWeight, 0.0f, 50.0f * deltaTime);
                    }

                    moveDir *= currentSpeed;

                    animator.SetFloat("Speed", currentSpeed);                    
                    animator.SetFloat("HorizonWeight", horizonWeight);
                    movement.Move(moveDir);
                }
                break;
            case PlayerState.TurnBack:
            case PlayerState.RunToStop:
                {
                    RestoreEnergy(deltaTime);

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, groundLayer))
                    {
                        moveDir = (Vector3.ProjectOnPlane(transform.forward, hit.normal)).normalized;
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

                    Vector3 plusDir = ((camForward * inputVertical) + (camRight * inputHorizontal));
                    movement.Move(plusDir * fallingControlSenstive);

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
                break;
            case PlayerState.Grab:
                {
                    if (isClimbingMove == true)
                    {
                        CheckLedge();
                    }
                }
                break;
            case PlayerState.HangRagdoll:
                {
                    //if (movement.GetGroundAngle() <= 40.0f)
                    //{
                    //    ragdoll.DisableFixRagdoll();
                    //}
                }
                break;
            case PlayerState.HangLedge:
                {
                }
                break;
            case PlayerState.Aiming:
                {
                    if (type == EMPLaunchType.ButtonDiff)
                    {
                        if (loading == true)
                        {
                            rigCtrl.SetAimingWeight(reloadWeightCurve.Evaluate(loadTime/loadTerm));
                            loadTime += deltaTime;
                            if(loadTime > loadTerm)
                            {
                                rigCtrl.SetAimingWeight(1f);
                                loadCount.Value++;
                                loadTime = 0;
                                loading = false;
                            }
                        }

                        if(impactLoading == true)
                        {
                            rigCtrl.SetAimingWeight(reloadWeightCurve.Evaluate(loadTime / impactTerm));
                            loadTime += deltaTime;
                            if (loadTime > impactTerm)
                            {
                                rigCtrl.SetAimingWeight(1f);
                                loadTime = 0;
                                impactLoading = false;
                            }
                        }
                    }

                    RaycastHit hit;
                    if(Physics.Raycast(mainCameraTrasform.position,mainCameraTrasform.forward,out hit,150f))
                    {
                        launchPos.LookAt(hit.point);
                    }
                    else
                    {
                        launchPos.LookAt(mainCameraTrasform.position + mainCameraTrasform.forward * 150.0f);
                    }

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
                        movement.Move(plusDir * fallingControlSenstive);

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
                        ChangeState(PlayerState.Jump);
                        if (climbingJumpDirection != ClimbingJumpDirection.Left && climbingJumpDirection != ClimbingJumpDirection.Right)
                            currentJumpPower = currentClimbingJumpPower;
                    }
                }
                break;
        }

        UpdateCurrentSpeed(deltaTime);
        prevDir = lookDir;
    }

    private void ProcessFixedUpdate()
    {
        switch(state)
        {
            case PlayerState.Grab:
            case PlayerState.HangLedge:
                {
                    UpdateGrab();
                }
                break;
        }
    }
    private void UpdateInputValue(float vertical, float horizontal)
    {
        animator.SetFloat("InputVertical", Mathf.Abs(inputVertical));
        animator.SetFloat("InputHorizon", Mathf.Abs(inputHorizontal));
        animator.SetFloat("InputHorizonNoAbs", inputHorizontal);

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

        if((state == PlayerState.Grab || state == PlayerState.HangLedge) && isClimbingMove == false)
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
                        if (UpDetection() == true)
                        {
                            animator.SetTrigger("UpClimbing");
                            isClimbingMove = true;
                        }
                    }
                    else
                    {
                        if (DownDetection() == true)
                        {
                            animator.SetTrigger("DownClimbing");
                            isClimbingMove = true;
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
                    ChangeState(PlayerState.Grab);
                    animator.SetBool("IsLedge", false);
                    animator.SetTrigger("DownClimbing");

                    isLedge = false;
                    isClimbingMove = true;
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
    }

    public void Jump()
    {
        currentJumpPower = jumpPower;
        movement.Jump();
        prevSpeed = currentSpeed;
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
                    GameManager.Instance.cameraManger.ActivePlayerFollowCamera();
                }
                break;
            case PlayerState.Jump:
                {
                    if(changeState == PlayerState.Default)
                    {
                        GameManager.Instance.soundManager.Play(18, Vector3.zero, transform);
                    }
                }
                break;
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
                    animator.applyRootMotion = false;
                    animator.SetBool("IsGrab", false);
                    animator.SetBool("IsLedge", false);
                    footIK.EnableFeetIk();
                    handIK.DisableHandIK();
                    GameManager.Instance.stateManager.Visible(false);

                    if (transform.parent == null)
                        GameManager.Instance.cameraManger.SetFollowCameraDistance(5.0f, 20.0f);
                    else
                        GameManager.Instance.cameraManger.SetFollowCameraDistance(10.0f, 20.0f);
                }
                break;
            case PlayerState.Grab:
                {
                    animator.SetBool("IsGrab", true);
                    currentJumpPower = 0.0f;
                    currentSpeed = 0.0f;

                    currentVerticalValue = 0.0f;
                    currentHorizontalValue = 0.0f;

                    handIK.ActiveHandIK(true);
                    handIK.ActiveLedgeIK(false);
                    footIK.DisableFeetIk();
                    isClimbingMove = false;
                    GameManager.Instance.cameraManger.SetFollowCameraDistance(12.0f, 20.0f);
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
                    transform.parent = null;
                    handIK.DisableHandIK();
                }
                break;
            case PlayerState.HangRagdoll:
                {
                    handIK.DisableHandIK();
                    ragdoll.ActiveRightHandFixRagdoll();
                }
                break;
            case PlayerState.Aiming:
                {
                    GameManager.Instance.cameraManger.ActiveAimCamera();
                    GameManager.Instance.stateManager.Visible(true);
                    footIK.DisableFeetIk();
                }
                break;
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
                }
                break;
            case PlayerState.ClimbingJump:
                {
                    climbingJumpStartTime = Time.time;

                    if (climbingJumpDirection == ClimbingJumpDirection.Left || climbingJumpDirection == ClimbingJumpDirection.Right)
                        currentClimbingJumpPower = climbingHorizonJumpPower;
                    else
                        currentClimbingJumpPower = climbingUpJumpPower;
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
                        climbingJumpDirection = ClimbingJumpDirection.Up;
                        if (inputHorizontal >= 0.5f)
                        {
                            climbingJumpDirection = ClimbingJumpDirection.Right;
                        }
                        else if (inputHorizontal <= -0.5f)
                        {
                            climbingJumpDirection = ClimbingJumpDirection.Left;
                        }
                    }

                    handIK.DisableHandIK();
                    animator.SetBool("IsGrab", false);
                    animator.SetTrigger("ClimbingJump");
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
            case PlayerState.Grab:
            case PlayerState.HangLedge:
                {
                    if (isClimbingMove == true)
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-wallHit.normal, Vector3.up), 5f * Time.deltaTime);

                    var p = transform.position;
                    p += animator.deltaPosition;
                    transform.position = p;
                }
                break;
            case PlayerState.LedgeUp:
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
        if (ledgeChecker.IsDetectedLedge() == false)
            return true;

        //RaycastHit hit;
        //Vector3 point1 = headTransfrom.position + transform.up * 0.2f;
        //Vector3 point2 = point1 + transform.forward * 1f;
        //if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 2f, detectionLayer))
        //{
        //    return true;
        //}

        //RaycastHit hit;
        //Vector3 point1 = headTransfrom.position + transform.up * 0.2f;
        //Vector3 point2 = point1 + transform.forward * 1f;
        //if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 2f, detectionLayer))
        //{
        //    MeshFilter wallMesh = hit.collider.GetComponent<MeshFilter>();
        //    int[] triangles = wallMesh.mesh.triangles;
        //    Color[] vertexColors = wallMesh.mesh.colors;

        //    if(vertexColors[triangles[hit.triangleIndex*3+0]] != Color.red
        //        && vertexColors[triangles[hit.triangleIndex * 3 + 1]] != Color.red
        //        && vertexColors[triangles[hit.triangleIndex * 3 + 2]] != Color.red)
        //    {
        //        return true;
        //    }
        //}

        return false;
    }

    private bool DownDetection()
    {
        RaycastHit hit;
        Vector3 point1 = transform.position + transform.up * -0.3f;
        Vector3 point2 = point1 + transform.forward * 1f;
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

    #region ��ǲ
    private bool InputTryGrab()
    {
        Vector3 point1;
        Vector3 point2;
        RaycastHit hit;
        if (InputManager.Instance.GetAction(KeybindingActions.Grab))
        {
            point1 = transform.position + collider.center - transform.forward;
            point2 = point1 + transform.forward * 2f;
            //Physics.CapsuleCast(point1, point2, collider.radius, transform.forward, out hit, 1f, detectionLayer)
            if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 2f, detectionLayer))
            {
                ChangeState(PlayerState.Grab);
            
                transform.rotation = Quaternion.LookRotation(-hit.normal);
                transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * 0.05f;

                rigidbody.velocity = Vector3.zero;

                prevSpeed = currentSpeed;

                moveDir = Vector3.zero;

                transform.SetParent(hit.collider.transform);
                movement.Attach();
                
                return true;
            }
            else
            {
                point1 = transform.position + Vector3.up;
                if(Physics.Raycast(point1, -transform.up, out hit, 1.5f, detectionLayer))
                {
                    transform.rotation = Quaternion.LookRotation(-hit.normal, transform.forward);
                    transform.position = (hit.point) + (hit.normal) * collider.radius;

                    ChangeState(PlayerState.Grab);

                    transform.SetParent(hit.collider.transform);
                    movement.Attach();
                    moveDir = Vector3.zero;
                    return true;
                }
            }
        }

        return false;
    }

    private bool InputReleaseGrab()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.ReleaseGrab))
        {
            switch(state)
            {
                case PlayerState.Grab:
                case PlayerState.HangLedge:
                    {
                        isClimbingMove = false;
                        isLedge = false;

                        Vector3 currentRot = transform.rotation.eulerAngles;
                        currentRot.x = 0.0f;
                        currentRot.z = 0.0f;
                        transform.rotation = Quaternion.Euler(currentRot);

                        ChangeState(PlayerState.Default);

                        movement.Detach();
                        return true;
                    }
                    break;
                case PlayerState.HangRagdoll:
                    {
                        ragdoll.ReleaseHangRagdoll();
                        return true;
                    }
                    break;
            }

            
        }
        return false;
    }

    private bool InputAiming()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.EMPAim))
        {
            ChangeState(PlayerState.Aiming);
            loadCount.Value = 1;
            if (rigCtrl != null)
            {
                rigCtrl.Active();
                gunObject.SetActive(true);
                animator.SetLayerWeight(2, 1.0f);
                animator.SetLayerWeight(3, 1.0f);
            }
            gunAnim.SetTrigger("Next");
            return true;
        }

        return false;
    }

    private bool InputAimingRelease()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.EMPAimRelease))
        {
            ChangeState(PlayerState.Default);
            loadCount.Value = 0;
            if (rigCtrl != null)
            {
                rigCtrl.Disable();
                gunObject.SetActive(false);
                animator.SetLayerWeight(2, 0.0f);
                animator.SetLayerWeight(3, 0.0f);
            }
            gunAnim.SetTrigger("Off");
            return true;
        }

        return false;
    }

    private void InputChargeShot()
    {
        switch (type)
        {
            case EMPLaunchType.ButtonDiff:
                {
                    if (InputManager.Instance.GetAction(KeybindingActions.Aiming) && energy.Value >= 25f)
                    {
                        chargeTime += Time.deltaTime;

                        if (chargeTime >= chargeNecessryTime)
                        {
                            chargeTime = 0.0f;
                            energy.Value -= 25f;

                            if (bulletPrefab != null)
                            {
                                if (isLayser)
                                    LaunchLayser(loadCount.Value);
                                else
                                    Instantiate(bulletPrefab, launchPos.position, launchPos.rotation);
                            }
                        }

                        return;
                    }
                    else
                    {
                        chargeTime = 0.0f;
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse1) && energy.Value >= 50f)
                    {
                        LaunchImpect();
                    }

                }
                break;
            case EMPLaunchType.Switching:
                {
                    if (InputManager.Instance.GetAction(KeybindingActions.Aiming))
                    {
                        if (launcherMode.Value == 1 && energy.Value < 25f)
                            return;

                        if (launcherMode.Value == 2 && energy.Value < 50f)
                            return;

                        chargeTime += Time.deltaTime;

                        if (chargeTime >= chargeNecessryTime)
                        {
                            chargeTime = 0.0f;
                            energy.Value -= 25f;

                            if (launcherMode.Value == 1)
                            {
                                if (bulletPrefab != null)
                                {
                                    if (isLayser)
                                        LaunchLayser();
                                    else
                                        Instantiate(bulletPrefab, launchPos.position, launchPos.rotation);
                                }
                            }
                            else
                            {
                                LaunchImpect();
                            }
                        }

                        return;
                    }
                    else
                    {
                        chargeTime = 0.0f;
                    }
                }
                break;
            case EMPLaunchType.Load:
                {
                    if (InputManager.Instance.GetAction(KeybindingActions.Aiming))
                    {
                        if (launcherMode.Value == 1 && loadCount.Value != 0 && energy.Value < 25f)
                            return;

                        if (launcherMode.Value == 2 && energy.Value < 50f)
                            return;

                        chargeTime += Time.deltaTime;

                        if (chargeTime >= chargeNecessryTime)
                        {
                            chargeTime = 0.0f;
                            int count = (int)Mathf.Abs(energy.Value / 25f);
                            energy.Value -= 25f * (loadCount.Value>count? count : loadCount.Value);

                            if (launcherMode.Value == 1)
                            {
                                if (bulletPrefab != null)
                                {
                                    if (isLayser)
                                        LaunchLayser(loadCount.Value);
                                    else
                                        Instantiate(bulletPrefab, launchPos.position, launchPos.rotation);
                                }
                            }
                            else
                            {
                                LaunchImpect();
                            }
                        }

                        return;
                    }
                    else
                    {
                        chargeTime = 0.0f;
                    }
                }
                break;
        }
        
    }

    private void InputChangeLauncherMode()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Equiment1th))
        {
            launcherMode.Value = 1;
        }
        else if (InputManager.Instance.GetAction(KeybindingActions.Equiment2th))
        {
            launcherMode.Value = 2;
        }
    }

    private bool InputLedgeUp()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Jump) && isLedge == true && isClimbingMove == false)
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
        return false;
    }

    private bool InputClimbingJump()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Jump))
        {
            //ChangeState(PlayerState.ClimbingJump);
            ChangeState(PlayerState.ReadyClimbingJump);
            return true;
        }

        return false;
    }

    #endregion

    #region ����
    public PlayerState GetState()
    {
        return state;
    }
    public void BackPrevState() { ChangeState(prevState); }

    #endregion

    #region ����
    public void SetClimbMove(bool move) { isClimbingMove = move; }

    #endregion

    #region EMP ������
    private void LaunchImpect()
    {
        energy.Value -= 50.0f;
        impectEffect.Play();
        impactLoading = true;
        loadTime = 0f;
        Collider[] coll = Physics.OverlapSphere(transform.position + transform.forward * 3f, 3f);
        for (int i = 0; i < coll.Length; i++)
        {
            if (coll[i].CompareTag("Moveable"))
            {
                coll[i].GetComponent<Rigidbody>().AddForce(transform.forward * impectPower, ForceMode.Impulse);
            }
            else if (coll[i].CompareTag("Destroyable"))
            {
                Instantiate(destroyEffect, coll[i].transform.position, Quaternion.identity);
                Destroy(coll[i].gameObject);
            }
            else if(coll[i].CompareTag("ImpactTarget"))
            {
                coll[i].GetComponent<ImpactTarget>().TriggerOn();
            }
        }
    }

    private void LaunchLayser()
    {
        if(line != null)
        {
            RaycastHit hit;
            if(Physics.Raycast(mainCameraTrasform.position, mainCameraTrasform.forward,out hit,100f))
            {
                //line.Active(launchPos.position, hit.point,0.1f, 0.1f, 0.15f);
                layserParticle.Play();
                EMPShield shield;
                if(hit.collider.TryGetComponent<EMPShield>(out shield))
                {
                    shield.Hit();
                }
            }
            else
            {
                //line.Active(launchPos.position, mainCameraTrasform.position+mainCameraTrasform.forward*100f,0.1f, 0.1f, 0.15f);
                layserParticle.Play();
            }
        }
    }

    private void LaunchLayser(int loadCount)
    {
        if (line != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCameraTrasform.position, mainCameraTrasform.forward, out hit, 100f))
            {
                //line.Active(launchPos.position, hit.point, 0.1f, 0.1f, 0.15f * loadCount);
                layserParticle.Play();
                EMPShield shield;
                bool destroy;
                if (hit.collider.TryGetComponent<EMPShield>(out shield))
                {
                    shield.Hit(loadCount * 40f, out destroy);
                    if(destroy == true && drone != null)
                    {
                        drone.OrderApproch(hit.collider.transform);
                    }
                }
            }
            else
            {
                //line.Active(launchPos.position, mainCameraTrasform.position + mainCameraTrasform.forward * 100f, 0.1f, 0.1f, 0.15f * loadCount);
                layserParticle.Play();
            }
        }
        this.loadCount.Value = 1;
    }
    #endregion

    private void UpdateGrab()
    {
        Vector3 startPos = transform.position + transform.up * (collider.height * 0.5f) + (-transform.forward * 1f);

        if (Physics.SphereCast(startPos, collider.radius, transform.forward, out wallHit, 3.0f, detectionLayer))
        {
            float distToWall = (wallHit.point - (transform.position + transform.up * (collider.height * 0.5f))).magnitude;
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
                transform.parent = wallHit.collider.transform;
            }
        }
        
    }

    private void CheckLedge()
    {
        bool dectect = false;
        RaycastHit hit;
        Vector3 point1 = headTransfrom.position + transform.up * 0.2f;
        Vector3 point2 = point1 + transform.forward * 1f;
        if (Physics.Raycast(point1, transform.forward, out hit, 2f, detectionLayer))
        {
            dectect = true;
        }

        if (ledgeChecker.IsDetectedLedge() == true && currentVerticalValue == 1.0f && dectect == false)
        {
            isClimbingMove = false;
            isLedge = true;
            ChangeState(PlayerState.HangLedge);
        }
    }

    private void RestoreEnergy(float deltaTime)
    {
        if (energy.Value != 100.0f)
        {
            energy.Value += restoreValuePerSecond * deltaTime;
            energy.Value = Mathf.Clamp(energy.Value, 0.0f, 100.0f);
        }
    }

    private void AdjustLedgeOffset()
    {
        Vector3 start = transform.position + transform.up * collider.height * 2;
        RaycastHit upHit;
        RaycastHit forwardHit;
        Vector3 finalPosition;
        if (Physics.SphereCast(start, collider.radius * 2f, -transform.up, out upHit, collider.height * 2, climbingLayer))
        {
            if (Physics.Raycast(transform.position,transform.forward,out forwardHit,1.5f,climbingLayer))
            {
                finalPosition = upHit.point + (transform.up * dectionOffset.y);
                finalPosition += forwardHit.normal * dectionOffset.z;
                transform.position = finalPosition;
            }
        }
    }

    //public float GetFootStepOffset()
    //{
    //    if (state != PlayerState.Default)
    //        return 0.0f;

    //    if(currentSpeed == 9.0f)
    //    {
    //        return runCurve.Evaluate(animationNormalizeTime);
    //    }
    //    else if(currentSpeed >= 5.5f)
    //    {
    //        return walkCurve.Evaluate(animationNormalizeTime);
    //    }
    //    else
    //    {
    //        return 0.0f;
    //    }
    //}

    #region �����
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
    }
    #endregion
}
