using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderPillarPattern : ObjectBase
{
    public enum State
    {
        Appear, Spawn, waitDisappear, Disappear, Stop
    }

    [SerializeField] private State state = State.Stop;
    private TimeCounterEx _timeCounter = new TimeCounterEx();
    public BirdyBoss_Database.ItemClass<B1_FlySpider> flySpiderCache = new BirdyBoss_Database.ItemClass<B1_FlySpider>();
    public List<SpiderPillar> pillars = new List<SpiderPillar>();
    public List<Transform> spawPoints = new List<Transform>();
    public int spawnCount = 3;
    public float spawnDuration = 6f;
    private float _spawnTerm;
    private int currentSpawnPillar = 0;
    private int currentSpawnCount = 0;
    private float _disappearWaitTime = 0;

    [SerializeField] private float pillarAppearDuration = 2.0f;
    [SerializeField] private float pillarDisapperDuration = 2.0f;

    public override void Assign()
    {
        base.Assign();

        //_timeCounter.CreateSequencer("Spider");

        //_timeCounter.AddSequence("Spider", 0.0f, null, (value) =>
        //{
        //    for (int i = 0; i < pillars.Count; i++)
        //    {
        //        pillars[i].gameObject.SetActive(true);
        //        pillars[i].Appear(pillarAppearDuration);
        //    }
        //});

        //_timeCounter.AddSequence("Spider", pillarAppearDuration, null, null);


        //for (int i = 0; i < spawnCount; i++)
        //{
        //    for(int j = 0; j<pillars.Count; j++)
        //    {
        //        int pointNum = j;
        //        _timeCounter.AddSequence("Spider", 2f, null, (value) =>
        //        {
        //            var spider = SpawnFlySpider(pointNum);
        //            spider.transform.SetPositionAndRotation(spawPoints[pointNum].position, spawPoints[pointNum].rotation);
        //        });
        //    }
        //}

        //_timeCounter.AddSequence("Spider", 15.0f, null, (value) =>
        // {
        //     for (int i = 0; i < pillars.Count; i++)
        //     {
        //         pillars[i].Disappear(pillarDisapperDuration);
        //     }
        // });

        //_timeCounter.AddSequence("Spider", pillarDisapperDuration, null, (value) =>
        //{
        //    for (int i = 0; i < pillars.Count; i++)
        //    {
        //        pillars[i].gameObject.SetActive(false);
        //    }
        //});

        // _timeCounter.InitSequencer("Spider");
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        //_timeCounter.ProcessSequencer("Spider", deltaTime);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        UpdatePattern(deltaTime);
    }

    public void Launch(int spawnCount, float duration, float waitTime)
    {
        this.spawnCount = spawnCount;
        this.spawnDuration = duration;
        this._disappearWaitTime = waitTime;
        _spawnTerm = (duration / (float)spawnCount);

        ChangeState(State.Appear);
    }

    private void UpdatePattern(float deltaTime)
    {
        switch(state)
        {
            case State.Appear:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if (limit == true)
                    {
                        ChangeState(State.Spawn);
                    }
                }
                break;
            case State.Spawn:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if(limit == true)
                    {
                        var spider = SpawnFlySpider(currentSpawnPillar);
                        spider.transform.SetPositionAndRotation(spawPoints[currentSpawnPillar].position, spawPoints[currentSpawnPillar].rotation);
                        currentSpawnPillar++;
                        currentSpawnCount++;

                        if(currentSpawnCount >= spawnCount)
                        {
                            currentSpawnCount = 0;
                            ChangeState(State.waitDisappear);
                            return;
                        }

                        if (currentSpawnPillar >= spawPoints.Count)
                            currentSpawnPillar = 0;

                        _timeCounter.InitTimer("timer", 0f, _spawnTerm);
                    }
                }
                break;
            case State.waitDisappear:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if (limit == true)
                    {
                        ChangeState(State.Disappear);
                    }
                }
                break;
            case State.Disappear:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if (limit == true)
                    {
                        ChangeState(State.Stop);
                    }
                }
                break;
            case State.Stop:
                break;
        }
    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.Appear:
                {
                    for (int i = 0; i < pillars.Count; i++)
                    {
                        pillars[i].gameObject.SetActive(true);
                        pillars[i].Appear(pillarAppearDuration);
                    }
                    _timeCounter.InitTimer("timer", 0f, pillarAppearDuration);
                }
                break;
            case State.Spawn:
                {
                    _timeCounter.InitTimer("timer", 0f, _spawnTerm);
                }
                break;
            case State.waitDisappear:
                {
                    _timeCounter.InitTimer("timer", 0f, _disappearWaitTime);
                }
                break;
            case State.Disappear:
                {
                    _timeCounter.InitTimer("timer", 0f, pillarDisapperDuration);
                    for (int i = 0; i < pillars.Count; i++)
                    {
                        pillars[i].Disappear(pillarDisapperDuration);
                    }
                }
                break;
            case State.Stop:
                {
                    for (int i = 0; i < pillars.Count; i++)
                    {
                        pillars[i].gameObject.SetActive(false);
                    }
                    this.gameObject.SetActive(false);
                }
                break;
        }
        this.state = state;
    }

    public void Respawn()
    {
        this.gameObject.SetActive(true);
        //_timeCounter.InitSequencer("Spider");
    }

    public B1_FlySpider SpawnFlySpider(int num)
    {
        var obj = flySpiderCache.GetCachedObject();
        obj.item.pathFollow = true;
        obj.item.path = "Spider_"+num;
        obj.item.Respawn();
        obj.item.launch = true;

        return obj.item;
    }
}
