using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Aiming
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

    [Header("Jump Value")]
    [SerializeField] private float currentJumpPower = 0f;
    [SerializeField] private float minJumpPower = -10.0f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravity = 20.0f;

    [Header("LayerMask")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask climbingLayer;

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
    private Vector3 ledgeOffsetPosition;

    [Header("EMP Lunacher")]
    [SerializeField]private float restoreValuePerSecond = 10f;
    [SerializeField] private float costValue = 25f;
    [SerializeField] private float chargeNecessryTime = 1f;
    private float chargeTime = 0.0f;
    [SerializeField] private Transform launchPos;
    [SerializeField] private GameObject bulletPrefab;

    private Rigidbody rigidbody;
    private CapsuleCollider collider;

    private Transform mainCameraTrasform;
    private Animator animator;
    private PlayerMovement movement;
    private PlayerRagdoll ragdoll;

    private RaycastHit wallHit;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        movement = GetComponent<PlayerMovement>();
        ragdoll = GetComponent<PlayerRagdoll>();
        launchPos = transform.Find("LunchPos");

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
        //GizmoHelper.Instance.DrawLine(headTransfrom.position + transform.up * 0.2f, headTransfrom.position + transform.up * 0.2f + transform.forward * 2f, Color.red);

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

                    if (InputTryGrab())
                        return;

                    //if (InputAiming())
                    //    return;
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
                    //InputChargeShot();

                    //if (InputAimingRelease())
                    //    return;
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

        if (state != PlayerState.Grab &&movement.isGrounded == false)
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
                        //moveDir = (Vector3.ProjectOnPlane(moveDir, hit.normal)).normalized;
                        moveDir = (Vector3.ProjectOnPlane(transform.forward, hit.normal)).normalized;
                    }



                    if (lookDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * rotateSpeed);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), deltaTime * rotateSpeed);
                    }
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), deltaTime * rotateSpeed);

                    moveDir *= currentSpeed;

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
                    CheckLedge();

                    UpdateGrab();

                    //if (movement.GetGroundAngle() > 40.0f)
                    //{
                    //    ChangeState(PlayerState.HangRagdoll);
                    //}
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
            case PlayerState.Aiming:
                {
                    RaycastHit hit;
                    if(Physics.Raycast(mainCameraTrasform.position,mainCameraTrasform.forward,out hit,150f))
                    {
                        launchPos.LookAt(hit.point);
                    }
                    else
                    {
                        launchPos.LookAt(mainCameraTrasform.position + mainCameraTrasform.forward * 150.0f);
                    }


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

                    //RaycastHit hit;
                    //if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, groundLayer))
                    //{
                    //    moveDir = (Vector3.ProjectOnPlane(transform.forward, hit.normal)).normalized;
                    //}

                    transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(aimDir, Vector3.up),30.0f * deltaTime);
                    moveDir *= currentSpeed;

                    animator.SetFloat("Speed", currentSpeed);
                    movement.Move(moveDir);
                }
                break;
        }

        UpdateCurrentSpeed(deltaTime);
        //prevDir = moveDir.normalized;
        prevDir = lookDir;
    }
    private void UpdateInputValue(float vertical, float horizontal)
    {
        animator.SetFloat("InputVertical", Mathf.Abs(inputVertical));
        animator.SetFloat("InputHorizon", Mathf.Abs(inputHorizontal));

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

        if(state == PlayerState.Grab && isClimbingMove == false)
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

        if(state == PlayerState.Grab)
        {
            return;
        }

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
        prevState = state;
        state = changeState;

        switch (prevState)
        {
            case PlayerState.Aiming:
                {
                    GameManager.Instance.cameraManger.ActivePlayerFollowCamera();
                }
                break;
        }

        switch (state)
        {
            case PlayerState.Default:
                {
                    animator.applyRootMotion = false;
                    animator.SetBool("IsGrab", false);
                    //transform.rotation = Quaternion.LookRotation(moveDir);
                }
                break;
            case PlayerState.Grab:
                {
                    animator.SetBool("IsGrab", true);
                    currentJumpPower = 0.0f;
                    currentSpeed = 0.0f;
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
            case PlayerState.Ragdoll:
                {
                    transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
                    transform.parent = null;
                }
                break;
            case PlayerState.HangRagdoll:
                {
                    ragdoll.ActiveRightHandFixRagdoll();
                }
                break;
            case PlayerState.Aiming:
                {
                    GameManager.Instance.cameraManger.ActiveAimCamera();
                }
                break;
        }
    }

    public void BackPrevState() { ChangeState(prevState); }

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

    void OnAnimatorMove()
    {
        switch (state)
        {
            case PlayerState.Default:
                {
                    //var p = transform.position;
                    //p += animator.deltaPosition;
                    //transform.position = p;
                }
                break;
            case PlayerState.TurnBack:
                {
                    //var p = transform.localPosition;
                    //p += animator.deltaPosition;
                    //transform.localPosition = p;
                    //var r = transform.localRotation;
                    //r *= animator.deltaRotation;
                    //transform.localRotation = r;

                    var p = transform.position;
                    p += animator.deltaPosition;
                    transform.position = p;
                    var r = transform.rotation;
                    r *= animator.deltaRotation;
                    transform.rotation = r;
                }
                break;
            case PlayerState.RunToStop:
                {
                    if (Physics.Raycast(transform.position + Vector3.up + moveDir.normalized*0.5f, Vector3.down, 1.5f, groundLayer))
                    {
                        //var p = transform.localPosition;
                        //p += animator.deltaPosition.magnitude * moveDir.normalized;
                        //transform.localPosition = p;
                        var p = transform.position;
                        p += animator.deltaPosition.magnitude * moveDir.normalized;
                        transform.position = p;
                    }
                }
                break;
            case PlayerState.Grab:
                {
                    //Debug.Log(Vector3.Angle(transform.up,Vector3.up));

                    if(isClimbingMove == true)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-wallHit.normal, Vector3.up),5f*Time.deltaTime);


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

    public PlayerState GetState()
    {
        return state;
    }

    private bool UpDetection()
    {
        //RaycastHit hit;
        //Vector3 point1 = headTransfrom.position + transform.up * 0.2f;
        //Vector3 point2 = point1 + transform.forward * 1f;
        //if(Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 2f, detectionLayer))
        //{
        //    return true;
        //}

        RaycastHit hit;
        Vector3 point1 = headTransfrom.position + transform.up * 0.2f;
        Vector3 point2 = point1 + transform.forward * 1f;
        if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 2f, detectionLayer))
        {
            MeshFilter wallMesh = hit.collider.GetComponent<MeshFilter>();
            int[] triangles = wallMesh.mesh.triangles;
            Color[] vertexColors = wallMesh.mesh.colors;

            if(vertexColors[triangles[hit.triangleIndex*3+0]] != Color.red
                && vertexColors[triangles[hit.triangleIndex * 3 + 1]] != Color.red
                && vertexColors[triangles[hit.triangleIndex * 3 + 2]] != Color.red)
            {
                return true;
            }
        }

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
                    {
                        isClimbingMove = false;

                        Vector3 currentRot = transform.rotation.eulerAngles;
                        currentRot.x = 0.0f;
                        currentRot.z = 0.0f;
                        transform.rotation = Quaternion.Euler(currentRot);

                        ChangeState(PlayerState.Default);

                        //transform.SetParent(null);
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
        if (InputManager.Instance.GetAction(KeybindingActions.Grab))
        {
            ChangeState(PlayerState.Aiming);
            return true;
        }

        return false;
    }

    private bool InputAimingRelease()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.ReleaseGrab))
        {
            ChangeState(PlayerState.Default);
            return true;
        }

        return false;
    }

    private void InputChargeShot()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Aiming) && energy.Value >= 25f)
        {
            chargeTime += Time.deltaTime;

            if(chargeTime >= chargeNecessryTime)
            {
                chargeTime = 0.0f;
                energy.Value -= 25f;

                if(bulletPrefab != null)
                {
                    Instantiate(bulletPrefab, launchPos.position, launchPos.rotation);
                }
            }
        }
        else
        {
            chargeTime = 0.0f;
        }
    }

    private void UpdateGrab()
    {
        //RaycastHit hit;
        //Vector3 point1 = transform.position + collider.center - transform.forward;

        //if (Physics.SphereCast(point1, collider.radius, transform.forward, out hit, 2f, detectionLayer))
        //{
        //    transform.position = (hit.point - transform.up * (collider.height * 0.5f)) + (hit.normal) * 0.05f;
        //    transform.rotation = Quaternion.LookRotation(-hit.normal, transform.up);
        //}
        //else
        //{
        //    ChangeState(PlayerState.Default);
        //}
        
        Vector3 startPos = transform.position + transform.up * (collider.height * 0.5f) + (-transform.forward * 1f);

        if (Physics.SphereCast(startPos, collider.radius, transform.forward, out wallHit, 3.0f, detectionLayer))
        {
            float distToWall = (wallHit.point - (transform.position + transform.up * (collider.height * 0.5f))).magnitude;
            if (distToWall > 0.6f)
            {
                transform.position = (wallHit.point - transform.up * (collider.height * 0.5f)) + wallHit.normal * 0.5f;
            }

            transform.rotation = Quaternion.LookRotation(-wallHit.normal, transform.up);
            //transform.rotation *= Quaternion.FromToRotation(transform.up, Vector3.up);

            if (wallHit.collider.transform != transform.parent)
            {
                transform.parent = wallHit.collider.transform;
            }
        }
        
    }

    private void CheckLedge()
    {
        if (isClimbingMove == true && currentVerticalValue == 1.0f && LedgeDetection() == false)
        {
            isClimbingMove = false;
            isLedge = true;
            ledgeOffsetPosition = transform.position;
            animator.SetBool("IsLedge", true);
        }
    }

    private bool InputLedgeUp()
    {
        if (InputManager.Instance.GetAction(KeybindingActions.Jump) && isLedge == true && isClimbingMove == false)
        {
            isLedge = false;
            animator.SetTrigger("LedgeUp");
            animator.SetBool("IsLedge",false);

            Vector3 currentRot = transform.rotation.eulerAngles;
            currentRot.x = 0.0f;
            currentRot.z = 0.0f;
            transform.rotation = Quaternion.Euler(currentRot);

            ChangeState(PlayerState.LedgeUp);

            return true;
        }
        return false;
    }

    public void SetClimbMove(bool move) { isClimbingMove = move;}

    private void LateUpdate()
    {
        //if(state == PlayerState.Aiming)
        //{
        //    Vector3 chestDir = mainCameraTrasform.right;
        //    Transform chestTransform = animator.GetBoneTransform(HumanBodyBones.Chest);
        //    //chestTransform.LookAt(chestTransform.position + chestDir * 5f);
        //    chestTransform.localRotation = Quaternion.LookRotation(chestTransform.position + (chestDir * 5f),chestTransform.right);
        //}
    }

    private void RestoreEnergy(float deltaTime)
    {
        if (energy.Value != 100.0f)
        {
            energy.Value += restoreValuePerSecond * deltaTime;
            energy.Value = Mathf.Clamp(energy.Value, 0.0f, 100.0f);
        }
    }
}
