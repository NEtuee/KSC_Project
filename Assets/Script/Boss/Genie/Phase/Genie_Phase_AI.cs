using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_Phase_AI : PathfollowObjectBase
{
    public StateProcessor stateProcessor;
    public Transform respawnPoint;
    public Transform respawnTarget;
    public Transform body;
    public Boogie_GridControll gridControll;
    public Animator animatorController;
    public float bodyRotateSpeed = 5f;

    private PlayerUnit _player;
    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private bool _respawn = false;
    private HexCube _respawnCube = null;
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
        _timeCounter.InitTimer("spawn");
        _respawn = false;
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        if(!_respawn)
            stateProcessor.StateProcess(deltaTime);
        else
        {
            var spawn = false;
            _timeCounter.IncreaseTimerSelf("spawn",out spawn, deltaTime);
            if(spawn)
            {
                _respawnCube.special = false;
                _respawn = false;
            }
        }
    }

    public void ChangeAnimation(int code)
    {
        animatorController.SetTrigger("Change");
        animatorController.SetInteger("Code",code);
    }

    public void Respawn()
    {
        _timeCounter.InitTimer("spawn");
        _respawn = true;

        if(_respawnCube != null)
            _respawnCube.special = false;

        _respawnCube = gridControll.cubeGrid.GetCubeFromWorld(respawnTarget.position,false);
        _respawnCube.MoveToUp();
        _respawnCube.special = true;

        respawnPoint.position = _respawnCube.transform.position + Vector3.up;
    }
}
