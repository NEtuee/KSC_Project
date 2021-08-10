using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MD;

public class PlayerManager : ManagerBase
{
    [SerializeField] private PlayerCtrl_Ver2 _player;
    [SerializeField] private EMPGun _emp;
    [SerializeField] private Renderer bagRenderer;
    private Material _bagMatrial;
    private Drone _drone;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("PlayerManager");

        MessageDataPooling.RegisterMessageData<PositionRotation>();

        AddAction(MessageTitles.playermanager_sendplayerctrl, (msg) => 
        {
            var target = (MessageReceiver)msg.sender;
            SendMessageQuick(target, MessageTitles.set_setplayer, _player);
        });

        AddAction(MessageTitles.playermanager_setPlayerTransform, (msg) =>
        {
            PositionRotation data = MessageDataPooling.CastData<PositionRotation>(msg.data);
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

        AddAction(MessageTitles.playermanager_addDamageToPlayer, (msg) =>
        {
            FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
            _player.TakeDamage(data.value);
        });

        AddAction(MessageTitles.playermanager_initPlayerStatus, (msg) => _player.InitStatus());
        AddAction(MessageTitles.playermanager_getPlayer,(msg)=>{
            var receiver = (MessageReceiver)msg.sender;
            SendMessageQuick(receiver,MessageTitles.playermanager_getPlayer,_player);
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        if(bagRenderer == null)
        {
            Debug.LogError("Not Set Bag Renderer");
        }

        _bagMatrial = bagRenderer.material;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _drone = _player.GetDrone();

        _player.hp.Subscribe(value =>
        {
            _bagMatrial.SetFloat("Vector1_5338de784f7d4439aba250082f9a53e3", value * 0.01f);

            StateBarSetValueType data = MessageDataPooling.GetMessageData<StateBarSetValueType>();
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
                BoolData setVisible = MessageDataPooling.GetMessageData<BoolData>();
                setVisible.value = true;
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), setVisible);
            }
        });

        _player.stamina.Subscribe(value =>
        {
            StateBarSetValueType data = MessageDataPooling.GetMessageData<StateBarSetValueType>();
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
                BoolData setVisible = MessageDataPooling.GetMessageData<BoolData>();
                setVisible.value = true;
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), setVisible);
            }
        });

        _player.energy.Subscribe(value =>
        {
            StateBarSetValueType data = MessageDataPooling.GetMessageData<StateBarSetValueType>();
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
                BoolData setVisible = MessageDataPooling.GetMessageData<BoolData>();
                setVisible.value = true;
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), setVisible);
            }
        });

        _player.hpPackCount.Subscribe(value =>
        {
            HpPackValueType data = MessageDataPooling.GetMessageData<HpPackValueType>();
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
                BoolData setVisible = MessageDataPooling.GetMessageData<BoolData>();
                setVisible.value = true;
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), setVisible);
            }
        });

        _player.chargeTime.Subscribe(value => {
            if (value >= 3f)
            {
                //crossHair.Third();
                IntData phase = MessageDataPooling.GetMessageData<IntData>();
                phase.value = 3;
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), phase);
            }
            else if (value >= 2f)
            {
                //crossHair.Second();
                IntData phase = MessageDataPooling.GetMessageData<IntData>();
                phase.value = 2;
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), phase);
            }
            else if (value >= 1f)
            {
                //crossHair.First();
                IntData phase = MessageDataPooling.GetMessageData<IntData>();
                phase.value = 1;
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), phase);
            }
        });

        _player.loadCount.Subscribe(value =>
        {
            IntData data = MessageDataPooling.GetMessageData<IntData>();
            data.value = value;
            SendMessageEx(MessageTitles.uimanager_setgunloadvalue, GetSavedNumber("UIManager"), data);
        });
        _player.chargeTime.Subscribe(value => 
        {
            FloatData data = MessageDataPooling.GetMessageData<FloatData>();
            data.value = value;
            SendMessageEx(MessageTitles.uimanager_setgunchargetimevalue, GetSavedNumber("UIManager"), data);
        });
        _player.energy.Subscribe(value =>
        {
            FloatData data = MessageDataPooling.GetMessageData<FloatData>();
            data.value = value;
            SendMessageEx(MessageTitles.uimanager_setgunenergyvalue, GetSavedNumber("UIManager"), data);
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

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_STANDALONE
        Application.Quit();
#endif

    }
}

namespace MD
{
    public class PositionRotation : MessageData
    {
        public Vector3 position;
        public Quaternion rotation;

        public PositionRotation() { }
        public PositionRotation(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
}
