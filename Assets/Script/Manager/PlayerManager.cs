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
    [SerializeField] private PlayerInput playerInput;
    private IKCtrl _playerFootIK;
    [SerializeField] private EMPGun _emp;
    [SerializeField] private Renderer bagRenderer;
    [SerializeField] private Renderer playerWeaponRenderer;
    [SerializeField] private Color radioColor;
    [SerializeField] private Transform radioLightPosition;
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
             _drone.transform.SetParent(null);
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
             if(_player.GetState == PlayerUnit.ragdollState)
                _player.Ragdoll.ResetRagdoll();
             _player.CapsuleCollider.enabled = true;

             _drone.InitFollowPosition();
             _player.hp.Value = 100f;
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

            //Debug.Log("Tlqkf :" + visible + "," + ((MessageReceiver)msg.sender).name);

            if (visible)
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
                FlickRadio();
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
            var data = MessageDataPooling.CastData<DroneTextKeyAndDurationData>(msg.data);
            _drone.DroneHelpCall(data.key);
         });

        AddAction(MessageTitles.playermanager_droneTextAndDurationByKey, (msg) =>
        {
            var data = MessageDataPooling.CastData<DroneTextKeyAndDurationData>(msg.data);
            _drone.DroneHelpCall(data.key,data.duration);
        });

        AddAction(MessageTitles.playermanager_DeactiveDialog, (msg) =>
        {
            _drone.DeactiveDialog();
        });

        AddAction(MessageTitles.playermanager_SetDialogName, (msg) =>
        {
            _drone.SetDialogName((string)msg.data);
        });

        AddAction(MessageTitles.playermanager_ActiveInput, (msg) =>
         {
             playerInput.ActivateInput();
         });

        AddAction(MessageTitles.playermanager_DeactivateInput, (msg) =>
        {
            playerInput.DeactivateInput();
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
            if (value >= _player.ChargeConsumeTime && _player.Energy >= _player.ChargeGunCost && _player.ChargeShotBlock == false)
            {
                //crossHair.Third();
                SendMessageEx(MessageTitles.uimanager_setChargeComplete, GetSavedNumber("UIManager"), null);
            }

            var chargeData = MessageDataPooling.GetMessageData<FloatData>();
            chargeData.value = value / _player.ChargeConsumeTime;
            SendMessageEx(MessageTitles.uimanager_chargeGageValue, GetSavedNumber("UIManager"), chargeData);
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

        //_drone.scanLeftCoolTime.Subscribe(value =>
        //{
        //    FloatData data = MessageDataPooling.GetMessageData<FloatData>();
        //    data.value = 1f - value / _drone.ScanCoolTime;
        //    SendMessageEx(MessageTitles.uimanager_setScanCoolTimeValue, GetSavedNumber("UIManager"), data);
        //    if(data.value >= 1f)
        //    {
        //        BoolData activeData = MessageDataPooling.GetMessageData<BoolData>();
        //        activeData.value = false;
        //        SendMessageEx(MessageTitles.uimanager_visibleScanCoolTimeUi, GetSavedNumber("UIManager"), activeData);
        //    }
        //});

        //_player.CurrentQuickStandCoolTime.Subscribe(value =>
        //{
        //    FloatData data = MessageDataPooling.GetMessageData<FloatData>();
        //    data.value = Mathf.Clamp(value / _player.QuickStandCoolTime, 0.0f, 1.0f);
        //    SendMessageEx(MessageTitles.uimanager_setFactorQuickStandingCoolTime, GetSavedNumber("UIManager"), data);
        //});

        //_player.CurrentDashCoolTime.Subscribe(value =>
        //{
        //    FloatData data = MessageDataPooling.GetMessageData<FloatData>();
        //    data.value = Mathf.Clamp(value / _player.DashCoolTime, 0.0f, 1.0f);
        //    SendMessageEx(MessageTitles.uimanager_setFactorDashCoolTime, GetSavedNumber("UIManager"), data);
        //});
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        //if (Keyboard.current.digit1Key.wasPressedThisFrame)
        //{
        //    SendMessageEx(MessageTitles.playermanager_SetDialogName, GetSavedNumber("PlayerManager"), "테스트");
        //    var data = MessageDataPooling.GetMessageData<DroneTextKeyAndDurationData>();
        //    data.key = "Test_1";
        //    data.duration = 10f;
        //    SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey, GetSavedNumber("PlayerManager"), data);
        //}

        //if (Keyboard.current.digit2Key.wasPressedThisFrame)
        //{
        //    SendMessageEx(MessageTitles.playermanager_SetDialogName, GetSavedNumber("PlayerManager"), "테스트");
        //    var data = MessageDataPooling.GetMessageData<DroneTextKeyAndDurationData>();
        //    data.key = "Test_2";
        //    data.duration = 10f;
        //    SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey, GetSavedNumber("PlayerManager"), data);
        //}

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

        //if (Keyboard.current.zKey.wasPressedThisFrame)
        //{
        //    _player.TakeDamage(0f);
        //}
    }

    public void FlickRadio()
    {
        StartCoroutine(Filck());
    }

    private IEnumerator Filck()
    {
        float intencity = 1f;
        float time = 0.0f;

        while(time<= 0.5f)
        {
            time += Time.deltaTime;
            intencity = Mathf.Lerp(1f, 500f, time / 0.5f);
            _playerWeaponMat.SetVector("_EmissionColor", radioColor * intencity);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        time = 0.0f;
        while (time <= 0.3f)
        {
            time += Time.deltaTime;
            intencity = Mathf.Lerp(500f, 10f, time / 0.3f);
            _playerWeaponMat.SetVector("_EmissionColor", radioColor * intencity);
            yield return null;
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
