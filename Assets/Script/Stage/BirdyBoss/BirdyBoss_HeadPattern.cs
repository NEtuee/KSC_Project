using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_HeadPattern : ObjectBase
{
    public List<MeshRenderer> dissolveTargets = new List<MeshRenderer>();
    public HexCubeGrid grid;

    [Header("Stemp")]
    public float dissolveTime = 1f;
    public float stempHeight = 10f;
    public AnimationCurve stempCurve;

    public float stempStartTime = 1f;
    public float stempTime = 1f;
    public float stempWaitTime = 1f;

    [Header("Ring")]
    public float ringTerm = 0.1f;
    public float ringSpeed = 1f;
    public float ringActiveTime = 0.1f;
    public HexCube stempCube;

    [Header("Explosion")]
    public Transform explosionPosition;
    public float explosionRadius = 5f;
    public float explosionPower = 150f;

    private Vector3 _localPosition;

    private TimeCounterEx _timeCounterEx = new TimeCounterEx();
    
    private List<HexCube> _ringList = new List<HexCube>();

    PlayerUnit _player;

    private bool _stemp = false;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
        });

        _localPosition = transform.localPosition;
        
        _timeCounterEx.CreateSequencer("Stemp");
        _timeCounterEx.AddSequence("Stemp",dissolveTime,DissolveOut,(x)=>{
            var pos = stempCube.transform.position;
            pos.y = stempHeight;
            transform.position = pos;
        });
        _timeCounterEx.AddSequence("Stemp",dissolveTime,DissolveIn,(x)=>{
            
        });
        _timeCounterEx.AddSequence("Stemp",stempStartTime,null,null);
        _timeCounterEx.AddSequence("Stemp",stempTime,Stemp,(x)=>{
            Ring();
            Explosion();

            SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse, GetSavedNumber("CameraManager"), null);
        });
        _timeCounterEx.AddSequence("Stemp",stempWaitTime,null,null);
        _timeCounterEx.AddSequence("Stemp",dissolveTime,DissolveOut,(x)=>{
            transform.localPosition = _localPosition;
        });
        _timeCounterEx.AddSequence("Stemp",dissolveTime,DissolveIn,(x)=>{
            _stemp = false;
        });

        _timeCounterEx.CreateSequencer("RingPattern");
        _timeCounterEx.AddSequence("RingPattern",dissolveTime,null,null);
    }

    public override void Initialize()
    {
        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!_stemp)
            return;
        
        _timeCounterEx.ProcessSequencer("Stemp",deltaTime);
    }

    public void StempTarget(HexCube target)
    {
        _stemp = true;
        stempCube = target;
        _timeCounterEx.InitSequencer("Stemp");
    }

    public void Explosion()
    {
        var dist = Vector3.Distance(_player.transform.position,explosionPosition.position);
        if(dist <= explosionRadius)
            _player.Ragdoll.ExplosionRagdoll(explosionPower,explosionPosition.position,explosionRadius);
    }

    public void Ring()
    {
        stempCube.SetMove(false,0f,ringSpeed,ringActiveTime);
        for(int i = 0; i < grid.mapSize; ++i)
        {
            _ringList.Clear();
            grid.GetCubeRing(ref _ringList,stempCube.cubePoint,i);
            foreach(var item in _ringList)
            {
                item.SetMove(false,(float)(i - 1) * ringTerm,ringSpeed,ringActiveTime);
            }
        }
    }

    public void Stemp(float x)
    {
        var time = x / stempTime;
        var factor = stempCurve.Evaluate(time);
        var y = Mathf.Lerp(stempHeight,stempCube.transform.position.y + 1f,factor);

        var pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }

    public void DissolveIn(float t)
    {
        foreach(var item in dissolveTargets)
        {
            item.material.SetFloat("Dissvole", 1f - (t / dissolveTime));
        }
    }

    public void DissolveOut(float t)
    {
        foreach(var item in dissolveTargets)
        {
            item.material.SetFloat("Dissvole", (t / dissolveTime));
        }
    }

}
