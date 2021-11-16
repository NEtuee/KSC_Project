using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaInFallPoint_AI : PathfollowObjectBase
{
    public StateProcessor stateProcessor;

    public List<IKLegMovement> legs = new List<IKLegMovement>();

    public float damage = 10f;

    public Transform target;
    public PlayerUnit player;
    public Animator animator;

    public Transform eyeLightPosition;
    public Transform hitPosition;

    public bool launch = false;
    public bool startActive = true;

    public override void Assign()
    {
        base.Assign();
        stateProcessor.InitializeProcessor(this);

        AddAction(MessageTitles.set_setplayer,(x)=>{
            player = (PlayerUnit)x.data;
            target = player.transform;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        if(startActive)
        {
            stateProcessor.StateChange("Active");
            AnimationChange(8);
            SetIKMovement(false);
        }
        
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!launch)
            return;
        stateProcessor.StateProcess(deltaTime);
    }

    public void Launch()
    {
        Debug.Log("aunch");
        launch = true;
        AnimationChange(7);
    }

    public void ChangeStateToDead()
    {
        stateProcessor.StateChange("Dead");
    }

    public void AnimationChange(int code)
    {
        animator.SetTrigger("Change");
        animator.SetInteger("Animation",code);
    }

    public void SetIKMovement(bool value)
    {
        foreach(var item in legs)
        {
            item.SetIKActive(value);
            item.enabled = value;
        }
    }
}
