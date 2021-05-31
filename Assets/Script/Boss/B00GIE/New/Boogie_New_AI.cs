using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boogie_New_AI : MonoBehaviour
{
    public delegate void StateDel(float deltaTime);
    public enum State
    {
        Wait = 0,
        SetCore,
        CubeRing,
        CubeSector,
        CubePathCut,
        CannonShot,
        Dead,
        AllOut,
    }
    public Boogie_GridControll gridControll;
    public EMPShield shield;
    public GameObject cannonballBase;
    public State currentState;

    public Vector3 shieldOffset;

    public Vector3 localDeadPosition;

    public int hitPoint = 3;

    public float setCoreTime;
    public float setCoreWaitTime;
    public float cubeRingTime;
    public float cubeRingWaitTime;
    public float cubeRingUpdateTime;
    public float cubeSectorTime;
    public float cubeSectorWaitTime;
    public float cubePathCutTime;
    public float cubePathCutWaitTime;
    public float cannonShotTime;
    public float cannonShotWaitTime;
    public Vector2 randomCannonTime;

    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private State _nextState;

    private Vector3 _deadPosition;

    private int _ringCount = 0;
    private int _sectorCount = 3;
    private int _currentStateNumber = 0;
    private float _initWaitTime = 1f;
    private int _cannonCount = 0;
    private int _shotCount = 0;

    private List<HexCube> _centerCubes;
    private List<HexCube> _cannonRange;
    private Dictionary<int,List<HexCube>> _rings;
    private Dictionary<int, StateDel> _stateDelegate;
    private Dictionary<int, State> _stateSave;

    private List<BoogieCannonball> _cannonBalls;

    public void Start()
    {
        gridControll.Init();

        //gridControll.GetCube_Near(Vector3Int.zero,6,false);
        //_centerCubes = new List<HexCube>(gridControll.GetTargetCubes());
        _cannonRange = new List<HexCube>();
        
        _centerCubes = new List<HexCube>();
        _centerCubes.Add(gridControll.cubeGrid.GetCube(Vector3Int.zero,false));

        _rings = new Dictionary<int, List<HexCube>>();
        _timeCounter.InitTimer("Wait",0f);
        _timeCounter.InitTimer("SetCore",0f,setCoreTime);
        _timeCounter.InitTimer("CubeRing",0f,cubeRingTime);
        _timeCounter.InitTimer("RingUpdate",0f,cubeRingUpdateTime);
        _timeCounter.InitTimer("CubeSector",0f,cubeSectorTime);
        _timeCounter.InitTimer("CubePathCut",0f,cubePathCutTime);
        _timeCounter.InitTimer("CannonShot",0f,cannonShotTime);
        _timeCounter.InitTimer("CannonShotWait",0f,cannonShotWaitTime);
        _timeCounter.InitTimer("CannonShotRandom",0f,3f);

        GetRings();

        _stateDelegate = new Dictionary<int, StateDel>();
        _stateDelegate.Add(0,StateWait);
        _stateDelegate.Add(1,StateSetCore);
        _stateDelegate.Add(2,StateCubeRing);
        _stateDelegate.Add(3,StateCubeSector);
        _stateDelegate.Add(4,StateCubePathCut);
        _stateDelegate.Add(5,StateCannonShot);
        _stateDelegate.Add(6,StateDead);
        _stateDelegate.Add(7,StateAllOut);

        _stateSave = new Dictionary<int, State>();
        _stateSave.Add(0,State.Wait);
        _stateSave.Add(1,State.SetCore);
        _stateSave.Add(2,State.CubeRing);
        _stateSave.Add(3,State.CubeSector);
        _stateSave.Add(4,State.CubePathCut);
        _stateSave.Add(5,State.CannonShot);
        _stateSave.Add(6,State.Dead);
        _stateSave.Add(7,State.AllOut);

        _cannonBalls = new List<BoogieCannonball>();

        _deadPosition = transform.position + localDeadPosition;

        CreateCannonballs();
        ChangeState(State.SetCore);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            ChangeState(State.Dead);
        }
    }

    public void FixedUpdate()
    {
        if(GameManager.Instance.PAUSE || GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Fixed)
            return;

        CannonUpdate(Time.fixedDeltaTime);
        Progress(Time.fixedDeltaTime);

        if(currentState != State.CannonShot && currentState != State.Dead)
        {
            _timeCounter.IncreaseTimerSelf("CannonShotRandom",out bool limit, Time.fixedDeltaTime);
            if(limit)
            {
                _timeCounter.InitTimer("CannonShotRandom",0f,Random.Range(randomCannonTime.x,randomCannonTime.y));
                Shot();
            }
            
        }
    }

    public void Progress(float deltaTime)
    {
        foreach(var cube in gridControll.GetTargetCubes())
        {
            cube.GetRenderer().material = gridControll.prev;
        }

        _stateDelegate[_currentStateNumber](deltaTime);

        foreach(var cube in gridControll.GetTargetCubes())
        {
            cube.GetRenderer().material = gridControll.curr;
        }
    }

    public void Hit()
    {
        if(--hitPoint <= 0)
        {
            ChangeState(State.Dead);
        }
        else if(hitPoint == 1)
        {
            ChangeState(State.AllOut);
        }
        else
        {
            ChangeState(State.CannonShot);
        }
    }

    public void StateWait(float deltaTime)
    {
        _timeCounter.IncreaseTimerSelf("Wait",out bool limit,deltaTime);
        if(limit)
        {
            ChangeState(_nextState);
        }
    }

    public void StateSetCore(float deltaTime)
    {
        _timeCounter.IncreaseTimerSelf("SetCore",out bool limit,deltaTime);
        if(limit)
        {
            var prevCube = gridControll.GetCoreCube();
            gridControll.GetCube_WeekPoint();

            if(hitPoint == 1)
            {
                prevCube.SetActive(false,false);
                gridControll.SetCoreCube(gridControll.GetRandomActiveCube(false));
                ChangeState(State.CannonShot);
            }

            if(gridControll.GetCoreCube() == null)
            {
                return;
            }

            shield.transform.SetParent(gridControll.GetCoreCube().transform);
            shield.transform.position = gridControll.GetCoreCube().transform.position + shieldOffset;
            shield.Reactive();

            if(hitPoint != 1)
            {
                ChangeRandomState(2,4,setCoreWaitTime);
            }
            
            // _nextState = State.CubeRing;
            // _initWaitTime = setCoreWaitTime;
            // ChangeState(State.Wait);
            
        }
    }

    public void StateCubeRing(float deltaTime)
    {
        _timeCounter.IncreaseTimerSelf("RingUpdate",out bool limit,deltaTime);
        
        if(_ringCount < 9)
        {
            foreach(var cube in _rings[_ringCount])
            {
                cube.GetRenderer().material = gridControll.curr;
            }
        }

        if(limit)
        {
            if(_ringCount < 9)
            {
                var list = _rings[_ringCount];
                gridControll.SetCubesActive(ref list,false);
                _timeCounter.InitTimer("RingUpdate",0f,cubeRingUpdateTime);
            }

            if(_ringCount - 2 >= 0 && _ringCount - 2 < 9)
            {
                foreach(var cube in _rings[_ringCount - 2])
                {
                    cube.GetRenderer().material = gridControll.prev;
                }

                var list = _rings[_ringCount - 2];
                gridControll.SetCubesActive(ref list,true);
                _timeCounter.InitTimer("RingUpdate",0f,cubeRingUpdateTime);
            }
            if(_ringCount >= 12)
            {
                //_timeCounter.IncreaseTimerSelf("CubeRing",out limit,deltaTime);

                if(limit)
                {
                    ChangeRandomState(2,4,cubeRingWaitTime);
                    // _nextState = State.CubeSector;
                    // _initWaitTime = cubeRingWaitTime;
                    // ChangeState(State.Wait);
                }
            }

            ++_ringCount;

        }
    }

    public void StateCubeSector(float deltaTime)
    {
        _timeCounter.IncreaseTimerSelf("CubeSector",out bool limit,deltaTime);
        if(limit)
        {
            gridControll.SetCubesActive(false,true,3f);

            ChangeRandomState(2,4,cubeSectorWaitTime);

            // _nextState = State.CubePathCut;
            // _initWaitTime = cubeSectorWaitTime;
            // ChangeState(State.Wait);
        }
    }

    public void StateCubePathCut(float deltaTime)
    {
        _timeCounter.IncreaseTimerSelf("CubePathCut",out bool limit,deltaTime);
        gridControll.GetCube_Walkway();

        if(limit)
        {
            gridControll.SetCubesActive(false,true,3f);

            ChangeRandomState(2,4,cubePathCutWaitTime);

            // _nextState = State.CannonShot;
            // _initWaitTime = cubePathCutWaitTime;
            // ChangeState(State.Wait);
        }
    }

    public void CannonUpdate(float deltaTime)
    {
        foreach(var ball in _cannonBalls)
        {
            if(ball.gameObject.activeSelf)
            {
                if(ball.Progress(deltaTime))
                {
                    //gridControll.cubeGrid.GetCubeNear.GetCube_Near(ball.targetCube,3,true);
                    _cannonRange.Clear();
                    gridControll.cubeGrid.GetCubeNear(ref _cannonRange,ball.targetCube,0,3,true);
                    var target = gridControll.cubeGrid.GetCube(ball.targetCube);

                    if(target != null)
                    {
                        target.SetActive(false,false);
                        target.special = true;
                        --_cannonCount;
                    }

                    foreach(var cube in _cannonRange)
                    {
                        cube.SetActive(false,false);
                        cube.special = true;
                    }
                }
            }
        }
    }

    public bool Shot()
    {
        foreach(var ball in _cannonBalls)
        {
            if(!ball.gameObject.activeSelf)
            {
                var randomCube = gridControll.GetRandomActiveCube(false);

                ball.Active(randomCube.cubePoint,transform.position,randomCube.transform.position,Random.Range(0.3f,0.5f),30f);
                ++_cannonCount;

                return true;
            }
        }

        return false;
    }

    public void HitAllCannon()
    {
        foreach(var ball in _cannonBalls)
        {
            if(ball.gameObject.activeSelf)
            {
                ball.Hit();
            }
        }
    }

    public void StateCannonShot(float deltaTime)
    {
        if(_shotCount < _cannonBalls.Count)
        {
            _timeCounter.IncreaseTimerSelf("CannonShot",out bool limit,deltaTime);
            if(limit)
            {
                _timeCounter.InitTimer("CannonShot",0f,cannonShotTime);

                // bool shot = false;
                // foreach(var ball in _cannonBalls)
                // {
                //     if(!ball.gameObject.activeSelf)
                //     {
                //         var randomCube = gridControll.GetRandomActiveCube(true);

                //         ball.Active(randomCube.cubePoint,transform.position,randomCube.transform.position,Random.Range(0.3f,0.5f),30f);
                //         shot = true;
                //         ++_cannonCount;

                //         break;
                //     }
                // }

                bool shot = Shot();
                if(shot)
                    ++_shotCount;

                if(!shot)
                {
                    _shotCount = _cannonBalls.Count;
                }
            }
        }
        else
        {
            _timeCounter.IncreaseTimerSelf("CannonShotWait",out bool limit,deltaTime);
            if(limit)
            {
                HitAllCannon();

                if(hitPoint == 1)
                {
                    ChangeState(State.CannonShot);
                }
                else
                {
                    ChangeState(State.SetCore);
                }
                
            }
        }
        
    }

    public void StateDead(float deltaTime)
    {
        Vector3 vel = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position,_deadPosition,ref vel,.3f);
    }

    public void StateAllOut(float deltaTime)
    {
        _timeCounter.IncreaseTimerSelf("AllOut",out bool limit,deltaTime);
        if(limit)
        {
            gridControll.SetCubesActive(false);

            ChangeState(State.SetCore);
        }
    }

    public void ChangeRandomState(int start, int end, float wait)
    {
        var target = 3;//Random.Range(start,end + 1);
        _nextState = _stateSave[target];
        _initWaitTime = wait;
        ChangeState(State.Wait);
    }

    public void ChangeStateInt(int state)
    {
        if(_stateSave.ContainsKey(state))
        {
            ChangeState(_stateSave[state]);
        }
    }

    public void ChangeState(State state)
    {
        if(state == State.Wait)
        {
            _timeCounter.InitTimer("Wait",0f,_initWaitTime);
        }
        else if(state == State.SetCore)
        {
            _timeCounter.InitTimer("SetCore",0f,setCoreTime);
            _timeCounter.InitTimer("Wait",0f);
        }
        else if(state == State.CubeRing)
        {
            _timeCounter.InitTimer("CubeRing",0f,cubeRingTime);
            _timeCounter.InitTimer("RingUpdate",0f,cubeRingUpdateTime);
            _ringCount = 0;
        }
        else if(state == State.CubeSector)
        {
            _timeCounter.InitTimer("CubeSector",0f,cubeSectorTime);
            gridControll.GetCube_Sector();
        }
        else if(state == State.CubePathCut)
        {
            _timeCounter.InitTimer("CubePathCut",0f,cubePathCutTime);
            gridControll.GetCube_Walkway();
        }
        else if(state == State.CannonShot)
        {
            _timeCounter.InitTimer("CannonShot",0f,cannonShotTime);
            _timeCounter.InitTimer("CannonShotWait",0f,cannonShotWaitTime);
            _shotCount = 0;

            foreach(var ball in _cannonBalls)
            {
                if(ball.gameObject.activeSelf)
                {
                    _shotCount++;
                }
            }
            
        }
        else if(state == State.Dead)
        {
            foreach(var cube in _centerCubes)
            {
                cube.SetTargetPosition(new Vector3(0f,-25f,0f));
            }

            HitAllCannon();
            
            //this.enabled = false;
        }
        else if(state == State.AllOut)
        {
            gridControll.GetCube_All(true);
            _timeCounter.InitTimer("AllOut",0f,3f);
            
            //this.enabled = false;
        }

        currentState = state;
        _currentStateNumber = (int)currentState;
    }

    public void CreateCannonballs()
    {
        for(int i = 0; i < 4; ++i)
        {
            var obj = Instantiate(cannonballBase);
            var script = obj.GetComponent<BoogieCannonball>();
            obj.SetActive(false);

            _cannonBalls.Add(script);
        }
    }

    public void GetRings()
    {
        gridControll.GetCube_Ring(2);
        var list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(0,list);

        gridControll.GetCube_Ring(3);
        list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(1,list);

        gridControll.GetCube_Ring(4);
        list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(2,list);

        gridControll.GetCube_Ring(5);
        list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(3,list);

        gridControll.GetCube_Ring(6);
        list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(4,list);

        gridControll.GetCube_Ring(7);
        list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(5,list);

        gridControll.GetCube_Ring(8);
        list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(6,list);

        gridControll.GetCube_Ring(9);
        list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(7,list);

        gridControll.GetCube_Ring(10);
        list = new List<HexCube>(gridControll.GetTargetCubes());
        _rings.Add(8,list);
    }
}

