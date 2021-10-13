using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_Platform : ObjectBase
{
    public enum PlatformState
    {
        UP,
        DOWN
    };

    public System.Action whenDeactive = ()=>{};

    public PlatformState state;
    public Transform platform;

    public AnimationCurve updownCurve;
    public AnimationCurve approachCurve;
    public AnimationCurve outCurve;

    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject frontWall;
    public GameObject backWall;

    public bool canAffectDrone = false;
    public float updownTime;

    public float upPosition;
    public float downPosition;

    public float approachTime;
    public float outTime;

    public float outTimeWhenConnected = 0f;


    private bool _approach = false;
    private int _approachDirection = 0;
    private Vector3 _approachPosition = Vector3.zero;
    private Vector3 _startPosition = Vector3.zero;
    private B1_Platform _approachTarget;

    protected bool _out = false;
    private Vector3 _outPosition = Vector3.zero;
    protected Vector3 _opositeSpawnPosition;

    public B1_Platform _leftConnect;
    public B1_Platform _rightConnect;
    public B1_Platform _frontConnect;
    public B1_Platform _backConnect;

    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private bool _updownProcessing = false;

    private float _outTimeWhenConnected = 0f;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.stage_droneSpecial,(msg)=>{
            if(canAffectDrone && !_updownProcessing)
            {
                state = state == PlatformState.UP ? PlatformState.DOWN : PlatformState.UP;
                _updownProcessing = true;
                _timeCounter.InitSequencer("Updown");
            }
        });

        _timeCounter.CreateSequencer("Updown");
        _timeCounter.AddSequence("Updown",updownTime,UpdownProcess,null);

        _timeCounter.CreateSequencer("Approach");
        _timeCounter.AddSequence("Approach",0.1f,null,(x)=>{ApproachInitialize();});
        _timeCounter.AddSequence("Approach",approachTime,ApproachProcess,ApproachConnect);

        _timeCounter.CreateSequencer("Out");
        _timeCounter.AddSequence("Out",0f,null,OutDisconnect);
        _timeCounter.AddSequence("Out",outTime,OutProcess,OutEnd);
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));

        SetupSide();
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(_updownProcessing)
        {
            _updownProcessing = !_timeCounter.ProcessSequencer("Updown",deltaTime);
        }

        if(_approach)
        {
            _approach = !_timeCounter.ProcessSequencer("Approach",deltaTime);
        }
        
        if(_out)
        {
            _out = !_timeCounter.ProcessSequencer("Out",deltaTime);
        }

        if(_outTimeWhenConnected != 0f)
        {
            _outTimeWhenConnected -= deltaTime;
            if(_outTimeWhenConnected <= 0f)
            {
                _outTimeWhenConnected = 0f;
                Out(_opositeSpawnPosition);
            }
        }
    }

    public virtual void WhenConnect()
    {

    }

    public virtual void WhenDisconnect()
    {

    }

    public virtual void WhenSpawn()
    {

    }

    public virtual void ApproachInitialize()
    {

    }

    public void UpdownProcess(float t)
    {
        var curr = GetCurrentSide();
        var oposite = GetOpositeSide();

        var pos = platform.localPosition;
        pos.y = Mathf.Lerp(oposite,curr,updownCurve.Evaluate(t / updownTime));

        platform.localPosition = pos;
    }

    public void ApproachProcess(float t)
    {
        var factor = t / approachTime;
        var time = approachCurve.Evaluate(factor);

        var pos = Vector3.Lerp(_startPosition,_approachPosition,time);
        transform.position = pos;
    }

    public void ApproachConnect(float t)
    {
        Connect(_approachTarget,GetOpositeDirection(_approachDirection));
        _approachTarget.Connect(this,_approachDirection);

        _outTimeWhenConnected = outTimeWhenConnected;

        WhenConnect();
    }

    public void OutProcess(float t)
    {
        var factor = t / outTime;
        var time = outCurve.Evaluate(factor);

        var pos = Vector3.Lerp(_startPosition,_outPosition,time);
        transform.position = pos;
    }

    public void OutEnd(float t)
    {
        gameObject.SetActive(false);
        whenDeactive();
    }

    public void OutDisconnect(float t)
    {
        DisconnectAll();

        WhenDisconnect();
    }

    public float GetCurrentSide()
    {
        return state == PlatformState.UP ? upPosition : downPosition;
    }

    public float GetOpositeSide()
    {
        return state == PlatformState.UP ? downPosition : upPosition;
    }

    public void SetupSide()
    {
        var pos = platform.localPosition;
        pos.y = GetCurrentSide();
        platform.localPosition = pos;
    }

    public void Out(Vector3 position)
    {
        _startPosition = transform.position;
        _outPosition = position;

        _out = true;
        _approach = false;

        _timeCounter.InitSequencer("Out");
        _timeCounter.InitSequencer("Approach");
    }

    public void ApproachToTarget(int direction, B1_Platform target)
    {
        var side = target.GetConnection(direction);
        _approachDirection = direction;
        _approachPosition = side.GetPosition(direction);
        _approachTarget = side;
        _startPosition = transform.position;
        _opositeSpawnPosition = _startPosition;
        _opositeSpawnPosition.z *= -1f;

        _approach = true;
        _out = false;

        _timeCounter.InitSequencer("Out");
        _timeCounter.InitSequencer("Approach");

        WhenSpawn();
    }

    public void Disconnect(int direction)
    {
        // var connection = GetConnection(direction);
        // if(connection == null)
        //     return;

        SetConnection(direction,null);
        SetWallActive(direction,true);
    }

    public void DisconnectAll()
    {
        for(int i = 0; i < 4; ++i)
        {
            var connection = GetConnection(i);
            if(connection != this)
            {
                Debug.Log("Check");
                connection.Disconnect(GetOpositeDirection(i));
            }

            Disconnect(i);
        }
    }

    public void Connect(B1_Platform target, int direction)
    {
        SetConnection(direction,target);
        SetWallActive(direction,false);
    }

#region Connection

    public int GetOpositeDirection(int direction)
    {
        if(direction == 0)
            return 1;
        else if(direction == 1)
            return 0;
        else if(direction == 2)
            return 3;
        else if(direction == 3)
            return 2;
        
        return -1;
    }

    public void SetWallActive(int direction, bool active)
    {
        if(direction == 0)
            leftWall.SetActive(active);
        else if(direction == 1)
            rightWall.SetActive(active);
        else if(direction == 2)
            frontWall.SetActive(active);
        else if(direction == 3)
            backWall.SetActive(active);
    }

    public void SetConnection(int direction, B1_Platform target)
    {
        if(direction == 0)
            _leftConnect = target;
        else if(direction == 1)
            _rightConnect = target;
        else if(direction == 2)
            _frontConnect = target;
        else if(direction == 3)
            _backConnect = target;
    }

    public B1_Platform GetConnection(int direction)
    {
        if(direction == 0)
            return GetLeftConnection();
        else if(direction == 1)
            return GetRightConnection();
        else if(direction == 2)
            return GetFrontConnection();
        else if(direction == 3)
            return GetBackConnection();

        return null;
    }

    public B1_Platform GetLeftConnection()
    {
        var left = this;
        if(_leftConnect != null)
            left = _leftConnect;

        return left;
    }

    public B1_Platform GetRightConnection()
    {
        var right = this;
        if(_rightConnect != null)
            right = _rightConnect;

        return right;
    }

    public B1_Platform GetFrontConnection()
    {
        var front = this;
        if(_frontConnect != null)
            front = _frontConnect;

        return front;
    }

    public B1_Platform GetBackConnection()
    {
        var back = this;
        if(_backConnect != null)
            back = _backConnect;

        return back;
    }

    public Vector3 GetPosition(int direction)
    {
        if(direction == 0)
            return GetLeftPosition();
        else if(direction == 1)
            return GetRightPosition();
        else if(direction == 2)
            return GetFrontPosition();
        else if(direction == 3)
            return GetBackPosition();

        return Vector3.zero;
    }

    public Vector3 GetLeftPosition()
    {
        return transform.position + new Vector3(-24f,0f,0f);
    }

    public Vector3 GetRightPosition()
    {
        return transform.position + new Vector3(24f,0f,0f);
    }

    public Vector3 GetFrontPosition()
    {
        return transform.position + new Vector3(0f,0f,39f);
    }

    public Vector3 GetBackPosition()
    {
        return transform.position + new Vector3(0f,0f,-39f);
    }

#endregion

}
