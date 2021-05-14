using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolBase<T> : MonoBehaviour
{
    public delegate void ActiveDelegate(T t,Vector3 pos, Quaternion rot);
    public delegate void SettingDelegate(T t);
    public delegate bool ConditionDelegate(T t);
    
    public GameObject baseObject;

    private readonly Queue<T> _cache = new Queue<T>();
    private List<T> _progressList = new List<T>();

    protected ActiveDelegate _activeDelegate = (t,p,r) => { };
    protected SettingDelegate _createDelegate = t => { };
    protected SettingDelegate _deleteProgressDelegate = t => { };
    protected SettingDelegate _progressDelegate = t => { };
    protected ConditionDelegate _deleteCondition = t => { return false;};

    protected virtual void Update()
    {
        for(int i = 0; i < _progressList.Count;)
        {
            _progressDelegate(_progressList[i]);
            
            if(_deleteCondition(_progressList[i]))
            {
                _deleteProgressDelegate(_progressList[i]);
                
                _cache.Enqueue(_progressList[i]);
                _progressList.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }

    public T Active(Vector3 position, Quaternion rotation)
    {
        var t = GetCachedItem();
        _activeDelegate(t,position,rotation);

        _progressList.Add(t);

        return t;
    }

    public void Init(int count = 1)
    {
        CreateCacheItems(count);
    }

    private T GetCachedItem()
    {
        if(_cache.Count == 0)
            CreateCacheItems(1);
        
        return _cache.Dequeue();
    }

    private void CreateCacheItems(int count)
    {
        for(int i = 0; i < count; ++i)
        {
            var obj = Instantiate(baseObject,Vector3.zero,Quaternion.identity);
            var t = obj.GetComponent<T>();

            obj.SetActive(false);
            obj.transform.SetParent(this.transform);

            _createDelegate(t);

            _cache.Enqueue(t);
        }
    }
}
