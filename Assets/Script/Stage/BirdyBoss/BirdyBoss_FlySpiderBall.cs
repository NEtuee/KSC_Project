using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_FlySpiderBall : ObjectBase
{
    public List<B1_FlySpider> spiders = new List<B1_FlySpider>();

    public float stempWaitTime;
    public float stempTime;
    public float stempHeight;
    public float stempOffset;

    public AnimationCurve stempCurve;

    private HexCube _stempTarget;
    private PlayerUnit _player;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private Vector3 _startPosition;

    public override void Assign()
    {
        base.Assign();

        _timeCounter.CreateSequencer("main");
        _timeCounter.AddSequence("main", stempWaitTime, null, null);
        _timeCounter.AddSequence("main", stempTime, Stemp, (x)=> {
            Explosion();
            DetatchAll();

            gameObject.SetActive(false);
        });

        AddAction(MessageTitles.set_setplayer, (x) => {
            _player = (PlayerUnit)x.data;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        _timeCounter.InitSequencer("main");
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        _timeCounter.ProcessSequencer("main", deltaTime);
    }

    public void Stemp(float t)
    {
        var factor = t / stempTime;
        transform.position = Vector3.Lerp(_startPosition, 
            _stempTarget.transform.position + Vector3.up * stempOffset, factor);
    }

    public void DetatchAll()
    {
        foreach(var item in spiders)
        {
            item.transform.SetParent(null);
        }
    }

    public void Explosion()
    {
        var dist = Vector3.Distance(_player.transform.position, transform.position);
        if (dist <= 10f)
            _player.Ragdoll.ExplosionRagdoll(150f, transform.position, 10f);

        MD.EffectActiveData data = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"), data);
    }

    public void StempTarget(HexCube target)
    {
        _stempTarget = target;
        _startPosition = target.transform.position + Vector3.up * stempHeight;
        transform.position = _startPosition;

        Respawn();

        _timeCounter.InitSequencer("main");
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        foreach(var item in spiders)
        {
            item.transform.SetParent(transform);
            item.Respawn();
            item.launch = true;
            item.direction = (item.transform.position - transform.position).normalized;//MathEx.RandomCircle(1f).normalized;
            item.stateProcessor.StateChange("RushQuick");
        }
    }
}
