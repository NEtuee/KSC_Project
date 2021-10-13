using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class B1_LoopBackground : MonoBehaviour
{
    [System.Serializable]
    public class LoopBackgroundItem
    {
        public GameObject prefab;
        public float length;
    }

    [System.Serializable]
    public class ObstacleItem
    {
        public GameObject prefab;
        public int id;

        private Queue<ObstacleCacheItme> _freeQueue = new Queue<ObstacleCacheItme>();

        public ObstacleCacheItme GetCachedObject()
        {
            ObstacleCacheItme cache;
            if(_freeQueue.Count == 0)
            {
                cache = new ObstacleCacheItme();
                cache.obj = Instantiate(prefab);
                cache.cacheId = id;
            }
            else
            {
                cache = _freeQueue.Dequeue();
            }

            cache.obj.SetActive(true);

            return cache;
        }
        
        public void ReturnCache(ObstacleCacheItme cache)
        {
            cache.obj.SetActive(false);
            _freeQueue.Enqueue(cache);
        }
    }

    public class ObstacleCacheItme
    {
        public GameObject obj;
        public int cacheId;
    }

    public class LoopItem
    {
        public Transform transform;
        public float length;

        public List<ObstacleCacheItme> obstacles = new List<ObstacleCacheItme>();
    }

    [System.Serializable]
    public class PlatformItem
    {
        public GameObject prefab;
        public int id;

        private Queue<PlatformCacheItme> _freeQueue = new Queue<PlatformCacheItme>();

        public PlatformCacheItme GetCachedObject()
        {
            PlatformCacheItme cache;
            if(_freeQueue.Count == 0)
            {
                cache = new PlatformCacheItme();
                cache.platform = Instantiate(prefab).GetComponent<B1_Platform>();
                cache.platform.whenDeactive += ()=>{ReturnCache(cache);};
                cache.cacheId = id;
            }
            else
            {
                cache = _freeQueue.Dequeue();
            }

            cache.platform.gameObject.SetActive(true);

            return cache;
        }
        
        public void ReturnCache(PlatformCacheItme cache)
        {
            cache.platform.gameObject.SetActive(false);
            _freeQueue.Enqueue(cache);
        }
    }

    public class PlatformCacheItme
    {
        public B1_Platform platform;
        public int cacheId;
    }






    public float maxLength = 150f;
    public float scrollSpeed = 10f;

    public List<LoopBackgroundItem> backgroundItems = new List<LoopBackgroundItem>();
    public List<ObstacleItem> obstacleItmes = new List<ObstacleItem>();
    public List<PlatformItem> platformItmes = new List<PlatformItem>();

    public List<int> backgroundPattern = new List<int>();

    private List<LoopItem> _backgrounds = new List<LoopItem>();

    private LoopItem _front;
    private float _backgroundLength;

    public void Start()
    {
        CreateBackground();
    }

    public void Update()
    {
        Scroll(Time.deltaTime);
    }

    public void SpawnPlatformFront(int id, int direction, B1_Platform target)
    {
        SpawnPlatform(id,direction,_front.transform.position,target);
    }

    public void SpawnPlatform(int id, int direction, Vector3 position, B1_Platform target)
    {
        var cache = FindPlatformItem(id);
        var platform = cache.GetCachedObject();

        platform.platform.transform.position = position + target.GetConnection(direction).GetPosition(direction);
        platform.platform.ApproachToTarget(direction,target);
    }

    public void SpawnObstacle(int id)
    {
        var cache = FindObstacleItem(id);
        var obstacle = cache.GetCachedObject();

        obstacle.obj.transform.SetParent(_front.transform);
        obstacle.obj.transform.localPosition = Vector3.zero;

        _front.obstacles.Add(obstacle);
    }

    public void ReturnObstacles(ref List<ObstacleCacheItme> list)
    {
        foreach(var item in list)
        {
            item.obj.transform.SetParent(transform);
            var cache = FindObstacleItem(item.cacheId);
            cache.ReturnCache(item);
        }

        list.Clear();
    }

    public PlatformItem FindPlatformItem(int id)
    {
        return platformItmes.Find((x)=>{return x.id == id;});
    }

    public ObstacleItem FindObstacleItem(int id)
    {
        return obstacleItmes.Find((x)=>{return x.id == id;});
    }

    public void Scroll(float deltaTime)
    {
        foreach(var item in _backgrounds)
        {
            item.transform.position += -Vector3.forward * scrollSpeed * deltaTime;
        }

        foreach(var item in _backgrounds)
        {
            if(item.transform.position.z < -(maxLength * 0.5f))
            {
                item.transform.position = _front.transform.position + Vector3.forward * (item.length * 0.5f + _front.length * 0.5f);
                _front = item;

                ReturnObstacles(ref _front.obstacles);
            }
        }
    }

    public void CreateBackground()
    {
        float len = 0f;
        while(len < maxLength)
        {
            for(int i = 0; i < backgroundPattern.Count; ++i)
            {
                var target = backgroundPattern[i];

                var wall = Instantiate(backgroundItems[target].prefab,Vector3.zero,Quaternion.identity).transform;
                wall.SetParent(transform);

                var item = new LoopItem();
                item.transform = wall;
                item.length = backgroundItems[target].length;

                _backgrounds.Add(item);

                len += item.length;
            }
        }

        _backgroundLength = len;
        SortBackground();

        _front = _backgrounds[0];
    }

    public void SortBackground()
    {
        float currLen = _backgroundLength * 0.5f;
        for(int i = 0; i < _backgrounds.Count; ++i)
        {
            currLen -= _backgrounds[i].length * 0.5f;
            if(i > 0)
            {
                currLen -= _backgrounds[i-1].length * 0.5f;
            }

            var pos = _backgrounds[i].transform.position;
            pos.z = currLen;
            
            _backgrounds[i].transform.position = pos;
        }
    }

}
