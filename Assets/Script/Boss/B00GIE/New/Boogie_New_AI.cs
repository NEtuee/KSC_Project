using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boogie_New_AI : MonoBehaviour
{
    public enum State
    {
        Wait,
        SetCore,
        CubeRing,
        CubeSector,
        CubePathCut,
        CannonShot,
    }
    public Boogie_GridControll gridControll;
    public State currentState;

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

    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private State _nextState;

    private int _ringCount = 0;
    private int _sectorCount = 3;
    private float _initWaitTime = 1f;
    private Dictionary<int,List<HexCube>> _rings;

    public void Start()
    {
        gridControll.Init();

        _rings = new Dictionary<int, List<HexCube>>();
        _timeCounter.InitTimer("Wait",0f);
        _timeCounter.InitTimer("SetCore",0f,setCoreTime);
        _timeCounter.InitTimer("CubeRing",0f,cubeRingTime);
        _timeCounter.InitTimer("RingUpdate",0f,cubeRingUpdateTime);
        _timeCounter.InitTimer("CubeSector",0f,cubeSectorTime);
        _timeCounter.InitTimer("CubeSector",0f,cubeSectorTime);
        _timeCounter.InitTimer("CubePathCut",0f,cubePathCutTime);
        _timeCounter.InitTimer("CannonShot",0f,cannonShotTime);

        GetRings();
    }

    public void FixedUpdate()
    {
        Progress(Time.fixedDeltaTime);
    }

    public void Progress(float deltaTime)
    {
        if(currentState == State.Wait)
        {
            _timeCounter.IncreaseTimerSelf("Wait",out bool limit,deltaTime);
            if(limit)
            {
                ChangeState(_nextState);
            }
        }
        else if(currentState == State.SetCore)
        {
            _timeCounter.IncreaseTimerSelf("SetCore",out bool limit,deltaTime);
            if(limit)
            {
                gridControll.GetCube_WeekPoint();

                _nextState = State.CubeRing;
                _initWaitTime = setCoreWaitTime;
                ChangeState(State.Wait);
                
            }
        }
        else if(currentState == State.CubeRing)
        {
            
            _timeCounter.IncreaseTimerSelf("RingUpdate",out bool limit,deltaTime);
            if(limit)
            {
                if(_ringCount < 5)
                {
                    var list = _rings[_ringCount];
                    gridControll.SetCubesActive(ref list,false);
                    _timeCounter.InitTimer("RingUpdate",0f,cubeRingUpdateTime);
                }

                if(_ringCount - 3 >= 0 && _ringCount - 3 < 5)
                {
                    var list = _rings[_ringCount - 3];
                    gridControll.SetCubesActive(ref list,true);
                    _timeCounter.InitTimer("RingUpdate",0f,cubeRingUpdateTime);
                }
                if(_ringCount >= 8)
                {
                    //_timeCounter.IncreaseTimerSelf("CubeRing",out limit,deltaTime);

                    if(limit)
                    {
                        _nextState = State.CubeSector;
                        _initWaitTime = cubeRingWaitTime;
                        ChangeState(State.Wait);
                    }
                }

                ++_ringCount;

            }
            
        
        }
        else if(currentState == State.CubeSector)
        {
            _timeCounter.IncreaseTimerSelf("CubeSector",out bool limit,deltaTime);
            if(limit)
            {
                gridControll.SetCubesActive(false,true,3f);

                _nextState = State.CubePathCut;
                _initWaitTime = cubeSectorWaitTime;
                ChangeState(State.Wait);
            }
        }
        else if(currentState == State.CubePathCut)
        {
            _timeCounter.IncreaseTimerSelf("CubePathCut",out bool limit,deltaTime);
            gridControll.GetCube_Walkway();

            if(limit)
            {
                gridControll.SetCubesActive(false,true,3f);

                _nextState = State.CubePathCut;
                _initWaitTime = cubePathCutWaitTime;
                ChangeState(State.CubeRing);
            }
        }
        else if(currentState == State.CannonShot)
        {
            _timeCounter.IncreaseTimerSelf("CannonShot",out bool limit,deltaTime);

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

        }

        currentState = state;
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
    }
}
