using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPillarPattern : ObjectBase
{
    private PillarObjectPool _pillarObjectPool;
    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private List<Vector3> _fallPosition = new List<Vector3>();
    private List<FallPillar> _activePillarList = new List<FallPillar>();
    
    public override void Assign()
    {
        base.Assign();

        _pillarObjectPool = GetComponent<PillarObjectPool>();

        _fallPosition.Capacity = 20;

        _timeCounter.CreateSequencer("Fall");

        _timeCounter.AddSequence("Fall", 1.0f, null, (value) =>
        {
            for (int i = 0; i < _fallPosition.Count; i++)
            {
                _activePillarList.Add(_pillarObjectPool.Active(_fallPosition[i], Quaternion.Euler(0.0f,UnityEngine.Random.Range(0.0f,360.0f),0.0f)));
            }
        });

        _timeCounter.AddSequence("Fall", 8.0f, null, (value) =>
        {
            for (int i = 0; i < _activePillarList.Count; i++)
            {
                _activePillarList[i].Fall();
            }
        });

        _timeCounter.AddSequence("Fall", 10.0f, null, (value) =>
        {
            gameObject.SetActive(false);
        });

        _timeCounter.InitSequencer("Fall");
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public void AddFallPosition(Vector3 position)
    {
        _fallPosition.Add(position);
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        _timeCounter.ProcessSequencer("Fall", deltaTime);
    }

    public void Respawn()
    {
        _timeCounter.InitSequencer("Fall");
        _fallPosition.Clear();
        _activePillarList.Clear();
    }
}
