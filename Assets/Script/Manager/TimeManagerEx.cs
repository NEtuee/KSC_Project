using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagerEx : ManagerBase
{
    private float _timeScaleLerp = 0f;
    private float _timeScaleStop = 0f;
    private float _timeScaleStart = 0f;

    private bool _timeScaled = false;

    private TimeCounterEx _timeCounter;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("TimeManager");

        AddAction(MessageTitles.timemanager_settimescale, SetTimeScale);
        AddAction(MessageTitles.timemanager_timestop, StopTime);

        AddAction(MessageTitles.scene_afterSceneChange, (msg) =>
         {
             StopTime(true);
         });
        AddAction(MessageTitles.scene_sceneChanged, (msg) =>
        {
            StopTime(false);
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        _timeCounter = new TimeCounterEx();
        _timeCounter.InitTimer("timeScaleLerp", 0f, 1f);
        _timeCounter.InitTimer("timeScaleStop", 0f, 1f);
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

    }

    private void Update()
    {
        if (_timeScaled)
        {
            _timeCounter.IncreaseTimerSelf("timeScaleStart", out bool limit, Time.unscaledDeltaTime);
            if (limit)
            {
                _timeCounter.IncreaseTimerSelf("timeScaleStop", out limit, Time.unscaledDeltaTime);
                if (limit)
                {
                    float currTime = _timeCounter.IncreaseTimerSelf("timeScaleLerp", out limit, Time.unscaledDeltaTime);
                    float limitTime = _timeCounter.GetTimeLimit("timeScaleLerp");

                    if (limit)
                    {
                        Time.timeScale = 1f;
                        _timeScaled = false;
                    }
                    else
                    {
                        Time.timeScale = Mathf.Lerp(_timeScaleLerp, 1f, currTime / limitTime);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Message Type = SetTimeScaleMsg
    /// </summary>
    /// <param name="msg"></param>
    public void SetTimeScale(Message msg)
    {
        SetTimeScaleMsg data = (SetTimeScaleMsg)msg.data;

        _timeCounter.InitTimer("timeScaleLerp", 0f, data.lerpTime);
        _timeCounter.InitTimer("timeScaleStop", 0f, data.stopTime);
        _timeCounter.InitTimer("timeScaleStart", 0f, data.startTime);

        _timeScaled = true;

        _timeScaleLerp = data.timeScale;
        Time.timeScale = data.startTime == 0f ? data.timeScale : Time.timeScale;
    }

    public void StopTime(Message msg)
    {
        bool active = (bool)msg.data;

        if (active)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    public void StopTime(bool stop)
    {
        if (stop)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }
}


struct SetTimeScaleMsg
{
    public float timeScale;
    public float lerpTime;
    public float stopTime;
    public float startTime;
}
