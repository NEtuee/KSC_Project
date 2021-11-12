using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderPillarPattern : ObjectBase
{
    private TimeCounterEx _timeCounter = new TimeCounterEx();
    public BirdyBoss_Database.ItemClass<B1_FlySpider> flySpiderCache = new BirdyBoss_Database.ItemClass<B1_FlySpider>();
    public List<SpiderPillar> pillars = new List<SpiderPillar>();
    public List<Transform> spawPoints = new List<Transform>();
    public int spawnCount = 3;

    [SerializeField] private float pillarAppearDuration = 2.0f;
    [SerializeField] private float pillarDisapperDuration = 2.0f;

    public override void Assign()
    {
        base.Assign();

        _timeCounter.CreateSequencer("Spider");

        _timeCounter.AddSequence("Spider", 0.0f, null, (value) =>
        {
            for (int i = 0; i < pillars.Count; i++)
            {
                pillars[i].gameObject.SetActive(true);
                pillars[i].Appear(pillarAppearDuration);
            }
        });

        _timeCounter.AddSequence("Spider", pillarAppearDuration, null, null);


        for (int i = 0; i < spawnCount; i++)
        {
            for(int j = 0; j<pillars.Count; j++)
            {
                int pointNum = j;
                _timeCounter.AddSequence("Spider", 2f, null, (value) =>
                {
                    var spider = SpawnFlySpider(pointNum);
                    spider.transform.SetPositionAndRotation(spawPoints[pointNum].position, spawPoints[pointNum].rotation);
                });
            }
        }

        _timeCounter.AddSequence("Spider", 15.0f, null, (value) =>
         {
             for (int i = 0; i < pillars.Count; i++)
             {
                 pillars[i].Disappear(pillarDisapperDuration);
             }
         });

        _timeCounter.AddSequence("Spider", pillarDisapperDuration, null, (value) =>
        {
            for (int i = 0; i < pillars.Count; i++)
            {
                pillars[i].gameObject.SetActive(false);
            }
        });

         _timeCounter.InitSequencer("Spider");
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        _timeCounter.ProcessSequencer("Spider", deltaTime);
    }

    public void Respawn()
    {
        this.gameObject.SetActive(true);
        _timeCounter.InitSequencer("Spider");
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
