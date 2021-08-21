using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnit : UnTransfromObjectBase
{

    public static PlayerState_Default defaultState;
    public PlayerState_Default DefaultState { get { return defaultState; } }
    public static PlayerState_Jump jumpState;
    public PlayerState_Jump JumpState => jumpState;

    public float InputVertical { get => _inputVertical; }
    public float InputHorizontal { get => _inputHorizontal; }
    public LayerMask GrounLayer { get => groundLayer; }

    public Transform Transform { get => _transform; }

    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    public float RotationSpeed { get => rotationSpeed; }
    public CapsuleCollider CapsuleCollider { get => _capsuleCollider; }
    public LayerMask FrontCheckLayer { get => frontCheckLayer; }
    
    public bool IsGround { get => isGrounded; }
    public float JumpPower { get => jumpTime; }
    public float MinJumpPower { get => minJumpPower; }
    public float Gravity { get => gravity; }

    public float CurrentJumpPower { get => currentJumpPower; set => currentJumpPower = value; }

    private PlayerState _currentState;
    private PlayerState _prevState;
    public string currentStateName;

    [Header("Moving")]
    [SerializeField] private bool isWalk;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float accelerateSpeed = 20.0f;
    [SerializeField] private float rotationSpeed = 6.0f;

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
    private Vector3 slidingVector = Vector3.zero;

    /// Input
    [SerializeField]private float _inputVertical;
    [SerializeField] private float _inputHorizontal;

    private Animator _animator;
    private Transform _transform;
    private CapsuleCollider _capsuleCollider;


    public override void Initialize()
    {
        base.Initialize();

        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if (defaultState == null) defaultState = new PlayerState_Default();
        if (jumpState == null) jumpState = new PlayerState_Jump(); 

        ChangeState(defaultState);
    }

    private void Update()
    {
        UpdateMoveSpeed();

        _currentState.UpdateState(this, _animator);
    }

    private void FixedUpdate()
    {
        CheckGround();

        _currentState.FixedUpdateState(this, _animator);
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
            transform.position += direction * deltaTime;
        else
            transform.position += direction;
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

        _animator.SetFloat("Speed", currentSpeed);
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

        _animator.SetBool("IsGround", isGrounded);
    }

    private void CheckGroundDistance()
    {
        RaycastHit groundHit;

        if (_capsuleCollider != null)
        {
            float radius = _capsuleCollider.radius;
            float dist = 10f;

            Ray ray2 = new Ray(transform.position + new Vector3(0, _capsuleCollider.height / 2, 0), Vector3.down);
            if (Physics.Raycast(ray2, out groundHit, (_capsuleCollider.height / 2) + dist, groundLayer) && !groundHit.collider.isTrigger)
            {
                dist = transform.position.y - groundHit.point.y;

                //detectObject = groundHit.collider.transform;
                groundAngle = Mathf.Acos(Vector3.Dot(groundHit.normal, Vector3.up)) * Mathf.Rad2Deg;
                slidingVector = (Vector3.Project(Vector3.down, groundHit.normal) - Vector3.down).normalized;
            }


            if (dist >= groundMinDistance)
            {
                Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
                Ray ray = new Ray(pos, -Vector3.up);
                if (Physics.SphereCast(ray, radius, out groundHit, _capsuleCollider.radius + groundMaxDistance, groundLayer) && !groundHit.collider.isTrigger)
                {
                    Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayer);
                    float newDist = transform.position.y - groundHit.point.y;
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

        _animator.SetTrigger("Jump");
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
