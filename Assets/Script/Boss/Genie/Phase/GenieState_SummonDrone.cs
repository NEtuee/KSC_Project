using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieState_SummonDrone : GenieStateBase
{
    public override string stateIdentifier => "SummonDrones";
    public List<Genie_BombDroneAI> droneAIs;

    public Transform droneSpawnPoint;

    public float droneSpawnStartTime;
    public float droneSpawnTiming;

    private int _currentDroneCount;
    private int _droneSpawnCount;
    private int _droneSpawnLimit;

    private bool _spawning = false;

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _timeCounter.InitTimer("droneSpawnTiming",0f,droneSpawnTiming);
        _timeCounter.InitTimer("droneSpawnStart",0f,droneSpawnStartTime);

        UpdateDroneCount();
        _droneSpawnLimit = _currentDroneCount;
        _droneSpawnCount = 0;

        _spawning = false;

        ((Genie_CoreDroneAI)droneAIs[droneAIs.Count - 1]).mirror = true;
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        if(!_spawning)
        {
            SpawnIdle(deltaTime);
            UpdateDroneCount();
        }
        else
        {
            SpawnProgress(deltaTime);
        }
        
    }

    public override void StateChanged(StateBase targetState)
    {
        base.StateChanged(targetState);
    }

    public void SpawnIdle(float deltaTime)
    {
        LookTarget(target.body,target.targetTransform.position, deltaTime);
        if(_currentDroneCount <= 1)
        {
            _timeCounter.IncreaseTimerSelf("droneSpawnStart",out _spawning,deltaTime);
            if(_spawning)
            {
                _timeCounter.InitTimer("droneSpawnTiming",0f,droneSpawnTiming);
                target.ChangeAnimation(3);

                _droneSpawnLimit = _currentDroneCount;
            }
        }
    }

    public void SpawnProgress(float deltaTime)
    {
        LookTarget(target.body,target.targetTransform.position, deltaTime);
        _timeCounter.IncreaseTimerSelf("droneSpawnTiming",out var limit,deltaTime);
        if(limit)
        {
            for(int i = 0; i < droneAIs.Count - _droneSpawnLimit; ++i)
            {
                RespawnDrone(i);
            }
            
            _spawning = false;
            _timeCounter.InitTimer("droneSpawnStart",0f,droneSpawnStartTime);
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
}
