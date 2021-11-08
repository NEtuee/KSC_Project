using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_DroneLine : ObjectBase
{
    public AnimationCurve apearCurve;
    public AnimationCurve spinCurve;

    public GameObject positionDrone;
    public Genie_CoreDroneAI coreDrone;
    public NewEmpShield empShield;

    private Quaternion _targetQuaternion;
    private float _targetHeight;
    private float _spinTime;
    private float _apearTime;

    private AnimationCurve _heightCurve;

    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private Quaternion _startQuaternion;
    private float _startHeight;

    public void Create(float spin, float apear, float startWait, float endWait)
    {
        _spinTime = spin;
        _apearTime = apear;

        _timeCounter.CreateSequencer("Process");
        _timeCounter.AddSequence("Process",apear,Apear,null);
        _timeCounter.AddSequence("Process",startWait,null,null);
        _timeCounter.AddSequence("Process",spin,Spin,null);
        _timeCounter.AddSequence("Process",endWait,null,null);
        _timeCounter.AddSequence("Process",apear,Disapear,(x)=>{gameObject.SetActive(false);});

        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public void Active(AnimationCurve heightCurve, Vector3 basePosition, Quaternion startQuat, Quaternion endQuat, float startHeight, float endHeight, bool isCore)
    {
        _heightCurve = heightCurve;

        _startQuaternion = startQuat;
        _targetQuaternion = endQuat;

        transform.rotation = startQuat;


        _startHeight = startHeight;
        _targetHeight = endHeight;

        basePosition.y = _startHeight;
        transform.position = basePosition;

        CoreSet(isCore);
        coreDrone.shield.Reactive();
        empShield?.Reactive();

        _timeCounter.InitSequencer("Process");
        _timeCounter.ProcessSequencer("Process",0f);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        _timeCounter.ProcessSequencer("Process",deltaTime);
    }

    public void CoreSet(bool isCore)
    {
        coreDrone.gameObject.SetActive(isCore);
        positionDrone.SetActive(!isCore);
    }

    public void Spin(float time)
    {
        var rot = transform.rotation;
        rot = Quaternion.Lerp(_startQuaternion, _targetQuaternion,spinCurve.Evaluate(time / _spinTime));
        transform.rotation = rot;

        var pos = transform.position;
        pos.y = _targetHeight + _heightCurve.Evaluate(time / _spinTime);
        transform.position = pos;
    }

    public void Disapear(float time)
    {
        var pos = transform.position;

        pos.y = Mathf.Lerp(_targetHeight,_startHeight,apearCurve.Evaluate(time / _apearTime));
        transform.position = pos;
    }

    public void Apear(float time)
    {
        var pos = transform.position;

        pos.y = Mathf.Lerp(_startHeight,_targetHeight,apearCurve.Evaluate(time / _apearTime));
        transform.position = pos;
    }


}
