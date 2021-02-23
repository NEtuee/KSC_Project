using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Ragdoll
    }

    [Header("State")]
    [SerializeField]private bool isRun = false;
    [SerializeField] private PlayerState state;

    [Header("Movement Speed Value")]
    [SerializeField] private float walkSpeed = 15.0f;
    [SerializeField] private float runSpeed = 25.0f;
    [SerializeField] private float rollingSpeed = 10.0f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float prevSpeed;
    [SerializeField] private float rotateSpeed = 6.0f;
    [Range(0, 5)] [SerializeField] private float fallingControlSenstive = 1f;

    [Header("Jump Value")]
    [SerializeField] private float currentJumpPower = 0f;
    [SerializeField] private float minJumpPower = -10.0f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravity = 20.0f;

    [Header("LayerMask")]
    [SerializeField] private LayerMask groundLayer;

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
    private Vector3 slidingDir;
    private Vector3 prevSlidingDir;
    private Vector3 camForward;
    private Vector3 camRight;
    private Quaternion climbingPrevRot;
    private Vector3 prevForward;

    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private CapsuleCollider collider;

    private Transform mainCameraTrasform;
    private Animator animator;
    private PlayerMovement movement;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        movement = GetComponent<PlayerMovement>();

        moveDir = Vector3.zero;
        currentSpeed = 0.0f;
        mainCameraTrasform = Camera.main.transform;

        StartCoroutine(StopCheck());
    }

    // Update is called once per frame
    void Update()
    {
        if (isPause == true)
        {
            return;
        }

        InputUpdate();
    }

    private void FixedUpdate()
    {
        if (isPause == true)
        {
            return;
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
                        //ChangeState(PlayerState.Jump);
                        animator.SetTrigger("Jump");
                        return;
                    }
                }
                break;
        }
    }

    private void ProcessFixedUpdate()
    {
        if (rigidbody.velocity != Vector3.zero)
        {
            rigidbody.velocity = Vector3.zero;
        }

        if(movement.isGrounded == false)
        {
            currentJumpPower -= gravity * Time.fixedDeltaTime;
            currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, 50f);
        }
        else
        {
            currentJumpPower = 0.0f;
        }

        animator.SetBool("IsGround", movement.isGrounded);

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
                        //moveDir = (Vector3.ProjectOnPlane(moveDir, hit.normal)).normalized;
                        moveDir = (Vector3.ProjectOnPlane(transform.forward, hit.normal)).normalized;
                    }

                    moveDir *= currentSpeed;

                    if (lookDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.fixedDeltaTime * rotateSpeed);
                    }

                    animator.SetFloat("Speed", currentSpeed);
                    movement.Move(moveDir);
                }
                break;
            case PlayerState.Jump:
                {
                    if(movement.isGrounded == true)
                    {
                        ChangeState(PlayerState.Default);
                        //animator.SetLayerWeight(1, 1.0f);
                        return;
                    }

                    Vector3 plusDir = ((camForward * inputVertical) + (camRight * inputHorizontal));
                    movement.Move(plusDir * fallingControlSenstive);

                    lookDir = ((camForward * inputVertical) + (camRight * inputHorizontal)).normalized;
                    if (lookDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, transform.up), Time.deltaTime * 1.0f);
                    }

                    movement.Move(moveDir + (Vector3.up * currentJumpPower));
                }
                break;
        }

        UpdateCurrentSpeed();
        //prevDir = moveDir.normalized;
        prevDir = lookDir;
    }
    private void UpdateInputValue(float vertical, float horizontal)
    {
        animator.SetFloat("InputVertical", inputVertical);
        animator.SetFloat("InputHorizon", inputHorizontal);

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
    }

    private void UpdateCurrentSpeed()
    {
        //if (state != PlayerState.Default)
        //{
        //    return;
        //}

        Vector3 moveForward = lookDir;
        Vector3 prevForward = prevDir;
        moveForward.y = prevForward.y = 0.0f;
        moveForward.Normalize();
        prevForward.Normalize();
        //Debug.Log(Vector3.Dot(moveForward, prevForward));
        if (state == PlayerState.Default && currentSpeed > 0.0f && Vector3.Dot(moveForward, prevForward) < -0.8f)
        {
            if (currentSpeed > walkSpeed)
            {
                ChangeState(PlayerState.TurnBack);
            }
            //currentSpeed = 0.0f;
            return;
        }

        if (inputVertical != 0 || inputHorizontal != 0)
        {
            if (isRun == true)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, Time.fixedDeltaTime * 20.0f);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, Time.fixedDeltaTime * 20.0f);
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, Time.fixedDeltaTime * 40.0f);
            //currentSpeed = 0.0f;
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
        state = changeState;
        switch (state)
        {
            case PlayerState.Default:
                {
                    animator.applyRootMotion = false;
                    //transform.rotation = Quaternion.LookRotation(moveDir);
                }
                break;
            case PlayerState.Jump:
                {
                    //animator.SetTrigger("Jump");
                    //currentSpeed = 0.0f;
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
        }
    }

    IEnumerator StopCheck()
    {
        float time = 0.0f;

        while(true)
        {
            if(time >= 0.05f)
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

    //void OnAnimatorMove()
    //{
    //    switch (state)
    //    {
    //        case DummyState.Default:
    //            {
    //                //var p = transform.position;
    //                //p += animator.deltaPosition;
    //                //transform.position = p;
    //            }
    //            break;
    //        case DummyState.TurnBack:
    //            {
    //                var p = transform.position;
    //                p += animator.deltaPosition;
    //                transform.position = p;
    //                var r = transform.rotation;
    //                r *= animator.deltaRotation;
    //                transform.rotation = r;
    //            }
    //            break;
    //        case DummyState.RunToStop:
    //            {
    //                var p = transform.position;
    //                p += animator.deltaPosition;
    //                transform.position = p;
    //            }
    //            break;
    //    }
    //}
}
