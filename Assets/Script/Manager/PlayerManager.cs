using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MD;

public class PlayerManager : ManagerBase
{
    [SerializeField] private PlayerUnit _player;
    private IKCtrl _playerFootIK;
    [SerializeField] private EMPGun _emp;
    [SerializeField] private Renderer bagRenderer;
    [SerializeField] private Renderer playerWeaponRenderer;
    [SerializeField] private Color radioColor;
    private Material _playerWeaponMat;
    private Material _bagMatrial;
    [SerializeField] private Drone _drone;

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
            //_player.GetPlayerRagdoll().transform.position = data.position;
            _player.FootIK.InitPelvisHeight();
        });

        AddAction(MessageTitles.playermanager_setDroneTransform, (msg) =>
        {
            PositionRotation data = MessageDataPooling.CastData<PositionRotation>(msg.data);
            _drone.transform.SetPositionAndRotation(data.position, data.rotation);
        });

        AddAction(MessageTitles.playermanager_setDroneCanMove, (msg) =>
        {
            var data = MessageDataPooling.CastData<BoolData>(msg.data);
            _drone.SetCanMove(data.value);
        });

        AddAction(MessageTitles.scene_beforeSceneChange, (msg) =>
         {
             _player.transform.SetParent(null);
             _drone.transform.SetParent(null);
             _player.LineTracker.SetParent(null);
             DontDestroyOnLoad(_player.transform);
             DontDestroyOnLoad(_drone.transform);
             DontDestroyOnLoad(_player.LineTracker);

             _player.CapsuleCollider.enabled = false;
             _player.canGroundCheck = false;
         });

        AddAction(MessageTitles.scene_beforeSceneChangeNotAsync, (msg) =>
         {
             SceneManager.MoveGameObjectToScene(_player.gameObject, SceneManager.GetActiveScene());
             SceneManager.MoveGameObjectToScene(_drone.gameObject, SceneManager.GetActiveScene());
             SceneManager.MoveGameObjectToScene(_player.LineTracker.gameObject, SceneManager.GetActiveScene());
         });

        AddAction(MessageTitles.scene_afterSceneChange, (msg) =>
        {
            _player.canGroundCheck = true;
        });

        AddAction(MessageTitles.scene_sceneChanged, (msg) =>
         {
             _player.InitializeMove();
             _player.InitVelocity();
             _player.CapsuleCollider.enabled = true;

             _drone.InitFollowPosition();
         });

        AddAction(MessageTitles.scene_restarted, (msg) =>
        {
            _player.InitStatus();
        });

        AddAction(MessageTitles.playermanager_addDamageToPlayer, (msg) =>
        {
            FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
            _player.TakeDamage(data.value);
        });

        AddAction(MessageTitles.playermanager_initPlayerStatus, (msg) =>
        { 
            _player.InitStatus(); 
        });

        AddAction(MessageTitles.playermanager_getPlayer,(msg)=>{
            var receiver = (MessageReceiver)msg.sender;
            SendMessageQuick(receiver,MessageTitles.playermanager_getPlayer,_player);
        });

        AddAction(MessageTitles.playermanager_hidePlayer, (msg) =>
        {
            bool visible = (bool)msg.data;
            _drone.gameObject.SetActive(visible);
            _player.gameObject.SetActive(visible);

            if(visible)
            {
                _player.ChangeState(PlayerUnit.defaultState);
            }
        });

        AddAction(MessageTitles.playermanager_ragdoll,(msg)=>{
            _player.Ragdoll.SlidingRagdoll(Vector3.zero);
        });

        AddAction(MessageTitles.playermanager_droneText,(msg)=>{
            var data = MessageDataPooling.CastData<StringData>(msg.data);
            _drone.DroneTextCall(data.value);
        });

        AddAction(MessageTitles.playermanager_setDroneVolume, (msg) =>
         {
             var data = MessageDataPooling.CastData<FloatData>(msg.data);
             _drone.GetComponent<VolumeChanger>().SetVolume(data.value);
         });

        
        AddAction(MessageTitles.playermanager_setSpineRotation, (msg) =>
         {
             var data = MessageDataPooling.CastData<Vector3Data>(msg.data);
             _player.addibleSpineVector = data.value;
         });

        AddAction(MessageTitles.playermanager_LightOnOffRadio, (msg) =>
        {
            var data = MessageDataPooling.CastData<BoolData>(msg.data);
            if(data.value)
            {
                _playerWeaponMat.SetVector("_EmissionColor", radioColor * 10f);
            }
            else
            {
                _playerWeaponMat.SetVector("_EmissionColor", radioColor * 0f);
            }
        });

        AddAction(MessageTitles.playermanager_resetScreenEffects, (msg) =>
        {
            _drone.ResetEffect();

            SetRadialBlurData blurData = MessageDataPooling.GetMessageData<SetRadialBlurData>();
            blurData.factor = .0f;
            blurData.radius = .0f;
            blurData.time = .0f;
            SendMessageEx(MessageTitles.cameramanager_setradialblur, UniqueNumberBase.GetSavedNumberStatic("CameraManager"), blurData);
        });

        AddAction(MessageTitles.playermanager_droneTextByKey, (msg) =>
         {
             _drone.DroneHelpCall((string)msg.data);
         });

        AddAction(MessageTitles.playermanager_droneTextAndDurationByKey, (msg) =>
        {
            var data = MessageDataPooling.CastData<DroneTextKeyAndDurationData>(msg.data);
            _drone.DroneHelpCall(data.key,data.duration);
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        //_playerFootIK = _player.GetComponent<IKCtrl>();

        //if(_playerFootIK == null)
        //{
        //    Debug.LogError("Not Exits Player in IKCtrl");
        //}

        if(bagRenderer == null)
        {
            Debug.Log("Not Set Bag Renderer");
        }
        else
        {
            _bagMatrial = bagRenderer.material;
        }

        if(playerWeaponRenderer == null)
        {
            Debug.Log("Not Set PlayerWeaponRender");
        }
        else
        {
            _playerWeaponMat = playerWeaponRenderer.material;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //_drone = _player.GetDrone();

        _player.hp.Subscribe(value =>
        {
            if(_bagMatrial != null)
            _bagMatrial.SetFloat("Vector1_5338de784f7d4439aba250082f9a53e3", value * 0.01f);

            StateBarSetValueType data = MessageDataPooling.GetMessageData<StateBarSetValueType>();
            data.type = UIManager.StateBarType.HP;
            data.value = value;
            if (_player.GetState == PlayerUnit.aimingState)
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
            if (_player.GetState == PlayerUnit.aimingState)
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

            if (_player.GetState == PlayerUnit.aimingState)
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
            data.value = value / _player.ChargeConsumeTime;
            SendMessageEx(MessageTitles.uimanager_setgunchargetimevalue, GetSavedNumber("UIManager"), data);
        });
        _player.energy.Subscribe(value =>
        {
            FloatData data = MessageDataPooling.GetMessageData<FloatData>();
            data.value = value;
            SendMessageEx(MessageTitles.uimanager_setgunenergyvalue, GetSavedNumber("UIManager"), data);
        });

        _drone.scanLeftCoolTime.Subscribe(value =>
        {
            FloatData data = MessageDataPooling.GetMessageData<FloatData>();
            data.value = 1f - value / _drone.ScanCoolTime;
            SendMessageEx(MessageTitles.uimanager_setScanCoolTimeValue, GetSavedNumber("UIManager"), data);
            if(data.value >= 1f)
            {
                BoolData activeData = MessageDataPooling.GetMessageData<BoolData>();
                activeData.value = false;
                SendMessageEx(MessageTitles.uimanager_visibleScanCoolTimeUi, GetSavedNumber("UIManager"), activeData);
            }
        });

        _player.CurrentQuickStandCoolTime.Subscribe(value =>
        {
            FloatData data = MessageDataPooling.GetMessageData<FloatData>();
            data.value = Mathf.Clamp(value / _player.QuickStandCoolTime, 0.0f, 1.0f);
            SendMessageEx(MessageTitles.uimanager_setFactorQuickStandingCoolTime, GetSavedNumber("UIManager"), data);
        });

        _player.CurrentDashCoolTime.Subscribe(value =>
        {
            FloatData data = MessageDataPooling.GetMessageData<FloatData>();
            data.value = Mathf.Clamp(value / _player.DashCoolTime, 0.0f, 1.0f);
            SendMessageEx(MessageTitles.uimanager_setFactorDashCoolTime, GetSavedNumber("UIManager"), data);
        });
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if (LevelEdit_TimelinePlayer.CUTSCENEPLAY == true)
            return;

        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            SendMessageEx(MessageTitles.scene_loadCurrentLevel, GetSavedNumber("SceneManager"), null);
        }

        if(Keyboard.current.leftCtrlKey.isPressed && Keyboard.current.nKey.wasPressedThisFrame)
        {
            SendMessageEx(MessageTitles.scene_loadNextLevel, GetSavedNumber("SceneManager"), null);
        }


        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            _player.TakeDamage(0f);
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

    public class DroneTextKeyAndDurationData :MessageData
    {
        public string key;
        public float duration;
    }
}
