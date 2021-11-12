using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_Database : MonoBehaviour
{
    [System.Serializable]
    public class ItemClass<T> where T : ObjectBase
    {
        public List<T> startCache = new List<T>();
        public GameObject prefab;
        public int id;


        public int updateCount{get{return _allCounter - _freeQueue.Count;}}

        private Queue<CacheItem<T>> _freeQueue = new Queue<CacheItem<T>>();
        private int _allCounter = 0;

        public void CreateStartCache()
        {
            foreach(var item in startCache)
            {
                CacheItem<T> cache = new CacheItem<T>();
                cache.cacheId = id;
                cache.item = item;
                cache.item.whenDeactive += ()=>{ReturnCache(cache);};
                
                if(!item.gameObject.activeSelf)
                    _freeQueue.Enqueue(cache);

                ++_allCounter;
            }
        }

        public CacheItem<T> GetCachedObject(bool deactiveEvent = true)
        {
            CacheItem<T> cache;
            if(_freeQueue.Count == 0)
            {
                cache = new CacheItem<T>();
                cache.item = Instantiate(prefab).GetComponent<T>();
                if(deactiveEvent)
                {
                    cache.item.whenDeactive += ()=>{ReturnCache(cache);};
                }
                cache.cacheId = id;

                _allCounter++;
            }
            else
            {
                cache = _freeQueue.Dequeue();
            }

            cache.item.gameObject.SetActive(true);

            return cache;
        }

        public void ReturnCache(CacheItem<T> cache)
        {
            cache.item.gameObject.SetActive(false);
            _freeQueue.Enqueue(cache);

        }
    }
    public class CacheItem<T>
    {
        public T item;
        public int cacheId;
    }


    public ItemClass<B1_Spider> spiderCache = new ItemClass<B1_Spider>();
    public ItemClass<B1_FlySpider> flySpiderCache = new ItemClass<B1_FlySpider>();
    public ItemClass<CommonDrone> droneCache = new ItemClass<CommonDrone>();
    public ItemClass<MedusaInFallPoint_AI> medusaCache = new ItemClass<MedusaInFallPoint_AI>();
    public ItemClass<BirdyBoss_FlySpiderBall> flyballCache = new ItemClass<BirdyBoss_FlySpiderBall>();
    public ItemClass<GiroPattern> giroCache = new ItemClass<GiroPattern>();
    public ItemClass<FallPillarPattern> fallPillarCache = new ItemClass<FallPillarPattern>();
    public ItemClass<HorizontalPillarPattern> horizonPillarCache = new ItemClass<HorizontalPillarPattern>();
    public ItemClass<SpiderPillarPattern> spiderPillarCache = new ItemClass<SpiderPillarPattern>();

    public int updateCount{
        get
        {
            int medusaCount = 0;
            foreach(var item in _medusaList)
            {
                if(item == null)
                    return -1;
                medusaCount += (item.enabled) ? (!item.gameObject.activeInHierarchy ? 0 : 1) : 0;
            }
            return spiderCache.updateCount + flySpiderCache.updateCount + medusaCount;
        }}

    private List<MedusaInFallPoint_AI> _medusaList = new List<MedusaInFallPoint_AI>();


    public void Awake()
    {
        spiderCache.CreateStartCache();
        flySpiderCache.CreateStartCache();
        droneCache.CreateStartCache();
        medusaCache.CreateStartCache();
        flyballCache.CreateStartCache();
    }

    public B1_Spider SpawnSpider()
    {
        var obj = spiderCache.GetCachedObject();
        obj.item.Respawn();
        obj.item.launch = true;

        return obj.item;
    }
    
    public B1_FlySpider SpawnFlySpider()
    {
        var obj = flySpiderCache.GetCachedObject();
        obj.item.pathFollow = true;
        obj.item.path = "Fly_" + Random.Range(0,8);
        obj.item.Respawn();
        obj.item.launch = true;

        return obj.item;
    }

    public CommonDrone SpawnCommonDrone()
    {
        var obj = droneCache.GetCachedObject();
        obj.item.Respawn();
        obj.item.launch = true;

        return obj.item;
    }

    public MedusaInFallPoint_AI SpawnMedusa()
    {
        var obj = medusaCache.GetCachedObject();
        obj.item.launch = true;//.Launch();

        _medusaList.Add(obj.item);

        return obj.item;
    }

    public BirdyBoss_FlySpiderBall SpawnFlySpiderBall()
    {
        var obj = flyballCache.GetCachedObject();
        obj.item.Respawn();

        return obj.item;
    }

    public GiroPattern SpawnGiroPattern()
    {
        var obj = giroCache.GetCachedObject();
        obj.item.Respawn();

        return obj.item;
    }

    public FallPillarPattern SpawnFallPillarPattern()
    {
        var obj = fallPillarCache.GetCachedObject();
        obj.item.Respawn();

        return obj.item;
    }

    public HorizontalPillarPattern SpawnHorizonPillarPattern()
    {
        var obj = horizonPillarCache.GetCachedObject();
        obj.item.Respawn();

        return obj.item;
    }

    public SpiderPillarPattern SpawnSpiderPillarPattern()
    {
        var obj = spiderPillarCache.GetCachedObject();
        obj.item.Respawn();

        return obj.item;
    }
}
