using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Genie_AI : MonoBehaviour
{
    public delegate void StateDel(float deltaTime);
    public enum State
    {
        Idle,
        LookTarget,
        SpawnDrone,
        GroundHitReady,
        GroundHitAttack,
        GroundHitWait,
        Hit,
        Groggy,
        Dead,
    }

    
    public State currentState;

    public Material dangerMat;
    public Material defaultMat;
    public Material safeZoneMat;

    [ColorUsage(true,true)]
    public List<Color> eyeColor;
    public List<Renderer> eyes;

    public Transform respawnPoint;

    public Transform head;
    public Transform droneSpawnPoint;

    public Transform leftChest;
    public Transform rightChest;
    public Boogie_GridControll gridControll;
    public Genie_CenterShield centerShield;

    public List<Genie_BombDroneAI> droneAIs;
    public List<Transform> handIKs;

    public float bodyRotateSpeed = 5f;
    public float headDownDistance = 15f;

    public int hitPoint = 3;

    [Header("GroundHitPattern")]
    public float groundHitStartTime;
    public float groundHitReadyTime;
    public float groundHitAttackTime;
    public float groundHitWaitTime;
    public float groundDisapearTime;
    public float groundHitAreaAngle;

    [Header("DroneSpawnPattern")]
    public float droneSpawnStartTime;
    public float droneSpawnTiming;

    [Header("HitPattern")]
    public float genieHitTime;

    public UnityEvent whenHitCore;
    public UnityEvent whenCancelHitGround;
    public UnityEvent whenGroggy;
    public UnityEvent whenDroneSpawn;



    private Dictionary<State, StateDel> _stateDic;
    private List<HexCube> _areaList;
    private List<HexCube> _safeArea;


    private StateDel _currentStateDelegate;

    private TimeCounterEx _timeCounterEx;
    private GraphAnimator _animator;
    private Animator _animatorController;
    private Transform _target;
    private Transform _groundLookTarget;
    private Vector3 _originPos;
    private Vector3 _headOriginPos;

    private string _currentAnimation;
    private int _currentDroneCount;
    private int _droneSpawnCount;
    private int _droneSpawnLimit;

    public void Start()
    {
        gridControll.Init();

        _originPos = transform.position;
        _target = GameManager.Instance.player.transform;

        _headOriginPos = head.position;
        _currentDroneCount = 0;

        _animator = GetComponent<GraphAnimator>();
        _animatorController = GetComponent<Animator>();
        _timeCounterEx = new TimeCounterEx();

        _areaList = new List<HexCube>();
        _stateDic = new Dictionary<State, StateDel>();
        _stateDic.Add(State.Idle,State_Idle);
        _stateDic.Add(State.LookTarget,State_LookTarget);
        _stateDic.Add(State.SpawnDrone,State_SpawnDrone);
        _stateDic.Add(State.GroundHitReady,State_GroundHitReady);
        _stateDic.Add(State.GroundHitAttack,State_GroundHitAttack);
        _stateDic.Add(State.GroundHitWait,State_GroundHitWait);
        _stateDic.Add(State.Hit,State_Hit);
        _stateDic.Add(State.Groggy,State_Groggy);

        _animator.IsPlaying("BodySpread");
        _currentAnimation = "BodySpread";

        _safeArea = new List<HexCube>();
        //SetSafeZone();

        ChangeState(currentState);
    }

    public void FixedUpdate()
    {
        if(GameManager.Instance.PAUSE || GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Fixed)
            return;

        UpdateDroneCount();
        _currentStateDelegate(Time.fixedDeltaTime);
    }

    public void ChangeState(State state)
    {
        if(state == State.LookTarget)
        {
            _timeCounterEx.InitTimer("groundHitStart",0f,groundHitStartTime);
            _timeCounterEx.InitTimer("droneSpawnStart",0f,droneSpawnStartTime);
        }
        else if(state == State.SpawnDrone)
        {
            _timeCounterEx.InitTimer("droneSpawnTiming",0f,droneSpawnTiming);
            _animator.Play("DroneSpawn",handIKs[1]);

            _droneSpawnLimit = _currentDroneCount;
            _droneSpawnCount = 0;
            UpdateDroneCount();

            whenDroneSpawn?.Invoke();
        }
        else if(state == State.GroundHitReady)
        {
            // gridControll.GetCube_Sector();
            // _groundLookTarget = gridControll.GetTargetCubes()[1].transform;
            // gridControll.GetTargetCubes()[1].GetRenderer().material = null;

            _groundLookTarget = _target;
            _areaList.Clear();
            BodyTilt();

            _timeCounterEx.InitTimer("groundHitReady",0f,groundHitReadyTime);
            _timeCounterEx.InitTimer("groundHitAttack",0f,groundHitAttackTime);
            _timeCounterEx.InitTimer("groundHitWait",0f,groundHitWaitTime);

            if(_animator.IsPlaying("ChestCloseLeft") != null)
            {
                _animator.StopTarget("ChestCloseLeft");
                _animator.StopTarget("ChestCloseRight");
            }

            _animator.Play("ChestOpenLeft",leftChest);
            _animator.Play("ChestOpenRight",rightChest);
            
        }
        else if(state == State.Hit)
        {
            _timeCounterEx.InitTimer("genieHit",0f,genieHitTime);
            _animator.Stop();
            _animator.Play("HitLeft",handIKs[0]);
            _animator.Play("HitRight",handIKs[1]);
            _animator.Play("HitHead",head);
        }
        else if(state == State.Groggy)
        {
            _animator.Stop();
            _animator.Play("GroggyLeft",handIKs[0]);
            _animator.Play("GroggyRight",handIKs[1]);
            _animator.Play("GroggyHead",head);
            _animator.Play("GroggyRotate",head);

            _animatorController.SetBool("RockHand",true);

            whenGroggy?.Invoke();
        }

        currentState = state;
        _currentStateDelegate = _stateDic[state];
    }

    public void HitGroundCancel()
    {
        if(currentState == State.GroundHitAttack)
        {
            ChangeState(State.Hit);
            SetGroundAreaMaterial(defaultMat);
            centerShield.ToOrigin();

            if(_animator.IsPlaying("ChestOpenLeft") != null)
            {
                _animator.StopTarget("ChestOpenLeft");
                _animator.StopTarget("ChestOpenRight");
            }

            _animator.Play("ChestCloseLeft",leftChest);
            _animator.Play("ChestCloseRight",rightChest);
            whenCancelHitGround?.Invoke();
            return;
        }
        whenHitCore?.Invoke();
    }

    public void SetRandomRespawnPoint()
    {
        var cube = _safeArea.Count == 0 ? gridControll.GetRandomActiveCube(true) : _safeArea[Random.Range(0,_safeArea.Count)];
        respawnPoint.position = cube.transform.position + Vector3.up;
    }

    public void Launch()
    {
        ChangeState(State.LookTarget);
    }

    public void HitPlayer(float attack)
    {
        (GameManager.Instance.player as PlayerCtrl_Ver2).TakeDamage(10f);
    }

    public void State_Idle(float deltaTime)
    {

    }

    public void State_Groggy(float deltaTime)
    {
        this.enabled = false;
    }

    public void State_Hit(float deltaTime)
    {
        _timeCounterEx.IncreaseTimerSelf("genieHit",out var limit,deltaTime);
        if(limit)
        {
            _animatorController.SetBool("RockHand",false);
            ChangeState(State.LookTarget);
        }
    }

    public void State_GroundHitReady(float deltaTime)
    {
        LookTargetRotate(_groundLookTarget.position, deltaTime,false);
        HeadLookTarget(_target.position);

        _timeCounterEx.IncreaseTimerSelf("groundHitReady",out var limit,deltaTime);
        if(limit)
        {
            _animatorController.SetBool("RockHand",true);
            _animator.Play("GroundAttackLeft",handIKs[0]);
            _animator.Play("GroundAttackRight",handIKs[1]);

            centerShield.ToTarget();
            SetSafeZone();
            GetGroundArea();
            SetGroundAreaMaterial(dangerMat);
            ChangeState(State.GroundHitAttack);
        }
    }

    public void State_GroundHitAttack(float deltaTime)
    {
        //HeadLookTarget(_target.position);

        _timeCounterEx.IncreaseTimerSelf("groundHitAttack",out var limit,deltaTime);
        if(limit)
        {
            gridControll.SetCubesActive(ref _areaList,false,true,groundDisapearTime);
            SetGroundAreaMaterial(defaultMat);
            // gridControll.GetCube_Sector(_groundLookTarget.position);
            // gridControll.SetCubesActive(false,true,groundDisapearTime);
            centerShield.ToOrigin();

            if(_animator.IsPlaying("ChestOpenLeft") != null)
            {
                _animator.StopTarget("ChestOpenLeft");
                _animator.StopTarget("ChestOpenRight");
            }

            _animator.Play("ChestCloseLeft",leftChest);
            _animator.Play("ChestCloseRight",rightChest);

            ChangeState(State.GroundHitWait);
        }
    }

    public void SetSafeZone()
    {
        foreach(var cube in _safeArea)
        {
            cube.GetComponent<MeshRenderer>().material = defaultMat;
            cube.special = false;
        }

        for(int i = 0; i < 6; ++i)
        {
            gridControll.GetCube_Sector(transform.position,i);

            AddSafeZone(gridControll.GetRandomTargetCube());
            AddSafeZone(gridControll.GetRandomTargetCube());
            AddSafeZone(gridControll.GetRandomTargetCube());
        }

        

    }

    public void AddSafeZone(HexCube cube)
    {
        if(!_safeArea.Find((x)=>{return cube == x;}))
        {
            cube.special = true;
            cube.GetComponent<MeshRenderer>().material = safeZoneMat;
            _safeArea.Add(cube);
        }

    }

    public void SetGroundAreaMaterial(Material mat)
    {
        foreach(var cube in _areaList)
        {
            cube.GetComponent<MeshRenderer>().material = mat;
        }
    }

    public void GetGroundArea()
    {
        var factor = groundHitAreaAngle * 0.25f;
        var dir = Vector3.ProjectOnPlane(_target.position - transform.position,Vector3.up).normalized;
        var start = gridControll.cubeGrid.GetCubePointFromWorld(transform.position + dir * 8f);
        var end = transform.position + dir * 30f;
        var endPoint = gridControll.cubeGrid.GetCubePointFromWorld(end);
        gridControll.cubeGrid.GetCubeLineHeavy(ref _areaList, start,endPoint,0,6);

        var left = Quaternion.Euler(0f,factor,0f) * dir;
        end = transform.position + left * 30f;
        endPoint = gridControll.cubeGrid.GetCubePointFromWorld(end);
        gridControll.cubeGrid.GetCubeLineHeavy(ref _areaList, start,endPoint,0,6,false);

        left = Quaternion.Euler(0f,factor,0f) * left;
        end = transform.position + left * 30f;
        endPoint = gridControll.cubeGrid.GetCubePointFromWorld(end);
        gridControll.cubeGrid.GetCubeLineHeavy(ref _areaList, start,endPoint,0,6,false);

        var right = Quaternion.Euler(0f,-factor,0f) * dir;
        end = transform.position + right * 30f;
        endPoint = gridControll.cubeGrid.GetCubePointFromWorld(end);
        gridControll.cubeGrid.GetCubeLineHeavy(ref _areaList, start,endPoint,0,6,false);

        right = Quaternion.Euler(0f,-factor,0f) * right;
        end = transform.position + right * 30f;
        endPoint = gridControll.cubeGrid.GetCubePointFromWorld(end);
        gridControll.cubeGrid.GetCubeLineHeavy(ref _areaList, start,endPoint,0,6,false);
    }

    public void State_GroundHitWait(float deltaTime)
    {
        HeadLookTarget(_target.position);

        _timeCounterEx.IncreaseTimerSelf("groundHitWait",out var limit,deltaTime);
        if(limit)
        {
            _animator.Play("GroundAttackLeftReturn",handIKs[0]);
            _animator.Play("GroundAttackRightReturn",handIKs[1]);
            _animatorController.SetBool("RockHand",false);

            ChangeState(State.LookTarget);
        }
    }

    public void State_SpawnDrone(float deltaTime)
    {
        LookTargetRotate(_target.position, deltaTime);
        HeadLookTarget(_target.position);

        _timeCounterEx.IncreaseTimerSelf("droneSpawnTiming",out var limit,deltaTime);
        if(limit)
        {
            _timeCounterEx.InitTimer("droneSpawnTiming",0f,droneSpawnTiming);
            RespawnDrone(_droneSpawnCount++);
            if(_droneSpawnCount >= droneAIs.Count - _droneSpawnLimit)
            {
                ChangeState(State.LookTarget);
            }
            
        }

    }

    public void State_LookTarget(float deltaTime)
    {
        LookTargetRotate(_target.position, deltaTime);
        HeadLookTarget(_target.position);

        if(_currentDroneCount <= 1)
        {
            _timeCounterEx.IncreaseTimerSelf("droneSpawnStart",out var spawn,deltaTime);
            if(spawn)
            {
                ChangeState(State.SpawnDrone);
            }
        }
        

        _timeCounterEx.IncreaseTimerSelf("groundHitStart",out var limit,deltaTime);
        if(limit)
        {
            ChangeState(State.GroundHitReady);
        }
    }

    public void Hit()
    {
        --hitPoint;
        UpdateEyeColor();
        SetGroundAreaMaterial(defaultMat);
        centerShield.ToOrigin();

        var currState = currentState;

        if(hitPoint == 0)
        {
            ChangeState(State.Groggy);
        }
        else
        {
            ChangeState(State.Hit);
        }

        if(currState == State.GroundHitAttack || currState == State.GroundHitReady)
        {
            if(_animator.IsPlaying("ChestOpenLeft") != null)
            {
                _animator.StopTarget("ChestOpenLeft");
                _animator.StopTarget("ChestOpenRight");
            }

            _animator.Play("ChestCloseLeft",leftChest);
            _animator.Play("ChestCloseRight",rightChest);
        }
        
        
    }

    public void UpdateEyeColor()
    {
        foreach(var eye in eyes)
        {
            eye.material.color = eyeColor[hitPoint];
        }
    }


    public void DeleteAllDrone()
    {
        foreach(var drone in droneAIs)
        {
            if(!drone.IsDead())
                drone.shield.Destroy();
        }
    }

    public void RespawnDrone(int num)
    {
        droneAIs[num].Respawn(droneSpawnPoint.position);
    }

    public void RespawnAllDrone()
    {
        foreach(var drone in droneAIs)
        {
            drone.Respawn(droneSpawnPoint.position);
        }
    }

    public void UpdateDroneCount()
    {
        _currentDroneCount = 0;
        foreach(var drone in droneAIs)
        {
            if(!drone.IsDead())
                ++_currentDroneCount;
        }
    }


    public void HeadLookTarget(Vector3 target)
    {
        var dir = (target - head.position).normalized;
        head.rotation = Quaternion.LookRotation(dir,Vector3.up);

    }

    public void LookTargetRotate(Vector3 target, float deltaTime, bool bodyAnimation = true)
    {
        var dir = target - _originPos;
        dir = Vector3.ProjectOnPlane(dir,Vector3.up).normalized;
        var dist = Vector3.Distance(MathEx.DeleteYPos(transform.position),MathEx.DeleteYPos(target));

        var speed = deltaTime * bodyRotateSpeed;
        if(dist <= headDownDistance && bodyAnimation)
        {
            speed += ((headDownDistance - dist) / headDownDistance) * 0.3f;
            BodyTilt();
        }
        else if(bodyAnimation)
        {
            BodySpread();
        }

        transform.position = _originPos + transform.forward * 3f;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir,Vector3.up), speed);
    }

    public void BodyTilt()
    {
        if(_currentAnimation != "BodyTilt" && _animator.IsPlaying("BodySpread") == null)
        {
            _currentAnimation = "BodyTilt";
            _animator.Play(_currentAnimation,head);
        }
    }

    public void BodySpread()
    {
        if(_currentAnimation != "BodySpread" && _animator.IsPlaying("BodyTilt") == null)
        {
            _currentAnimation = "BodySpread";
            _animator.Play(_currentAnimation,head);
        }
    }
}
