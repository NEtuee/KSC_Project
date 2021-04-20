using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GraphAnimator : MonoBehaviour
{
    [System.Serializable]
    public class GraphAnimation
    {
        public struct TargetInfo
        {
            public Transform target;
            public Vector3 origin;
        }
        public enum AnimationType
        {
            Translate,
            Rotation,
            AxisRotation,
        };

        public string name;
        public bool loop;
        public float speed;
        public AnimationType animationType;
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;
        public AnimationCurve zCurve;

        private List<TargetInfo> _targets = new List<TargetInfo>();

        public void ApplyCurve(float time)
        {
            var x = xCurve.Evaluate(time);
            var y = yCurve.Evaluate(time);
            var z = zCurve.Evaluate(time);

            foreach (var t in _targets)
            {
                var target = t.origin;

                if(animationType == AnimationType.Translate)
                {
                    target.x += x;
                    target.y += y;
                    target.z += z;
                    
                    t.target.localPosition = target;
                }
                else if(animationType == AnimationType.Rotation)
                {
                    target.x += x;
                    target.y += y;
                    target.z += z;
                    
                    t.target.localEulerAngles = target;
                }
                else if (animationType == AnimationType.AxisRotation)
                {
                    var rotation = Quaternion.Euler(target);
                    rotation *= Quaternion.AngleAxis(x, Vector3.right);
                    rotation *= Quaternion.AngleAxis(y, Vector3.up);
                    rotation *= Quaternion.AngleAxis(z, Vector3.forward);

                    t.target.localRotation = rotation;
                }
            }

        }

        public bool TargetExistsCheck()
        {
            if (_targets.Count == 0)
                return false;

            foreach (var target in _targets)
            {
                if (target.target == null)
                {
                    _targets.Clear();
                    return false;
                }
                    
            }

            return true;
        }

        public void ReturnOrigin()
        {
            foreach (var target in _targets)
            {
                if (target.target != null)
                {
                    if(animationType == AnimationType.Translate)
                    {
                        target.target.localPosition = target.origin;
                    }
                    else if(animationType == AnimationType.Rotation || animationType == AnimationType.AxisRotation)
                    {
                        target.target.localEulerAngles = target.origin;
                    }
                }
                    
            }
            
            _targets.Clear();
        }

        public void AddTargetList(Transform t)
        {
            TargetInfo info = new TargetInfo();
            info.target = t;
            
            if(animationType == AnimationType.Translate)
            {
                info.origin = info.target.localPosition;
            }
            else if(animationType == AnimationType.Rotation || animationType == AnimationType.AxisRotation)
            {
                info.origin = info.target.localEulerAngles;
            }
            
            _targets.Add(info);
        }

        public void Set(Transform t) 
        {
            ReturnOrigin();

            AddTargetList(t);
        }

        public void Set(List<Transform> t)
        {
            ReturnOrigin();

            foreach (var tp in t)
            {
                AddTargetList(tp);
            }
            
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
    
    public void Play(string animation, List<Transform> targets, bool forcePlay = false)
    {
        var ani = FindAnimation(animation);
        if(ani == null)
        {
            Debug.Log("Animation does not exists");
            return;
        }
        
        _timeCounter.InitTimer(animation);

        ani.Set(targets);
        var play = IsPlaying(animation);
        if(play == null)
        {
            _playList.Add(ani);
        }
        else if(forcePlay)
        {
            play.ReturnOrigin();
            
        }
    }

    public void Play(string animation, Transform target, bool forcePlay = false)
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
        else
        {
            
        }
    }

    public void Update()
    {
        for(int i = 0; i < _playList.Count;)
        {
            int curr = i;
            var time = _timeCounter.IncreaseTimer(_playList[i].name,out bool limit, _playList[i].speed);
            bool remove = false;
            if(limit)
            {
                if(_playList[i].loop)
                {
                    time = _timeCounter.InitTimer(_playList[i].name,time - 1f);
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
