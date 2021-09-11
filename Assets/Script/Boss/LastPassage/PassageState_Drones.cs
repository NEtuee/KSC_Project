using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageState_Drones : PassageStateBase
{
    public override string stateIdentifier => "Drones";

    public List<PassageDrone> drones = new List<PassageDrone>();

    public string nextState;

    public float startTime;
    public float spawnTurm = 0.1f;
    
    private List<Vector3> _startPosition = new List<Vector3>();
    private int _droneCount;
    private int _spawnCount;
    private bool _respawn = false;

    public override void Assign()
    {
        base.Assign();

        for(int i = 0; i < drones.Count; ++i)
        {
            _startPosition.Add(drones[i].transform.position);
        }
    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);

        _respawn = false;
        _droneCount = 0;
        _spawnCount = 0;
        _timeCounter.InitTimer("Start",0f,startTime);
        _timeCounter.InitTimer("Turm",0f,spawnTurm);
        
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);

        if(!_respawn)
        {
            _timeCounter.IncreaseTimerSelf("Start",out var limit, deltaTime);
            if(limit)
            {
                _respawn = Respawn(deltaTime);
            }
        }
        else
        {
            if(_droneCount <= 0)
            {
                StateChange(nextState);
            }
        }
        

    }

    public bool Respawn(float deltaTime)
    {
        _timeCounter.IncreaseTimerSelf("Turm",out var limit, deltaTime);
        if(limit)
        {
            _droneCount += 1;
            drones[_spawnCount].Respawn(_startPosition[_spawnCount++]);

            if(_spawnCount >= drones.Count)
            {
                return true;
            }

            _timeCounter.InitTimer("Turm",0f,spawnTurm);
        }

        return false;
        // for(int i = 0; i < drones.Count; ++i)
        // {
        //     _droneCount += 1;
        //     drones[i].Respawn(_startPosition[i]);
        // }
    }

    public void DroneExplosion()
    {
        --_droneCount;
    }
}
