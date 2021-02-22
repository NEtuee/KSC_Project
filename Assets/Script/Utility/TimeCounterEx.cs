using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounterEx
{
    private Dictionary<string, float> _timerSet = new Dictionary<string, float>();

    public void InitTimer(string target, float time = 0f)
    {
        if(!_timerSet.ContainsKey(target))
        {
            _timerSet.Add(target,time);
        }
        else
        {
            _timerSet[target] = time;
        }
    }

    public float IncreaseTimer(string target, float limit, out bool overLimit, float timeScale = 1f)
    {
        if(!_timerSet.ContainsKey(target))
        {
            _timerSet.Add(target,0f);
        }

        var curr = _timerSet[target] += timeScale * Time.deltaTime;
        overLimit = false;

        if(curr >= limit)
        {
            curr = limit;
            overLimit = true;
        }

        return curr;
    }

    public float DecreaseTimer(string target, float limit, out bool overLimit)
    {
        if(!_timerSet.ContainsKey(target))
        {
            _timerSet.Add(target,0f);
        }

        var curr = _timerSet[target] -= Time.deltaTime;
        overLimit = false;
        
        if(curr <= limit)
        {
            curr = limit;
            overLimit = true;
        }

        return curr;
    }
}
