using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class PlayerUnit : UnTransfromObjectBase
{

    public static PlayerState_Default defaultState;
    public static PlayerState_Jump jumpState;
    public static PlayerState_RunToStop runToStopState;
    public static PlayerState_TurnBack turnBackState;
    public static PlayerState_Aiming aimingState;

    public float InputVertical { get => _inputVertical; }
    public float InputHorizontal { get => _inputHorizontal; }
    public LayerMask GrounLayer { get => groundLayer; }

    public Transform Transform => _transform;

    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    public float WalkSpeed => walkSpeed;
    public float RotationSpeed { get => rotationSpeed; }
    public CapsuleCollider CapsuleCollider { get => _capsuleCollider; }
    public LayerMask FrontCheckLayer { get => frontCheckLayer; }
    
    public bool IsGround { get => isGrounded; }
    public float JumpPower { get => jumpTime; }
    public float MinJumpPower { get => minJumpPower; }
    public float Gravity { get => gravity; }

    public float CurrentJumpPower { get => currentJumpPower; set => currentJumpPower = value; }

    public Vector3 MoveDir { get => _moveDir; set => _moveDir = value; }
    public Vector3 PrevDir { get => _prevDir; set => _prevDir = value; }
    public Vector3 LookDir { get => _lookDir; set => _lookDir = value; }

    public bool JumpStart { get => _jumpStart; set => _jumpStart = value; }

    public float HorizonWeight { get => _horizonWeight; set => _horizonWeight = value; }

    [SerializeField] private PlayerState _currentState;
    public PlayerState GetState => _currentState;

    private PlayerState _prevState;
    public string currentStateName;

    [Header("Moving")]
    [SerializeField] private bool isWalk;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float accelerateSpeed = 20.0f;
    [SerializeField] private float rotationSpeed = 6.0f;
    private float _horizonWeight = 0.0f;
    private Vector3 _moveDir;
    private Vector3 _prevDir;
    private Vector3 _lookDir;

    float _runToStopTime = 0.0f;

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
    private bool _jumpStart = false;
    private Vector3 slidingVector = Vector3.zero;

    /// Input
    [SerializeField]private float _inputVertical;
    [SerializeField] private float _inputHorizontal;

    private Animator _animator;
    private Transform _transform;
    private CapsuleCollider _capsuleCollider;


    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("Player");
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("PlayerManager"));

        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if (defaultState == null) defaultState = gameObject.AddComponent<PlayerState_Default>();
        if (jumpState == null) jumpState = gameObject.AddComponent<PlayerState_Jump>();
        if (runToStopState == null) runToStopState = gameObject.AddComponent<PlayerState_RunToStop>();
        if (turnBackState == null) turnBackState = gameObject.AddComponent<PlayerState_TurnBack>();
        if (aimingState == null) aimingState = gameObject.AddComponent<PlayerState_Aiming>();

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

        CheckGround();
        CheckRunToStop(Time.fixedDeltaTime);

        _currentState.FixedUpdateState(this, _animator);

        CheckTurnBack();
    }

    private void OnAnimatorMove()
    {
        _currentState.AnimatorMove(this, _animator);
    }

    public void ChangeState(PlayerState state)
    {
        if (_currentState != null)
        {
            _prevState = _currentState;
            _prevState.Exit(this, _animator);
        }

        _currentState = state;
        _currentState.Enter(this, _animator);
    }

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
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, Time.deltaTime * accelerateSpeed *2);
        }
    }

    private void CheckGround()
    {
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
                isGrounded = false;
            }
        }

        _animator.SetBool("IsGround", JumpStart == false ? isGrounded : false);
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

                //detectObject = groundHit.collider.transform;
                groundAngle = Mathf.Acos(Vector3.Dot(groundHit.normal, Vector3.up)) * Mathf.Rad2Deg;
                slidingVector = (Vector3.Project(Vector3.down, groundHit.normal) - Vector3.down).normalized;
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
        //리지드 바디 벨로시티 설정
    }

    public void AddVelocity(Vector3 velocity)
    {
        //rigidbody.velocity += velocity;
    }

    public void InitVelocity()
    {
        SetVelocity(Vector3.zero);
    }

    public void SetAimLock(bool value)
    {
        //_aimLock = value;
    }

    public void SetRunningLock(bool value)
    {
        //_runLock = value;
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
        _animator.SetFloat("InputHorizontal", Mathf.Abs(_inputHorizontal));
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

    #endregion
}
