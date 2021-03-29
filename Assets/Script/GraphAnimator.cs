using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphAnimator : MonoBehaviour
{
    [System.Serializable]
    public class GraphAnimation
    {
        public string name;
        public bool loop;
        public float speed;
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;
        public AnimationCurve zCurve;

        private Transform _target;
        private Vector3 _origin;

        public void ApplyCurve(float time)
        {
            var pos = _origin;
            pos.x += xCurve.Evaluate(time);
            pos.y += yCurve.Evaluate(time);
            pos.z += zCurve.Evaluate(time);

            _target.localPosition = pos;
        }

        public bool TargetExistsCheck()
        {
            return _target != null;
        }

        public void ReturnOrigin()
        {
            if(_target != null)
                _target.localPosition = _origin;
        }

        public void Set(Transform t) 
        {
            if(_target != null)
            {
                _target.localPosition = _origin;
            }

            _target = t;
            _origin = _target.localPosition;
        }
    }

    public List<GraphAnimation> animations = new List<GraphAnimation>();

    private List<GraphAnimation> _playList = new List<GraphAnimation>();
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    public GraphAnimation FindAnimation(string n)
    {
        return animations.Find((x)=>{return x.name == n;});
    }

    public GraphAnimation IsPlaying(string n)
    {
        return _playList.Find((x)=>{return x.name == n;});
    }

    public void Stop()
    {
        foreach(var ani in _playList)
        {
            ani.ReturnOrigin();
        }
        _playList.Clear();
    }

    public void Play(string animation, Transform target)
    {
        var ani = FindAnimation(animation);
        if(ani == null)
        {
            Debug.Log("Animation does not exists");
            return;
        }
        
        _timeCounter.InitTimer(animation);

        ani.Set(target);
        if(IsPlaying(animation) == null)
        {
            _playList.Add(ani);
        }
    }

    public void Update()
    {
        for(int i = 0; i < _playList.Count;)
        {
            int curr = i;
            var time = _timeCounter.IncreaseTimer(_playList[i].name,1f,out bool limit, _playList[i].speed);
            bool remove = false;
            if(limit)
            {
                if(_playList[i].loop)
                {
                    time = _timeCounter.InitTimer(_playList[i].name);
                    ++curr;
                }
                else if(!_playList[i].TargetExistsCheck())
                {
                    remove = true;
                }
                else
                {
                    remove = true;
                }
            }
            else
            {
                ++curr;
            }

            if(remove)
            {
                _playList.RemoveAt(i);
            }
            else
            {
                _playList[i].ApplyCurve(time);
            }
            
            i = curr;
        }

    }
}
