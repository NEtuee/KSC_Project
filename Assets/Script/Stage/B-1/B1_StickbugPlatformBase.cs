using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_StickbugPlatformBase : B1_Platform
{
    public GameObject dronePrefab;
    public Transform spawnPosition;
    
    public B1_Stickbug stickbug;

    public float spawnTerm = 4f;
    public int spawnLimit = 5;

    public List<B1_FlySpider> flySpiders = new List<B1_FlySpider>();

    private List<CommonDrone> _droneList = new List<CommonDrone>();
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private bool _launch = false;

    public override void Assign()
    {
        base.Assign();

        _timeCounter.CreateSequencer("Spawn");
        _timeCounter.AddSequence("Spawn",spawnTerm,null,Spawn);

        for(int i = 0; i < spawnLimit; ++i)
        {
            var drone = Instantiate(dronePrefab,spawnPosition.position,spawnPosition.rotation);
            drone.transform.SetParent(this.transform);

            _droneList.Add(drone.GetComponent<CommonDrone>());
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        _timeCounter.InitSequencer("Spawn");
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if(!_launch)
            return;

        if(_timeCounter.ProcessSequencer("Spawn",deltaTime))
        {
            _timeCounter.InitSequencer("Spawn");
        }
    }

    public override void WhenSpawn()
    {
        base.WhenSpawn();

        _timeCounter.InitSequencer("Spawn");
        _launch = false;

        stickbug.Respawn();

        foreach(var item in flySpiders)
        {
            item.Respawn();
        }
    }

    public override void WhenConnect()
    {
        base.WhenConnect();

        _launch = true;

        foreach(var item in flySpiders)
        {
            item.launch = true;
        }
    }

    public override void WhenDisconnect()
    {
        base.WhenDisconnect();
        DestroyMonsterAll();

        _launch = false;
    }

    public void DestroyMonsterAll()
    {
        foreach(var item in _droneList)
        {
            if(item.gameObject.activeInHierarchy)
                item.shield.Hit();
        }

        foreach(var item in flySpiders)
        {
            if(item.gameObject.activeInHierarchy)
                item.Explosion();
        }
    }

    public void Spawn(float t)
    {
        foreach(var item in _droneList)
        {
            if(!item.gameObject.activeInHierarchy)
            {
                item.Respawn();
                item.LaunchUp(10f);
                return;
            }
        }
    }

    public void OutBack()
    {
        Out(_opositeSpawnPosition);
    }
}
