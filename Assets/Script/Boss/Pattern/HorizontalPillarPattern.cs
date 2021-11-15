using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPillarPattern : ObjectBase
{
    public enum State
    {
        Appear, Wait, Launch, WaitStop, Stop
    }

    [SerializeField] private State state = State.Stop;
    [SerializeField] private float pillarForce = 5000.0f;
    private HorizontalObjectPool _pillarObjectPool;
    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private List<Vector3> _pillarStartPoint = new List<Vector3>();
    private List<HorizontalPillar> _horizonPillars = new List<HorizontalPillar>();

    public const int HORIZONTAL_PILLAR_PATTERN_POINT_COUNT = 6;
    private int _currentLaunchCount = 0;

    private float _launchWaitTime = 0;
    private float _launchTerm = 0;
    private float _stopWaitTime = 10f;

    private PlayerUnit _player;
    private Transform _target;
    public override void Assign()
    {
        base.Assign();

        //AddAction(MessageTitles.set_setplayer, (x) => {
        //    _player = (PlayerUnit)x.data;
        //    _target = _player.transform;
        //});

        _pillarObjectPool = GetComponent<HorizontalObjectPool>();

        _pillarStartPoint.Capacity = 6;
        _horizonPillars.Capacity = 6;

        //_timeCounter.CreateSequencer("Horizon");

        //_timeCounter.AddSequence("Horizon",0.0f,null,(value)=>
        //{
        //    for(int i = 0; i < HORIZONTAL_PILLAR_PATTERN_POINT_COUNT; i++)
        //    {
        //        _horizonPillars.Add(_pillarObjectPool.Active(_pillarStartPoint[i], Quaternion.identity));
        //        _horizonPillars[i].SetLookAtTarget(_target);
        //    }
        //});

        //_timeCounter.AddSequence("Horizon", 5.0f, null, null);

        //for (int i = 0; i < HORIZONTAL_PILLAR_PATTERN_POINT_COUNT; i++)
        //{
        //    int count = i;
        //    _timeCounter.AddSequence("Horizon", 2.0f, null, (value) =>
        //    {
        //        _horizonPillars[count].Launch(_target.position, pillarForce);
        //    });
        //}

        //_timeCounter.AddSequence("Horizon", 10.0f, null, (value)=>
        //{
        //    gameObject.SetActive(false);
        //});

        //_timeCounter.InitSequencer("Horizon");
    }
    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));
        //SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);
        UpdatePattern(deltaTime);
    }

    private void UpdatePattern(float deltaTime)
    {
        switch (state)
        {
            case State.Appear:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if (limit == true)
                    {
                        ChangeState(State.Wait);
                    }
                }
                break;
            case State.Wait:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if (limit == true)
                    {
                        ChangeState(State.Launch);
                    }
                }
                break;
            case State.Launch:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if (limit == true)
                    {
                        _horizonPillars[_currentLaunchCount].Launch(_target.position, pillarForce);
                        _currentLaunchCount++;

                        if(_currentLaunchCount >= HORIZONTAL_PILLAR_PATTERN_POINT_COUNT)
                        {
                            _currentLaunchCount = 0;
                            ChangeState(State.WaitStop);
                            return;
                        }

                        _timeCounter.InitTimer("timer", 0f, _launchTerm);
                    }
                }
                break;
            case State.WaitStop:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if (limit == true)
                    {
                        ChangeState(State.Stop);
                    }
                }
                break;
            case State.Stop:
                break;
        }
    }

    private void ChangeState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.Appear:
                {
                    for (int i = 0; i < HORIZONTAL_PILLAR_PATTERN_POINT_COUNT; i++)
                    {
                        _horizonPillars.Add(_pillarObjectPool.Active(_pillarStartPoint[i], Quaternion.identity));
                        _horizonPillars[i].SetLookAtTarget(_target);
                    }
                    _timeCounter.InitTimer("timer", 0f, 4f);
                }
                break;
            case State.Wait:
                {
                    _timeCounter.InitTimer("timer", 0f, _launchWaitTime);
                }
                break;
            case State.Launch:
                {
                    _timeCounter.InitTimer("timer", 0f, _launchTerm);
                }
                break;
            case State.WaitStop:
                {
                    _timeCounter.InitTimer("timer", 0f, 10f);
                }
                break;
            case State.Stop:
                {
                    this.gameObject.SetActive(false);
                }
                break;
        }
    }

    public void Respawn()
    {
    }

    public void Launch(ref List<Transform> points,Transform target ,float launchWaitTime, float launchTermTime, float launchForce)
    {
        _pillarStartPoint.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            _pillarStartPoint.Add(points[i].position);
        }

        _target = target;

        this._launchWaitTime = launchWaitTime;
        this._launchTerm = launchTermTime;
        this.pillarForce = launchForce;

        ChangeState(State.Appear);
    }
}
