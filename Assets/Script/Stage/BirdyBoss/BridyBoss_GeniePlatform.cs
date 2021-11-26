using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridyBoss_GeniePlatform : MonoBehaviour
{
    public List<Animator> genies;
    public HexCube[] edge;

    public HexCubeGrid grid;

    public float startTime = 3f;
    public float endTime = 3f;

    public float cubeTerm = 0.1f;
    public float cubeWaitTime = 0.1f;
    public float cubeSpeed = 1f;

    public bool _active = false;
    public bool _ground = false;

    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private List<HexCube> _ringList = new List<HexCube>();

    public void Awake()
    {
        _timeCounter.CreateSequencer("main");
        _timeCounter.AddSequence("main", startTime, null, (x)=> {
            for(int i = 0; i < edge.Length; ++i)
            {
                var sound = MessageDataPooling.GetMessageData<MD.SoundPlayData>();
                sound.id = 1536;
                sound.position = edge[i].transform.position;
                sound.dontStop = false;
                sound.returnValue = false;

                var msg = MessagePool.GetMessage();
                msg.Set(MessageTitles.fmod_play, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), sound, null);

                MasterManager.instance.HandleMessage(msg);
            }
            

            _ground = true;
        });
        _timeCounter.AddSequence("main", endTime, null, End);

        _timeCounter.CreateSequencer("grid");
        for(int i = 0; i < grid.mapSize; ++i)
        {
            int target = i;
            _timeCounter.AddSequence("grid", cubeTerm, null, (x) =>
               {
                   
                   for(int j = 0; j < edge.Length; ++j)
                   {
                       _ringList.Clear();
                       grid.GetCubeRing(ref _ringList, edge[j].cubePoint, target);
                       foreach(var item in _ringList)
                       {
                           if (item.IsActive())
                           {
                               item.SetMove(false, 0f, cubeSpeed, 0.1f);
                           }
                       }
                   }
               });
        }

        _timeCounter.AddSequence("grid", 0f, null, (x) => { _ground = false; });
    }

    public void Update()
    {
        if(_active)
        {
            _timeCounter.ProcessSequencer("main", Time.deltaTime);
        }

        if (_ground)
            _timeCounter.ProcessSequencer("grid", Time.deltaTime);
    }

    public void Active()
    {
        if (_active)
            return;

        foreach(var item in genies)
        {
            item.gameObject.SetActive(true);
            item.SetTrigger("Play");
        }

        _timeCounter.InitSequencer("main");
        _timeCounter.InitSequencer("grid");

        _active = true;
        _ground = false;
    }

    public void End(float x)
    {
        foreach (var item in genies)
        {
            item.gameObject.SetActive(false);
        }

        _active = false;
    }
}
