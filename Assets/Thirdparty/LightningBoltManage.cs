using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;
using System;

public class LightningBoltManage : MonoBehaviour
{
    public GameObject targetPrefab;
    public Transform startTarget;
    public float endTime = 1f;

    private SimpleCache<LightningBoltScript> _cache;
    private Action<int, LightningBoltScript> _progressAction;
    private Dictionary<LightningBoltScript, float> _timeCounter;
    public void Start()
    {
        _timeCounter = new Dictionary<LightningBoltScript, float> ();
        _cache = new SimpleCache<LightningBoltScript>(targetPrefab,(x)=>{});
        _progressAction = (i, x)=>{
            _timeCounter[x] -= Time.deltaTime;
            if(_timeCounter[x] <= 0f)
            {
                x.gameObject.SetActive(false);
            }
        };
    }

    public void Update()
    {
        _cache.Loop(_progressAction);
    }

    public void Active(Transform end)
    {
        Active(startTarget,end,endTime);
    }

    public void Active(Transform start, Transform end, float time = 0.1f)
    {
        var item = _cache.ActiveObject(out var count);
        if(!_timeCounter.ContainsKey(item))
        {
            _timeCounter.Add(item,time);
        }
        else
        {
            _timeCounter[item] = time;
        }

        item.StartObject = start.gameObject;
        item.EndObject = end.gameObject;

        item.gameObject.SetActive(true);
    }
}
