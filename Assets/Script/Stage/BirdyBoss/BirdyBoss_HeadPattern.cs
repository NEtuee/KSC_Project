using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_HeadPattern : PathfollowObjectBase
{
    public enum State
    {
        PlayerLook,
        HeadStemp,
        FogMove,
        GroundShot,
    }

    public State currentState = State.HeadStemp;
    public List<MeshRenderer> dissolveTargets = new List<MeshRenderer>();
    public HexCubeGrid grid;

    public Transform shieldObj;
    public NewEmpShield shieldTarget;

    [Header("Stemp")]
    public float dissolveTime = 1f;
    public float stempHeight = 10f;
    public float stempYOffset = 3f;
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
    private bool _lookDown = false;
    private bool _inout = false;
    private bool _groggy = false;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
        });

        _localPosition = transform.localPosition;
        
        _timeCounterEx.CreateSequencer("Stemp");
        _timeCounterEx.AddSequence("Stemp",dissolveTime, (x) => { DissolveOut(x); ShieldLookPlayer(); }, (x)=>{
            var pos = stempCube.transform.position;
            pos.y = stempHeight;
            transform.position = pos;
        });
        _timeCounterEx.AddSequence("Stemp",dissolveTime, (x) => { DissolveIn(x); ShieldLookPlayer(); }, (x)=>{
            
        });
        _timeCounterEx.AddSequence("Stemp",stempStartTime,(x)=> {
            var dir = MathEx.DeleteYPos(_player.transform.position - shieldObj.position).normalized;
            shieldObj.rotation = Quaternion.Slerp(shieldObj.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.2f);
        },(x)=> {
            //_lookDown = true;
        });
        _timeCounterEx.AddSequence("Stemp",stempTime,Stemp,(x)=>{
            Ring();
            Explosion();

            SendMessageEx(MessageTitles.cameramanager_generaterecoilimpluse, GetSavedNumber("CameraManager"), null);

            //shieldTarget.gameObject.SetActive(true);
            //shieldTarget.VisibleVisual();
            _inout = true;
        });

        _timeCounterEx.CreateSequencer("InOut");
        _timeCounterEx.AddSequence("InOut", stempWaitTime,null,null);
        _timeCounterEx.AddSequence("InOut", dissolveTime,DissolveOut, (x)=>{
            transform.localPosition = _localPosition;

            //shieldTarget.Reactive();
            //shieldTarget.gameObject.SetActive(false);

            _lookDown = false;
            _stemp = false;
        });
        _timeCounterEx.AddSequence("InOut", dissolveTime,DissolveIn, (x)=>{
            
        });

        _timeCounterEx.CreateSequencer("RingPattern");
        _timeCounterEx.AddSequence("RingPattern",dissolveTime,null,null);

        _timeCounterEx.InitTimer("groggy", 0f, 0f);
    }

    public override void Initialize()
    {
        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(_inout)
        {
            _inout = !_timeCounterEx.ProcessSequencer("InOut", deltaTime);
        }

        if(_groggy)
        {
            _timeCounterEx.IncreaseTimerSelf("groggy", out var limit, deltaTime);
            _groggy = !limit;
            if(limit)
            {
                shieldTarget.Reactive();
                shieldTarget.gameObject.SetActive(false);
            }

            return;
        }

        if(currentState == State.PlayerLook)
        {
            ShieldLookPlayer();
        }
        if(currentState == State.HeadStemp)
        {
            if (!_stemp)
            {
                ShieldLookPlayer();
                return;
            }

            if (_lookDown)
            {
                ShieldLookDown();
            }


            if (!_inout)
            {
                _timeCounterEx.ProcessSequencer("Stemp", deltaTime);
            }
        }
        else if(currentState == State.FogMove)
        {
            ShieldLookPlayer();
            FollowPathInDirection(deltaTime);
        }
        else if(currentState == State.GroundShot)
        {

        }
        
        
    }

    public void DisableShield()
    {
        shieldTarget.Reactive();
        shieldTarget.gameObject.SetActive(false);
    }

    public void FogPathFollow()
    {
        currentState = State.FogMove;

        shieldTarget.gameObject.SetActive(true);
        shieldTarget.Reactive();

        SetPath("FogBirdyPath", true);
    }

    public void Groggy(float time)
    {
        _groggy = true;
        shieldTarget.gameObject.SetActive(true);
        shieldTarget.VisibleVisual();
        _timeCounterEx.InitTimer("groggy", 0f, time);

        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), data);
    }

    public void QuickOut()
    {
        currentState = State.PlayerLook;
        _stemp = true;
        _lookDown = false;
        _inout = true;
        _timeCounterEx.InitSequencer("InOut");
        _timeCounterEx.SkipSequencer("InOut", stempWaitTime);
    }

    public void StempTarget(HexCube target)
    {
        currentState = State.HeadStemp;
        _stemp = true;
        _lookDown = false;
        _inout = false;
        stempCube = target;
        _timeCounterEx.InitSequencer("Stemp");
        _timeCounterEx.InitSequencer("InOut");
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
                item.SetAlertTime(1f);
            }
        }
    }

    public void ShieldLookPlayer()
    {
        var dir = (_player.transform.position - shieldObj.position).normalized;
        shieldObj.rotation = Quaternion.Slerp(shieldObj.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.2f);
    }

    public void ShieldLookDown()
    {
        var dir = Vector3.down;
        shieldObj.rotation = Quaternion.Slerp(shieldObj.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.2f);
    }

    public void Stemp(float x)
    {
        var time = x / stempTime;
        var factor = stempCurve.Evaluate(time);
        var y = Mathf.Lerp(stempHeight,stempCube.transform.position.y + stempYOffset, factor);

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
