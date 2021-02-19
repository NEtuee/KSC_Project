using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl_Dummy : MonoBehaviour
{
    public enum DummyState
    {
        Default,
        TurnBack,
        RunToStop
    }

    [Header("Movement Speed Value")]
    [SerializeField] private float walkSpeed = 15.0f;
    [SerializeField] private float runSpeed = 25.0f;
    [SerializeField] private float rollingSpeed = 10.0f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float prevSpeed;
    [Range(0, 5)] [SerializeField] private float fallingControlSenstive = 1f;

    [SerializeField] private DummyState state;

    private bool isRun = false;

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
        InputUpdate();
    }

    private void FixedUpdate()
    {
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
    }

    private void ProcessFixedUpdate()
    {
        fixedVertical = InputManager.Instance.GetMoveAxisVertical();
        fixedHorizontal = InputManager.Instance.GetMoveAxisHorizontal();

        if (rigidbody.velocity != Vector3.zero)
        {
            rigidbody.velocity = Vector3.zero;
        }

        //if (state != DummyState.RunToStop && currentSpeed > walkSpeed && inputVertical == 0.0f && inputHorizontal == 0.0f)
        //{
        //    ChangeState(DummyState.RunToStop);
        //}

        switch (state)
        {
            case DummyState.Default:
                {
                    if (inputVertical != 0.0f || inputHorizontal != 0.0f)
                    {
                        moveDir = (camForward * inputVertical) + (camRight * inputHorizontal);
                        moveDir.Normalize();
                        //prevDir = moveDir;
                    }
                    else
                    {
                        moveDir = prevDir;
                        moveDir.Normalize();
                    }

                    moveDir *= currentSpeed;

                    if (moveDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.fixedDeltaTime * 6.0f);
                    }

                    animator.SetFloat("Speed", currentSpeed);
                    movement.Move((moveDir));
                }
                break;
        }

        UpdateCurrentSpeed();
        prevDir = moveDir.normalized;
    }
    private void UpdateInputValue(float vertical, float horizontal)
    {
        if (state == DummyState.Default)
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
        if (state == DummyState.TurnBack)
        {
            return;
        }

        Vector3 moveForward = moveDir;
        Vector3 prevForward = prevDir;
        moveForward.y = prevForward.y = 0.0f;
        moveForward.Normalize();
        prevForward.Normalize();
        //Debug.Log(Vector3.Dot(moveForward, prevForward));
        if (currentSpeed > 0.0f && Vector3.Dot(moveForward, prevForward) < -0.8f)
        {
            if (currentSpeed > walkSpeed)
            {
                ChangeState(DummyState.TurnBack);
            }
            currentSpeed = 0.0f;
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

    public void ChangeState(DummyState changeState)
    {
        state = changeState;
        switch (state)
        {
            case DummyState.Default:
                {
                    animator.applyRootMotion = false;
                    //transform.rotation = Quaternion.LookRotation(moveDir);
                }
                break;
            case DummyState.TurnBack:
                {
                    animator.applyRootMotion = true;
                    animator.SetTrigger("TurnBack");
                }
                break;
            case DummyState.RunToStop:
                {
                    animator.applyRootMotion = true;
                    animator.SetTrigger("RunToStop");
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
                ChangeState(DummyState.RunToStop);
                time = 0.0f;
            }

            if (currentSpeed>walkSpeed && inputVertical == 0.0f && inputHorizontal == 0.0f)
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
}
