using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounterEx
{
    private Dictionary<string, float> _timerSet = new Dictionary<string, float>();
    private Dictionary<string, float> _timeLimitSet = new Dictionary<string, float>();

    public float InitTimer(string target, float time = 0f, float timeLimit = 1f)
    {
        if(!_timerSet.ContainsKey(target))
        {
            _timerSet.Add(target,time);
            _timeLimitSet.Add(target,timeLimit);
        }
        else
        {
            _timerSet[target] = time;
            _timeLimitSet[target] = timeLimit;
        }

        return time;
    }

    public float IncreaseTimerSelf(string target, float limit, out bool overLimit, float factor)
    {
        if(!_timerSet.ContainsKey(target))
        {
            _timerSet.Add(target,0f);
        }

        var curr = _timerSet[target] += factor;
        overLimit = false;

        if(curr >= limit)
        {
            curr = limit;
            overLimit = true;
        }

        return curr;
    }

    public float IncreaseTimer(string target, out bool overLimit, float timeScale = 1f)
    {
        float limit = _timeLimitSet[target];
        return IncreaseTimer(target,limit,out overLimit,timeScale);
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
