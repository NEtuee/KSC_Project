using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

public class PlayerManager : ManagerBase
{
    [SerializeField] private PlayerCtrl_Ver2 _player;
    [SerializeField] private EMPGun _emp;
    private Drone _drone;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("PlayerManager");

        AddAction(MessageTitles.playermanager_sendplayerctrl, (msg) => 
        {
            var target = (MessageReceiver)msg.sender;
            SendMessageQuick(target, MessageTitles.set_setplayer, _player);
        });

        AddAction(MessageTitles.playermanager_setPlayerTransform, (msg) =>
        {
            PositionRotation data = (PositionRotation)msg.data;
            _player.transform.SetPositionAndRotation(data.position, data.rotation); 
        });

        AddAction(MessageTitles.scene_beforeSceneChange, (msg) =>
         {
             DontDestroyOnLoad(_player.transform);
             DontDestroyOnLoad(_drone.transform);
         });

        AddAction(MessageTitles.scene_afterSceneChange, (msg) =>
        {
            _player.InitializeMove();
            _player.InitVelocity();
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _drone = _player.GetDrone();

        _player.hp.Subscribe(value =>
        {
            StateBarSetValueType data;
            data.type = UIManager.StateBarType.HP;
            data.value = value;
            if (_player.GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
            {
                data.visible = false;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
            }
            else
            {
                data.visible = true;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), true);
            }
        });

        _player.stamina.Subscribe(value =>
        {
            StateBarSetValueType data;
            data.type = UIManager.StateBarType.Stamina;
            data.value = value;
            if (_player.GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
            {
                data.visible = false;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
            }
            else
            {
                data.visible = true;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), true);
            }
        });

        _player.energy.Subscribe(value =>
        {
            StateBarSetValueType data;
            data.type = UIManager.StateBarType.Energy;
            data.value = value;
            if (_player.GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
            {
                data.visible = false;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
            }
            else
            {
                data.visible = true;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), true);
            }
        });

        _player.hpPackCount.Subscribe(value =>
        {
            HpPackValueType data;
            data.value = value;

            if (_player.GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
            {
                data.visible = false;
                SendMessageEx(MessageTitles.uimanager_setvaluehppackui, GetSavedNumber("UIManager"), data);
            }
            else
            {
                data.visible = true;
                SendMessageEx(MessageTitles.uimanager_setvaluehppackui, GetSavedNumber("UIManager"), data);
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), true);
            }
        });

        _player.chargeTime.Subscribe(value => {
            if (value >= 3f)
            {
                //crossHair.Third();
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 3);
            }
            else if (value >= 2f)
            {
                //crossHair.Second();
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 2);
            }
            else if (value >= 1f)
            {
                //crossHair.First();
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 1);
            }
        });

        _player.loadCount.Subscribe(value =>
        {
            SendMessageEx(MessageTitles.uimanager_setgunloadvalue, GetSavedNumber("UIManager"), value);
        });
        _player.chargeTime.Subscribe(value => 
        {
            SendMessageEx(MessageTitles.uimanager_setgunchargetimevalue, GetSavedNumber("UIManager"), value);
        });
        _player.energy.Subscribe(value =>
        {
            SendMessageEx(MessageTitles.uimanager_setgunenergyvalue, GetSavedNumber("UIManager"), value);
        });
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if(Keyboard.current.pKey.wasPressedThisFrame)
        {
            SendMessageEx(MessageTitles.scene_loadNextLevel, GetSavedNumber("SceneManager"), null);
        }
    }
}

public struct PositionRotation
{
    public Vector3 position;
    public Quaternion rotation;

    public PositionRotation(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
}
