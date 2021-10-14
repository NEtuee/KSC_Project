using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounterEx
{
    public class SequenceProcessor
    {
        public delegate bool FenceDelegate();
        public class SequenceItem
        {
            public float time;
            public System.Action<float> process;
            public System.Action<float> triggerd;
        };

        public List<SequenceItem> sequences = new List<SequenceItem>();
        public Dictionary<int, FenceDelegate> fences = new Dictionary<int, FenceDelegate>();
        public float currentTime = 0f;
        public float processTime = 0f;
        public int current = 0;
        public bool isEnd = false;

        public float length{get{return _length;}}

        private float _length;

        public void SkipTo(float skipTime)
        {
            if(currentTime >= skipTime)
            {
                Debug.LogError("not yet");
                return;
            }

            skipTime = skipTime > _length ? _length : skipTime;
            skipTime -= currentTime;
            currentTime += skipTime;
            processTime += skipTime;

            var sq = sequences[current];
            while(processTime >= sq.time)
            {
                processTime -= sq.time;
                currentTime -= sq.time;
                //sq.process?.Invoke(sq.time);
                sq.triggerd?.Invoke(sq.time);

                if(++current >= sequences.Count)
                {
                    isEnd = true;
                    break;
                }
                sq = sequences[current];
            }
        }

        public bool Process(float deltaTime)
        {
            if(isEnd || sequences.Count == 0)
                return true;

            if(fences.ContainsKey(current) && !fences[current]())
                return false;

            currentTime += deltaTime;
            processTime += deltaTime;

            var sq = sequences[current];
            if(sq.time <= processTime)
            {
                sq.process?.Invoke(sq.time);
                sq.triggerd?.Invoke(sq.time);
                processTime = processTime - sq.time;
                if(++current >= sequences.Count)
                {
                    isEnd = true;
                    return true;
                }
            }
            else
            {
                sq.process?.Invoke(processTime);
            }

            return false;
        }

        public void InitSequencer()
        {
            isEnd = false;
            current = 0;
            currentTime = 0f;
            processTime = 0f;
        }

        public void AddSequence(float time, System.Action<float> processEvent = null, System.Action<float> triggerEvent = null)
        {
            SequenceItem sq = new SequenceItem();
            sq.time = time;
            sq.process = processEvent;
            sq.triggerd = triggerEvent;

            _length += time;

            sequences.Add(sq);
        }

        public void AddFence(int point, FenceDelegate del)
        {
            fences.Add(point,del);
        }
    };

    private Dictionary<string, SequenceProcessor> _sequenceSet = new Dictionary<string, SequenceProcessor>();

    private Dictionary<string, float> _timerSet = new Dictionary<string, float>();
    private Dictionary<string, float> _timeLimitSet = new Dictionary<string, float>();

    public static bool _isUpdate;

    public void CreateSequencer(string name)
    {
        if(_sequenceSet.ContainsKey(name))
        {
            Debug.LogError("key already exists");
            return;
        }

        _sequenceSet.Add(name, new SequenceProcessor());
    }

    public void AddFence(string name, SequenceProcessor.FenceDelegate del)
    {
        if(!_sequenceSet.ContainsKey(name))
        {
            Debug.LogError("key Does not exists");
            return;
        }

        _sequenceSet[name].AddFence(_sequenceSet[name].sequences.Count - 1,del);
    }

    public void AddSequence(string name, float limitTime, System.Action<float> processEvent, System.Action<float> triggerEvent)
    {
        if(!_sequenceSet.ContainsKey(name))
        {
            Debug.LogError("key Does not exists");
            return;
        }

        _sequenceSet[name].AddSequence(limitTime, processEvent, triggerEvent);
    }

    public void InitSequencer(string name)
    {
        if(!_sequenceSet.ContainsKey(name))
        {
            Debug.LogError("key Does not exists");
            return;
        }

        _sequenceSet[name].InitSequencer();
    }

    public bool ProcessSequencer(string name, float deltaTime)
    {
        return _sequenceSet[name].Process(deltaTime);
    }

    public void SkipSequencer(string name, float time)
    {
        if(!_sequenceSet.ContainsKey(name))
        {
            Debug.LogError("key Does not exists");
            return;
        }

        _sequenceSet[name].SkipTo(time);
    }

    public SequenceProcessor GetSequencer(string name)
    {
        if(!_sequenceSet.ContainsKey(name))
        {
            Debug.LogError("key Does not exists");
            return null;
        }

        return _sequenceSet[name];
    }

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

    public float IncreaseTimerSelf(string target, out bool overLimit, float factor)
    {
        float limit = _timeLimitSet[target];
        return IncreaseTimerSelf(target,limit,out overLimit,factor);
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

        var curr = _timerSet[target] += timeScale * GetDeltaTime();
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

        var curr = _timerSet[target] -= GetDeltaTime();
        overLimit = false;
        
        if(curr <= limit)
        {
            curr = limit;
            overLimit = true;
        }

        return curr;
    }

    public float GetCurrentTime(string target)
    {
        return _timerSet[target];
    }

    public float GetTimeLimit(string target)
    {
        if(_timeLimitSet.ContainsKey(target))
        {
            return _timeLimitSet[target];
        }
        else
        {
            return -1f;
        }
    }

    public float GetDeltaTime()
    {
        return _isUpdate ? Time.deltaTime : Time.fixedDeltaTime;
    }
}
