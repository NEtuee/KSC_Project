using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_Phase_AI : PathfollowObjectBase
{
    public StateProcessor stateProcessor;
    public Transform body;
    public Boogie_GridControll gridControll;
    public Animator animatorController;
    public float bodyRotateSpeed = 5f;

    private PlayerUnit _player;
    public override void Assign()
    {
        base.Assign();
        stateProcessor.InitializeProcessor(this);

        AddAction(MessageTitles.set_setplayer,(x)=>{
            _player = (PlayerUnit)x.data;
            targetTransform = _player.transform;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        stateProcessor.StateChange("Idle");
        gridControll.Init();
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);
        stateProcessor.StateProcess(deltaTime);
    }

    public void ChangeAnimation(int code)
    {
        animatorController.SetTrigger("Change");
        animatorController.SetInteger("Code",code);
    }
}
