using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanko_AI : PathfollowObjectBase
{
    public float hitTargetDist = 3f;
    public float hitGague = 5f;
    public float cooldownFactor = 0.1f;
    public float maxAngry = 15f;

    public float lifeTime = 30f;

    public MeshRenderer angryGagueRenderer;
    public MeshRenderer lifeGagueRenderer;

    public Transform head;
    public Transform model;
    public GraphAnimator graphAnimator;

    [HideInInspector]public bool pluged = true;
    [HideInInspector]public float angryGague = 0f;
    [HideInInspector]public PlayerUnit target;

    public StateProcessor stateProcessor;

    private bool _active = true;
    private float _lifeTime = 0f;

    public override void Assign()
    {
        base.Assign();

        stateProcessor.InitializeProcessor(this);

        AddAction(MessageTitles.set_setplayer,(x)=>{
            target = (PlayerUnit)x.data;
        });

        AddAction(MessageTitles.player_EMPHit,(x)=>{
            stateProcessor.StateChange("HitMove");
            angryGague += hitGague;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl,GetSavedNumber("PlayerManager"),null);

        stateProcessor.StateChange("PlugedChase");

        _lifeTime = lifeTime;
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        graphAnimator.Progress(deltaTime);

        if(!_active)
            return;

        if(pluged)
        {
            if(cooldownFactor > 0f)
            {
                angryGague -= deltaTime * cooldownFactor;
            }   
        }
        else
        {
            _lifeTime -= deltaTime;

            lifeGagueRenderer.material.SetFloat("Factor",(_lifeTime) / lifeTime);

            if(_lifeTime <= 0f)
            {
                _active = false;
                graphAnimator.Play("Dead",model);
                return;
            }
        }

        angryGagueRenderer.material.SetFloat("Factor",angryGague / maxAngry);
        stateProcessor.StateProcess(deltaTime);

        var dist = Vector3.Distance(head.position,target.transform.position);
        if(dist <= hitTargetDist && stateProcessor.currentState != "HitTarget")
        {
            stateProcessor.StateChange("HitTarget");
            target.Ragdoll.ExplosionRagdoll(300f,transform.forward);
        }
    }

    public void RemovePlug()
    {
        pluged = false;
        angryGague = maxAngry;
        stateProcessor.StateChange("Chase");
        stateProcessor.StateChange("HitMove");
    }
}
