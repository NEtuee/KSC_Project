using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UniRx;
using MD;

public partial class PlayerUnit : UnTransfromObjectBase
{
    #region State
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
    public static PlayerState_ClimbingUpperLine climbingUpperLineState;
    public static PlayerState_ReadyClimbingJump readyClimbingJumpState;
    public static PlayerState_HangEdge hangEdgeState;
    public static PlayerState_Ragdoll ragdollState;
    public static PlayerState_HighLanding highLandingState;
    public static PlayerState_Respawn respawnState;
    public static PlayerState_Dash dashState;
    public static PlayerState_Dead deadState;
    public static PlayerState_Kick kickState;
    #endregion

    #region Move Property
    public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    public float JogSpeed { get => jogSpeed; set => jogSpeed = value; }
    public float RunSpeed { get => runSpeed; set => runSpeed = value; }
    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    public float RotationSpeed { get => rotationSpeed; }
    public float AccelerateSpeed { get => accelerateSpeed; set => accelerateSpeed = value; }
    public Vector3 MoveDir { get => _moveDir; set => _moveDir = value; }
    public Vector3 PrevDir { get => _prevDir; set => _prevDir = value; }
    public Vector3 LookDir { get => _lookDir; set => _lookDir = value; }
    public float HorizonWeight { get => _horizonWeight; set => _horizonWeight = value; }

    public bool canGroundCheck = true;
    public bool CanSkipRunToStop { get => _canSkipRunToStop; set => _canSkipRunToStop = value; }

    public float RunTime { get => _runTime; set => _runTime = value; }
    #endregion

    #region Climbing Property
    public bool IsClimbingMove { get => isClimbingMove; set => isClimbingMove = value; }
    public bool IsCanClimbingCancel { get => isCanClimbingCancel; set => isCanClimbingCancel = value; }
    public bool IsClimbingGround { get => isClimbingGround; set => isClimbingGround = value; }
    public bool IsCanReadyClimbingCancel { get => isCanReadyClimbingCancel; set => isCanReadyClimbingCancel = value; }
    public bool IsLedge { get => isLedge; set => isLedge = value; }
    public float HangAbleEdgeDist => hangAbleEdgeDist;
    public ClimbingJumpDirection ClimbingJumpDirection { get => climbingJumpDirection; set => climbingJumpDirection = value; }
    #endregion

    #region Layer Property
    public LayerMask AdjustAbleLayer => adjustAbleLayer;
    public LayerMask DetectionLayer { get => detectionLayer; }
    public LayerMask LedgeAbleLayer => ledgeAbleLayer;
    public LayerMask GrounLayer { get => groundLayer; }
    public LayerMask FrontCheckLayer { get => frontCheckLayer; }
    #endregion

    #region Jump Property
    public float JumpPower { get => jumpPower; set => jumpPower = value; }
    public float MinJumpPower { get => minJumpPower; }
    public float CurrentJumpPower { get => currentJumpPower; set => currentJumpPower = value; }
    public bool IsGround { get => isGrounded; set => isGrounded = value; }
    public bool IsNearGround { get => isNearGround; }
    public float Gravity { get => gravity; }
    public bool JumpStart { get => _jumpStart; set => _jumpStart = value; }
    public bool IsJump { get => isJumping; set => isJumping = value; }
    public float LandingFactor => landingFactor;
    public float AirTime { get => airTime; set => airTime = value; }
    public float ClimbingJumpStartTime { get => climbingJumpStartTime; set => climbingJumpStartTime = value; }
    public float CurrentClimbingJumpPower { get => currentClimbingJumpPower; set => currentClimbingJumpPower = value; }
    public float ClimbingHorizonJumpPower { get => climbingHorizonJumpPower; set => climbingHorizonJumpPower = value; }
    public float ClimbingUpJumpPower { get => climbingUpJumpPower; set => climbingUpJumpPower = value; }
    public float KeepClimbingUpJumpTime { get => keepClimbingUpJumpTime; set => keepClimbingUpJumpTime = value; }
    public float KeepClimbingHorizonJumpTime { get => keepClimbingHorizonJumpTime; set => keepClimbingHorizonJumpTime = value; }



    public AnimationCurve ClimbingHorizonJumpSpeedCurve => climbingHorizonJumpSpeedCurve;
    public AnimationCurve ClimbingUpperLineInterpolateCurve => climbingUpperLineInterpolateCurve;

    #endregion

    #region Energy Property
    public float Energy { get => energy.Value; set { energy.Value = value; } }
    public float WalkRestoreEnergyValue => walkRestoreEnergyValue;
    public float RunRestoreEnergyValue => runRestoreEnergyValue;
    public float AimRestoreEnergyValue => aimRestoreEnergyValue;
    public float ClimbingRestoreEnergyValue => climbingJumpEnergyRestoreValue;
    public float HitEnergyRestoreEnergyValue => HitEnergyRestoreEnergyValue;
    public float JumpEnergyRestoreEnergyValue => jumpEnergyRestoreValue;
    public float ClimbingJumpRestoreEnrgyValue => climbingJumpEnergyRestoreValue;
    #endregion

    #region Stamina Property
    public float MaxStamina => maxStamina;
    public float ClimbingJumpConsumeValue => climbingJumpConsumeValue;
    #endregion

    #region Input Property
    public float InputVertical { get => _inputVertical; }
    public float InputHorizontal { get => _inputHorizontal; }
    #endregion

    #region Detect Property
    public LedgeChecker LedgeChecker => ledgeChecker;
    public SpaceChecker SpaceChecker => spaceChecker;
    public Vector3 WallUnderCheckOffset => wallUnderCheckOffset;
    public Vector3 DetectionOffset => detectionOffset;
    public bool LedgeUpAdjust { get => _ledUpAdjust; set => _ledUpAdjust = value; }

    #endregion

    #region EMPGun Property
    public bool AimLock { get => _aimLock; set => _aimLock = value; }
    public bool Decharging { get => decharging; set => decharging = value; }
    public Transform DechargingEffectTransform => dechargingEffectTransform;
    public Transform SteamPosition => steamPosition;
    public bool CanCharge { get => _bCanCharge; set => _bCanCharge = value; }
    public float NoramlGunCost { get => normalCost; set => normalCost = value; }
    public float ChargeGunCost { get => chargeCost; set => chargeCost = value; }
    public float ChargeConsumeTime { get => chargeConsumeTime; set => chargeConsumeTime = value; }
    #endregion

    #region QuickStanding
    public float QuickStandCoolTime { get => quickStandingCoolTime; set => quickStandingCoolTime = value; }
    public FloatReactiveProperty CurrentQuickStandCoolTime => currentQuickStandingCoolTime;
    public bool CanQuickStanding => bCanQuickStanding;

    #endregion

    #region Dash Property
    public float DashTime { get => dashTime; set => dashTime = value; }
    public float DashSpeed { get => dashSpeed; set => dashSpeed = value; }
    public float DashCoolTime { get => dashCoolTime; set => dashCoolTime = value; }
    public FloatReactiveProperty CurrentDashCoolTime => currentDashCoolTime;
    public bool CanDash { get => bCanDash; }
    #endregion

    public Transform Transform => _transform;
    public Vector3 Position => _transform.position;
    public Quaternion Rotation => _transform.rotation;

    public CapsuleCollider CapsuleCollider { get => _capsuleCollider; }

    public PlayerState GetState => _currentState;

    public PlayerState GetPrevState => _prevState;

    public Drone Drone => drone;

    private IEnumerator restoreHpPackCoroutine;

    private Animator _animator;
    private Transform _transform;
    private CapsuleCollider _capsuleCollider;
    private EMPGun _empGun;
    private Rigidbody _rigidbody;
    private PlayerRagdoll _ragdoll;
    private PlayerHandIK _handIk;
    private IKCtrl _footIk;
    public EMPGun EmpGun => _empGun;
    public Animator GunAnimator => gunAnim;
    public Rigidbody Rigidbody => _rigidbody;
    public PlayerRagdoll Ragdoll => _ragdoll;
    public PlayerHandIK HandIK => _handIk;
    public IKCtrl FootIK => _footIk;

    private RaycastHit _wallHit;

    private Transform _leftFootTransform;
    private Transform _rightFootTransform;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("Player");

        AddAction(MessageTitles.player_initalizemove, (msg) =>
        {
            InitializeMove();
        });

        AddAction(MessageTitles.player_initVelocity, (msg) =>
        {
            InitVelocity();
        });

        AddAction(MessageTitles.player_visibledrone, (msg) =>
        {
            bool visible = (bool)msg.data;
            drone.Visible = visible;
        });

        AddAction(MessageTitles.fmod_soundEmitter, (msg) =>
        {
            _chargeSoundEmitter = (FMODUnity.StudioEventEmitter)msg.data;
        });

        AddAction(MessageTitles.set_climbingLineManager, (msg) =>
        {
            _climbingLineManager = (ClimbingLineManager)msg.data;
        });

        InputUser.onChange +=
            (user, change, device) =>
            {
                if(change == InputUserChange.ControlSchemeChanged)
                {
                    switch(user.controlScheme.Value.name)
                    {
                        case "Gamepad":
                            _gamepadMode = true;
                            break;
                        case "Keyboard":
                            _gamepadMode = false;
                            break;
                    }
                }
            };
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
        _handIk = GetComponent<PlayerHandIK>();
        _footIk = GetComponent<IKCtrl>();
        _ragdoll = GetComponent<PlayerRagdoll>();

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
        if (ragdollState == null) ragdollState = gameObject.AddComponent<PlayerState_Ragdoll>();
        if (highLandingState == null) highLandingState = gameObject.AddComponent<PlayerState_HighLanding>();
        if (respawnState == null) respawnState = gameObject.AddComponent<PlayerState_Respawn>();
        if (deadState == null) deadState = gameObject.AddComponent<PlayerState_Dead>();
        if (dashState == null) dashState = gameObject.AddComponent<PlayerState_Dash>();
        if (kickState == null) kickState = gameObject.AddComponent<PlayerState_Kick>();
        if(climbingUpperLineState == null) climbingUpperLineState = gameObject.AddComponent<PlayerState_ClimbingUpperLine>();

        pelvisGunObject = _empGun.PelvisGunObject;

        if (pelvisGunObject != null)
        {
            foreach (var renderer in pelvisGunObject.GetComponentsInChildren<Renderer>())
            {
                pelvisGunMaterial.Add(renderer.material);
                _originalEmissionColor = renderer.material.GetColor("_EmissionColor");
            }
        }

        _staminaTimer = new TimeCounterEx();
        _staminaTimer.InitTimer("Stamina", 0.0f, staminaRestoreDelayTime);

        _timer = new TimeCounterEx();
        _timer.InitTimer("Dash", 0.0f, DashCoolTime);
        _timer.InitTimer("QuickStand", 0.0f, quickStandingCoolTime);

        spine = _animator.GetBoneTransform(HumanBodyBones.Spine);

        _leftFootTransform = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        _rightFootTransform = _animator.GetBoneTransform(HumanBodyBones.RightFoot);

        CurrentQuickStandCoolTime.Value = quickStandingCoolTime;
        CurrentDashCoolTime.Value = dashCoolTime;

        ChangeState(defaultState);
    }

    private void Update()
    {
        UpdateMoveSpeed();

        if(_inputHorizontal != 0.0f)
        {
            //_climbingWeight = _inputHorizontal;
            _climbingWeight = Mathf.MoveTowards(_climbingWeight, _inputHorizontal, 4f * Time.deltaTime);
        }
        else
        {
            _climbingWeight = Mathf.MoveTowards(_climbingWeight, 0.0f, 2f * Time.deltaTime);
        }
        _animator.SetFloat("ClimbingWeight", _climbingWeight);

        _currentState.UpdateState(this, _animator);

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            energy.Value = 100.0f;
        }
    }

    private void FixedUpdate()
    {
        if (_climbingLineManager == null)
            _currentTestClimbingLines = null;
        else
        {
            _currentTestClimbingLines = _climbingLineManager.GetCurrentCheckClimbingLines(transform.position);
        }

        _prevDir = _lookDir;

        UpdateCoolTime();

        UpdateStamina(Time.fixedDeltaTime);

        if (canGroundCheck)
            CheckGround();

        CheckRunToStop(Time.fixedDeltaTime);

        _currentState.FixedUpdateState(this, _animator);

        CheckTurnBack();

        MoveConservation();

        if (_currentState != jumpState && _currentState != dashState)
        {
            InitVelocity();
        }

        RaycastHit nearHit;
        isNearGround = Physics.Raycast(transform.position, -transform.up, out nearHit, 1.0f, groundLayer);
        float nearGroundAngle = Mathf.Acos(Vector3.Dot(nearHit.normal, Vector3.up)) * Mathf.Rad2Deg;
        if (float.IsNaN(nearGroundAngle)) nearGroundAngle = 0f;

        if (nearGroundAngle > invalidityAngle)
            isNearGround = false;
        else
            isNearGround = true;

        _animator.SetBool("IsNearGround", isNearGround);
    }

    private void LateUpdate()
    {
        //Debug.Log("PlayerUnit LateUpdate");

        if (_currentState == aimingState)
        {
            Vector3 dir = (spine.position - lookAtAim.position).normalized;
            Quaternion originalRot = spine.rotation;
            var spineRotation = spine.rotation;
            spineRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(relativeVector + addibleSpineVector);
            spineRotation *= Quaternion.Inverse(transform.rotation);
            spineRotation *= originalRot;
            spine.rotation = spineRotation;
        }

        _empGun.UpdateLaunchPos();
    }

    private void OnAnimatorMove()
    {
        _currentState.AnimatorMove(this, _animator);
    }

    public void ChangeState(PlayerState state)
    {
        //Debug.Log(state);

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

    public void Move(Vector3 direction, float deltaTime = 0f, bool noDelta = false)
    {
        if (noDelta == false)
            transform.position += direction * deltaTime;
        else
            transform.position += direction;
    }

    public void Jump()
    {
        if (_currentState == readyGrabState)
            return;

        isJumping = true;
        jumpTime = Time.time;
        currentJumpPower = jumpPower;
        isGrounded = false;
        AirTime = 0.0f;

        keepSpeed = true;
        prevParent = transform.parent;
        detachTime = Time.time;
        transform.SetParent(null);
        if (prevParent != null)
        {
            prevParentPrevPos = prevParent.position;
            keepSpeed = true;
        }

        AddEnergy(jumpEnergyRestoreValue);

        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1003; soundData.localPosition = Vector3.zero; soundData.parent = transform; soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);

        ChangeState(jumpState);
    }

    public bool IsClimbing()
    {
        return _currentState == climbingJumpState || _currentState == grabState || 
                _currentState == readyClimbingJumpState || _currentState == readyGrabState ||
                _currentState == ledgeUpState || _currentState == hangLedgeState ||
                _currentState == hangEdgeState || _currentState == climbingUpperLineState;
    }

    public bool IsMoving()
    {
        return currentSpeed != 0f;
    }

    private void UpdateMoveSpeed()
    {
        _animator.SetFloat("Speed", currentSpeed);
        //_animator.SetFloat("HorizonWeight", currentSpeed == runSpeed ? _horizonWeight : 0.0f);
        _animator.SetFloat("HorizonWeight", _horizonWeight);

        if (_currentState == grabState ||
            _currentState == runToStopState ||
            _currentState == highLandingState ||
            _currentState == ragdollState ||
            _currentState == respawnState ||
            _currentState == hangLedgeState||
            _currentState == climbingJumpState)
        {
            currentSpeed = 0.0f;
            return;
        }

        if (_gamepadMode == false)
        {
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
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, Time.deltaTime * accelerateSpeed * 2);
            }
        }
        else
        {
            if (_inputVertical != 0 || _inputHorizontal != 0)
            {
                if (_currentState != aimingState)
                {
                    if(_inputSum >= 1.0f)
                    {
                        currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, Time.deltaTime * accelerateSpeed);
                    }
                    else if(_inputSum < 1.0f)
                    {
                        currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, Time.deltaTime * accelerateSpeed);
                    }
                }
                else
                {
                    float factor = Mathf.Clamp(_inputSum, 0.0f, 1.0f);
                    currentSpeed = aimingWalkSpeed * factor;
                }
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, Time.deltaTime * accelerateSpeed * 0.5f);
            }
        }
    }

    private void UpdateStamina(float deltaTime)
    {
        if (_currentState == grabState ||
            _currentState == readyGrabState ||
            _currentState == hangLedgeState ||
            _currentState == readyClimbingJumpState ||
            _currentState == climbingJumpState ||
            _currentState == ledgeUpState)
        {
            _staminaTimer.InitTimer("Stamina", 0.0f, staminaRestoreDelayTime);

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
            _currentState == ragdollState ||
            _currentState == hangLedgeState)
        {
            groundDistance = 0.0f;
            return;
        }

        if (isJumping == true && (Time.time - jumpTime < jumpMinTime))
        {
            _animator.SetBool("IsGround", JumpStart == false ? isGrounded : false);
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

                if (detectObject != null && detectObject.CompareTag("Enviroment"))
                {
                    transform.SetParent(detectObject);
                }
                else
                {
                    if (JumpStart == false &&
                        _currentState != grabState &&
                        _currentState != ledgeUpState &&
                        _currentState != hangLedgeState &&
                        _currentState != ragdollState &&
                        _currentState != climbingUpperLineState &&
                        _currentState != readyGrabState)
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
                if (isGrounded == true)
                {
                    prevParent = transform.parent;
                    detachTime = Time.time;

                    if (prevParent != null)
                    {
                        prevParentPrevPos = prevParent.position;
                        keepSpeed = true;
                    }

                    if (_currentState == grabState || _currentState == readyGrabState)
                    {
                        keepSpeed = false;
                    }
                }

                isGrounded = false;
                if (JumpStart == false &&
                        _currentState != grabState &&
                        _currentState != readyGrabState &&
                        _currentState != ledgeUpState &&
                        _currentState != hangLedgeState &&
                        _currentState != readyGrabState &&
                        _currentState != readyClimbingJumpState &&
                        _currentState != climbingJumpState &&
                        _currentState != ragdollState &&
                        _currentState != climbingUpperLineState)
                {
                    transform.SetParent(null);
                }
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

                detectObject = groundHit.collider.transform;
                groundAngle = Mathf.Acos(Vector3.Dot(groundHit.normal, Vector3.up)) * Mathf.Rad2Deg;
                //slidingVector = (Vector3.Project(Vector3.down, groundHit.normal) - Vector3.down).normalized;
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
                    //slidingVector = (Vector3.Project(Vector3.down, groundHit.normal) - Vector3.down).normalized;
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
        if (_runTime > _runToStopMinmumTime)
        {
            if (_runToStopTime >= 0.01f && currentSpeed > walkSpeed && _inputVertical == 0.0f && _inputHorizontal == 0.0f)
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

        if (currentSpeed > walkSpeed)
        {
            _runTime += deltaTime;
        }
        else if (currentSpeed == 0f)
        {
            _runTime = 0f;
        }
    }

    public void CheckLedge()
    {
        if (climbingVertical == -1.0)
            return;

        if (ledgeChecker.IsDetectedLedge() == true && isClimbingGround == false)
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
        Vector3 startPos = transform.position + transform.up * (_capsuleCollider.height * 0.5f) + (-transform.forward * 1f);

        if (Physics.SphereCast(startPos, _capsuleCollider.radius, transform.forward, out _wallHit, 3.0f, adjustAbleLayer))
        {
            float distToWall = (_wallHit.point - (transform.position + transform.up * (_capsuleCollider.height * 0.5f)))
                .magnitude;
            if (distToWall > 0.65f || distToWall < 0.35f)
            {
                transform.position = (_wallHit.point - transform.up * (_capsuleCollider.height * 0.5f)) + _wallHit.normal * 0.35f;
                //Debug.Log(distToWall);
            }

            if (isClimbingMove == true)
            {
                transform.rotation = Quaternion.LookRotation(-_wallHit.normal, transform.up);
            }

            if (_wallHit.collider.transform != transform.parent)
            {
                transform.SetParent(_wallHit.collider.transform);
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

    public void TakeDamage(float damage, bool restoreEnergy = true)
    {
        if (_currentState == respawnState && _currentState == ragdollState)
            return;

        hp.Value -= damage;
        if (restoreEnergy == true)
        {
            AddEnergy(hitEnergyRestoreValue);
        }

        if (isHpRestore == true)
        {
            isHpRestore = false;
            StopCoroutine(restoreHpPackCoroutine);
        }

        if (hp.Value <= 0.0f)
        {
            PlayerDead();
        }

        SendMessageEx(MessageTitles.uimanager_damageEffect, GetSavedNumber("UIManager"), null);
    }

    public void TakeDamage(float damage, float ragdollPower, Vector3 ragdollDir)
    {
        if (_currentState == ragdollState)
            return;

        hp.Value -= damage;

        if (isHpRestore == true)
        {
            isHpRestore = false;
            StopCoroutine(restoreHpPackCoroutine);
        }

        if (hp.Value <= 0.0f)
        {
            PlayerDead();
        }

        SendMessageEx(MessageTitles.uimanager_damageEffect, GetSavedNumber("UIManager"), null);

        _ragdoll.ExplosionRagdoll(ragdollPower, ragdollDir);
    }

    public void InitializeMove()
    {
        _animator.SetFloat("Speed", 0.0f);
        _animator.SetBool("Respawn", false);
        ChangeState(defaultState);

        bCanDash = true;
        bCanQuickStanding = true;

        currentDashCoolTime.Value = dashCoolTime;
        currentQuickStandingCoolTime.Value = quickStandingCoolTime;
    }

    public void InitStatus()
    {
        stamina.Value = 100.0f;
        hp.Value = 100.0f;
        energy.Value = 0.0f;
        _ragdoll.ResetRagdoll();
        _rigidbody.velocity = Vector3.zero;
        _footIk.InitPelvisHeight();
        dead = false;
        airTime = 0.0f;
        ChangeState(defaultState);
    }

    private void CheckTurnBack()
    {
        Vector3 moveForward = _moveDir;
        Vector3 prevForward = _prevDir;
        moveForward.y = prevForward.y = 0.0f;
        moveForward.Normalize();
        prevForward.Normalize();

        if (_runTime > _runToStopMinmumTime)
        {
            if (_currentState == defaultState && currentSpeed > 0.0f && Vector3.Dot(moveForward, prevForward) < -0.5f)
            {
                if (currentSpeed > walkSpeed)
                {
                    _runTime = 0.0f;
                    ChangeState(turnBackState);
                }
            }
        }
    }

    public bool IsNowClimbingBehavior()
    {
        if (_currentState == defaultState ||
            _currentState == jumpState ||
            _currentState == runToStopState ||
            _currentState == turnBackState ||
            _currentState == aimingState ||
            _currentState == ragdollState ||
            _currentState == respawnState ||
            _currentState == highLandingState)
            return false;
        else
            return true;
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
    public void AddEnergy(float value)
    {
        energy.Value += value;
        energy.Value = Mathf.Clamp(energy.Value, 0.0f, 100.0f);
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

    public void PlayerDead()
    {
        dead = true;
        ChangeState(deadState);
        SendMessageEx(MessageTitles.uimanager_activeGameOverUi, GetSavedNumber("UIManager"), null);
    }
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


    public IEnumerator DechargingCoroutine()
    {
        Decharging = true;
        //if (chargingCountText != null)
        //    chargingCountText.color = Color.red;
        ColorData red = MessageDataPooling.GetMessageData<ColorData>();
        red.value = Color.red;
        SendMessageEx(MessageTitles.uimanager_setChargingTextColor, GetSavedNumber("UIManager"), red);

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

        ColorData white = MessageDataPooling.GetMessageData<ColorData>();
        white.value = Color.white;
        SendMessageEx(MessageTitles.uimanager_setChargingTextColor, GetSavedNumber("UIManager"), white);
        //if (chargingCountText != null)
        //    chargingCountText.color = _chargingCountTextColor;
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

    public IEnumerator StartDashCoolTime()
    {
        bCanDash = false;
        currentDashCoolTime.Value = 0.0f;
        while (currentDashCoolTime.Value < DashCoolTime)
        {
            currentDashCoolTime.Value += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        bCanDash = true;
    }

    private void UpdateCoolTime()
    {
        currentDashCoolTime.Value = _timer.IncreaseTimerSelf("Dash", out bool limit, Time.fixedDeltaTime);

        if(limit)
            bCanDash = true;

        currentQuickStandingCoolTime.Value += _timer.IncreaseTimerSelf("QuickStand", out limit, Time.fixedDeltaTime);

        if (limit)
            bCanQuickStanding = true;
    }

    public void UseDash()
    {
        bCanDash = false;
        _timer.InitTimer("Dash", 0.0f, DashCoolTime);
    }

    public void UseQuickStand()
    {
        bCanQuickStanding = false;
        _timer.InitTimer("QuickStand", 0.0f, quickStandingCoolTime);
    }

    public IEnumerator StartQuickStandingTime()
    {
        bCanQuickStanding = false;
        currentQuickStandingCoolTime.Value = 0.0f;
        while (currentQuickStandingCoolTime.Value < QuickStandCoolTime)
        {
            currentQuickStandingCoolTime.Value += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        bCanQuickStanding = true;
    }

    public void TryGrab()
    {
        bool detect = false;
        Vector3 nearPosition = new Vector3();
        Line line = new Line();

        ClimbingLine detectLine = null;
        Line detectLineElement = new Line();
        Vector3 prevNearPosition = new Vector3();
        Vector3 finalNearPosition = new Vector3();

        if (TestClimbingLines != null)
        {
            for (int i = 0; i < TestClimbingLines.Count; i++)
            {
                if (TestClimbingLines[i].DetectLine(CapsuleStart, CapsuleEnd, CapsuleRadius, Transform, out nearPosition, ref line))
                {
                    detect = true;
                    if (detectLine == null)
                    {
                        detectLine = TestClimbingLines[i];
                        detectLineElement = line;
                        prevNearPosition = nearPosition;
                        finalNearPosition = nearPosition;
                    }
                    else
                    {
                        if (Vector3.SqrMagnitude(nearPosition - CapsuleStart) < Vector3.SqrMagnitude(prevNearPosition - CapsuleStart))
                        {
                            detectLine = TestClimbingLines[i];
                            detectLineElement = line;
                            prevNearPosition = nearPosition;
                            finalNearPosition = nearPosition;
                        }
                    }
                }
            }
        }

        if (detect == false)
        {
            for (int i = 0; i< ClimbingLineManager.dynamicClimbingLines.Count; i++)
            {
                if (ClimbingLineManager.dynamicClimbingLines[i].DetectLine(CapsuleStart, CapsuleEnd, CapsuleRadius, Transform, out nearPosition, ref line))
                {
                    detect = true;
                    if (detectLine == null)
                    {
                        detectLine = ClimbingLineManager.dynamicClimbingLines[i];
                        detectLineElement = line;
                        prevNearPosition = nearPosition;
                        finalNearPosition = nearPosition;
                    }
                    else
                    {
                        if (Vector3.SqrMagnitude(nearPosition - CapsuleStart) < Vector3.SqrMagnitude(prevNearPosition - CapsuleStart))
                        {
                            detectLine = ClimbingLineManager.dynamicClimbingLines[i];
                            detectLineElement = line;
                            prevNearPosition = nearPosition;
                            finalNearPosition = nearPosition;
                        }
                    }
                }
            }
        }


        if (detect == true)
        {
            prevFollowLine = currentFollowLine;
            CurrentFollowLine = detectLine;
            lineTracker.position = finalNearPosition;
            lineTracker.SetParent(detectLine.transform);
            StartLineClimbing(finalNearPosition);         

            if (CurrentFollowLine.directionType == DirectionType.LeftMin)
            {
                leftPointNum = Mathf.Min(detectLineElement.p1, detectLineElement.p2);
                rightPointNum = Mathf.Max(detectLineElement.p1, detectLineElement.p2);
            }
            else
            {
                leftPointNum = Mathf.Max(detectLineElement.p1, detectLineElement.p2);
                rightPointNum = Mathf.Min(detectLineElement.p1, detectLineElement.p2);
            }
        }
    }

    public bool CheckUpClimbingLine()
    {
        Vector3 nearPosition = new Vector3();
        Line line = new Line();

        bool detect = false;
        ClimbingLine detectLine = null;
        Line detectLineElement = new Line();
        Vector3 prevNearPosition = new Vector3();
        Vector3 finalNearPosition = new Vector3();

        if (TestClimbingLines != null)
        {
            for (int i = 0; i < TestClimbingLines.Count; i++)
            {
                if (TestClimbingLines[i].DetectLine(UpperCheckCapsuleStart, UpperCheckCapsuleEnd, upCheckCapsuleRadius, Transform, out nearPosition, ref line))
                {
                    detect = true;
                    if (detectLine == null)
                    {
                        detectLine = TestClimbingLines[i];
                        detectLineElement = line;
                        prevNearPosition = nearPosition;
                        finalNearPosition = nearPosition;
                    }
                    else
                    {
                        if (Vector3.SqrMagnitude(nearPosition - UpperCheckCapsuleStart) < Vector3.SqrMagnitude(prevNearPosition - UpperCheckCapsuleStart))
                        {
                            detectLine = TestClimbingLines[i];
                            detectLineElement = line;
                            prevNearPosition = nearPosition;
                            finalNearPosition = nearPosition;
                        }
                    }
                }
            }
        }

        if (detect == false)
        {
            for(int i = 0; i < ClimbingLineManager.dynamicClimbingLines.Count; i++)
            {
                if (ClimbingLineManager.dynamicClimbingLines[i].DetectLine(UpperCheckCapsuleStart, UpperCheckCapsuleEnd, upCheckCapsuleRadius, Transform, out nearPosition, ref line))
                {
                    detect = true;
                    if (detectLine == null)
                    {
                        detectLine = ClimbingLineManager.dynamicClimbingLines[i];
                        detectLineElement = line;
                        prevNearPosition = nearPosition;
                        finalNearPosition = nearPosition;
                    }
                    else
                    {
                        if (Vector3.SqrMagnitude(nearPosition - UpperCheckCapsuleStart) < Vector3.SqrMagnitude(prevNearPosition - UpperCheckCapsuleStart))
                        {
                            detectLine = ClimbingLineManager.dynamicClimbingLines[i];
                            detectLineElement = line;
                            prevNearPosition = nearPosition;
                            finalNearPosition = nearPosition;
                        }
                    }
                }
            }
        }

        if (detect == true)
        {
            prevFollowLine = currentFollowLine;
            CurrentFollowLine = detectLine;
            transform.SetParent(null);
            lineTracker.position = finalNearPosition;
            lineTracker.SetParent(detectLine.transform);

            if (CurrentFollowLine.directionType == DirectionType.LeftMin)
            {
                leftPointNum = Mathf.Min(detectLineElement.p1, detectLineElement.p2);
                rightPointNum = Mathf.Max(detectLineElement.p1, detectLineElement.p2);
            }
            else
            {
                leftPointNum = Mathf.Max(detectLineElement.p1, detectLineElement.p2);
                rightPointNum = Mathf.Min(detectLineElement.p1, detectLineElement.p2);
            }
        }

        return detect;
    }


    [SerializeField] private PlayerState _currentState;
    private PlayerState _prevState;

    #region Status
    public FloatReactiveProperty stamina = new FloatReactiveProperty(100);
    public FloatReactiveProperty hp = new FloatReactiveProperty(100f);
    public FloatReactiveProperty charge = new FloatReactiveProperty(0.0f);
    public FloatReactiveProperty energy = new FloatReactiveProperty(0.0f);
    public IntReactiveProperty loadCount = new IntReactiveProperty(0);
    public FloatReactiveProperty chargeTime = new FloatReactiveProperty(0.0f);
    public IntReactiveProperty hpPackCount = new IntReactiveProperty(0);

    [SerializeField] protected bool dead = false;
    public bool Dead => dead;

    #endregion

    public string currentStateName;

    [Header("Moving")]
    [SerializeField] private bool isWalk;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jogSpeed = 5.5f;
    [SerializeField] private float runSpeed;
    [SerializeField] private float aimingWalkSpeed = 5.5f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float accelerateSpeed = 20.0f;
    [SerializeField] private float rotationSpeed = 6.0f;
    [SerializeField] private float _runTime = 0.0f;
    private float _runToStopMinmumTime = 2f;
    private float _horizonWeight = 0.0f;
    private Vector3 _moveDir;
    private Vector3 _prevDir;
    private Vector3 _lookDir;
    private float _runToStopTime = 0.0f;
    private bool _canSkipRunToStop = false;
    private bool _runKeyRevert = false;

    [Header("Climbing")]
    [SerializeField] private bool isClimbingMove = false;
    [SerializeField] private bool isCanClimbingCancel = false;
    [SerializeField] private bool isClimbingGround = false;
    [SerializeField] private bool isCanReadyClimbingCancel = false;
    [SerializeField] private bool isLedge = false;
    [SerializeField] private float hangAbleEdgeDist = 2f;
    [SerializeField] private ClimbingJumpDirection climbingJumpDirection;

    [Header("Layer")]
    [SerializeField] private LayerMask adjustAbleLayer;
    [SerializeField] private LayerMask climbingPaintLayer;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask ledgeAbleLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask frontCheckLayer;
    [SerializeField] private LayerMask kickLayer;

    [Header("Jump")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float minJumpPower = -20;
    [SerializeField] private float currentJumpPower;
    [SerializeField] private float groundMinDistance = 0.1f;
    [SerializeField] private float groundMaxDistance = 0.5f;
    [SerializeField] private float groundSlopMinDistanc = 0.6f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isNearGround;
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
    [SerializeField] private float keepClimbingUpJumpTime = 0.4f;
    [SerializeField] private float keepClimbingHorizonJumpTime = 0.4f;
    [SerializeField] private AnimationCurve climbingHorizonJumpSpeedCurve;
    [SerializeField] private AnimationCurve climbingUpperLineInterpolateCurve;

    private bool _jumpStart = false;
    private float climbingJumpStartTime;

    [Header("Energy")]
    [SerializeField] private float walkRestoreEnergyValue;
    [SerializeField] private float runRestoreEnergyValue;
    [SerializeField] private float aimRestoreEnergyValue;
    [SerializeField] private float climbingRestoreEnergyValue;
    [SerializeField] private float hitEnergyRestoreValue = 0.0f;
    [SerializeField] private float jumpEnergyRestoreValue = 5.0f;
    [SerializeField] private float climbingJumpEnergyRestoreValue;


    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private float idleConsumeValue = 1f;
    [SerializeField] private float climbingMoveConsumeValue = 2f;
    [SerializeField] private float climbingJumpConsumeValue = 5f;
    [SerializeField] private float wallJumpConsumeValue = 5f;
    [SerializeField] private float staminaRestoreValue = 2f;
    [SerializeField] private float staminaRestoreDelayTime = 2f;
    private TimeCounterEx _staminaTimer;

    [Header("Input")]
    [SerializeField] private float _inputVertical;
    [SerializeField] private float _inputHorizontal;
    [SerializeField] private float _inputSum;
    private float climbingVertical = 0.0f;
    private float climbingHorizon = 0.0f;
    [SerializeField]private bool _gamepadMode = false;

    public bool GamepadMode => _gamepadMode;

    [Header("Spine")]
    [SerializeField] private Transform lookAtAim;
    [SerializeField] private Vector3 relativeVector;
    [HideInInspector]public Vector3 addibleSpineVector;
    private Transform spine;

    [Header("Detect")]
    [SerializeField] private LedgeChecker ledgeChecker;
    [SerializeField] private SpaceChecker spaceChecker;
    [SerializeField] private Vector3 wallUnderCheckOffset;
    [SerializeField] private Vector3 detectionOffset;
    private bool _ledUpAdjust = false;

    [Header("Gun")]
    [SerializeField] private Animator gunAnim;
    [SerializeField] private bool decharging = false;
    [SerializeField] private float normalCost = 25.0f;
    [SerializeField] private float chargeCost = 50.0f;
    [SerializeField] private float chargeConsumeTime = 3.0f;
    private bool _bCanCharge = true;
    private bool _aimLock = false;
    private float dechargingDuration = 2.5f;
    private GameObject pelvisGunObject;
    private List<Material> pelvisGunMaterial = new List<Material>();
    private float _emissionTargetValue = 10f;
    private Color _originalEmissionColor;

    public FMODUnity.StudioEventEmitter _chargeSoundEmitter = null;

    [Header("Drone")]
    [SerializeField] private Drone drone;

    [Header("HpPack")]
    [SerializeField] private float hpPackRestoreValue = 6.0f;
    [SerializeField] private float _hpPackRestoreDuration = 10.0f;
    [SerializeField] private bool isHpRestore = false;

    [Header("Dash")]
    [SerializeField] private float dashTime = 0.3f;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashCoolTime = 5.0f;
    private FloatReactiveProperty currentDashCoolTime = new FloatReactiveProperty(0.0f);
    [SerializeField] private bool bCanDash = true;

    [Header("QuickStanding")]
    [SerializeField] private float quickStandingCoolTime = 5.0f;
    private FloatReactiveProperty currentQuickStandingCoolTime = new FloatReactiveProperty(0.0f);
    private bool bCanQuickStanding = true;

    [Header("Reference")]
    [SerializeField] private Transform steamPosition;
    [SerializeField] private Transform dechargingEffectTransform;

    [Header("ClimbingLine")]
    [SerializeField] private ClimbingLine currentFollowLine;
    [SerializeField] private ClimbingLine prevFollowLine;
    [SerializeField] private Transform lineTracker;
    public Transform leftPoint;
    public Transform rightPoint;
    public int leftPointNum;
    public int rightPointNum;
    public ClimbDir climbDir;
    public ClimbDir prevClimbDir;
    private float _climbingWeight = 0.0f;
    private ClimbingLineManager _climbingLineManager;
    private List<ClimbingLine> _currentTestClimbingLines = null;

    public Transform LineTracker => lineTracker;
    public ClimbingLine CurrentFollowLine { get => currentFollowLine; set => currentFollowLine = value; }
    public ClimbingLine PrevFollowLine { get => prevFollowLine; set => prevFollowLine = value; }
    public ClimbingLineManager ClimbingLineManager => _climbingLineManager;
    public List<ClimbingLine> TestClimbingLines => _currentTestClimbingLines;

    [Header("Detection Capsule")]
    [SerializeField] private Vector3 start;
    [SerializeField] private Vector3 end;
    [SerializeField] private float radius;
    [SerializeField] private Vector3 upCheckCapsuleStart;
    [SerializeField] private Vector3 upCheckCapsuleEnd;
    [SerializeField] private float upCheckCapsuleRadius;

    private TimeCounterEx _timer;

    public Vector3 CapsuleStart { get { return transform.TransformPoint(start); } }
    public Vector3 CapsuleEnd { get { return transform.TransformPoint(end); } }
    public float CapsuleRadius => radius;

    public Vector3 UpperCheckCapsuleStart { get { return transform.TransformPoint(upCheckCapsuleStart); } }
    public Vector3 UpperCheckCapsuleEnd { get { return transform.TransformPoint(upCheckCapsuleEnd); } }

    public Transform LeftPoint { get => leftPoint; set => leftPoint = value; }
    public Transform RightPoint { get => rightPoint; set => rightPoint = value; }

    public void SetLine(Line line)
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(CapsuleStart, radius);
        Gizmos.DrawWireSphere(CapsuleEnd, radius);
        Gizmos.DrawLine(CapsuleStart, CapsuleEnd);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(UpperCheckCapsuleStart, upCheckCapsuleRadius);
        Gizmos.DrawWireSphere(UpperCheckCapsuleEnd, upCheckCapsuleRadius);
        Gizmos.DrawLine(UpperCheckCapsuleStart, UpperCheckCapsuleEnd);
    }
#endif

    protected void OnCollisionEnter(Collision collision)
    {
        if(_currentState == dashState)
        {
            MessageReceiver receiver;
            if (collision.gameObject.TryGetComponent<MessageReceiver>(out receiver))
            {
                Message msg = new Message();
                msg.Set(MessageTitles.object_kick, receiver.uniqueNumber, this, this);
                receiver.ReceiveMessage(msg);
            }
        }
    }

    #region InputSystem

    public void OnMove(InputAction.CallbackContext value)
    {
        Vector2 inputVector = value.ReadValue<Vector2>();
        _inputVertical = inputVector.y;
        _inputHorizontal = inputVector.x;
        _inputSum = Mathf.Abs(_inputVertical) + Mathf.Abs(_inputHorizontal);

        _animator.SetFloat("InputVertical", Mathf.Abs(_inputVertical));
        _animator.SetFloat("InputHorizon", Mathf.Abs(_inputHorizontal));

        _animator.SetFloat("InputHorizonNoAbs", _inputHorizontal);
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        _currentState.OnJump(this, _animator);
    }

    public void OnRun(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        isWalk = !isWalk;
    }

    public void OnRunConvert(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        //Debug.Log("RunConvert");
        //_runKeyRevert = !_runKeyRevert;
        isWalk = !isWalk;
    }

    public void OnAim(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        _currentState.OnAim(value,this,_animator);
    }

    public void OnShot(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        _currentState.OnShot(value, this, _animator);
    }

    public void OnGrab(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        _currentState.OnGrab(value, this, _animator);
    }

    public void OnGrabRelease(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        _currentState.OnGrabRelease(value, this, _animator);
    }

    public void OnUseHpPack(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        if (hp.Value < 100.0f && hpPackCount.Value > 0 && isHpRestore == false)
        {
            hpPackCount.Value--;
            restoreHpPackCoroutine = HpRestore();
            StartCoroutine(restoreHpPackCoroutine);
        }
    }

    public void OnDash(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        _currentState.OnDash(value, this, _animator);
    }

    public void OnQuickStand(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        _currentState.OnQuickStand(value, this, _animator);
    }

    public void OnKick(InputAction.CallbackContext value)
    {
        if (value.performed == false || Time.timeScale == 0f)
            return;

        //_currentState.OnKick(value, this, _animator);
    }

    #endregion
}

public enum ClimbDir
{
    Left,Right,Stop
}
