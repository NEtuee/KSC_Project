using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMono<TimeManager>
{
    private float _timeScaleLerp = 0f;
    private float _timeScaleStop = 0f;

    private bool _timeScaled = false;

    private TimeCounterEx _timeCounter;

    void Start()
    {
        SetSingleton(this);

        _timeCounter = new TimeCounterEx();
        _timeCounter.InitTimer("timeScaleLerp",0f,1f);
        _timeCounter.InitTimer("timeScaleStop",0f,1f);
    }

    void Update()
    {
        if(_timeScaled)
        {
            _timeCounter.IncreaseTimerSelf("timeScaleStop",out bool limit, Time.unscaledDeltaTime);
            if(limit)
            {
                float currTime = _timeCounter.IncreaseTimerSelf("timeScaleLerp",out limit, Time.unscaledDeltaTime);
                float limitTime = _timeCounter.GetTimeLimit("timeScaleLerp");

                if(limit)
                {
                    Time.timeScale = 1f;
                    _timeScaled = false;
                }
                else
                {
                    
                    Time.timeScale = Mathf.Lerp(_timeScaleLerp,1f,currTime / limitTime);
                }

            }


        }



    }

    public void SetTimeScale(float timeScale, float lerpTime, float stopTime)
    {
        _timeCounter.InitTimer("timeScaleLerp",0f,lerpTime);
        _timeCounter.InitTimer("timeScaleStop",0f,stopTime);

        _timeScaled = true;

        _timeScaleLerp = timeScale;
        Time.timeScale = timeScale;
    }
}
