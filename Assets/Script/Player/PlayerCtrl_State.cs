using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerCtrl_State : MonoBehaviour
{
    public enum PlayerState
    {
        Default,
        Jump,
        Rolling,
        Grab,
        HangLedge,
        ClimbingLedge,
        HangRope,
        Absorb,
        SlidingStagger,
        Sliding,
        Stagger,
        Nuckback
    }

    [Header("Movement Speed Value")]
    [SerializeField] private float walkSpeed = 15.0f;
    [SerializeField] private float runSpeed = 25.0f;
    [SerializeField] private float rollingSpeed = 10.0f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float prevSpeed;
    [Range(0, 5)] [SerializeField] private float fallingControlSenstive = 1f;

    [Header("Jump Value")]
    [SerializeField] private float currentJumpPower = 0f;
    [SerializeField] private float minJumpPower = -10.0f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravity = 20.0f;

    [Header("State Conditions")]
    [SerializeField] private PlayerState state;
    [SerializeField] private bool isPause;
    [SerializeField] private bool isPauseControl;
    [SerializeField] private bool isRun;
    [SerializeField] private bool isMustClimbing;
    [SerializeField] private bool isAim;
    [SerializeField] private bool isRollingReady;
    [SerializeField] private bool isGround;
    [SerializeField] private bool isEnoughGround;
    [SerializeField] private bool isSideDetect;
    [SerializeField] private bool isUpDetect;
    [SerializeField] private bool SuperMode;
    [SerializeField] private bool isCanInputClimbing = true;
    [SerializeField] private bool isLedgeSideMove;
    [SerializeField] private bool isCanAbsorb;
    [SerializeField] private bool isActiveSlidingCheck = false;
    public FloatReactiveProperty stamina = new FloatReactiveProperty(100);
    public FloatReactiveProperty hp = new FloatReactiveProperty(100f);
    [SerializeField] private float fallingTime = 0.0f;
    [SerializeField] private bool nonStaminaMode;
    [SerializeField] private float climbingUpAngle;
    [SerializeField] private bool isSpearDetect;
    [SerializeField] private int abosrbCount = 0;

    [Header("Spear")]
    [SerializeField] private SpearCtrl spearPrefab;
    [SerializeField] private SpecialSpearCtrl specialSpearPrefab;
    [SerializeField] private GameObject specialSpearVisualPrefab;
    [SerializeField] private Transform launchPos;
    [SerializeField] private bool isSpacialSpearMode;
    [SerializeField] private bool isCanEquipSpeicalSpear;
    [SerializeField] private int currentSpearNum = 6;
    [SerializeField] private Transform rightHandBone;
    [SerializeField] private GameObject specialSpearVisualObject;
    [SerializeField] private GameObject specialSpearVisualFloat;
    [SerializeField] private GameObject specialSpearDissolve;
    private Material spearDissolve;
    private bool isSpearDissolve;

    [Header("Rope")]
    [SerializeField] private RopeBuiltIn ropePrefab;
    [SerializeField] private RopeBuiltIn currentHangingRope = null;
    [SerializeField] private float ropeClimbingSpeed = 3f;
    [SerializeField] private float ropeDetectRange = 4f;
    [SerializeField] private bool ropeHandLeft; //로프 짚는 손이 왼손 차례 여부

    [Header("Collision Layer")]
    [SerializeField] private LayerMask climbingLayer;
    [SerializeField] private int detectionLayer;
    private int spearLayer;
    [SerializeField] private LayerMask floorLayer;
    private int headCheckLayer;
    private int ropeCheckLayer;
    [SerializeField] private LayerMask spacialSpearShotCheckLayer;

    [Header("Detection Sensor")]
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private ClimbingAbleSensor sideCheck;
    [SerializeField] private ClimbingAbleSensor upCheck;
    [SerializeField] private ClimbingAbleSensor enoughGroundCheck;
    [SerializeField] private DetectTrigger spearDetect;
    [SerializeField] private Transform ledgeDetectPoint;
    [SerializeField] private Transform handOffset;
    [SerializeField] private Transform upPoint;
    [SerializeField] private Transform downPoint;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Transform centerPoint;

    [Header("Input Record")]
    [SerializeField] private float currentVerticalValue = 0.0f;
    [SerializeField] private float currentHorizontalValue = 0.0f;
    [SerializeField] private float inputVertical;
    [SerializeField] private float inputHorizontal;

    [Header("Balance")]
    [SerializeField] private float balanceLimitMinAngle = 40f;
    [SerializeField] private float balanceLimitMaxAngle = 60f;
    [SerializeField] private float canStandUpAngle = 5f;
    [SerializeField] private float holdOutTime = 2.5f;
    [SerializeField] private float currentSlidingSpeed = 0.0f;
    [SerializeField] private float maxSlidingSpeed = 10.0f;

    [Header("Move Direction")]
    [SerializeField] private Vector3 moveDir;
    private Vector3 prevDir;
    private Vector3 slidingDir;
    private Vector3 prevSlidingDir;
    private Vector3 camForward;
    private Vector3 camRight;
    private Quaternion climbingPrevRot;
    private Vector3 prevForward;

    [Header("Damage")]
    [SerializeField] private float damage = 25f;

    private float rollingTime;

    [SerializeField]private Rigidbody rigidbody;
    [SerializeField]private CapsuleCollider collider;
    [SerializeField]private float colliderRadius;
    [SerializeField]private float colliderHeight;

    private Transform mainCameraTrasform;
    private Animator animator;
    private CameraCtrl cameraCtrl;
    private IKCtrl ikCtrl;
    private HandIKCtrl handIKCtrl;
    private EnergyCore currentDetectEnergyCore;
    private PlayerAnimCtrl playerAnimCtrl;

    private PlayerRagdoll ragdoll;

    [SerializeField] private float offsetDist;

    private float groundAngle;

    public delegate void OnAimEvent();
    public OnAimEvent OnAim;
    public delegate void OnShotEvent();
    public OnShotEvent OnShot;
    public delegate void OnAimOffEvent();
    public OnAimOffEvent OnAimOff;
    public delegate void OnDeadEvent();
    public OnAimOffEvent OnDead;
    public delegate void OnSpearDropEvent();
    public OnAimOffEvent OnSpearDrop;
    public delegate void OnAbsorbAllCoreEvent();
    public OnAbsorbAllCoreEvent OnAbsorbAllCore;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        ikCtrl = GetComponent<IKCtrl>();
        handIKCtrl = GetComponent<HandIKCtrl>();
        collider = GetComponent<CapsuleCollider>();
        ragdoll = GetComponent<PlayerRagdoll>();

        GameManager.Instance.SetPlayer(this);

    }
    // Start is called before the first frame update
    void Start()
    {
        headCheckLayer = (-1) - (1 << LayerMask.NameToLayer("DetectCollider"));
        ropeCheckLayer = 1 << LayerMask.NameToLayer("Rope");
        spearLayer = 1 << LayerMask.NameToLayer("Spear");
        detectionLayer = 1 << LayerMask.NameToLayer("ClimbingAble");

        if (specialSpearVisualPrefab != null)
        {
            specialSpearVisualObject = Instantiate(specialSpearVisualPrefab, Vector3.zero, Quaternion.identity);
            specialSpearVisualObject.SetActive(false);
        }

        if (specialSpearVisualFloat != null)
        {
            specialSpearVisualFloat.SetActive(false);
        }

        if (specialSpearDissolve != null)
        {
            spearDissolve = specialSpearDissolve.GetComponent<Renderer>().material;
        }

        moveDir = Vector3.zero;
        currentSpeed = walkSpeed;
        mainCameraTrasform = Camera.main.transform;
        //cameraCtrl = mainCameraTrasform.parent.GetComponent<CameraCtrl>();
        playerAnimCtrl = GetComponent<PlayerAnimCtrl>();

        colliderRadius = collider.radius;
        colliderHeight = collider.height;

        //StartCoroutine(UpdateMovementVelocity());

        OnShot += () =>
        {
            isAim = false;
            cameraCtrl.SetCamMode(CamMode.Default);
        };

        if (nonStaminaMode == false)
        {
            StartCoroutine(StaminaTick());
        }
        StartCoroutine(StaminaCheck());
    }

    // Update is called once per frame
    void Update()
    {
        if(isPause == true)
        {
            return;
        }

        InputUpdate();

        ProcessUpdate();
    }

    private void FixedUpdate()
    {
        if(state == PlayerState.Grab)
        {
            CheckLedge();
        }
    }

    private void InputUpdate()
    {
        camForward = mainCameraTrasform.forward;
        camRight = mainCameraTrasform.right;
        camForward.y = 0;
        camRight.y = 0;

        inputVertical = InputManager.Instance.GetMoveAxisVertical();
        inputHorizontal = InputManager.Instance.GetMoveAxisHorizontal();

        if (isPauseControl == true)
            return;

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
                    if(InputManager.Instance.GetAction(KeybindingActions.Jump))
                    {
                        currentJumpPower = jumpPower;
                        isGround = false;
                        groundCheck.Lock();
                        prevSpeed = currentSpeed;

                        ChangeState(PlayerState.Jump);
                        return;
                    }

                    if (InputRolling())
                        return;

                    if (InputTryGrab())
                        return;

                    if (InputHangingRope())
                        return;

                    if (InputAbsorb())
                        return;

                    InputShot();

                    InputDropSpear();
                }
                break;
            case PlayerState.Jump:
                {
                    if (InputTryGrab())
                        return;

                    if (InputHangingRope())
                        return;

                    InputShot();

                    InputDropSpear();
                }
                break;
            case PlayerState.Rolling:
                {
                    InputShot();

                    InputDropSpear();
                }
                break;
            case PlayerState.Grab:
                {
                    if (InputRelaseGrab())
                        return;

                    if (InputJumpFromWall())
                        return;

                    InputDropSpear();
                }
                break;
            case PlayerState.HangLedge:
                {
                    if (InputRelaseGrab())
                        return;

                    if (InputClimbingLedge())
                        return;

                    InputShot();

                    InputDropSpear();
                }
                break;
            case PlayerState.ClimbingLedge:
                {
                    
                }
                break;
            case PlayerState.HangRope:
                {
                    if (InputReleaseRope())
                        return;

                    InputRopeCtrl();

                    InputShot();

                    InputDropSpear();
                }
                break;
            case PlayerState.Absorb:
                break;
            case PlayerState.Sliding:
                break;
            case PlayerState.Stagger:
                break;
            case PlayerState.Nuckback:
                break;
        }

        InputSpacialSpear();
    }

    private void ProcessUpdate()
    {
        if (rigidbody.velocity != Vector3.zero)
        {
            rigidbody.velocity = Vector3.zero;
        }

        if (state == PlayerState.Jump)
        {
            currentJumpPower -= gravity * Time.deltaTime;
            currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, 50f);
        }
        else
        {
            currentJumpPower = 0f;
        }

        UpdateFallingTime();

        UpdateCurrentSpeed();

        UpdateDetect();

        switch (state)
        {
            case PlayerState.Default:
                {
                    if (isGround == false && isEnoughGround == false)
                    {
                        ChangeState(PlayerState.Jump);
                        return;
                    }

                    UpdateSliding();

                    currentJumpPower = 0.0f;

                    if (inputVertical != 0.0f || inputHorizontal != 0.0f)
                    {
                        moveDir = (camForward * inputVertical) + (camRight * inputHorizontal);
                        moveDir.Normalize();
                        prevDir = moveDir;
                    }
                    else
                    {
                        moveDir = prevDir;
                        moveDir.Normalize();
                    }

                    moveDir *= currentSpeed;

                    if (moveDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 6.0f);
                    }

                    if (CheckMoveCollision(moveDir) == true)
                    {
                        transform.position = transform.position + (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;
                    }
                    else
                    {
                        transform.position = transform.position + (Vector3.up * currentJumpPower) * Time.deltaTime;
                    }
                    animator.SetFloat("Speed", currentSpeed);
                }
                break;
            case PlayerState.Jump:
                {
                    if (isGround == true)
                    {
                        state = PlayerState.Default;
                        animator.SetBool("IsJump", false);
                        return;
                    }

                    Vector3 plusDir = ((camForward * inputVertical) + (camRight * inputHorizontal));
                    transform.position += plusDir * fallingControlSenstive * Time.deltaTime;

                    Vector3 lookDir = ((camForward * inputVertical) + (camRight * inputHorizontal)).normalized;
                    if (lookDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, transform.up), Time.deltaTime * 1.0f);
                    }

                    if (CheckMoveCollision(moveDir) == true)
                    {
                        transform.position = transform.position + (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;
                    }
                    else
                    {
                        transform.position = transform.position + (Vector3.up * currentJumpPower) * Time.deltaTime;
                    }
                }
                break;
            case PlayerState.Rolling:
                {
                    if (isRollingReady == true)
                    {
                        moveDir.Normalize();
                    }
                    else
                    {
                        moveDir *= currentSpeed;
                    }

                    if (moveDir == Vector3.zero)
                    {
                        moveDir = prevDir;
                    }

                    moveDir.Normalize();
                    currentSpeed = Mathf.Lerp(currentSpeed, rollingSpeed, 15f * Time.deltaTime);
                    moveDir *= currentSpeed;
                    Vector3 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 5.0f).eulerAngles;
                    transform.rotation = Quaternion.Euler(0.0f, targetRot.y, 0.0f);

                    if (CheckMoveCollision(moveDir) == true)
                    {
                        transform.position = transform.position + (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;
                    }
                    else
                    {
                        transform.position = transform.position + (Vector3.up * currentJumpPower) * Time.deltaTime;
                    }
                }
                break;
            case PlayerState.Grab:
                {
                    //CheckLedge();

                    UpdateGrab();
                }
                break;
            case PlayerState.HangLedge:
                {
                    UpdateGrab();
                }
                break;
            case PlayerState.ClimbingLedge:
                break;
            case PlayerState.HangRope:
                break;
            case PlayerState.Absorb:
                break;
            case PlayerState.Sliding:
                {
                    currentSlidingSpeed = Mathf.MoveTowards(currentSlidingSpeed, maxSlidingSpeed, 5f * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(slidingDir.x, 0, slidingDir.z), Vector3.up), 5f * Time.deltaTime);
                    prevSlidingDir = slidingDir;
                    prevSlidingDir.y = 0;
                    //ikCtrl.DisableFeetIk();

                    transform.position = transform.position + slidingDir * currentSlidingSpeed * Time.deltaTime;
                    if (groundAngle < balanceLimitMinAngle)
                    {
                        currentSlidingSpeed = 0.0f;
                        prevDir = prevSlidingDir;
                        animator.SetTrigger("EndSliding");

                        state = PlayerState.Default;
                        //ikCtrl.EnableFeetIk();
                    }

                    if (!Physics.Raycast(transform.position, -transform.up, 6f))
                    {
                        currentSlidingSpeed = 0.0f;
                        prevDir = prevSlidingDir;
                        animator.SetTrigger("EndSliding");

                        ChangeState(PlayerState.Jump);
                    }
                }
                break;
            case PlayerState.Stagger:
                {
                    moveDir = prevDir;
                    if (CheckMoveCollision(moveDir) == true)
                    {
                        transform.position = transform.position + (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;
                    }
                    else
                    {
                        transform.position = transform.position + (Vector3.up * currentJumpPower) * Time.deltaTime;
                    }
                }
                break;
            case PlayerState.Nuckback:
                {
                    transform.position = transform.position + (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;
                }
                break;
        }
    }

    #region 매 프레임 처리 함수(ProcessUpdate에서 매 프레임 호출되는 함수들)

    private void UpdateFallingTime()
    {
        if (isGround == true || state == PlayerState.Grab || state == PlayerState.HangRope || state == PlayerState.HangLedge)
        {
            fallingTime = 0.0f;
        }
        else
        {
            fallingTime += Time.deltaTime;
        }
    }
    private void UpdateSliding()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * 0.3f, Color.blue);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            float angle = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up)) * Mathf.Rad2Deg;
            groundAngle = angle;

            if (groundAngle >= balanceLimitMinAngle)
            {
                if (state == PlayerState.Default && isActiveSlidingCheck == false)
                {
                    if (groundAngle >= balanceLimitMaxAngle)
                    {
                        return;
                    }
                    else
                    {
                        isActiveSlidingCheck = true;
                        StartCoroutine(SlidingCheck());
                    }
                }
                slidingDir = MathEx.GetSlidingVector(Vector3.down, hit.normal).normalized;
            }
        }
    }
    private void UpdateDetect()
    {
        Vector3 sideCheckPos = sideCheck.transform.localPosition;
        sideCheckPos.x = currentHorizontalValue * 0.8f;
        sideCheck.transform.localPosition = sideCheckPos;

        isUpDetect = upCheck.GetIsDetected();
        isSideDetect = sideCheck.GetIsDetected();
        isEnoughGround = enoughGroundCheck.GetIsDetected();

    }
    private void UpdateGrab()
    {
        RaycastHit hit;
        Vector3 startPos = transform.position + transform.up * (colliderHeight * 0.5f) + (-transform.forward * 1f);

        if (Physics.SphereCast(startPos, colliderRadius, transform.forward, out hit, 3.0f, climbingLayer))
        {
            if ((hit.point - (transform.position + transform.up * (colliderHeight * 0.5f))).magnitude > 0.5f)
            {
                transform.position = (hit.point - transform.up * (colliderHeight * 0.5f)) + hit.normal * 0.3f;
            }

            transform.rotation = Quaternion.LookRotation(-hit.normal, transform.up);

            if (hit.collider.transform != transform.parent)
            {
                transform.parent = hit.collider.transform;
            }

            Vector3 slidingDir = -MathEx.GetSlidingVector(Vector3.down, hit.normal).normalized;
            climbingUpAngle = Mathf.Acos(Vector3.Dot(slidingDir, transform.up)) * Mathf.Rad2Deg;
        }
        else
        {
            return;
        }

        if (state == PlayerState.HangLedge)
        {
            bool hitResult = Physics.SphereCast(handOffset.position, 0.1f, -transform.up, out hit, 1f, climbingLayer);
            if (hitResult == true)
            {
                Vector3 dist = hit.point - handOffset.position;
                transform.position += dist;
            }
        }
    }
    private void UpdateCurrentSpeed()
    {
        if (state == PlayerState.Stagger)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, Time.deltaTime * 6.0f);
            return;
        }

        if (inputVertical != 0 || inputHorizontal != 0)
        {
            if (isRun == true)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, Time.deltaTime * 4.0f);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, Time.deltaTime * 4.0f);
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, Time.deltaTime * 8.0f);
        }
    }
    public void SaveHandPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            offsetDist = (hit.point - transform.position).magnitude;
        }
    }
    public void AdjustLedgeOffset()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, climbingLayer))
        {
            float currentDist = (hit.point - transform.position).magnitude;
            if (currentDist > offsetDist)
            {
                float gap = currentDist - offsetDist;
                transform.position += transform.forward * gap;
            }
        }
    }

    #endregion

    #region 인풋 처리 함수
    private void UpdateInputValue(float vertical, float horizontal)
    {
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

        if (state == PlayerState.HangLedge && isCanInputClimbing == true)
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
                currentVerticalValue = -1.0f;
            }

            if (horizontal > 0.0f)
            {
                currentHorizontalValue = Mathf.Ceil(horizontal);
            }
            else
            {
                currentHorizontalValue = Mathf.Floor(horizontal);
            }

            if (currentVerticalValue == -1.0f)
            {
                animator.SetBool("IsReverseClimbing", true);
                animator.SetBool("IsLedgeHanging", false);

                state = PlayerState.Grab;
                return;
            }
            else if (currentVerticalValue == 0.0f && currentHorizontalValue != 0.0f)
            {
                if (currentHorizontalValue == 1.0f)
                {
                    animator.SetBool("IsLedgeMoveRight", true);
                    animator.SetBool("IsLedgeMoving", true);
                }
                else
                {
                    animator.SetBool("IsLedgeMoveLeft", true);
                    animator.SetBool("IsLedgeMoving", true);
                }
                return;
            }
        }

        if (state == PlayerState.HangRope && isCanInputClimbing == true)
        {
            if (vertical > 0.0f)
            {
                currentVerticalValue = 1.0f;

                animator.SetTrigger("RopeClimbing");
                animator.SetInteger("RopeVertical", (int)currentVerticalValue);
                isCanInputClimbing = false;
            }
            else if (vertical < 0.0f)
            {
                currentVerticalValue = -1.0f;

                animator.SetTrigger("RopeClimbing");
                animator.SetInteger("RopeVertical", (int)currentVerticalValue);
                isCanInputClimbing = false;
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

        if (isCanInputClimbing == true && GameManager.Instance.GetBossState() != LevelEdit_BehaviorControll.State.Disturbance)
        {
            if (vertical == 0.0f)
            {
                currentVerticalValue = 0.0f;
                //    animator.SetBool("IsClimbing", false);
                //    animator.SetBool("IsReverseClimbing", false);
            }
            else if (vertical >= 0.0f)
            {
                //if (HeadCheck() == false)
                //{
                //    currentVerticalValue = 1.0f;
                //    //animator.SetBool("IsClimbing", true);
                //    //animator.SetBool("IsReverseClimbing", false);
                //}
                //else
                //{
                //    currentVerticalValue = 0.0f;
                //    //animator.SetBool("IsClimbing", false);
                //    //animator.SetBool("IsReverseClimbing", false);
                //}
                currentVerticalValue = 1.0f;
            }
            else
            {
                if (isGround == false)
                {
                    currentVerticalValue = -1.0f;
                    //animator.SetBool("IsClimbing", false);
                    //animator.SetBool("IsReverseClimbing", true);
                }
                else
                {
                    currentVerticalValue = 0.0f;
                    //animator.SetBool("IsClimbing", false);
                    //animator.SetBool("IsReverseClimbing", false);
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

            RaycastHit hit;
            if (currentVerticalValue != 0.0f)
            {
                if (currentVerticalValue > 0.0f)
                {

                    //if (Physics.SphereCast(upPoint.position, 0.2f, transform.forward, out hit, 1f,detectionLayer))
                    //{
                    //    Debug.Log(LayerMask.LayerToName(hit.collider.gameObject.layer));
                    //}
                    if (isUpDetect == true)
                    {
                        if (Physics.SphereCast(upPoint.position, 0.27f, transform.forward, out hit, 1f, detectionLayer))
                        {
                            animator.SetBool("IsClimbing", true);
                            animator.SetBool("IsReverseClimbing", false);
                            animator.SetBool("IsSideClimbing", false);
                        }
                    }
                    else
                    {
                        animator.SetBool("IsClimbing", true);
                        animator.SetBool("IsReverseClimbing", false);
                        animator.SetBool("IsSideClimbing", false);
                    }
                }
                else
                {
                    //if (Physics.SphereCast(transform.position - transform.up * 0.2f, collider.radius, transform.forward, out hit,1f, climbingLayer))
                    //{
                    animator.SetBool("IsClimbing", false);
                    animator.SetBool("IsReverseClimbing", true);
                    animator.SetBool("IsSideClimbing", false);
                    //}
                }
            }
            else
            {
                animator.SetBool("IsClimbing", false);
                animator.SetBool("IsReverseClimbing", false);

                if (currentHorizontalValue > 0.0f)
                {
                    if (Physics.SphereCast(transform.position + transform.right * collider.radius * 2f, collider.radius, transform.forward, out hit, 1f, climbingLayer))
                    {
                        animator.SetBool("IsSideClimbing", true);
                        animator.SetBool("Left", false);
                    }
                }
                else if (currentHorizontalValue < 0.0f)
                {
                    if (Physics.SphereCast(transform.position - transform.right * collider.radius * 2f, collider.radius, transform.forward, out hit, 1f, climbingLayer))
                    {
                        animator.SetBool("IsSideClimbing", true);
                        animator.SetBool("Left", true);
                    }
                }
                else
                {
                    animator.SetBool("IsSideClimbing", false);
                }
            }
        }

        //if (currentVerticalValue == 0.0f && horizontal != 0.0f)
        //{
        //    animator.SetBool("IsClimbing", true);
        //    animator.SetBool("IsReverseClimbing", false);
        //}

    }

    private bool InputRolling()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.RollingStart))
        {
            rollingTime = Time.time;
            return true;
        }

        if (InputManager.Instance.GetAction(KeybindingActions.Rolling))
        {
            if (Time.time - rollingTime < 0.2f)
            {
                animator.SetTrigger("Rolling");
                animator.SetBool("IsRolling", true);
                isRollingReady = true;

                state = PlayerState.Rolling;

                return true;
            }
        }

        return false;
    }

    private bool InputTryGrab()
    {
        Vector3 startPos;
        if (InputManager.Instance.GetAction(KeybindingActions.Grab))
        {
            startPos = transform.position + transform.up * collider.height * 0.5f;
            Collider[] colliders = Physics.OverlapCapsule(startPos, startPos + transform.forward * 0.5f, 0.4f, climbingLayer);
            if (colliders.Length != 0)
            {
                animator.SetBool("IsGrab", true);
                transform.SetParent(colliders[0].transform);
                RaycastHit hit;
                if (Physics.Raycast(startPos, transform.forward, out hit, 3.0f, climbingLayer))
                {
                    Vector3 normalReverse = -hit.normal;
                    transform.rotation = Quaternion.LookRotation(normalReverse);
                    transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * collider.radius;
                }

                animator.SetBool("IsJump", false);

                currentSpeed = 0.0f;
                currentJumpPower = 0.0f;

                rigidbody.useGravity = false;
                rigidbody.velocity = Vector3.zero;

                isCanInputClimbing = true;

                prevSpeed = currentSpeed;

                groundCheck.UnLock();

                moveDir = Vector3.zero;

                state = PlayerState.Grab;
                return true;
            }
            else
            {
                RaycastHit hit;
                startPos = transform.position;
                if (Physics.Raycast(startPos, -transform.up, out hit, 0.5f, climbingLayer))
                {
                    if (state != PlayerState.Sliding)
                    {
                        transform.rotation = Quaternion.LookRotation(-hit.normal, transform.forward);
                    }
                    else
                    {
                        transform.rotation = Quaternion.LookRotation(-hit.normal, transform.up);
                        currentSlidingSpeed = 0.0f;
                    }

                    transform.position = (hit.point) + (hit.normal) * collider.radius;

                    state = PlayerState.Grab;
                    currentSpeed = 0.0f;
                    currentJumpPower = 0.0f;

                    transform.SetParent(hit.transform);

                    animator.SetBool("IsGrab", true);

                    isCanInputClimbing = true;

                    moveDir = Vector3.zero;
                    return true;
                }
            }
        }

        return false;
    }
    private bool InputRelaseGrab()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.ReleaseGrab))
        {
            isMustClimbing = false;

            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.x = 0.0f;
            currentRot.z = 0.0f;
            transform.rotation = Quaternion.Euler(currentRot);
            //animator.SetBool("IsGrab", false);
            //animator.SetBool("IsLedgeHanging", false);
            //animator.SetBool("IsLedgeMoving", false);
            //animator.SetBool("IsLedgeMoveLeft", false);
            //animator.SetBool("IsLedgeMoveRight", false);
            //animator.SetBool("IsSideClimbing", false);

            groundCheck.RequestDetach();
            //rigidbody.useGravity = true;
            //transform.parent = null;
            handIKCtrl.DisableLeftHandIk();
            handIKCtrl.DisableRightHandIk();

            animator.applyRootMotion = false;

            //collider.isTrigger = false;
            ColliderInit();

            ChangeState(PlayerState.Jump);
            return true;
        }
        return false;
    }

    private bool InputJumpFromWall()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Jump))
        {
            isMustClimbing = false;

            Vector3 backVector = -transform.forward;
            backVector.y = 0f;
            transform.rotation = Quaternion.LookRotation(backVector);

            moveDir = transform.forward;
            moveDir.Normalize();
            moveDir *= runSpeed;
            currentJumpPower = jumpPower;
            transform.position = transform.position + (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;

            animator.SetBool("IsGrab", false);
            groundCheck.RequestDetach();

            animator.applyRootMotion = false;

            ColliderInit();

            ChangeState(PlayerState.Jump);

            return true;
        }

        return false;
    }

    private bool InputClimbingLedge()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Jump) && isLedgeSideMove == false)
        {
            if (climbingUpAngle > 22.5f)
            {
                return false;
            }

            animator.applyRootMotion = true;
            animator.SetBool("IsGrab", false);
            animator.SetBool("IsLedgeHanging", false);
            animator.SetBool("IsClimbing", false);
            animator.SetTrigger("LedgeClimbing");

            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.x = 0.0f;
            currentRot.z = 0.0f;
            transform.rotation = Quaternion.Euler(currentRot);

            climbingPrevRot = transform.rotation;
            prevForward = transform.forward;

            ColliderInit();

            state = PlayerState.ClimbingLedge;

            return true;
        }

        return false;
    }

    private bool InputHangingRope()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Interaction))
        {
            Vector3 startPos = transform.position + transform.up * (colliderHeight * 0.5f);

            Collider[] colliders = Physics.OverlapSphere(startPos, ropeDetectRange, ropeCheckLayer);

            if (colliders.Length != 0)
            {
                int targetCount = 0;
                for (int i = 1; i < colliders.Length; i++)
                {
                    float dist1 = (transform.position - colliders[i].transform.position).sqrMagnitude;
                    float dist2 = (transform.position - colliders[targetCount].transform.position).sqrMagnitude;
                    if (dist1 < dist2 && colliders[i].GetComponent<RopeBuiltIn>().canHanging == true)
                    {
                        targetCount = i;
                    }
                }

                if (colliders[targetCount].GetComponent<RopeBuiltIn>().canHanging == false)
                {
                    return false;
                }

                currentHangingRope = colliders[targetCount].GetComponent<RopeBuiltIn>();

                currentHangingRope.InitializeRope(transform.position + transform.up * colliderHeight);
                transform.parent = currentHangingRope.transform;
                transform.position = currentHangingRope.transform.position - transform.up * colliderHeight;
                isCanInputClimbing = true;

                rigidbody.isKinematic = true;

                animator.SetBool("IsHangingRope", true);
                currentVerticalValue = 0.0f;

                state = PlayerState.HangRope;

                return true;
            }
        }

        return false;
    }

    private bool InputReleaseRope()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Interaction))
        {
            rigidbody.isKinematic = false;
            //rigidbody.constraints = defaultConstrains;
            transform.parent = null;

            moveDir = Vector3.zero;
            currentJumpPower = 0.0f;

            currentJumpPower = jumpPower * 0.5f;
            isGround = false;
            moveDir = camForward;
            currentSpeed = walkSpeed;

            animator.SetBool("IsHangingRope", false);

            transform.rotation = Quaternion.LookRotation(camForward, transform.up);

            ChangeState(PlayerState.Jump);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void InputRopeCtrl()
    {
        if (currentHangingRope != null)
        {
            currentHangingRope.ClimbingRope(-currentVerticalValue, ropeClimbingSpeed, transform.right, ropeHandLeft);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(camForward, transform.up), 4f * Time.deltaTime);
        }
    }

    private bool InputAbsorb()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Interaction) && isCanAbsorb == true)
        {
            animator.SetTrigger("Absorb");

            if (currentDetectEnergyCore != null)
            {
                transform.SetPositionAndRotation(currentDetectEnergyCore.absorbTransform.position, currentDetectEnergyCore.absorbTransform.rotation);

                AbsorbEvent absorbEvent = currentDetectEnergyCore.GetComponent<AbsorbEvent>();

                playerAnimCtrl.BindPierceEvent(absorbEvent.GetPierceEvent());
                playerAnimCtrl.BindPullEvent(absorbEvent.GetPullEvent());
                playerAnimCtrl.BindAbsorbEndEvent(absorbEvent.GetAbsorbEndEvent());

                currentDetectEnergyCore.Over();

                isCanAbsorb = false;
                currentDetectEnergyCore = null;

                abosrbCount++;

                state = PlayerState.Absorb;
                return true;
            }
        }

        return false;
    }

    private void InputAim()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && currentSpearNum != 0)
        {
            isAim = true;
            cameraCtrl.SetCamMode(CamMode.Aim);
            GameManager.Instance.timeManager.OnBulletTime();

            OnAim?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && isAim == true)
        {
            isAim = false;
            cameraCtrl.SetCamMode(CamMode.Default);
            GameManager.Instance.timeManager.OffBulletTime();
            OnAimOff?.Invoke();
        }
    }

    private void InputShot()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Aiming) && currentSpearNum != 0)
        {
            isAim = true;
            //cameraCtrl.SetCamMode(CamMode.Aim);
            //GameManager.Instance.cameraManger.ActiveAimCamera(() => { GameManager.Instance.timeManager.OnBulletTime(); });
            GameManager.Instance.cameraManger.ActiveAimCamera();
            //GameManager.Instance.timeManager.OnBulletTime();

            OnAim?.Invoke();
        }

        if (isAim == true && InputManager.Instance.GetAction(KeybindingActions.AimingCancle))
        {
            isAim = false;
            //cameraCtrl.SetCamMode(CamMode.Default);
            GameManager.Instance.cameraManger.ActivePlayerFollowCamera();
            GameManager.Instance.timeManager.OffBulletTime();
            OnAimOff?.Invoke();
        }

        if (isAim == true && InputManager.Instance.GetAction(KeybindingActions.Shot) && currentSpearNum != 0)
        {
            if (isSpacialSpearMode == false)
            {
                if (spearPrefab != null && launchPos != null)
                {
                    SpearCtrl spear = Instantiate(spearPrefab, launchPos.position, launchPos.rotation);
                    spear.Launch(launchPos.position);
                }
                ReleaseAim();

                currentSpearNum -= 1;
            }
            else
            {
                RaycastHit hit;
                if (Physics.SphereCast(mainCameraTrasform.position, 1f, camForward, out hit, 4f, spacialSpearShotCheckLayer))
                {
                    ReleaseAim();

                }
                else
                {
                    SpecialSpearCtrl specialSpear = Instantiate(specialSpearPrefab, launchPos.position, launchPos.rotation);
                    specialSpear.Launch(GameManager.Instance.GetCoreTransform());
                    specialSpear.AddListener(GameManager.Instance.bossControll.ExplosionProgress);

                    GameManager.Instance.CameraEventIntroduction_Immediate(GameManager.Instance.GetKillEventTransform());
                    GameManager.Instance.LookingEvent_CameraCollision(specialSpear.transform);


                    GameManager.Instance.SetCameraFov();

                    ReleaseAim();
                }
            }
        }
    }

    public void ReleaseAim()
    {
        isAim = false;
        //cameraCtrl.SetCamMode(CamMode.Default);
        GameManager.Instance.cameraManger.ActivePlayerFollowCamera();
        GameManager.Instance.timeManager.OffBulletTime();

        OnAimOff?.Invoke();
    }

    private bool InputDropSpear()
    {
        Collider[] coll = Physics.OverlapSphere(centerPoint.position, 2f, spearLayer);
        if (coll.Length != 0)
        {
            isSpearDetect = true;
        }
        else
        {
            isSpearDetect = false;
        }


        if (isSpearDetect == true && InputManager.Instance.GetAction(KeybindingActions.Interaction) && isCanAbsorb == false)
        {
            Collider[] spears = Physics.OverlapSphere(spearDetect.transform.position, 2f, spearLayer);
            if (spears.Length != 0)
            {
                Destroy(spears[0].gameObject);
                currentSpearNum++;
                OnSpearDrop?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void InputSpacialSpear()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && isCanEquipSpeicalSpear == true && isSpearDissolve == false)
        {
            isSpacialSpearMode = !isSpacialSpearMode;
            if (isSpacialSpearMode == true)
            {
                //specialSpearVisualFloat.SetActive(true);
                StartCoroutine(SpearAppear());
            }
            else
            {
                //specialSpearVisualFloat.SetActive(false);
                StartCoroutine(SpearExit());
            }
        }
    }

    #endregion

    #region 애니메이션 이벤트 호출 함수
    public void SetIsClimbingLedge(bool result)
    {
        if (result == false)
        {
            state = PlayerState.Default;
            collider.enabled = true;
            rigidbody.velocity = Vector3.zero;
            currentJumpPower = 0.0f;
        }
        else
        {
            collider.enabled = false;
            currentJumpPower = 0.0f;
            state = PlayerState.ClimbingLedge;
        }
    }
    public void SetIsMustClimbing(bool result) { isMustClimbing = result; }
    public void SetIsCanInputClimbing(bool result)
    {
        isCanInputClimbing = result;
    }

    public void SetIsLedgeSideMove(bool result)
    {
        isLedgeSideMove = result;
    }
 
    public void SetRopeHandInfo(bool isLeft) { ropeHandLeft = isLeft; }

    public void StartRolling()
    {
        isRollingReady = false;
        //animator.SetBool("IsRolling",true);
    }
    public void ReleaseRolling()
    {
        animator.SetBool("IsRolling", false);
        currentSpeed = walkSpeed;
        //Debug.Log("ReleaseRolling");
        state = PlayerState.Default;
    }

    public void ActiveSetSpecialSpearVisual_Absorb(bool value)
    {
        if (specialSpearVisualObject == null)
            return;

        specialSpearVisualObject.transform.parent = rightHandBone;
        specialSpearVisualObject.transform.localPosition = Vector3.zero;
        specialSpearVisualObject.transform.localRotation = Quaternion.Euler(180.0f, 0.0f, 0.0f);
        specialSpearVisualObject.SetActive(value);
    }

    public void EndAbsorb()
    {
        state = PlayerState.Default;
    }

    public void EndStagger()
    {
        state = PlayerState.Sliding;
    }


    #endregion

    private bool CheckMoveCollision(Vector3 moveDir)
    {
        bool result = Physics.Raycast(transform.position + transform.up * (colliderHeight * 0.5f + 0.1f), moveDir, colliderRadius, floorLayer);
        if (result == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void CheckLedge()
    {
        if (isMustClimbing  && currentVerticalValue == 1.0f && isUpDetect == false)
        {
            isMustClimbing = false;
            animator.applyRootMotion = false;
            state = PlayerState.HangLedge;
            animator.SetBool("IsLedgeHanging", true);
        }
    }

    private bool UpClimbingDetection(float length)
    {
        //Vector3 layStartPos = transform.position + transform.up * controller.height;
        Vector3 layStartPos = transform.position + transform.up * colliderHeight;
        RaycastHit hit;
        Vector3 sphereStart;

        Debug.DrawRay(layStartPos, transform.forward * 2.0f);
        if (Physics.Raycast(layStartPos, transform.forward, out hit, 2.0f, climbingLayer))
        {
            sphereStart = hit.point;
        }
        else
        {
            //Debug.Log("false");
            return false;
        }

        Vector3 upDir = Quaternion.Euler(-90.0f, 0.0f, 0.0f) * hit.normal;
        //if (Physics.SphereCast(sphereStart, controller.radius, upDir*length, out hit, climbingLayer ))
        if (Physics.SphereCast(sphereStart, colliderRadius, transform.up, out hit, length, climbingLayer))
        {
            //Debug.Log("true");
            return true;
        }
        else
        {
            //Debug.Log("false");
            return false;
        }

    }

   
    private void ColliderInit()
    {
        //collider.height = colliderHeight;
        //collider.radius = colliderRadius;
    }

    IEnumerator SlidingCheck()
    {
        bool isReadySliding = true;
        float time = 0.0f;

        while (time < holdOutTime)
        {
            if (state== PlayerState.Grab|| isEnoughGround == false || state == PlayerState.Absorb)
            {
                isReadySliding = false;
                break;
            }

            if (groundAngle < balanceLimitMinAngle)
            {
                isReadySliding = false;
                break;
            }

            if (groundAngle > balanceLimitMaxAngle)
            {
                isReadySliding = false;
                break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        if (isReadySliding == true)
        {
            animator.SetTrigger("Stagger");

            state = PlayerState.SlidingStagger;
        }

        isActiveSlidingCheck = false;
    }


    public void Quake()
    {
        if (state == PlayerState.Default || state == PlayerState.Rolling)
        {
            animator.SetTrigger("Quake");
            state = PlayerState.Stagger;
        }
    }

    public void NuckBack(Vector3 causePos)
    {
        if(state == PlayerState.Nuckback)
        {
            return;
        }

        if (transform.position.y > causePos.y)
        {
            return;
        }

        Vector3 causeDir = causePos - transform.position;
        causeDir.y = 0.0f;
        causeDir.Normalize();

        moveDir = -causeDir;
        moveDir *= 8f;

        currentJumpPower = jumpPower;
        //transform.position = transform.position + (moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;
        //transform.position = transform.position + moveDir + Vector3.up * currentJumpPower * Time.deltaTime;
        //transform.rotation = Quaternion.LookRotation(causeDir,transform.up);
        isGround = false;

        collider.isTrigger = true;

        StartCoroutine(PogressNuckBack(2f, 8f));
        TakeDamage();
    }

    IEnumerator PogressNuckBack(float time, float startSpeed)
    {
        state = PlayerState.Nuckback;
        animator.SetBool("IsNuckback", true);

        float currentTime = 0.0f;
        float currentNuckBackSpeed = startSpeed;

        while (currentTime < time)
        {
            if (currentTime > 0.5f)
            {
                collider.isTrigger = false;
            }

            currentNuckBackSpeed = Mathf.Lerp(startSpeed, 0.0f, currentTime / time);
            moveDir.Normalize();
            moveDir *= currentNuckBackSpeed;
            prevDir = -moveDir;

            transform.rotation = Quaternion.LookRotation(-moveDir, transform.up);

            isGround = false;

            currentTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("IsNuckback", false);
        state = PlayerState.Default;
    }

    private void TakeDamage()
    {
        Debug.Log("데미지!");
        hp.Value -= damage;
        if (hp.Value <= 0f)
        {
            OnDead?.Invoke();
        }
    }

    public void SetIsGround(bool result)
    {
        if (result == false)
        {
            isGround = false;
        }
        else
        {
            bool hitResult = Physics.Raycast(transform.position, Vector3.down, 0.5f, floorLayer);
            if (hitResult == true)
            {
                isGround = true;
                //ikCtrl.EnableFeetIk();
                CheckFallingDamage();

                //Debug.Log("SetIsGround");
                //state = PlayerState.Default;
            }
            else
            {
                isGround = false;
            }
        }
        //animator.SetBool("IsGround", isGround);
    }

    private void CheckFallingDamage()
    {
        if (fallingTime >= 3.5f)
        {
            TakeDamage();
            fallingTime = 0.0f;
            //Debug.Log("Damage");
        }
    }

    public void SetParent(Transform parent)
    {
        if (state != PlayerState.Grab && state != PlayerState.HangRope&& state != PlayerState.ClimbingLedge)
        {
            transform.parent = parent;
        }
    }

    private void ChangeState(PlayerState changeState)
    {
        state = changeState;
        switch(state)
        {
            case PlayerState.Jump:
                {
                    animator.SetBool("IsGrab", false);
                    animator.SetBool("IsLedgeHanging", false);
                    animator.SetBool("IsLedgeMoving", false);
                    animator.SetBool("IsLedgeMoveLeft", false);
                    animator.SetBool("IsLedgeMoveRight", false);
                    animator.SetBool("IsSideClimbing", false);

                    animator.SetBool("IsJump", true);
                }
                break;
        }
    }

    IEnumerator SpearAppear()
    {
        float weight = 1f;
        isSpearDissolve = true;
        spearDissolve.SetFloat("_Weight", weight);
        specialSpearDissolve.SetActive(true);
        while (weight > 0f)
        {
            weight -= 2f * Time.deltaTime;
            spearDissolve.SetFloat("_Weight", weight);

            yield return null;
        }
        specialSpearDissolve.SetActive(false);
        specialSpearVisualFloat.SetActive(true);
        isSpearDissolve = false;
    }

    IEnumerator SpearExit()
    {
        float weight = 0f;
        isSpearDissolve = true;
        spearDissolve.SetFloat("_Weight", weight);
        specialSpearVisualFloat.SetActive(false);
        specialSpearDissolve.SetActive(true);
        while (weight < 1f)
        {
            weight += 2f * Time.deltaTime;
            spearDissolve.SetFloat("_Weight", weight);

            yield return null;
        }
        specialSpearDissolve.SetActive(false);
        isSpearDissolve = false;
    }

    public void Pause() { isPause = true; }

    public void PauseControl(bool result) { isPause = result; }
    public void Resume() { isPause = false; }
    public void ClearAllCore() { isCanEquipSpeicalSpear = true; OnAbsorbAllCore?.Invoke(); }

    #region 겟터
    public float GetStamina() { return stamina.Value; }

    public int GetCurrentSpearNum() { return currentSpearNum; }

    public bool CheckInterantion()
    {
        if ((isCanAbsorb == true || isSpearDetect == true) && state != PlayerState.HangLedge && state != PlayerState.HangRope)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region 유니티 충돌 콜백
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Core"))
        {
            isCanAbsorb = true;
            currentDetectEnergyCore = other.GetComponent<EnergyCore>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Core"))
        {
            isCanAbsorb = false;
            currentDetectEnergyCore = null;
        }
    }

    #endregion

    IEnumerator StaminaTick()
    {
        while (true)
        {
            if (state == PlayerState.Grab || state == PlayerState.HangLedge || state == PlayerState.HangRope)
            {
                stamina.Value -= 4f;
            }
            else
            {
                stamina.Value += 25f;
            }

            stamina.Value = Mathf.Clamp(stamina.Value, 0.0f, 100.0f);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator StaminaCheck()
    {
        while (true)
        {
            if (stamina.Value == 0.0f)
            {
                if ((state == PlayerState.Grab))
                {                                 
                    isLedgeSideMove = false;
               
                    groundCheck.RequestDetach();
                    handIKCtrl.DisableLeftHandIk();
                    handIKCtrl.DisableRightHandIk();

                    ColliderInit();

                    ChangeState(PlayerState.Jump);
                }
                else if (state == PlayerState.HangRope)
                {
                    rigidbody.isKinematic = false;
                    transform.parent = null;
                  

                    moveDir = Vector3.zero;
                    currentJumpPower = 0.0f;

                    ChangeState(PlayerState.Jump);
                }
                else if(state == PlayerState.HangLedge)
                {
                    ChangeState(PlayerState.Jump);
                }
            }

            yield return null;
        }
    }
}
