using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieState_DroneWave : GenieStateBase
{
    [System.Serializable]
    public struct WavePatternEvent
    {
        public enum Direction
        {
            Left,
            Right,
        };

        public enum Height
        {
            Up,
            Down,
        };

        public enum DroneType
        {
            HitDrone,
            CoreDrone,
        };

        public AnimationCurve heightGraph;

        public Direction direction;
        public Height height;
        public DroneType type;

        public float invokeTime;
    };
    

    public override string stateIdentifier => "Wave";

    public List<WavePatternEvent> patternEvents = new List<WavePatternEvent>();

    [Header("GroundHit")]
    public float beforeGroundHitTime = 1f;
    public float groundHitStartTime = 3f;
    public float groundHitAngle = 30f;
    public float groundHitTime = 3f;

    [Header("Pattern")]
    public float patternStartTime = 4f;

    [Space(10f)]
    public float droneSpinTime = 0.3f;
    public float droneApearTime = 3f;
    [Space(10f)]
    public float droneStartWaitTime = 0.5f;
    public float droneEndWaitTime = 0.5f;
    [Space(10f)]
    public float droneStartHeight = -5f;
    public float droneUpHeight = 7f;
    public float droneDownHeight = 4f;


    private Genie_DroneLinePool _droneLinePool;
    private List<HexCube> _groundList = new List<HexCube>();
    private bool _patternStart = false;
    private float _deltaTime;

    private int _currentPattern = 0;

    public override void Assign()
    {
        base.Assign();

        _droneLinePool = GetComponent<Genie_DroneLinePool>();
        _droneLinePool.AddCreateDelegate(target, droneSpinTime,droneApearTime,droneStartWaitTime,droneEndWaitTime);

        _timeCounter.CreateSequencer("Start");
        _timeCounter.AddSequence("Start",beforeGroundHitTime,LookTarget,(x)=>{target.ChangeAnimation(10);target.CreateEyeLight();});
        _timeCounter.AddSequence("Start",groundHitStartTime,BeforeGroundCut,GroundDisapear);
        _timeCounter.AddSequence("Start",patternStartTime,null,null);
        
        _timeCounter.CreateSequencer("Pattern");
        for(int i = 0; i < patternEvents.Count; ++i)
        {
            _timeCounter.AddSequence("Pattern",patternEvents[i].invokeTime,null,ProcessPattern);
        }

    }

    public override void StateInitialize(StateBase prevState)
    {
        base.StateInitialize(prevState);
        _droneLinePool.Init(3);
        _currentPattern = 0;
        _patternStart = false;

        _timeCounter.InitSequencer("Start");
        _timeCounter.InitSequencer("Pattern");
    }

    public override void StateProgress(float deltaTime)
    {
        base.StateProgress(deltaTime);
        _deltaTime = deltaTime;

        if(!_patternStart)
        {
            _patternStart = _timeCounter.ProcessSequencer("Start",deltaTime);
        }
        else
        {
            if(_timeCounter.ProcessSequencer("Pattern",deltaTime))
            {
                _timeCounter.InitSequencer("Pattern");
            }
        }
    }

    public override void StateChanged(StateBase targetState)
    {
        base.StateChanged(targetState);
        foreach(var item in _groundList)
        {
            item.SetMove(true,Random.Range(0f,0.2f),1f);
        }

        foreach(var item in _droneLinePool.GetActiveObjects())
        {
            item.gameObject.SetActive(false);
        }
    }

    public void ProcessPattern(float t)
    {
        var info = patternEvents[_currentPattern];
        var height = info.height == WavePatternEvent.Height.Up ? droneUpHeight : droneDownHeight;
        var startDir = target.body.rotation * Quaternion.Euler(0f,groundHitAngle * (info.direction == WavePatternEvent.Direction.Left ? 1f : -1f),0f);
        var endDir = target.body.rotation * Quaternion.Euler(0f,groundHitAngle * (info.direction == WavePatternEvent.Direction.Left ? -1f : 1f),0f);
        var type = info.type == WavePatternEvent.DroneType.CoreDrone;

        var droneLine = _droneLinePool.Active(transform.position,Quaternion.identity);
        droneLine.Active(info.heightGraph,transform.position,startDir,endDir,droneStartHeight,height,type);

        _currentPattern = _currentPattern + 1 >= patternEvents.Count ? 0 : _currentPattern + 1;

        if(info.direction == WavePatternEvent.Direction.Left)
        {
            target.ChangeAnimation(7);
        }
        else
        {
            target.ChangeAnimation(8);
        }
    }

    public void BeforeGroundCut(float t)
    {
        LookTarget(target.body,target.targetTransform.position,_deltaTime);

        var dir = target.targetTransform.position - target.body.position;
        dir.y = 0f;
        var inverseDir = dir;
        dir = Quaternion.Euler(0f,groundHitAngle,0f) * dir.normalized;
        inverseDir = Quaternion.Euler(0f,-groundHitAngle,0f) * inverseDir.normalized;

        foreach(var item in _groundList)
        {
            item.GetRenderer().material = target.gridControll.prev;
        }

        _groundList.Clear();
        GetGridLine(ref _groundList,dir,6);
        GetGridLine(ref _groundList,inverseDir,6);

        foreach(var item in _groundList)
        {
            item.GetRenderer().material = target.gridControll.curr;
        }
    }

    public void GroundDisapear(float processTime)
    {
        foreach(var item in _groundList)
        {
            item.SetMove(false,Random.Range(0f,0.2f),1f);
            item.GetRenderer().material = target.gridControll.prev;
        }
    }

    public void LookTarget(float t)
    {
        LookTarget(target.body,target.targetTransform.position,_deltaTime);
    }

}
