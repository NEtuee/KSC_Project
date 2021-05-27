using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePool : ObjectPoolBase<Drone_Ai>
{
    public string path;
    public LevelEdit_Controll controll;
    
    private TimeCounterEx _timeCounter = new TimeCounterEx();
    
    public void Awake()
    {
        _activeDelegate += (t,position,rotation) =>
        {
            t.gameObject.SetActive(true);
            t.path = path;
            
            t.Setup();
        };

        _createDelegate += t =>
        {
            t.controll = controll;
        };

        _deleteProgressDelegate += t =>
        {
            t.gameObject.SetActive(false);
        };
        
        _deleteCondition += t =>
        {
            return (t.GetPathArrived() && (t.drop ? !t.bomb.gameObject.activeInHierarchy : true));
        };

        _timeCounter.InitTimer("spawnTime");
    }

    protected override void Update()
    {
        base.Update();
        _timeCounter.IncreaseTimer("spawnTime", out bool limit);
        if (limit)
        {
            _timeCounter.InitTimer("spawnTime");
            Active(Vector3.zero,Quaternion.identity);
        }
    }
}
