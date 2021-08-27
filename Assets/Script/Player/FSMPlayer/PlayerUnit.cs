using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public partial class PlayerUnit : UnTransfromObjectBase
{
    public static PlayerState_Default defaultState;
    public static PlayerState_Jump jumpState;
    public static PlayerState_RunToStop runToStopState;
    public static PlayerState_TurnBack turnBackState;
    public static PlayerState_Aiming aimingState;
    public static PlayerState_Grab grabState;
    public static PlayerState_ReadyGrab readyGrabState;
    public static PlayerState_HangLedge hangLedgeState;
    public static PlayerState_LedgeUp ledgeUpState;
    public static PlayerState_ClimbingJump climbingJumpState;
    public static PlayerState_ReadyClimbingJump readyClimbingJumpState;
    public static PlayerState_HangEdge hangEdgeState;

    public float InputVertical { get => _inputVertical; }
    public float InputHorizontal { get => _inputHorizontal; }
    public LayerMask GrounLayer { get => groundLayer; }

    public Transform Transform => _transform;

    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float RotationSpeed { get => rotationSpeed; }
    public CapsuleCollider CapsuleCollider { get => _capsuleCollider; }
    public LayerMask FrontCheckLayer { get => frontCheckLayer; }
    public LayerMask DetectionLayer { get => detectionLayer; }

    public bool IsGround { get => isGrounded; set => isGrounded = value; }
    public float JumpPower { get => jumpPower; set => jumpPower = value; }
    public float MinJumpPower { get => minJumpPower; }
    public float Gravity { get => gravity; }

    public float CurrentJumpPower { get => currentJumpPower; set => currentJumpPower = value; }

    public Vector3 MoveDir { get => _moveDir; set => _moveDir = value; }
    public Vector3 PrevDir { get => _prevDir; set => _prevDir = value; }
    public Vector3 LookDir { get => _lookDir; set => _lookDir = value; }

    public bool JumpStart { get => _jumpStart; set => _jumpStart = value; }

    public float HorizonWeight { get => _horizonWeight; set => _horizonWeight = value; }

    public float Energy { get => energy.Value;
        set{energy.Value = value;}}

    public void AddEnergy(float value)
    {
        energy.Value += value;
        energy.Value = Mathf.Clamp(energy.Value, 0.0f, 100.0f);
    }

    public bool IsJump { get => isJumping; set => isJumping = value; }

    public float WalkRestoreEnergyValue => walkRestoreEnergyValue;
    public float RunRestoreEnergyValue => runRestoreEnergyValue;
    public float AimRestoreEnergyValue => aimRestoreEnergyValue;
    public float ClimbingRestoreEnergyValue => climbingJumpEnergyRestoreValue;
    public float HitEnergyRestoreEnergyValue => HitEnergyRestoreEnergyValue;
    public float JumpEnergyRestoreEnergyValue => jumpEnergyRestoreValue;
    public float ClimbingJumpRestoreEnrgyValue => climbingJumpEnergyRestoreValue;


    [SerializeField] private PlayerState _currentState;
    public PlayerState GetState => _currentState;

    private PlayerState _prevState;
    public PlayerState GetPrevState => _prevState;
    public string currentStateName;

    [Header("Moving")]
    [SerializeField] private bool isWalk;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float aimingWalkSpeed = 5.5f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float accelerateSpeed = 20.0f;
    [SerializeField] private float rotationSpeed = 6.0f;
    private float _horizonWeight = 0.0f;
    private Vector3 _moveDir;
    private Vector3 _prevDir;
    private Vector3 _lookDir;

    float _runToStopTime = 0.0f;

    [Header("Climbing")]
    [SerializeField] private bool isClimbingMove = false;
    [SerializeField] private bool isCanClimbingCancel = false;
    [SerializeField] private bool isClimbingGround = false;
    [SerializeField] private bool isCanReadyClimbingCancel = false;
    [SerializeField] private bool isLedge = false;
    [SerializeField] private float hangAbleEdgeDist = 2f;
    [SerializeField] private ClimbingJumpDirection climbingJumpDirection;

    public float HangAbleEdgeDist => hangAbleEdgeDist;
    public bool IsClimbingMove { get => isClimbingMove; set => isClimbingMove = value; }
    public bool IsCanClimbingCancel { get => isCanClimbingCancel; set => isCanClimbingCancel = value; }
    public bool IsClimbingGround { get => isClimbingGround; set => isClimbingGround = value; }
    public bool IsCanReadyClimbingCancel { get => isCanReadyClimbingCancel; set => isCanReadyClimbingCancel = value; }
    public bool IsLedge { get => isLedge; set => isLedge = value; }
    public ClimbingJumpDirection ClimbingJumpDirection{ get => climbingJumpDirection; set => climbingJumpDirection = value; }
    public void SetClimbMove(bool move)
    {
        isClimbingMove = move;
        if( move == false)
        {
            isCanClimbingCancel = false;
        }
    }

    public void SetCanClimbingCancel(bool result)
    {
        isCanClimbingCancel = result;
        isClimbingMove = false;
    }

    [Header("Layer")]
    [SerializeField] private LayerMask adjustAbleLayer;
    [SerializeField] private LayerMask climbingPaintLayer;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask ledgeAbleLayer;
    [SerializeField] private Vector3 detectionOffset;

    public LayerMask LedgeAbleLayer => ledgeAbleLayer;
    public LayerMask AdjustAbleLayer => adjustAbleLayer;

    [Header("Jump")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float minJumpPower = -20;
    [SerializeField] private float currentJumpPower;
    [SerializeField] private float groundMinDistance = 0.1f;
    [SerializeField] private float groundMaxDistance = 0.5f;
    [SerializeField] private float groundSlopMinDistanc = 0.6f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask frontCheckLayer;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private float jumpMinTime = 0.5f;
    [SerializeField] private float jumpTime;
    [SerializeField] private float invalidityAngle = 70.0f;
    [SerializeField] private float groundDistance;
    [SerializeField] private float groundAngle = 0.0f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float currentClimbingJumpPower = 0f;
    [SerializeField] private float climbingHorizonJumpPower = 5.0f;
    [SerializeField] private float climbingUpJumpPower = 8.0f;
    [SerializeField] private float airTime = 0.0f;
    [SerializeField] private float landingFactor = 2.0f;
    [SerializeField] private float keepClimbingJumpTime = 0.8f;
    [SerializeField] private AnimationCurve climbingHorizonJumpSpeedCurve;

    private bool _jumpStart = false;
    private Vector3 slidingVector = Vector3.zero;
    private float climbingJumpStartTime;

    public float AirTime { get => airTime; set => airTime = value; }
    public float ClimbingJumpStartTime { get => climbingJumpStartTime; set => climbingJumpStartTime = value; }
    public float CurrentClimbingJumpPower { get => currentClimbingJumpPower; set => currentClimbingJumpPower = value; }

    public float ClimbingHorizonJumpPower => climbingHorizonJumpPower;
    public float ClimbingUpJumpPower => climbingUpJumpPower;
    public float KeepClimbingJumpTime => keepClimbingJumpTime;
    public AnimationCurve ClimbingHorizonJumpSpeedCurve => climbingHorizonJumpSpeedCurve;

    private bool decharging = false;
    private bool _aimLock = false;

    public bool AimLock { get => _aimLock; set => _aimLock = value; }
    public bool Decharging { get => decharging; set => decharging = value; }

    public Transform DechargingEffectTransform => dechargingEffectTransform;

    [Header("Energy")]
    [SerializeField] private float walkRestoreEnergyValue;
    [SerializeField] private float runRestoreEnergyValue;
    [SerializeField] private float aimRestoreEnergyValue;
    [SerializeField] private float climbingRestoreEnergyValue;
    [SerializeField] private float hitEnergyRestoreValue = 0.0f;
    [SerializeField] private float jumpEnergyRestoreValue = 5.0f;
    [SerializeField] private float climbingJumpEnergyRestoreValue;

    //public float ClimbingRestoreEnergyValue => climbingRestoreEnergyValue;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private float idleConsumeValue = 1f;
    [SerializeField] private float climbingMoveConsumeValue = 2f;
    [SerializeField] private float climbingJumpConsumeValue = 5f;
    [SerializeField] private float wallJumpConsumeValue = 5f;
    [SerializeField] private float staminaRestoreValue = 2f;
    [SerializeField] private float staminaRestoreDelayTime = 2f;
    private TimeCounterEx _staminaTimer;

    public float MaxStamina => maxStamina;
    public float ClimbingJumpConsumeValue => climbingJumpConsumeValue;

    [Header("Input")]
    /// Input
    [SerializeField] private float _inputVertical;
    [SerializeField] private float _inputHorizontal;

    [Header("Spine")]
    [SerializeField] private Transform lookAtAim;
    [SerializeField] private Vector3 relativeVector;
    private Transform spine;

    [Header("Detect")]
    [SerializeField] private LedgeChecker ledgeChecker;
    [SerializeField] private SpaceChecker spaceChecker;
    [SerializeField] private Vector3 wallUnderCheckOffset;


    public LedgeChecker LedgeChecker => ledgeChecker;
    public SpaceChecker SpaceChecker => spaceChecker;
    public Vector3 WallUnderCheckOffset => wallUnderCheckOffset;

    [Header("Gun")]
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Transform dechargingEffectTransform;
    private float dechargingDuration = 2.5f;
    private GameObject pelvisGunObject;
    private List<Material> pelvisGunMaterial = new List<Material>();
    private float _emissionTargetValue = 10f;
    private Color _originalEmissionColor;

    public FMODUnity.StudioEventEmitter _chargeSoundEmitter = null;

    private Animator _animator;
    private Transform _transform;
    private CapsuleCollider _capsuleCollider;
    private EMPGun _empGun;
    private Rigidbody _rigidbody;
    public EMPGun EmpGun => _empGun;
    public Animator GunAnimator => gunAnim;
    public Rigidbody Rigidbody => _rigidbody;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("Player");

        AddAction(MessageTitles.player_initalizemove, (msg) =>
        {
            //InitializeMove();
        });

        AddAction(MessageTitles.player_initVelocity, (msg) =>
        {
            //InitVelocity();
        });

        AddAction(MessageTitles.player_visibledrone, (msg) =>
        {
            //bool visible = (bool)msg.data;
            //drone.Visible = visible;
        });

        AddAction(MessageTitles.fmod_soundEmitter, (msg) =>
        {
            _chargeSoundEmitter = (FMODUnity.StudioEventEmitter)msg.data;
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("PlayerManager"));

        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _empGun = GetComponent<EMPGun>();
        _rigidbody = GetComponent<Rigidbody>();

        if (defaultState == null) defaultState = gameObject.AddComponent<PlayerState_Default>();
        if (jumpState == null) jumpState = gameObject.AddComponent<PlayerState_Jump>();
        if (runToStopState == null) runToStopState = gameObject.AddComponent<PlayerState_RunToStop>();
        if (turnBackState == null) turnBackState = gameObject.AddComponent<PlayerState_TurnBack>();
        if (aimingState == null) aimingState = gameObject.AddComponent<PlayerState_Aiming>();
        if (grabState == null) grabState = gameObject.AddComponent<PlayerState_Grab>();
        if (readyGrabState == null) readyGrabState = gameObject.AddComponent<PlayerState_ReadyGrab>();
        if (hangLedgeState == null) hangLedgeState = gameObject.AddComponent<PlayerState_HangLedge>();
        if (ledgeUpState == null) ledgeUpState = gameObject.AddComponent<PlayerState_LedgeUp>();
        if (hangEdgeState == null) hangEdgeState = gameObject.AddComponent<PlayerState_HangEdge>();
        if (climbingJumpState == null) climbingJumpState = gameObject.AddComponent<PlayerState_ClimbingJump>();
        if (readyClimbingJumpState == null) readyClimbingJumpState = gameObject.AddComponent<PlayerState_ReadyClimbingJump>();

        pelvisGunObject = _empGun.PelvisGunObject;
        foreach (var renderer in pelvisGunObject.GetComponentsInChildren<Renderer>())
        {
            pelvisGunMaterial.Add(renderer.material);
            _originalEmissionColor = renderer.material.GetColor("_EmissionColor");
        }

        _staminaTimer = new TimeCounterEx();
        _staminaTimer.InitTimer("Stamina", 0.0f, staminaRestoreDelayTime);

        spine = _animator.GetBoneTransform(HumanBodyBones.Spine);

        ChangeState(defaultState);
    }

    private void Update()
    {
        UpdateMoveSpeed();

        _currentState.UpdateState(this, _animator);
    }

    private void FixedUpdate()
    {
        _prevDir = _lookDir;

        UpdateStamina(Time.fixedDeltaTime);

        CheckGround();
        CheckRunToStop(Time.fixedDeltaTime);

        _currentState.FixedUpdateState(this, _animator);

        CheckTurnBack();

        MoveConservation();
    }

    private void LateUpdate()
    {
        if(_currentState == aimingState)
        {
            Vector3 dir = (spine.position - lookAtAim.position).normalized;
            Quaternion originalRot = spine.rotation;
            var spineRotation = spine.rotation;
            spineRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(relativeVector);
            spineRotation *= Quaternion.Inverse(transform.rotation);
            spineRotation *= originalRot;
            spine.rotation = spineRotation;
        }
    }

    private void OnAnimatorMove()
    {
        _currentState.AnimatorMove(this, _animator);
    }

    public void ChangeState(PlayerState state)
    {
        if (_currentState == state)
            return;

        if (_currentState != null)
        {
            _prevState = _currentState;
            _prevState.Exit(this, _animator);
        }

        _currentState = state;
        _currentState.Enter(this, _animator);
    }


    /// <summary>
    /// 방향, 델타 타임, 델타 타임 영향 여부
    /// 일반적으로 꼭 델타 타임을 넘겨주자.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="deltaTime"></param>
    /// <param name="noDelta"></param>
    public void Move(Vector3 direction, float deltaTime = 0f ,bool noDelta = false)
    {
        if(noDelta == false)
            base.transform.position += direction * deltaTime;
        else
            base.transform.position += direction;
    }

    public void Jump()
    {
        isJumping = true;
        jumpTime = Time.time;
        currentJumpPower = jumpPower;
        isGrounded = false;
        ChangeState(jumpState);
    }

    private void UpdateMoveSpeed()
    {
        _animator.SetFloat("Speed", currentSpeed);
        _animator.SetFloat("HorizonWeight", _horizonWeight);

        if (_currentState == runToStopState)
            return;

        if (_inputVertical != 0 || _inputHorizontal != 0)
        {
            if (_currentState != aimingState)
            {
                if (isWalk == true)
                {
                    currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, Time.deltaTime * accelerateSpeed);
                }
                else
                {
                    currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, Time.deltaTime * accelerateSpeed);
                }
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, aimingWalkSpeed, Time.deltaTime * accelerateSpeed);
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, Time.deltaTime * accelerateSpeed *2);
        }
    }

    private void UpdateStamina(float deltaTime)
    {
        if(_currentState == grabState ||
            _currentState == readyGrabState||
            _currentState == hangLedgeState||
            _currentState == readyClimbingJumpState||
            _currentState == climbingJumpState||
            _currentState == ledgeUpState)
        {
            _staminaTimer.InitTimer("Stamina", 0.0f, staminaRestoreDelayTime);

            if(isClimbingMove == false)
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

    private void CheckGround()
    {
        if (_currentState == grabState ||
            _currentState == ledgeUpState ||
            //_currentState == PlayerCtrl_Ver2.PlayerState.Ragdoll ||
            //player.GetState() == PlayerCtrl_Ver2.PlayerState.HangRagdoll ||
            _currentState == hangLedgeState)
            //player.GetState() == PlayerCtrl_Ver2.PlayerState.HangShake)
        {
            groundDistance = 0.0f;
            return;
        }

        if (isJumping == true && (Time.time - jumpTime < jumpMinTime))
        {
            return;
        }

        CheckGroundDistance();

        if (groundDistance <= groundMinDistance)
        {
            if (groundAngle < invalidityAngle)
            {
                //isGrounded = true;
                isGrounded = true;
                isJumping = false;

                if(detectObject != null && detectObject.CompareTag("Enviroment"))
                {
                    transform.SetParent(detectObject);
                }
                else
                {
                    if(JumpStart == false &&
                        _currentState != grabState &&
                        _currentState != ledgeUpState &&
                        _currentState != hangLedgeState)
                    {
                        transform.SetParent(null);
                    }
                }

                keepSpeed = false;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            if (groundDistance >= groundMaxDistance)
            {
                if(isGrounded == true)
                {
                    prevParent = transform.parent;
                    detachTime = Time.time;

                    if(prevParent != null)
                    {
                        prevParentPrevPos = prevParent.position;
                        keepSpeed = true;
                    }

                    //if(_currentState == grabstate)
                    //{
                    //    keepSpeed = false;
                    //}
                }

                isGrounded = false;
                if(JumpStart == false &&
                        _currentState != grabState &&
                        _currentState != readyGrabState &&
                        _currentState != ledgeUpState &&
                        _currentState != hangLedgeState &&
                        _currentState != readyGrabState &&
                        _currentState != readyClimbingJumpState &&
                        _currentState != climbingJumpState)
                {
                    transform.SetParent(null);
                }
            }
        }

        _animator.SetBool("IsGround", JumpStart == false ? isGrounded : false);
        bool isNearGround = Physics.Raycast(transform.position, -transform.up, 1.0f, groundLayer);
        _animator.SetBool("IsNearGround", isNearGround);
    }
    private void CheckGroundDistance()
    {
        RaycastHit groundHit;

        if (_capsuleCollider != null)
        {
            float radius = _capsuleCollider.radius;
            float dist = 10f;

            Ray ray2 = new Ray(base.transform.position + new Vector3(0, _capsuleCollider.height / 2, 0), Vector3.down);
            if (Physics.Raycast(ray2, out groundHit, (_capsuleCollider.height / 2) + dist, groundLayer) && !groundHit.collider.isTrigger)
            {
                dist = base.transform.position.y - groundHit.point.y;

                detectObject = groundHit.collider.transform;
                groundAngle = Mathf.Acos(Vector3.Dot(groundHit.normal, Vector3.up)) * Mathf.Rad2Deg;
                slidingVector = (Vector3.Project(Vector3.down, groundHit.normal) - Vector3.down).normalized;
            }
            else
            {
                detectObject = null;
            }


            if (dist >= groundMinDistance)
            {
                Vector3 pos = base.transform.position + Vector3.up * (_capsuleCollider.radius);
                Ray ray = new Ray(pos, -Vector3.up);
                if (Physics.SphereCast(ray, radius, out groundHit, _capsuleCollider.radius + groundMaxDistance, groundLayer) && !groundHit.collider.isTrigger)
                {
                    Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayer);
                    float newDist = base.transform.position.y - groundHit.point.y;
                    if (dist > newDist)
                    {
                        dist = newDist;
                    }

                    groundAngle = Mathf.Acos(Vector3.Dot(groundHit.normal, Vector3.up)) * Mathf.Rad2Deg;
                    slidingVector = (Vector3.Project(Vector3.down, groundHit.normal) - Vector3.down).normalized;
                }
            }
            groundDistance = (float)System.Math.Round(dist, 2);
            if (float.IsNaN(groundAngle) == true)
            {
                groundAngle = 0.0f;
            }
        }
    }

    private void CheckRunToStop(float deltaTime)
    {
        if (_runToStopTime >= 0.05f && currentSpeed > walkSpeed && _inputVertical == 0.0f && _inputHorizontal == 0.0f)
        {
            ChangeState(runToStopState);
            _runToStopTime = 0.0f;
        }

        if (_currentState == defaultState && currentSpeed > 0.0f && _inputVertical == 0.0f && _inputHorizontal == 0.0f)
        {
            _runToStopTime += deltaTime;
        }
        else
        {
            _runToStopTime = 0.0f;
        }
    }

    public void CheckLedge()
    {
        if (InputVertical.Equals(-1.0f))
            return;

        if(ledgeChecker.IsDetectedLedge() == true && isClimbingGround == false)
        {
            if (DetectLedgeCanHangLedgeByVertexColor() == true)
            {
                return;
            }
            isClimbingMove = false;
            isLedge = true;
            ChangeState(hangLedgeState);
        }
    }

    public void UpdateGrab()
    {
        RaycastHit wallHit;
        Vector3 startPos = transform.position + transform.up * (_capsuleCollider.height * 0.5f) + (-transform.forward * 1f);

        if (Physics.SphereCast(startPos, _capsuleCollider.radius, transform.forward, out wallHit, 3.0f, adjustAbleLayer))
        {
            float distToWall = (wallHit.point - (transform.position + transform.up * (_capsuleCollider.height * 0.5f)))
                .magnitude;
            if (distToWall > 0.6f || distToWall < 0.35f)
            {
                transform.position = (wallHit.point - transform.up * (_capsuleCollider.height * 0.5f)) + wallHit.normal * 0.35f;
            }

            if (isClimbingMove == true)
            {
                transform.rotation = Quaternion.LookRotation(-wallHit.normal, transform.up);
            }

            if (wallHit.collider.transform != transform.parent)
            {
                transform.SetParent(wallHit.collider.transform);
            }
        }
        else
        {
            ChangeState(defaultState);
        }
    }

    public bool DetectLedgeCanHangLedgeByVertexColor()
    {
        Vector3 start = transform.position + transform.up * _capsuleCollider.height * 2;
        RaycastHit hit;
        if (Physics.SphereCast(start, _capsuleCollider.radius * 2f, -transform.up, out hit, _capsuleCollider.height * 2,
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

    public bool CheckCanClimbingMoveByVertexColor()
    {
        if (isClimbingMove == false)
            return true;

        Vector3 start = Vector3.zero;

        if (_inputVertical != 0)
        {
            start = _inputVertical == 1
                ? transform.position + transform.up * _capsuleCollider.height * 0.7f
                : transform.position;
        }
        else
        {
            start = _inputHorizontal == 1
                ? transform.position + transform.up * _capsuleCollider.height * 0.5f + transform.right * _capsuleCollider.radius
                : transform.position + transform.up * _capsuleCollider.height * 0.5f + transform.right * -_capsuleCollider.radius;
        }

        RaycastHit hit;
        if (Physics.SphereCast(start, _capsuleCollider.radius, transform.forward, out hit, 2f, climbingPaintLayer))
        {
            MeshFilter wallMesh = hit.collider.GetComponent<MeshFilter>();
            int[] triangles = wallMesh.mesh.triangles;
            Color[] vertexColors = wallMesh.mesh.colors;

            //Debug.Log(vertexColors[triangles[hit.triangleIndex * 3 + 0]].ToString() + vertexColors[triangles[hit.triangleIndex * 3 + 1]].ToString() + vertexColors[triangles[hit.triangleIndex * 3 + 2]].ToString());
            if (vertexColors[triangles[hit.triangleIndex * 3 + 0]] == Color.red
                || vertexColors[triangles[hit.triangleIndex * 3 + 1]] == Color.red
                || vertexColors[triangles[hit.triangleIndex * 3 + 2]] == Color.red)
            {
                //handIK.TraceCenter();
                return false;
            }
        }
        else
        {
            return true;
        }

        return true;
    }

    public bool DetectionCanClimbingAreaByVertexColor(Vector3 startPoint, Vector3 dir, float dist = 2f)
    {
        RaycastHit hit;
        if (Physics.SphereCast(startPoint, _capsuleCollider.radius, dir, out hit, dist, climbingPaintLayer))
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

    public void TakeDamage(float damage)
    {
        hp.Value -= damage;
    }

    private void CheckTurnBack()
    {
        Vector3 moveForward = _moveDir;
        Vector3 prevForward = _prevDir;
        moveForward.y = prevForward.y = 0.0f;
        moveForward.Normalize();
        prevForward.Normalize();

        if (_currentState == defaultState && currentSpeed > 0.0f && Vector3.Dot(moveForward, prevForward) < -0.5f)
        {
            if (currentSpeed > walkSpeed)
            {
                ChangeState(turnBackState);
            }
        }
    }

    public bool IsNowClimbingBehavior()
    {
        return false;
    }

    public void SetJumpPower(float value)
    {
        if (dead)
            return;
        currentJumpPower = value;
    }

    public void SetVelocity(Vector3 velocity)
    {
        _rigidbody.velocity = velocity;
    }

    public void AddVelocity(Vector3 velocity)
    {
        _rigidbody.velocity += velocity;
    }

    public void InitVelocity()
    {
        SetVelocity(Vector3.zero);
    }

    public void SetAimLock(bool value)
    {
        _aimLock = value;
    }

    public void SetRunningLock(bool value)
    {
        //_runLock = value;
    }

    public IEnumerator DechargingCoroutine()
    {
        Decharging = true;
        //if (chargingCountText != null)
        //    chargingCountText.color = Color.red;

        float time = 0;
        while (time < dechargingDuration)
        {
            time += Time.deltaTime;
            float intencity = (dechargingDuration - time) / dechargingDuration * _emissionTargetValue;
            ////pelvisGunMaterial.SetColor("_EmissionColor", _originalEmissionColor * intencity);
            foreach (var mat in pelvisGunMaterial)
            {
                mat.SetColor("_EmissionColor", _originalEmissionColor * intencity);
            }

            yield return null;
        }

        Decharging = false;
        //if (chargingCountText != null)
        //    chargingCountText.color = _chargingCountTextColor;
    }


    #region Status
    public FloatReactiveProperty stamina = new FloatReactiveProperty(100);
    public FloatReactiveProperty hp = new FloatReactiveProperty(100f);
    public FloatReactiveProperty charge = new FloatReactiveProperty(0.0f);
    public FloatReactiveProperty energy = new FloatReactiveProperty(0.0f);
    public IntReactiveProperty loadCount = new IntReactiveProperty(0);
    public FloatReactiveProperty chargeTime = new FloatReactiveProperty(0.0f);
    public IntReactiveProperty hpPackCount = new IntReactiveProperty(0);

    protected bool dead = false;
    #endregion

    #region InputSystem

    public void OnMove(InputAction.CallbackContext value)
    {
        Vector2 inputVector = value.ReadValue<Vector2>();
        _inputVertical = inputVector.y;
        _inputHorizontal = inputVector.x;

        _animator.SetFloat("InputVertical", Mathf.Abs(_inputVertical));
        _animator.SetFloat("InputHorizon", Mathf.Abs(_inputHorizontal));

        _animator.SetFloat("InputHorizonNoAbs", _inputHorizontal);
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        _currentState.OnJump(this, _animator);
    }

    public void OnRun(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        if (isWalk)
        {
            isWalk = false;
        }
        else
        {
            isWalk = true;
        }
    }

    public void OnAim(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        _currentState.OnAim(value,this,_animator);
    }

    public void OnShot(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        _currentState.OnShot(value, this, _animator);
    }

    public void OnGrab(InputAction.CallbackContext value)
    {
        if (value.performed == false)
            return;

        _currentState.OnGrab(value, this, _animator);
    }

    #endregion
}
