using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieState_SummonDroneWithRingGround : GenieStateBase
{
    public override string stateIdentifier => "SummonDronesWithRingGround";
    public List<Genie_BombDroneAI> droneAIs;

    public Transform droneSpawnPoint;


    public float patternStartTime = 0f;

    [Header("GroundHitPattern")]
    public float beforeGroundHitTime;
    public float groundHitTime;
    public float groundHitWaitTime;
    public float groundDisapearTime;
    public float groundReturnTime;

    [Header("DroneSpawnPattern")]
    public float droneSpawnStartTime;
    public float droneSpawnTiming;

    private int _currentDroneCount;
    private int _droneSpawnCount;
    private int _droneSpawnLimit;

    private bool _pattern = false;
    private bool _hitGround = false;
    private float _deltaTime;

    private List<HexCube> _areaList = new List<HexCube>();
    private List<HexCube> _safeArea = new List<HexCube>();

    private Genie_BombDroneAI _toPlayerDrone;

    public override void Assign()
    {
        base.Assign();

        _timeCounter.CreateSequencer("PatternWait");
        _timeCounter.AddSequence("PatternWait",patternStartTime,null,SpawnCheck);

        _timeCounter.CreateSequencer("GroundHit");
        _timeCounter.AddSequence("GroundHit", beforeGroundHitTime, LookTarget,GroundHitReady);
        _timeCounter.AddSequence("GroundHit", groundHitTime, null,GroundHit);
        _timeCounter.AddSequence("GroundHit", groundHitWaitTime, null,null);
        
        _timeCounter.CreateSequencer("SpawnDrones");
        _timeCounter.AddSequence("SpawnDrones", droneSpawnStartTime, null,SpawnReady);
        _timeCounter.AddSequence("SpawnDrones", droneSpawnTiming, null,SpawnProgress);
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitSequencer("GroundHit");
        _timeCounter.InitSequencer("SpawnDrones");
        _timeCounter.InitSequencer("PatternWait");

        UpdateDroneCount();
        _droneSpawnLimit = _currentDroneCount;
        _droneSpawnCount = 0;

        _pattern = false;

        ((Genie_CoreDroneAI)droneAIs[droneAIs.Count - 1]).mirror = true;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);
        _deltaTime = deltaTime;

        if(!_pattern)
        {
            LookTarget(target.body,target.targetTransform.position, deltaTime);
            _pattern = _timeCounter.ProcessSequencer("PatternWait",deltaTime);
            if(_pattern)
            {
                _timeCounter.InitSequencer("PatternWait");
                _timeCounter.InitSequencer("GroundHit");
                _timeCounter.InitSequencer("SpawnDrones");
            }
        }
        else
        {
            if(_hitGround)
            {
                _pattern = !_timeCounter.ProcessSequencer("GroundHit",deltaTime);
            }
            else
            {
                LookTarget(target.body,target.targetTransform.position, deltaTime);
                _pattern = !_timeCounter.ProcessSequencer("SpawnDrones",deltaTime);
            }
        }

        if(_toPlayerDrone != null && !_toPlayerDrone.gameObject.activeInHierarchy)
        {
            for(int i = 0; i < droneAIs.Count - 1; ++i)
            {
                if(droneAIs[i].gameObject.activeInHierarchy)
                {
                    _toPlayerDrone = droneAIs[i];
                    _toPlayerDrone.ToMainTarget();
                    _toPlayerDrone.targetExplosion = true;

                    break;
                }
            }
        }
    }

    public override void StateChanged(StateBase targetState)
    {
        base.StateChanged(targetState);

        foreach(var cube in _safeArea)
        {
            cube.GetRenderer().material = target.gridControll.prev;
            cube.special = false;
        }

        SetGroundAreaMaterial(target.gridControll.prev);

        foreach(var cube in _areaList)
        {
            cube.special = false;
        }

        DeleteAllDrone();
    }

    public void SetGroundAreaMaterial(Material mat)
    {
        foreach(var cube in _areaList)
        {
            cube.GetRenderer().material = mat;
        }
    }

    public void LookTarget(float t)
    {
        LookTarget(target.body,target.targetTransform.position, _deltaTime);
    }

    public void GroundHit(float t)
    {
        for(int i = 2; i <= (target.gridControll.cubeGrid.mapSize - 1) / 2 + 1; ++i)
        {
            _areaList.Clear();
            target.gridControll.cubeGrid.GetCubeRing(ref _areaList, target.gridControll.centerCube.cubePoint,i);
            foreach(var item in _areaList)
            {
                item.SetMove(false,(float)(i - 2) * groundDisapearTime + Random.Range(0f,0.2f),1f,groundReturnTime);
            }
        }
        
        // foreach(var item in _areaList)
        // {
        //     item.SetMove(false,Random.Range(0f,0.2f),1f,groundDisapearTime);
        //     SetGroundAreaMaterial(target.gridControll.prev);
        // }
    }

    public void GroundHitReady(float t)
    {
        _areaList.Clear();
        target.ChangeAnimation(4);
        target.CreateEyeLight();
        //SetGroundAreaMaterial(target.gridControll.curr);
    }

    public void SpawnReady(float t)
    {
        target.ChangeAnimation(3);
        target.CreateEyeLight();
    }

    public void SpawnCheck(float t)
    {
        UpdateDroneCount();
        _hitGround = _currentDroneCount > 1;
        if(!_hitGround)
        {
            _droneSpawnLimit = _currentDroneCount;
        }
    }

    public void SpawnProgress(float t)
    {
        for(int i = 0; i < droneAIs.Count - _droneSpawnLimit; ++i)
        {
            if(i == 0)
            {
                _toPlayerDrone = droneAIs[i];
                _toPlayerDrone.ToMainTarget();
                _toPlayerDrone.targetExplosion = true;
            }
            else if(i < droneAIs.Count - 1)
            {
                droneAIs[i]._mainTarget = droneAIs[droneAIs.Count - 1].transform;
                droneAIs[i].targetExplosion = false;
            }
            RespawnDrone(i);
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

    public void SetSafeZone()
    {
        Debug.Log("Two");
        foreach(var cube in _safeArea)
        {
            cube.GetRenderer().material = target.gridControll.prev;
            cube.special = false;
        }

        _safeArea.Clear();

        for(int i = 0; i < 6; ++i)
        {
            target.gridControll.GetCube_Sector(transform.position,i);

            AddSafeZone(target.gridControll.GetRandomTargetCube());
            AddSafeZone(target.gridControll.GetRandomTargetCube());
            AddSafeZone(target.gridControll.GetRandomTargetCube());
        }

    }

    public void AddSafeZone(HexCube cube)
    {
        if(!_safeArea.Find((x)=>{return cube == x;}))
        {
            cube.special = true;
            cube.GetRenderer().material = target.gridControll.prev;
            _safeArea.Add(cube);
        }

    }
}
