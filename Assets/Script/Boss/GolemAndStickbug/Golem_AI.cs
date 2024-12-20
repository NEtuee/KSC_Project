using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_AI : PathfollowObjectBase
{
    public string mainPath;
    public float limitTime = 300f;

    public MeshRenderer gague;

    public Transform mainBody;
    public GraphAnimator graphAnimator;

    public Stickbug_AI stickbug;

    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private bool _launch = false;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.player_EMPHit,EMPHit);
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SetPath(mainPath,true);

        _timeCounter.InitTimer("TimeCheck",0f,limitTime);
        graphAnimator.Play("Idle",mainBody);

        Launch();
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);
        graphAnimator.Progress(deltaTime);

        if(!_launch)
            return;
        
        FollowPath(deltaTime);

        var time = _timeCounter.IncreaseTimerSelf("TimeCheck",out var limit,deltaTime);

        gague.material.SetFloat("Factor",time / limitTime);
        if(limit)
        {
            var effectData = MessageDataPooling.GetMessageData<MD.EffectActiveData>();

            var floatData = MessageDataPooling.GetMessageData<MD.FloatData>();
            floatData.value = 99999f;
            SendMessageEx(MessageTitles.playermanager_addDamageToPlayer,GetSavedNumber("PlayerManager"),floatData);

            _launch = false;
        }

    }

    public void EMPHit(Message msg)
    {
        stickbug.WhenHit();
    }

    public void Launch()
    {
        _launch = true;
    }

    public void Stop()
    {
        _launch = false;
    }
}
