using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GiroPattern : ObjectBase
{
    public enum State
    {
        Appear, Wait, Launch, WaitStop, Stop
    }

    [SerializeField] private State state = State.Stop;
    [SerializeField] private List<GiroObject> giroObjects = new List<GiroObject>();
    private List<Vector3> _initPosition = new List<Vector3>();
    [SerializeField] private Transform pivot;
    [SerializeField] private float _rotationAccelerationSpeed = 0.1f;

    private PlayerUnit _player;
    private Transform _target;

    private float _rotationSpeed = 0.0f;
    private bool play = false;
    private bool _rotate = false;
    //[SerializeField]private bool _done = true;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private HexCubeGrid _hex;
    private List<HexCube> _hexList = new List<HexCube>();

    private float _launchWaitTime = 0;
    private float _launchTermTime = 0;
    private int _launchCount = 0;
    public int ObjectCount => giroObjects.Count;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer, (x) => {
            _player = (PlayerUnit)x.data;
            _target = _player.transform;
        });

        _initPosition.Capacity = giroObjects.Count;
        for(int i = 0; i < giroObjects.Count; i++)
        {
            _initPosition.Add(giroObjects[i].transform.position);
        }

        //_timeCounter.CreateSequencer("Launch");

        //_timeCounter.AddSequence("Launch", 0.0f, null, (value) =>
        //{
        //    for (int i = 0; i < giroObjects.Count; i++)
        //    {
        //        giroObjects[i].Appear(2f);
        //    }
        //    _rotate = true;
        //});

        //_timeCounter.AddSequence("Launch", 3.0f, null, null);

        //for (int i = 0; i < giroObjects.Count; i++)
        //{
        //    int count = i;
        //    _timeCounter.AddSequence("Launch", 1f, null, (value) =>
        //     {
        //         giroObjects[count].LaunchObject(_target.position, 5000f);
        //     });
        //}

        //_timeCounter.AddSequence("Launch", 3.0f, null, (value) =>
        //{
        //    for (int i = 0; i < giroObjects.Count; i++)
        //    {
        //        giroObjects[i].transform.SetParent(pivot);
        //        giroObjects[i].transform.position = _initPosition[i];
        //    }

        //    _rotate = false;
        //    gameObject.SetActive(false);
        //});

        //_timeCounter.InitSequencer("Launch");
    }

    public void Respawn()
    {
        _rotationSpeed = 0.0f;
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);        
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        //if(Keyboard.current.rKey.wasPressedThisFrame)
        //{
        //    play = true;
        //}

        if(_rotate)
        {
            pivot.Rotate(Vector3.up * _rotationSpeed * deltaTime);
            _rotationSpeed += _rotationAccelerationSpeed * deltaTime;
        }
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
                        giroObjects[_launchCount].LaunchObject(_target.position, 5000f);

                        _launchCount++;
                        if(_launchCount >= giroObjects.Count)
                        {
                            _launchCount = 0;
                            ChangeState(State.WaitStop);
                            return;
                        }

                        _timeCounter.InitTimer("timer", 0f, _launchTermTime);
                    }
                }
                break;
            case State.WaitStop:
                {
                    _timeCounter.IncreaseTimerSelf("timer", out bool limit, deltaTime);
                    if (limit == true)
                    {
                        for (int i = 0; i < giroObjects.Count; i++)
                        {
                            giroObjects[i].transform.SetParent(pivot);
                            giroObjects[i].transform.position = _initPosition[i];
                        }

                        _rotate = false;
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
                    for (int i = 0; i < giroObjects.Count; i++)
                    {
                        giroObjects[i].Appear(2f);
                    }
                    _rotate = true;
                    _timeCounter.InitTimer("timer", 0f, 2f);
                }
                break;
            case State.Wait:
                {
                    _timeCounter.InitTimer("timer", 0f, _launchWaitTime);
                }
                break;
            case State.Launch:
                {
                    _timeCounter.InitTimer("timer", 0f, _launchTermTime);
                }
                break;
            case State.WaitStop:
                {
                    _timeCounter.InitTimer("timer", 0f, 3f);
                }
                break;
            case State.Stop:
                {
                    this.gameObject.SetActive(false);
                }
                break;
        }
    }

    //public void Appear()
    //{
    //    for (int i = 0; i < giroObjects.Count; i++)
    //    {
    //        giroObjects[i].transform.SetParent(pivot);
    //        giroObjects[i].transform.position = _initPosition[i];
    //    }
    //    _rotationSpeed = 0.0f;
    //    _rotate = false;

    //    pivot.gameObject.SetActive(true);

    //    for (int i = 0; i < giroObjects.Count; i++)
    //    {
    //        giroObjects[i].Appear(2f);
    //    }
    //    _rotate = true;
    //    //_done = false;
    //}

    public void Launch(float launchWaitTime, float launchTermTime)
    {
        this._launchWaitTime = launchWaitTime;
        this._launchTermTime = launchTermTime;

        ChangeState(State.Appear);
    }
}
