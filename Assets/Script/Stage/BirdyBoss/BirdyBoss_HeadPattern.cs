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
    public DissolveControl dissolveControl;
    //public List<MeshRenderer> dissolveTargets = new List<MeshRenderer>();
    public HexCubeGrid grid;

    public Transform shieldObj;
    public NewEmpShield shieldTarget;

    public GameObject disapearTarget;

    public Transform birdyRoot;
    public Transform birdyInside;
    public Transform birdyOutside;

    public Animator birdyAnimator;

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

    [Header("shot")]
    public float distanceFactor = 0.3f;
    public float distanceTerm = 0.3f;
    public float shotTerm = 3f;
    public float shotWait = 1f;
    public float shotDownTime = 2f;
    public int shotCount = 3;

    private Vector3 _localPosition;

    private TimeCounterEx _timeCounterEx = new TimeCounterEx();
    
    private List<HexCube> _ringList = new List<HexCube>();
    private List<HexCube> _lineList = new List<HexCube>();

    private Transform _birdyTarget;

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
            shieldObj.rotation = Quaternion.Slerp(shieldObj.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.1f);
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

        _timeCounterEx.CreateSequencer("Shot");
        _timeCounterEx.AddSequence("Shot",dissolveTime,null,null);
        for(int i = 0; i < shotCount; ++i)
        {
            _timeCounterEx.AddSequence("Shot",shotTerm,(x)=>{
                var dir = MathEx.DeleteYPos(_player.transform.position - shieldObj.position).normalized;
                shieldObj.rotation = Quaternion.Slerp(shieldObj.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.1f);
            },null);
            _timeCounterEx.AddSequence("Shot",shotWait,null,(x)=>{
                Line(shieldObj.forward);
            });
        }

        _timeCounterEx.AddSequence("Shot",1f,null,(x)=>{
            currentState = State.PlayerLook;
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

        dissolveControl.SetDissolve(1f);

        _birdyTarget = birdyInside;
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(_inout)
        {
            _inout = !_timeCounterEx.ProcessSequencer("InOut", deltaTime);
        }

        birdyRoot.position = Vector3.Lerp(birdyRoot.position, _birdyTarget.position, 0.1f);

        if (_groggy)
        {
            _timeCounterEx.IncreaseTimerSelf("groggy", out var limit, deltaTime);
            _groggy = !limit;
            if(limit)
            {
                shieldTarget.Reactive();
                shieldTarget.gameObject.SetActive(false);


                _birdyTarget = currentState == State.FogMove ? birdyOutside : birdyInside;
            }

            //return;
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
            //ShieldLookPlayer();
            FollowPathInDirection(deltaTime);
            var angle = Vector3.SignedAngle(shieldObj.forward, targetDirection, shieldObj.up);

            if (Mathf.Abs(angle) > turnAccuracy)
            {
                if (angle > 0)
                    Turn(true, shieldObj, rotationSpeed, deltaTime);
                else
                    Turn(false, shieldObj, rotationSpeed, deltaTime);
            }

        }
        else if(currentState == State.GroundShot)
        {
            _timeCounterEx.ProcessSequencer("Shot",deltaTime);
        }
        
        
    }

    public void DisableShield()
    {
        shieldTarget.Reactive();
        shieldTarget.gameObject.SetActive(false);
        _birdyTarget = birdyInside;
    }

    public void FogPathFollow()
    {
        currentState = State.FogMove;

        shieldTarget.Reactive();
        shieldTarget.gameObject.SetActive(true);
        _birdyTarget = birdyOutside;

        SetPath("FogBirdyPath", true);
        ChangeAnimation(1);
    }

    public void Shot()
    {
        currentState = State.GroundShot;
        _timeCounterEx.InitSequencer("Shot");

        _inout = true;
        _timeCounterEx.InitSequencer("InOut");
        _timeCounterEx.SkipSequencer("InOut", stempWaitTime);
    }

    public void ShieldActive()
    {
        shieldTarget.gameObject.SetActive(true);
        shieldTarget.VisibleVisual();

        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), data);
    }

    public void Groggy(float time)
    {
        ShieldActive();

        _groggy = true;
        _timeCounterEx.InitTimer("groggy", 0f, time);
        _birdyTarget = birdyOutside;

        ChangeAnimation(0);
    }

    public void QuickOut()
    {
        _birdyTarget = birdyInside;

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

    public void Line(Vector3 direction)
    {
        _lineList.Clear();

        var start = grid.GetCubeFromWorld(transform.position);
        var end = transform.position + direction * 60f;
        var endPoint = grid.GetCubePointFromWorld(end);

        grid.GetCubeLineHeavy(ref _lineList,start.cubePoint,endPoint,0,6);
        foreach(var item in _lineList)
        {
            if (!item.IsActive())
                continue;

            var dist = Vector3.Distance(start.originWorldPosition.position,item.originWorldPosition.position);
            item.SetMove(false,dist * distanceFactor * distanceTerm,1f,shotDownTime);
            item.SetAlertTime(1f);
        }
    }

    public void ChangeAnimation(int target)
    {
        birdyAnimator.SetTrigger("Change");
        birdyAnimator.SetInteger("Target",target);
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
                //item.SetAlertTime(1f);
            }
        }
    }

    public void ShieldLookPlayer()
    {
        var dir = (_player.transform.position - shieldObj.position).normalized;
        shieldObj.rotation = Quaternion.Slerp(shieldObj.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.1f);
    }

    public void ShieldLookDown()
    {
        var dir = Vector3.down;
        shieldObj.rotation = Quaternion.Slerp(shieldObj.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.1f);
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
        dissolveControl.SetDissolve(1f - (t / dissolveTime));
        disapearTarget.SetActive(true);
    }

    public void DissolveOut(float t)
    {
        dissolveControl.SetDissolve(t / dissolveTime);
        disapearTarget.SetActive(false);
    }

}
