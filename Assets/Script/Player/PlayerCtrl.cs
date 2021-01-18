using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBehaviorState
{
    Ground,
    Climbing,
    Hanging,
    JumpFromWall
}

public class PlayerCtrl : MonoBehaviour
{
    [Header("Movement Speed Value")]
    [SerializeField] private float walkSpeed = 15.0f;
    [SerializeField] private float runSpeed = 25.0f;
    [SerializeField] private float rollingSpeed = 10.0f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float jumpFromWallSpeed = 3f;
    [SerializeField] private float prevSpeed;
    [SerializeField] private float climbingSpeed = 3f;
    [Range(0,5)][SerializeField] private float fallingControlSenstive = 1f;

    [Header("Jump Value")]
    [SerializeField] private float currentJumpPower = 0f;
    [SerializeField] private float minJumpPower = -10.0f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravity = 20.0f;

    [Header("State Conditions")]
    [SerializeField] private bool isPause;
    [SerializeField] private bool isRun;
    [SerializeField] private bool isGrab;
    [SerializeField] private bool isMustClimbing;
    [SerializeField] private bool isAim;
    [SerializeField] private bool isRolling;
    [SerializeField] private bool isRollingReady;
    [SerializeField] private bool isGround;
    [SerializeField] private bool isGround2;
    [SerializeField] private bool isEnoughGround;
    [SerializeField] private bool isSideDetect;
    [SerializeField] private bool isUpDetect;
    [SerializeField] private bool isHangingLedge;
    [SerializeField] private bool isClimbingLedge;
    [SerializeField] private bool isRopeHanging;
    [SerializeField] private bool isRopeClimbing;
    [SerializeField] private bool isJumpFromWall;
    [SerializeField] private bool isReadyRolling;
    [SerializeField] private bool isDontControl;
    [SerializeField] private bool SuperMode;
    [SerializeField] private bool isCanInputClimbing = true;
    [SerializeField] private bool isSideClimbing;
    [SerializeField] private bool isLedgeSideMove;
    [SerializeField] private bool isCanAbsorb;
    [SerializeField] private bool isAbsorbing;
    [SerializeField] private bool isStagger;
    [SerializeField] private bool isQuake;
    [SerializeField] private bool isMustHoldOut;
    [SerializeField] private bool isNuckBack = false;
    [SerializeField] private PlayerBehaviorState playerBehaviorState = PlayerBehaviorState.Ground;
    [SerializeField] private float stamina;
    [SerializeField] private float hp = 100f;
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
    [SerializeField] private bool ropeHandLeft;

    [Header("Collision Layer")]
    [SerializeField] private LayerMask climbingLayer;
    [SerializeField ]private int detectionLayer;
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
    [SerializeField] private float prevInputVertical;
    [SerializeField] private float prevInputHorizontal;

    [Header("Balance")]
    [SerializeField] private float balanceLimitMinAngle = 40f;
    [SerializeField] private float balanceLimitMaxAngle = 60f;
    [SerializeField] private float canStandUpAngle = 5f;
    [SerializeField] private float holdOutTime = 2.5f;
    [SerializeField] private float currentSlidingSpeed = 0.0f;
    [SerializeField] private float maxSlidingSpeed = 10.0f;

    [Header("Move Velocity")]
    [SerializeField] private float gapPerFrame = 0.0f;
    [Range(0.0f, 5.0f)] [SerializeField] private float impactDesired = 4f;
    private Vector3 prevFramePos;

    [Header("Move Direction")]
    [SerializeField] private Vector3 moveDir;
    private Vector3 jumpDir;
    private Vector3 prevDir;
    private Vector3 slidingDir;
    private Vector3 prevSlidingDir;
    private Vector3 camForward;
    private Vector3 camRight;
    private Quaternion climbingPrevRot;
    private Vector3 prevForward;
    private Vector3 prevCalcDir;

    [Header("Damage")]
    [SerializeField] private float damage = 25f;

    private float rollingTime;

    private Rigidbody rigidbody;
    private CharacterController controller;

    private CapsuleCollider collider;
    private float colliderRadius;
    private float colliderHeight;

    private Transform mainCameraTrasform;
    private Animator animator;
    private CameraCtrl cameraCtrl;
    private IKCtrl ikCtrl;
    private HandIKCtrl handIKCtrl;
    private EnergyCore currentDetectEnergyCore;
    private PlayerAnimCtrl playerAnimCtrl;

    [SerializeField] private bool leftCount;

    [SerializeField] private float parentGapAdjustValue = 10.0f;
    [SerializeField] private float offsetDist;

    private RigidbodyConstraints defaultConstrains = RigidbodyConstraints.FreezeRotation;
    private RigidbodyConstraints ropeHangingConstrains = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

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
        controller = GetComponent<CharacterController>();
        //animator = transform.Find("PlayerVisual").GetComponent<Animator>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        ikCtrl = GetComponent<IKCtrl>();
        handIKCtrl = GetComponent<HandIKCtrl>();

        leftCount = true;
        stamina = 100f;
    }

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

        if(specialSpearVisualFloat != null)
        {
            specialSpearVisualFloat.SetActive(false);
        }

        if(specialSpearDissolve != null)
        {
            spearDissolve = specialSpearDissolve.GetComponent<Renderer>().material;
        }

        moveDir = Vector3.zero;
        currentSpeed = walkSpeed;
        mainCameraTrasform = Camera.main.transform;
        cameraCtrl = mainCameraTrasform.parent.GetComponent<CameraCtrl>();
        prevFramePos = transform.position;
        playerAnimCtrl = GetComponent<PlayerAnimCtrl>();

        colliderRadius = collider.radius;
        colliderHeight = collider.height;

        StartCoroutine(UpdateMovementVelocity());

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

    void Update()
    {
        if(isPause == true)
        {
            return;
        }

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    GameManager.Instance.RequstGameResult();
        //}

        CheckRigid();
        UpdateFallingTime();

        InputSpacialSpear();
        if(InputDropSpear())
        {
            return;
        }

        if (isAbsorbing == true || isStagger == true)
        {
            return;
        }

        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");


        camForward = mainCameraTrasform.forward;
        camRight = mainCameraTrasform.right;
        camForward.y = 0;
        camRight.y = 0;

        //Debug.DrawRay(transform.position, transform.up * 2.0f, Color.green);
        //Debug.DrawRay(transform.position, transform.right * 2.0f, Color.magenta);

        if (isClimbingLedge == true)
        {
            //Vector3 target = climbingStartPos + ledgeClimbingDir;
            //transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * 2f);
            //transform.rotation = climbingPrevRot;
            //transform.rotation = Quaternion.LookRotation(prevForward);
            //Debug.Log("dd");
            return;
        }

        UpdateInputValue(inputVertical, inputHorizontal);

        DetectGround();

        UpdateSliding();

        if (InputHangingRope())
        {
            return;
        }

        if (InputRopeCtrl())
        {
            return;
        }

        //if(InputRopeAttach())
        //{
        //    return;
        //}

        UpdateDetect();

        InputAim();

        InputShot();

        InputGrab();

        InputRun();

        InputLedgeClimbing();

        InputRolling();

        InputSuperMode();

        InputAbsorb();
       
        if (isGrab == false)
        { 
            if(isDontControl == false && isClimbingLedge == false)
            {
                UpdateCurrentSpeed();

                if (isNuckBack == false)
                {
                    if (isEnoughGround == true && isRolling == false)
                    {
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

                        if (moveDir != Vector3.zero)
                        {
                            if (isQuake != true)
                            {
                                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 6.0f);
                            }
                            else
                            {
                                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 2.0f);
                            }
                        }

                        if (isRollingReady == true)
                        {
                            moveDir.Normalize();
                        }
                        else
                        {
                            moveDir *= currentSpeed;
                        }

                        if (Input.GetKeyDown(KeyCode.Space) && isQuake == false && isNuckBack == false)
                        {

                            currentJumpPower = jumpPower;
                            isGround = false;
                            groundCheck.Lock();
                            animator.SetBool("IsJump", true);
                            //animator.SetTrigger("Jump");
                            prevSpeed = currentSpeed;

                            //ikCtrl.DisableFeetIk();
                        }
                    }

                    if (isEnoughGround == false && isRolling == false)
                    {
                        float velocity = moveDir.magnitude;
                        Vector3 plusDir = ((camForward * inputVertical) + (camRight * inputHorizontal));

                        //moveDir += plusDir;
                        //moveDir.Normalize();
                        //moveDir *= velocity;
                        //prevDir = moveDir;

                        transform.position += plusDir * fallingControlSenstive * Time.deltaTime;

                        Vector3 lookDir = (camForward * inputVertical) + (camRight * inputHorizontal);
                        lookDir.Normalize();
                        if (lookDir != Vector3.zero)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, transform.up), Time.deltaTime * 1.0f);
                        }

                    }
                }
                else
                {

                }


                if (isRolling == true)
                {
                    if(moveDir == Vector3.zero)
                    {
                        moveDir = prevDir;
                    }

                    moveDir.Normalize();
                    currentSpeed = Mathf.Lerp(currentSpeed, rollingSpeed, 15f * Time.deltaTime);
                    moveDir *= currentSpeed;
                    Vector3 targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 5.0f).eulerAngles;
                    transform.rotation = Quaternion.Euler(0.0f, targetRot.y, 0.0f);
                    
                }

                if (isGround == false)
                {
                    currentJumpPower -= gravity * Time.deltaTime;
                    currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, 50f);
                }
                else
                {
                    currentJumpPower = 0f;
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
            else
            {
                currentSlidingSpeed = Mathf.MoveTowards(currentSlidingSpeed, maxSlidingSpeed, 5f * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(slidingDir.x,0,slidingDir.z), Vector3.up), 5f * Time.deltaTime);
                prevSlidingDir = slidingDir;
                prevSlidingDir.y = 0;
                //ikCtrl.DisableFeetIk();

                transform.position = transform.position + slidingDir * currentSlidingSpeed * Time.deltaTime;
                if(groundAngle < balanceLimitMinAngle )
                {
                    isDontControl = false;
                    isStagger = false;
                    currentSlidingSpeed = 0.0f;
                    prevDir = prevSlidingDir;
                    animator.SetTrigger("EndSliding");

                    //ikCtrl.EnableFeetIk();
                }

                if(!Physics.Raycast(transform.position,-transform.up,6f))
                {
                    isDontControl = false;
                    isStagger = false;
                    currentSlidingSpeed = 0.0f;
                    prevDir = prevSlidingDir;
                    animator.SetTrigger("EndSliding");
                }
            }
        }
        else
        {
            CheckLedge();

            if (Input.GetKeyDown(KeyCode.Space) && isHangingLedge == false)
            {

                isGrab = false;
                isMustClimbing = false;

                Vector3 backVector = -transform.forward;
                backVector.y = 0f;
                transform.rotation = Quaternion.LookRotation(backVector);

                moveDir = transform.forward;
                moveDir.Normalize();
                moveDir *= runSpeed;
                currentJumpPower = jumpPower;
                transform.position = transform.position+ ( moveDir + (Vector3.up * currentJumpPower)) * Time.deltaTime;
                prevSpeed = jumpFromWallSpeed;

                animator.SetBool("IsJump", true);
                animator.SetBool("IsGrab", false);

                playerBehaviorState = PlayerBehaviorState.Ground;
                groundCheck.RequestDetach();

                animator.applyRootMotion = false;

                ColliderInit();

                return;
            }

            if (isMustClimbing == true)
            {
                RaycastHit hit;
                Vector3 startPos = animator.targetPosition + transform.up * (collider.height * 0.5f) + (-transform.forward * 0.5f);

                bool result =Physics.SphereCast(startPos, collider.radius, transform.forward, out hit, 3.0f, climbingLayer);
                //if (result == false)
                //{
                //    animator.applyRootMotion = false;
                //}

                //if (upCheck.GetIsCanMove() == false)
                //{
                //    animator.applyRootMotion = false;
                //}

                //collider.radius = colliderRadius * 0.5f;
                //collider.height = colliderHeight * 0.5f;
                //collider.isTrigger = true;
            }
            else
            {
                //collider.radius = colliderRadius;
                //collider.height = colliderHeight;
                //collider.isTrigger = false;
            }
        }

        if (isAim == true && isGrab == false)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(camForward), Time.unscaledDeltaTime * 20.0f);
        }

        if (playerBehaviorState != PlayerBehaviorState.Climbing)
        {
            if (isGround == true)
            {
                animator.SetBool("IsJump", false);
            }
 
            if(isEnoughGround == false)
            {
                animator.SetBool("IsJump", true);
            }
        }

    }

    private void FixedUpdate()
    {
        //UpdateParentPrevPos();
        //UpdateDetect();
        if(isPause == true)
        {
            return;
        }

        if(isGrab == true)
        {
            UpdateGrab();
        }
    }

   

    private bool CheckWall()
    {
        //Vector3 startPos = transform.position + new Vector3(0.0f, controller.height * 0.5f, 0.0f);
        Vector3 startPos = transform.position + transform.up * collider.height * 0.5f;
        Collider[] colliders = Physics.OverlapCapsule(startPos, startPos + transform.forward * 0.5f, 0.4f,climbingLayer);
        if(colliders.Length != 0)
        {
            animator.SetBool("IsGrab", true);
            transform.SetParent(colliders[0].transform);
            RaycastHit hit;
            if(Physics.Raycast(startPos, transform.forward,out hit,3.0f,climbingLayer))
            {
                Vector3 normalReverse = -hit.normal;
                transform.rotation = Quaternion.LookRotation(normalReverse);
                transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * collider.radius;
            }

            return true;
        }

        return false;
    }

    private void UpdateGrab()
    {
        //Vector3 startPos = transform.position + transform.up * (collider.height * 0.5f);

        //RaycastHit hit;
        //if (Physics.Raycast(startPos, transform.forward, out hit, 3.0f, climbingLayer))
        //{
        //    transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * collider.radius;
        //}
        RaycastHit hit;
        Vector3 startPos = transform.position + transform.up * (colliderHeight * 0.5f) + (-transform.forward * 1f);

        if (Physics.SphereCast(startPos, colliderRadius, transform.forward, out hit, 3.0f, climbingLayer))
        {
            if ((hit.point - (transform.position + transform.up * (colliderHeight * 0.5f))).magnitude > 0.5f)
            {
                transform.position = (hit.point - transform.up * (colliderHeight * 0.5f)) + hit.normal * 0.3f;
            }

            //transform.position = (hit.point - transform.up * (colliderHeight * 0.5f)) + hit.normal * 0.3f;

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

        if(isHangingLedge == true)
        {
            bool hitResult = Physics.SphereCast(handOffset.position, 0.1f, -transform.up, out hit, 1f, climbingLayer);
            if (hitResult == true)
            {
                Vector3 dist = hit.point - handOffset.position;
                transform.position += dist;
            }
        }
    }

    public void SetIsMustClimbing(bool result) { isMustClimbing = result; }

    private void UpdateInputValue(float vertical, float horizontal)
    {
        if (isGrab == false && isRopeHanging == false)
        {
            if(vertical > 0.0f)
            {
                currentVerticalValue = 1.0f;
            }
            else if(vertical < 0.0f)
            {
                currentVerticalValue = -1.0f;
            }
            else
            {
                currentVerticalValue = 0.0f;
            }

            if(horizontal > 0.0f)
            {
                currentHorizontalValue = 1.0f;
            }
            else if(horizontal < 0.0f)
            {
                currentHorizontalValue = -1.0f;
            }
            else
            {
                currentHorizontalValue = 0.0f;
            }

            return;
        }

        if (isHangingLedge == true && isCanInputClimbing == true)
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
                isHangingLedge = false;
                animator.SetBool("IsReverseClimbing", true);
                animator.SetBool("IsLedgeHanging", false);
                return;
            }
            else if(currentVerticalValue == 0.0f && currentHorizontalValue != 0.0f)
            {
                if(currentHorizontalValue == 1.0f)
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

        if(isRopeHanging == true && isCanInputClimbing == true)
        {
            if (vertical > 0.0f)
            {
                currentVerticalValue = 1.0f;

                animator.SetTrigger("RopeClimbing");
                animator.SetInteger("RopeVertical", (int)currentVerticalValue);
                isCanInputClimbing = false;
                isRopeClimbing = true;
            }
            else if (vertical < 0.0f)
            {
                currentVerticalValue = -1.0f;

                animator.SetTrigger("RopeClimbing");
                animator.SetInteger("RopeVertical", (int)currentVerticalValue);
                isCanInputClimbing = false;
                isRopeClimbing = true;
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
            if(vertical == 0.0f)
            {
                currentVerticalValue = 0.0f;
            //    animator.SetBool("IsClimbing", false);
            //    animator.SetBool("IsReverseClimbing", false);
            }
            else if(vertical >= 0.0f)
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

            if(horizontal > 0.0f)
            {
                currentHorizontalValue = Mathf.Ceil(horizontal);
            }
            else
            {
                currentHorizontalValue = Mathf.Floor(horizontal);
            }

            RaycastHit hit;
            if(currentVerticalValue != 0.0f)
            {
                if(currentVerticalValue > 0.0f)
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
                    if (Physics.SphereCast(transform.position + transform.right * collider.radius *2f, collider.radius, transform.forward, out hit,1f, climbingLayer))
                    {
                        animator.SetBool("IsSideClimbing", true);
                        animator.SetBool("Left", false);
                    }
                }
                else if (currentHorizontalValue < 0.0f)
                {
                    if (Physics.SphereCast(transform.position - transform.right * collider.radius * 2f, collider.radius, transform.forward, out hit,1f, climbingLayer))
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

    private void UpdateCurrentSpeed()
    {
        if(isQuake == true)
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

    private bool GroundCheckByLay(float checkDist)
    {
        Bounds bounds = controller.bounds;
        RaycastHit hit;

        var foot = bounds.center;
        foot.y -= bounds.extents.y;

        Debug.DrawRay(foot, Vector3.down* checkDist, Color.red);
        if(Physics.Raycast(bounds.min,Vector3.down,out hit, checkDist, floorLayer))
        {
            return true;
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
        if (Input.GetKeyUp(KeyCode.Mouse0) && isAim == true && currentSpearNum != 0 && isAbsorbing == false)
        {
            if (isSpacialSpearMode == false)
            {
                SpearCtrl spear = Instantiate(spearPrefab, launchPos.position, launchPos.rotation);
                spear.Launch(launchPos.position);

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
        cameraCtrl.SetCamMode(CamMode.Default);
        GameManager.Instance.timeManager.OffBulletTime();

        OnAimOff?.Invoke();
    }

    private void InputGrab()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && isRopeHanging == false)
        {
            if (CheckWall() == true)
            {
                isGrab = true;
                animator.SetBool("IsJump", false);
                //controller.enabled = false;
                RaycastHit hit;
                //Vector3 startPos = transform.position + transform.up * (controller.height * 0.5f);
                Vector3 startPos = transform.position + transform.up * (collider.height * 0.5f);
                if (Physics.Raycast(startPos, transform.forward, out hit, 1f, climbingLayer))
                {
                    //Debug.Log(hit.collider);
                    //controller.enabled = false;
                    //transform.position = (hit.point - transform.up * (controller.height * 0.5f)) + (hit.normal) * controller.radius;
                    //transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * collider.radius;
                    //controller.enabled = true;
                }

                playerBehaviorState = PlayerBehaviorState.Climbing;
                //Debug.Log(hit.point);

                currentSpeed = 0.0f;
                currentJumpPower = 0.0f;

                //Debug.Log(transform.localScale);
                //transform.localScale = new Vector3(1f, 1f, 1f);
                //transform.parent = hit.transform;
                rigidbody.useGravity = false;
                rigidbody.velocity = Vector3.zero;

                isCanInputClimbing = true;

                prevSpeed = currentSpeed;

                groundCheck.UnLock();

                moveDir = Vector3.zero;

                //ColliderReduction();

                //collider.isTrigger = true;
                //ikCtrl.DisableFeetIk();
                //Debug.Log("Wall Grab");
            }
            else
            {
                RaycastHit hit;
                Vector3 startPos = transform.position;
                if (Physics.Raycast(startPos, -transform.up, out hit, 0.5f, climbingLayer))
                {
                    if (isDontControl == false)
                    {
                        transform.rotation = Quaternion.LookRotation(-hit.normal, transform.forward);
                    }
                    else
                    {
                        transform.rotation = Quaternion.LookRotation(-hit.normal, transform.up);
                        isDontControl = false;
                        isStagger = false;
                        currentSlidingSpeed = 0.0f;
                    }

                    transform.position = (hit.point) + (hit.normal) * collider.radius;

                    playerBehaviorState = PlayerBehaviorState.Climbing;
                    currentSpeed = 0.0f;
                    currentJumpPower = 0.0f;

                    transform.SetParent(hit.transform);

                    isGrab = true;
                    animator.SetBool("IsGrab", true);

                    isCanInputClimbing = true;

                    moveDir = Vector3.zero;

                    //collider.isTrigger = true;
                    //ikCtrl.DisableFeetIk();
                    //Debug.Log("Ground Grab");

                    //ColliderReduction();
                }

            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (playerBehaviorState == PlayerBehaviorState.JumpFromWall)
                return;

            isGrab = false;
            isHangingLedge = false;
            isMustClimbing = false;
            isSideClimbing = false;
            
            //controller.enabled = true;
            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.x = 0.0f;
            currentRot.z = 0.0f;
            transform.rotation = Quaternion.Euler(currentRot);
            animator.SetBool("IsGrab", false);
            animator.SetBool("IsLedgeHanging", false);
            animator.SetBool("IsLedgeMoving", false);
            animator.SetBool("IsLedgeMoveLeft", false);
            animator.SetBool("IsLedgeMoveRight", false);
            animator.SetBool("IsSideClimbing", false);

            playerBehaviorState = PlayerBehaviorState.Ground;
            groundCheck.RequestDetach();
            //rigidbody.useGravity = true;
            //transform.parent = null;
            handIKCtrl.DisableLeftHandIk();
            handIKCtrl.DisableRightHandIk();

            animator.applyRootMotion = false;

            //collider.isTrigger = false;
            ColliderInit();
        }
    }

    private void InputRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }

        //if (Input.GetKeyDown(KeyCode.LeftShift)&&isRun == false && isGround == true && isRolling == false && isDontControl == false)
        //{
        //    //isRolling = true;
        //    animator.SetTrigger("Rolling");
        //    animator.SetBool("IsRolling", true);
        //    isRollingReady = true;
        //}
    }

    private void InputRolling()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGround == true && isRolling == false && isDontControl == false)
        {
            //isRolling = true;
            //animator.SetTrigger("Rolling");
            //animator.SetBool("IsRolling", true);
            //isRollingReady = true;
            rollingTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isGround == true && isRolling == false && isDontControl == false)
        {
            //Debug.Log(Time.time - rollingTime);
            if (Time.time - rollingTime < 0.2f)
            {
                isRolling = true;
                animator.SetTrigger("Rolling");
                animator.SetBool("IsRolling", true);
                isRollingReady = true;
            }
        }
    }

    public void ReleaseRolling()
    {
        animator.SetBool("IsRolling", false);
        isRolling = false;
        currentSpeed = walkSpeed;
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
            }
            else
            {
                isGround = false;
            }
        }
        //animator.SetBool("IsGround", isGround);
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

    private void CheckLedge()
    {
        if (isMustClimbing &&UpClimbingDetection(1.0f * Time.deltaTime) == false  && isGrab == true && isHangingLedge == false && currentVerticalValue == 1.0f && isUpDetect == false)
        {
            isHangingLedge = true;
            isMustClimbing = false;
            animator.applyRootMotion = false;
            animator.SetBool("IsLedgeHanging", true);
        }
    }

    private void ClimbingMovement()
    {
        RaycastHit hit;
        //Vector3 startPos = transform.position + transform.up * (controller.height * 0.5f) + (-transform.forward * 0.5f);
        Vector3 startPos = transform.position + transform.up * (collider.height * 0.5f) + (-transform.forward * 0.5f);

        //Debug.DrawRay(startPos, transform.forward * 1.0f,Color.blue);
        //if (Physics.SphereCast(startPos, controller.radius,transform.forward, out hit, 3.0f, climbingLayer))
        if (Physics.SphereCast(startPos, collider.radius, transform.forward, out hit, 3.0f, climbingLayer))
        {
            //transform.position = (hit.point - transform.up * (controller.height * 0.5f)) + hit.normal * 0.3f;
            transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + hit.normal * 0.3f;

            //Debug.DrawRay(hit.point, hit.normal,Color.red);
            //transform.rotation = Quaternion.LookRotation(hit.point - startPos);
            transform.rotation = Quaternion.LookRotation(-hit.normal,transform.up);
            //Debug.Log("detect");

            if(hit.collider.transform != transform.parent)
            {
                transform.parent = hit.collider.transform;
            }
        }
        else
        {
            return;
        }

        return;

        float sideMoveFactor = 0f;
        float upMoveFactor = 0f;

        if (isSideDetect == true)
        {
            sideMoveFactor = 1f;
        }
        else
        {
            sideMoveFactor = 0f;
        }


        if (isHangingLedge == false)
        {
            
            //isUpDetect == true
            if (isUpDetect == true||UpClimbingDetection(1f * Time.deltaTime) == true || currentVerticalValue == -1.0f )
            {
                if (HeadCheck() == false)
                    upMoveFactor = 1f;
                else
                    upMoveFactor = 0f;
            }
            else
            {
                upMoveFactor = 0f;
            }
        }

        if(currentVerticalValue == -1f && isGround == true)
        {
            return;
        }

        //Vector3 climbingUp = Quaternion.Euler(90.0f,0.0f,0.0f)*hit.normal;
        //Debug.DrawRay(startPos, transform.up * 2.0f, Color.green);
        //Debug.DrawRay(startPos, transform.right * 2.0f, Color.magenta);

        //Vector3 climbingUp = -Vector3.Cross(hit.normal,Vector3.right).normalized;
        //Vector3 climbingUp = -Vector3.Cross(hit.normal,transform.right).normalized;
        Vector3 climbingUp = Vector3.up;
        

        //Vector3 climbingRight = Quaternion.Euler(0.0f, -90.0f, 0.0f) * hit.normal;
        //Vector3 climbingRight = Vector3.Cross(hit.normal,transform.up).normalized;
        //Vector3 climbingRight = transform.right;
        Vector3 climbingRight = Vector3.right;

        //Debug.DrawRay(startPos, climbingUp * 1.5f,Color.blue);


        //Vector3 climbingDir = transform.up * currentVerticalValue * upMoveFactor + Vector3.right * currentHorizontalValue;
        Vector3 climbingDir = climbingUp * currentVerticalValue * upMoveFactor + climbingRight * currentHorizontalValue * sideMoveFactor;
        Vector3 climbingUpDir = (climbingUp * currentVerticalValue * upMoveFactor).normalized;
        Vector3 climbingHorizonDir = (climbingRight * currentHorizontalValue).normalized;

        Debug.DrawRay(startPos, climbingDir * 1.5f, Color.blue);

        climbingDir.Normalize();

        if (currentVerticalValue != 0.0f && currentHorizontalValue != 0.0f)
        {
            transform.Translate((climbingHorizonDir * climbingSpeed * Time.deltaTime) * 0.5f);
            //transform.position += climbingUpDir * 1f * Time.deltaTime * 0.5f;
            transform.Translate(climbingUpDir * climbingSpeed * Time.deltaTime * 0.5f);
        }
        else
        {
            transform.Translate((climbingHorizonDir * climbingSpeed * Time.deltaTime));
            //transform.position += climbingUpDir * 1f * Time.deltaTime;
            transform.Translate(climbingUpDir * climbingSpeed * Time.deltaTime );
            //Debug.Log(climbingUpDir);
        }

        //transform.Translate(climbingDir * 1f * Time.deltaTime);
        //transform.position += climbingDir * 1f * Time.deltaTime;
        //transform.Translate((climbingHorizonDir * 1f * Time.deltaTime));
        //transform.position += climbingUpDir * 1f * Time.deltaTime;
    }

    private void ClimbingRotationAdjust()
    {
        RaycastHit hit;
        //Vector3 startPos = transform.position + transform.up * (controller.height * 0.5f) + (-transform.forward * 0.5f);
        Vector3 startPos = transform.position + transform.up * (collider.height * 0.5f) + (-transform.forward * 0.5f);

        //Debug.DrawRay(startPos, transform.forward * 1.0f,Color.blue);
        //if (Physics.SphereCast(startPos, controller.radius,transform.forward, out hit, 3.0f, climbingLayer))
        if (Physics.SphereCast(startPos, collider.radius, transform.forward, out hit, 3.0f, climbingLayer))
        {
            //transform.position = (hit.point - transform.up * (controller.height * 0.5f)) + hit.normal * 0.3f;
            transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + hit.normal * 0.3f;

            //Debug.DrawRay(hit.point, hit.normal,Color.red);
            //transform.rotation = Quaternion.LookRotation(hit.point - startPos);
            transform.rotation = Quaternion.LookRotation(-hit.normal, transform.up);
            //Debug.Log("detect");

            if (hit.collider.transform != transform.parent)
            {
                transform.parent = hit.collider.transform;
            }
        }
        else
        {
            return;
        }
    }

    private void InputLedgeClimbing()
    {
        if(isHangingLedge == true && isLedgeSideMove == false &&Input.GetKeyDown(KeyCode.Space))
        {
            if(climbingUpAngle > 22.5f)
            {
                return;
            }

            animator.applyRootMotion = true;
            animator.SetBool("IsGrab", false);
            animator.SetBool("IsLedgeHanging", false);
            animator.SetBool("IsClimbing", false);
            animator.SetTrigger("LedgeClimbing");
            isHangingLedge = false;
            isClimbingLedge = true;
            isGrab = false;

            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.x = 0.0f;
            currentRot.z = 0.0f;
            transform.rotation = Quaternion.Euler(currentRot);

            climbingPrevRot = transform.rotation;
            prevForward = transform.forward;

            ColliderInit();
            //ledgeClimbingDir = (upCheck.transform.position - new Vector3(0.0f, upCheck.GetCapsuleHeight() * 0.5f, 0.0f)) - transform.position;

            //climbingStartPos = transform.localPosition;


            //Debug.Log(transform.parent.Find("LedgeDummy"));
            //if(transform.parent.Find("LedgeDummy") != null)
            //{
            //    transform.parent = transform.parent.Find("LedgeDummy");
            //}


            //originPos = transform.localPosition;

            //isGrab = false;
            //isHangingLedge = false;
            //isMustClimbing = false;

            //controller.enabled = true;
            //Vector3 currentRot = transform.rotation.eulerAngles;
            //currentRot.x = 0.0f;
            //transform.rotation = Quaternion.Euler(currentRot);
            //animator.SetBool("IsGrab", false);
            //animator.SetBool("IsLedgeHanging", false);
            //animator.SetBool("IsClimbing", false);
            //animator.SetTrigger("LedgeClimbing");

            //playerBehaviorState = PlayerBehaviorState.Ground;
        }
    }

    public void SetIsClimbingLedge(bool result)
    {
        isClimbingLedge = result;
        if(isClimbingLedge == false)
        {
            //transform.position = upCheck.transform.position - new Vector3(0.0f, upCheck.GetCapsuleHeight() * 0.5f, 0.0f);
            //controller.enabled = true;
            playerBehaviorState = PlayerBehaviorState.Ground;
            //rigidbody.useGravity = true;
            collider.enabled = true;
            rigidbody.velocity = Vector3.zero;
            currentJumpPower = 0.0f;
        }
        else
        {
            collider.enabled = false;
            currentJumpPower = 0.0f;
            //controller.enabled = false;
            //Debug.Log(handTransform.position);
            //transform.position += dist;           
        }
    }

    public PlayerBehaviorState GetPlayerBehaviorState() { return playerBehaviorState; }

    public void StartRolling() 
    { 
        isRolling = true;
        isRollingReady = false;
        //animator.SetBool("IsRolling",true);
    }

    private bool UpClimbingDetection(float length)
    {
        //Vector3 layStartPos = transform.position + transform.up * controller.height;
        Vector3 layStartPos = transform.position + transform.up * colliderHeight;
        RaycastHit hit;
        Vector3 sphereStart;

        Debug.DrawRay(layStartPos, transform.forward* 2.0f);
        if(Physics.Raycast(layStartPos,transform.forward,out hit,2.0f,climbingLayer))
        {
            sphereStart = hit.point;
        }
        else
        {
            //Debug.Log("false");
            return false;
        }

        Vector3 upDir = Quaternion.Euler(-90.0f,0.0f,0.0f)*hit.normal;
        //if (Physics.SphereCast(sphereStart, controller.radius, upDir*length, out hit, climbingLayer ))
        if (Physics.SphereCast(sphereStart, colliderRadius, transform.up, out hit,length, climbingLayer))
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

    private bool InputRopeCtrl()
    {
        if(isRopeHanging == true && currentHangingRope != null)
        {
            //currentHangingRope.ClimbingRope(-currentVerticalValue,ropeClimbingSpeed);
            if (isRopeClimbing == true)
            {
                currentHangingRope.ClimbingRope(-currentVerticalValue, ropeClimbingSpeed,transform.right,ropeHandLeft);

                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(camForward, transform.up),4f*Time.deltaTime);
            }

            return true;
        }

        return false;
    }

    private bool  InputHangingRope()
    {
        if (Input.GetKeyDown(KeyCode.F) && isRopeHanging == false && isGrab == false && isCanAbsorb == false)
        {
            Vector3 startPos = transform.position + transform.up * (colliderHeight * 0.5f);

            Collider[] colliders = Physics.OverlapSphere(startPos, ropeDetectRange, ropeCheckLayer);
            
            if (colliders.Length !=0)
            {
                int targetCount = 0;
                for(int i = 1; i<colliders.Length;i++)
                {
                    float dist1 = (transform.position - colliders[i].transform.position).sqrMagnitude;
                    float dist2 = (transform.position - colliders[targetCount].transform.position).sqrMagnitude;
                    if(dist1 < dist2 && colliders[i].GetComponent<RopeBuiltIn>().canHanging == true)
                    {
                        targetCount = i;
                    }
                }

                if(colliders[targetCount].GetComponent<RopeBuiltIn>().canHanging == false)
                {
                    return false;
                }

                currentHangingRope = colliders[targetCount].GetComponent<RopeBuiltIn>();

                currentHangingRope.InitializeRope(transform.position + transform.up * colliderHeight);
                transform.parent = currentHangingRope.transform;
                transform.position = currentHangingRope.transform.position - transform.up * colliderHeight;
                //transform.position = currentHangingRope.transform.position;
                isRopeHanging = true;
                isCanInputClimbing = true;

                rigidbody.isKinematic = true;
                //rigidbody.constraints = ropeHangingConstrains;

                animator.SetBool("IsHangingRope", true);
                currentVerticalValue = 0.0f;

                return true;
            }
            else
            {
                return false;
            }
        }
        else if(Input.GetKeyDown(KeyCode.F) && isRopeHanging == true)
        {
            rigidbody.isKinematic = false;
            //rigidbody.constraints = defaultConstrains;
            transform.parent = null;
            isRopeHanging = false;

            moveDir = Vector3.zero;
            currentJumpPower = 0.0f;

            isRopeClimbing = false;

            currentJumpPower = jumpPower*0.5f;
            isGround = false;
            animator.SetBool("IsJump", true);
            moveDir = camForward;
            currentSpeed = walkSpeed;

            animator.SetBool("IsHangingRope", false);

            transform.rotation = Quaternion.LookRotation(camForward, transform.up);

            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void SetParent(Transform parent)
    {
        if (isGrab == false && isRopeHanging == false && isClimbingLedge == false)
        {
            transform.parent = parent;
        }
    }

    private bool HeadCheck()
    {
        //Vector3 startPos = transform.position + transform.up * controller.height;
        Vector3 startPos = transform.position + transform.up * colliderHeight;

        RaycastHit hit;
        //Vector3 sphereStart;

        Debug.DrawRay(startPos, transform.up * 0.5f ,Color.red);
        //Physics.SphereCast(startPos, controller.radius, transform.up, out hit, 1f * Time.deltaTime)
        if (Physics.Raycast(startPos,transform.up,out hit ,0.5f,headCheckLayer))
        {
            //Debug.Log("HeadDetect");
            return true;
        }

        return false;
    }

    public void SwitchGroundCheck(bool result)
    {
        groundCheck.gameObject.SetActive(result);
    }

    private void InputSuperMode()
    {
        // if(Input.GetKeyDown(KeyCode.K))
        // {
        //     SuperMode = !SuperMode;
        // }
    }

    private void UpdateParentPrevPos()
    {
        if (transform.parent != null)
        {
            //parentPrevPos = transform.parent.position;
        }
    }
    private Vector3 CalcFinalFowardMoveVector(Vector3 forward)
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position  , Vector3.down * 0.3f, Color.blue);
        if(Physics.Raycast(transform.position,Vector3.down,out hit,0.3f))
        {
            //float angle = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up)) * Mathf.Rad2Deg;
            float angle = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up)) * Mathf.Rad2Deg;

            //Debug.Log(angle);
            //Debug.DrawRay(transform.position, hit.normal * 2f, Color.red);
            //Debug.DrawRay(transform.position, forward * 2f, Color.red);
            //prevCalcAngle = Quaternion.Euler(-angle, 0.0f, 0.0f);
            groundAngle = angle;

            if (angle > 0.0f)
            {
                prevCalcDir = -MathEx.GetSlidingVector(Vector3.down, hit.normal).normalized;
            }
            else
            {
                prevCalcDir = forward;
            }
            
            return prevCalcDir;
        }
        //Debug.Log("not calc");
        return prevCalcDir;
    }

    private void UpdateSliding()
    {
        if(isGrab == true || isEnoughGround == false || isRopeHanging == true || isAbsorbing == true)
        {
            return;
        }

        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * 0.3f, Color.blue);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            float angle = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up)) * Mathf.Rad2Deg;
            groundAngle = angle;

            if (groundAngle >= balanceLimitMinAngle)
            {
                if(isReadyRolling == false && isDontControl == false)
                {
                    if (groundAngle >= balanceLimitMaxAngle)
                    {
                        return;
                    }
                    else
                    {
                        StartCoroutine(SlidingCheck());
                    }
                }
                slidingDir = MathEx.GetSlidingVector(Vector3.down, hit.normal).normalized;
            }
        }
    }

    IEnumerator SlidingCheck()
    {
        isReadyRolling = true;
        float time = 0.0f;

        while(time<holdOutTime)
        {
            if(isGrab == true || isEnoughGround == false || isAbsorbing == true)
            {
                isReadyRolling = false;
                break;
            }

            if(groundAngle < balanceLimitMinAngle)
            {
                isReadyRolling = false;
                break;
            }

            if(groundAngle > balanceLimitMaxAngle)
            {
                isReadyRolling = false;
                break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        if(isReadyRolling == true)
        {
            isReadyRolling = false;
            isDontControl = true;
            isStagger = true;
            animator.SetTrigger("Stagger");
            //ikCtrl.DisableFeetIk();
        }
    }

    private void DetectGround()
    {
        RaycastHit hit;
        //Physics.SphereCast(transform.position, 0.25f, Vector3.down, out hit, 1f);
        if (Physics.Raycast(transform.position,-transform.up,out hit,0.5f))
        {
            //Debug.Log(hit.collider);
            isGround2 = true;
        }
        else
        {
            isGround2 = false;
        }
    }

    public void SaveHandPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            offsetDist = (hit.point - transform.position).magnitude;
        }
        //prevHandPos = handTransform.position;
        //bool hitResult = Physics.SphereCast(handOffset.position, 0.1f, -transform.up, out hit, 1f, climbingLayer);
        //if (hitResult == true)
        //{
        //    Debug.Log("Adjust");
        //    Vector3 dist = hit.point - handOffset.position;
        //    transform.position += dist;
        //}
    }

    public void AdjustLedgeOffset()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, climbingLayer))
        {
            //Debug.Log("Adjust");
            float currentDist = (hit.point - transform.position).magnitude;
            if(currentDist > offsetDist)
            {
                float gap = currentDist - offsetDist;
                transform.position += transform.forward * gap;
            }
            //Debug.Log("PrevDist : " + offsetDist +" CurDist : "+currentDist+" Gap : "+ (currentDist - offsetDist));
        }


    }

    private bool CheckMoveCollision(Vector3 moveDir)
    {
        bool result = Physics.Raycast(transform.position + transform.up * (colliderHeight * 0.5f +0.1f), moveDir, colliderRadius, floorLayer);
        if(result == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void CheckRigid()
    {
        if(rigidbody.velocity != Vector3.zero)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    public void SetIsCanInputClimbing(bool result)
    {
        isCanInputClimbing = result;
    }

    public void SetIsSideClimbing(bool result)
    {
        isSideClimbing = result;
    }

    public void SetIsLedgeSideMove(bool result)
    {
        isLedgeSideMove = result;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Core"))
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

    private void InputAbsorb()
    {
        if(Input.GetKeyDown(KeyCode.F) && isCanAbsorb == true && isGrab == false && isRolling == false)
        {
            //Debug.Log("Absorbing");

            isAbsorbing = true;
            animator.SetTrigger("Absorb");

            if(currentDetectEnergyCore != null)
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
            }
        }
    }

    public void EndAbsorb()
    {
        isAbsorbing = false;
    }

    public void EndStagger()
    {
        isStagger = false;
        isQuake = false;
    }

    IEnumerator UpdateMovementVelocity()
    {
        while(true)
        {
            gapPerFrame = (transform.position - prevFramePos).magnitude * 10.0f;
            prevFramePos = transform.position;
            //if(gapPerFrame > 0.3f)
            //{
            //    Debug.Log(gapPerFrame);
            //}
            if(gapPerFrame >= impactDesired)
            {
                isMustHoldOut = true;
            }
            else
            {
                isMustHoldOut = false;
            }


            yield return new WaitForFixedUpdate();


        }
    }

    public void Quake()
    {
        //isStagger = true;
 
        if (isGround == true && isGrab == false && isAbsorbing == false)
        {
            if(isRolling == true)
            {
                isRolling = false;
            }

            isQuake = true;
            animator.SetTrigger("Quake");
        }
    }

    public void NuckBack(Vector3 causePos)
    {
        if(transform.position.y > causePos.y)
        {
            return;
        }

        isNuckBack = true;

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

        StartCoroutine(PogressNuckBack(2f,8f));
        TakeDamage();
    }

    private void OnCollisionStay(Collision collision)
    {
        //isNuckBack = false;
    }

    IEnumerator PogressNuckBack(float time,float startSpeed)
    {
        //yield return new WaitForSeconds(0.5f);
        //collider.isTrigger = false;
        //yield return new WaitForSeconds(time - 0.5f);
        //isNuckBack = false;

        //Debug.Log("NuckBack Start");

        animator.SetBool("IsNuckback", true);

        float currentTime = 0.0f;
        float currentNuckBackSpeed = startSpeed;

        while(currentTime < time)
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

        isNuckBack = false;
        animator.SetBool("IsNuckback", false);

        //isQuake = true;
        //animator.SetTrigger("Quake");
        //Debug.Log("NuckBack End");
    }



    IEnumerator StaminaTick()
    {
        while(true)
        {
            if(isGrab == true || isRopeHanging == true)
            {
                stamina -= 4f;
            }
            else
            {
                stamina += 25f;
            }

            stamina = Mathf.Clamp(stamina, 0.0f, 100.0f);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator StaminaCheck()
    {
        while (true)
        {
            if(stamina == 0.0f)
            {
                if(isGrab == true)
                {
                    if (playerBehaviorState == PlayerBehaviorState.JumpFromWall)
                        continue;

                    isGrab = false;
                    isHangingLedge = false;
                    isMustClimbing = false;
                    isSideClimbing = false;
                    isLedgeSideMove = false;

                    //controller.enabled = true;
                    Vector3 currentRot = transform.rotation.eulerAngles;
                    currentRot.x = 0.0f;
                    currentRot.z = 0.0f;
                    transform.rotation = Quaternion.Euler(currentRot);
                    animator.SetBool("IsGrab", false);
                    animator.SetBool("IsLedgeHanging", false);
                    animator.SetBool("IsLedgeMoving", false);

                    playerBehaviorState = PlayerBehaviorState.Ground;
                    groundCheck.RequestDetach();
                    //rigidbody.useGravity = true;
                    //transform.parent = null;

                    handIKCtrl.DisableLeftHandIk();
                    handIKCtrl.DisableRightHandIk();

                    ColliderInit();
                }
                else if(isRopeHanging == true)
                {
                    rigidbody.isKinematic = false;
                    transform.parent = null;
                    isRopeHanging = false;
                    isRopeClimbing = false;

                    moveDir = Vector3.zero;
                    currentJumpPower = 0.0f;

                    animator.SetBool("IsHangingRope", false);
                }
            }

            yield return null;
        }
    }

    public void SetIsRopeClimbing(bool value) { isRopeClimbing = value; }

    private void ColliderInit()
    {
        collider.height = colliderHeight;
        collider.radius = colliderRadius;
    }

    private void ColliderReduction()
    {
        collider.radius = colliderRadius * 0.5f;
        collider.height = 0f;
    }

    public void SetIsGrab(bool value) { isGrab = value; }
    public void SetRopeHandInfo(bool isLeft) { ropeHandLeft = isLeft; }

    public void Pause() { isPause = true; }
    public void Resume() { isPause = false; }

    public float GetStamina() { return stamina; }

    public float GetHp() { return hp; }

    private void UpdateFallingTime()
    {
        if(isGround == true || isGrab == true || isRopeHanging == true)
        {
            fallingTime = 0.0f;
        }
        else
        {
            fallingTime += Time.deltaTime;
        }
    }

    private void CheckFallingDamage()
    {
        if(fallingTime >= 3.5f)
        {
            TakeDamage();
            fallingTime = 0.0f;
            //Debug.Log("Damage");
        }
    }

    private void TakeDamage()
    {
        hp -= damage;
        if(hp<= 0f)
        {
            OnDead?.Invoke();
        }
    }

    private bool InputDropSpear()
    {
        Collider[] coll = Physics.OverlapSphere(centerPoint.position, 2f, spearLayer);
        if(coll.Length != 0)
        {
            isSpearDetect = true;
        }
        else
        {
            isSpearDetect = false;
        }


        if(isSpearDetect == true && Input.GetKeyDown(KeyCode.F) && isCanAbsorb == false && isRopeHanging == false && isHangingLedge == false && isRolling == false && isQuake ==false)
        {
            Collider[] spears = Physics.OverlapSphere(spearDetect.transform.position, 2f, spearLayer);
            if(spears.Length != 0)
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
//========================================================================================================================================================================================================================
    private void InputSpacialSpear()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && isCanEquipSpeicalSpear == true && isSpearDissolve == false)
        {
            isSpacialSpearMode = !isSpacialSpearMode;
            if(isSpacialSpearMode == true)
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

    public bool CheckInterantion()
    {
        if((isCanAbsorb == true || isSpearDetect == true)&& isHangingLedge == false && isRopeHanging == false)
        {
            return true;
        }

        return false;
    }

    public int GetCurrentSpearNum() { return currentSpearNum; }

    public void ClearAllCore() { isCanEquipSpeicalSpear = true; OnAbsorbAllCore?.Invoke(); }

    public void ActiveSetSpecialSpearVisual_Absorb(bool value)
    {
        if (specialSpearVisualObject == null)
            return;

        specialSpearVisualObject.transform.parent = rightHandBone;
        specialSpearVisualObject.transform.localPosition = Vector3.zero;
        specialSpearVisualObject.transform.localRotation = Quaternion.Euler(180.0f, 0.0f, 0.0f);
        specialSpearVisualObject.SetActive(value); 
    }

    //     private void OnGUI()
    //     {
    // #if UNITY_EDITOR
    //         GUIStyle st = new GUIStyle();
    //         st.normal.textColor = Color.white;
    //         st.fontSize = 25;

    //         GUI.Label(new Rect(10, 10, 100, 20), "IsRun : "+isRun,st);
    //         GUI.Label(new Rect(10, 30, 100, 20), "IsGrab : " + isGrab, st);
    //         GUI.Label(new Rect(10, 50, 100, 20), "IsMustClimbing : " + isMustClimbing, st);
    //         GUI.Label(new Rect(10, 70, 100, 20), "IsAim : " + isAim, st);
    //         GUI.Label(new Rect(10, 90, 100, 20), "IsRolling : " + isRolling, st);
    //         GUI.Label(new Rect(10, 110, 100, 20), "IsGround : " + isGround, st);
    //         GUI.Label(new Rect(10, 130, 100, 20), "IsSideDetect : " + isSideDetect, st);
    //         GUI.Label(new Rect(10, 150, 100, 20), "IsUpDetect : " + isUpDetect, st);
    //         GUI.Label(new Rect(10, 170, 100, 20), "IsHangingLedge : " + isHangingLedge, st);
    //         GUI.Label(new Rect(10, 190, 100, 20), "IsClimbingLedge : " + isHangingLedge, st);
    //         GUI.Label(new Rect(10, 210, 100, 20), "IsRopeHanging : " + isHangingLedge, st);
    //         GUI.Label(new Rect(10, 230, 100, 20), "IsGround2 : " + isGround2, st);
    //         //st.normal.textColor = Color.red;
    //         GUI.Label(new Rect(10, 250, 100, 20), "SuperMode : " + SuperMode, st);
    //         GUI.Label(new Rect(10, 270, 100, 20), "GroundAngle : " + groundAngle, st);
    //         GUI.Label(new Rect(10, 290, 100, 20), "IsReadyRolling : " + isReadyRolling, st);
    //         GUI.Label(new Rect(10, 310, 100, 20), "IsDontControl : " + isDontControl, st);
    //         GUI.Label(new Rect(10, 330, 100, 20), "IsEnoughGround : " + isEnoughGround, st);
    //         GUI.Label(new Rect(10, 350, 100, 20), "isCanInputClimbing : " + isCanInputClimbing, st);
    //         GUI.Label(new Rect(10, 370, 100, 20), "Current Parent : " + transform.parent, st);
    //         GUI.Label(new Rect(10, 390, 100, 20), "World Position : " + transform.position, st);
    //         GUI.Label(new Rect(10, 410, 100, 20), "Current JumpPower : " + currentJumpPower, st);
    //         GUI.Label(new Rect(10, 430, 100, 20), "isCanAbsorb : " + isCanAbsorb, st);
    //         GUI.Label(new Rect(10, 450, 100, 20), "isAbsorbing : " + isAbsorbing, st);
    //         GUI.Label(new Rect(10, 470, 100, 20), "GapPerFrame : " + gapPerFrame, st);
    //         GUI.Label(new Rect(10, 490, 100, 20), "isMustHoldOut : " + isMustHoldOut, st);
    //         GUI.Label(new Rect(10, 510, 100, 20), "Stamina : " + stamina, st);


    // #endif
    //     }
}
