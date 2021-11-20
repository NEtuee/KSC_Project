using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using MD;

public class UIManager : ManagerBase
{
    public InputAction pauseAction;

    [Header("PauseUI")]
    [SerializeField] private PauseMenuState _currentPauseState;
    private MenuPage _currentPage;
    [SerializeField] private MenuPage pausePage;
    [SerializeField] private MenuPage optionPage;
    [SerializeField] private MenuPage soundPage;
    [SerializeField] private MenuPage displayPage;
    [SerializeField] private MenuPage controlPage;
    [SerializeField] private MenuPage keybindingPage;
    [SerializeField] private MenuPage tutorialPage;
    [SerializeField] private MenuPage gameoverPage;
    [SerializeField] private Canvas backGroundCanvas;

    [Header("CrossHair")]
    [SerializeField] private Canvas crossHairCanvas;
    [SerializeField] private CrossHair _crossHair;

    [Header("StateUI")]
    [SerializeField] private Canvas stateUiCanvas;
    [SerializeField] private FadeUI statusUi;
    [SerializeField] private FadeUI _hpBar;
    [SerializeField] private EnergyUI _energyIcon;
    [SerializeField] private FadeUI _staminaBar;
    [SerializeField] private FadeUI _energyBar;
    [SerializeField] private HpPackUI _hpPackUI;
    [SerializeField] private DamageEffect damageEffect;

    [Header("GunUI")]
    [SerializeField] private Canvas gunUiCanvas;
    [SerializeField] private TextMeshProUGUI gunLoadValueText;
    [SerializeField] private TextMeshProUGUI gunChargeValueText;
    [SerializeField] private GunGageUi aimEnergyBar;
    [SerializeField] private Image aimEnergyBarImage;
    [SerializeField] private Image aimEnergyBar2;

    [Header("TutorialMenu")]
    [SerializeField] private RawImage videoRawImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private InGameTutorialCtrl inGameTutorialCtrl;

    [Header("Fade")]
    [SerializeField] private Canvas fadeCanvas;
    [SerializeField] private Image fadeImage;

    [Header("LoadingUI")]
    [SerializeField] private LoadingUI loadingUI;

    [Header("SettingSlider")]
    [SerializeField] private Slider yawRotateSpeedSlider;
    [SerializeField] private Slider pitchRotateSpeedSlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

    [Header("SettingDropdown")]
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown vsyncDropdown;

    [Header("GuideText")]
    [SerializeField] private GuideText guideText;

    [Header("ScanMaker")]
    [SerializeField] private Canvas scanMakerCanvas;
    [SerializeField] private ScanMakerPool scanMakerPool;

    //[Header("DroneUI")]
    //[SerializeField] private Canvas droneUiCanvas;
    //[SerializeField] private Image droneCoolTimeCircle;

    //[Header("3D Ojbect UI")]
    //[SerializeField] private HorizontalGageCtrl quickStandingGage;
    //[SerializeField] private HorizontalGageCtrl dashGage;

    [Header("KeyGuide Sprite")]
    [SerializeField] private Image keyGuideImage;
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Sprite gamepadSprite;

    [Header("Drone Status UI")]
    [SerializeField] private DroneStatusUI droneStatusUI;

    [Header("Stack Camera Canvas")]
    [SerializeField] private Transform stackCameraCanvasTransform;
    private bool _stackCameraCanvasShake = false;
    private float _stackCameraCanvasShakeTime = 0.0f;
    private float _stackCameraCanvasShakePower = 0.0f;
    private float _stackCameraCanvasShakeCurrentPower = 0.0f;
    private Vector3 _stackCameraCanvasInitPosition = new Vector3();

    [Header("CrossHair")]
    [SerializeField] private Image centerGage;
    private const int MAX_LOAD_COUNT = 4;
    [SerializeField] private Image[] loadCountUi = new Image[MAX_LOAD_COUNT];
    [SerializeField] private Color highlightColor = Color.white;
    private int prevLoadCount;

    [Header("TargetMaker")]
    [SerializeField] private TargetMakerUI targetMakerUi;

    [Header("LevelLineUI")]
    [SerializeField] private LevelLineUI levelLineUi;

    [Header("MissionUI")]
    [SerializeField] private MissionUI missionUi;

    [Header("InformationUI")]
    [SerializeField] private InformationUI informationUi;
    

    private EventSystem _eventSystem;
    private PlayerUnit _player;

    private void Start()
    {
        pauseAction.performed += _ => OnPauseButton();

        if(_crossHair == null)
        {
            Debug.LogError("Not Set CrossHair");
        }

        if (_staminaBar == null)
        {
            Debug.LogError("Not Set StaminaBar");
        }

        if (_energyBar == null)
        {
            Debug.LogError("Not Set EnergyBar");
        }

        if (_hpPackUI == null)
        {
            Debug.LogError("Not Set HpPackUi");
        }

        if(videoRawImage == null)
        {
            Debug.LogError("Not Set VideoRawImage");
        }

        if (descriptionText == null)
        {
            Debug.LogError("Not Set DescriptionText");
        }

        if(fadeImage == null)
        {
            Debug.LogError("Not Set FadeImage");
        }

        if(GameObject.Find("EventSystem").TryGetComponent<EventSystem>(out _eventSystem) == false)
        {
            Debug.LogError("Not Exist EventSystem");
        }

        if (damageEffect == null)
        {
            Debug.LogError("Not Set DamageEffect");
        }

        if (stackCameraCanvasTransform != null)
        {
            _stackCameraCanvasInitPosition = stackCameraCanvasTransform.localPosition;
        }
    }

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("UIManager",true);

        MessageDataPooling.RegisterMessageData<StateBarSetValueType>();
        MessageDataPooling.RegisterMessageData<HpPackValueType>();

        AddAction(MessageTitles.uimanager_activecrosshair, ActiveCrossHair);
        AddAction(MessageTitles.uimanager_setChargeComplete, (msg)=>
        {
            _crossHair.ChargeComplete();
        });
        AddAction(MessageTitles.uimanager_chargeGageValue, (msg) =>
        {
            var data = MessageDataPooling.CastData<FloatData>(msg.data);
            _crossHair.SetChargeGageValue(data.value);
        });

        AddAction(MessageTitles.uimanager_setvaluestatebar, SetValueStateBar);
        AddAction(MessageTitles.uimanager_setvisibleallstatebar, SetVisibleAllStateBar);
        AddAction(MessageTitles.uimanager_setvaluehppackui, SetValueHpPackUI);

        AddAction(MessageTitles.uimanager_settutorialdescription, (msg) =>
        {
            StringData data = MessageDataPooling.CastData<StringData>(msg.data);
            SetDescription(data.value);
        });

        AddAction(MessageTitles.uimanager_fadein, (msg) => FadeIn());
        AddAction(MessageTitles.uimanager_fadeout, (msg) => FadeOut());

        AddAction(MessageTitles.uimanager_activeloadingui, (msg) => 
        {
            BoolData data = MessageDataPooling.CastData<BoolData>(msg.data);
            ActiveLoadingUI(data.value);
        });
        AddAction(MessageTitles.uimanager_setloadinggagevalue, (msg) => 
        {
            FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
            loadingUI.SetLoadingGageValue(data.value);
        });
        AddAction(MessageTitles.uimanager_setloadingtiptext, (msg) =>
        {
            StringData data = MessageDataPooling.CastData<StringData>(msg.data);
            loadingUI.SetLoadingTipText(data.value);
        });

        AddAction(MessageTitles.uimanager_setvaluecamerarotatespeedslider, (msg) =>
         {
             CameraRotateSpeedData data = MessageDataPooling.CastData<CameraRotateSpeedData>(msg.data);
             SetValueCameraRotateSpeedSlider(data.yaw, data.pitch);
         });
        AddAction(MessageTitles.uimanager_setvaluevolumeslider, (msg) =>
        {
            VolumeData data = MessageDataPooling.CastData<VolumeData>(msg.data);
            SetValueVolumeSlider(data.master, data.sfx,data.ambient, data.bgm);
        });

        AddAction(MessageTitles.uimanager_setresolutiondropdown, (msg) => 
        {
            ResolutionData data = MessageDataPooling.CastData<ResolutionData>(msg.data);
            resolutionDropdown.AddOptions(data.resolutionStrings);
        });

        AddAction(MessageTitles.uimanager_setvalueresolutiondropdown,(msg)=>
        {
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            resolutionDropdown.value = data.value;
            Debug.Log("Set Resolution");
        });
        AddAction(MessageTitles.uimanager_setvaluescreenmodedropdown, (msg) =>
        {
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            screenModeDropdown.value = data.value;
        });
        AddAction(MessageTitles.uimanager_setvaluevsyncdropdown, (msg) =>
        {
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            vsyncDropdown.value = data.value;
            Debug.Log("Set Vsync");
        });

        AddAction(MessageTitles.uimanager_fadeinout, (msg) =>
        {
            ActionData data = MessageDataPooling.CastData<ActionData>(msg.data);
            FadeInOut(data.value);
        });

        AddAction(MessageTitles.uimanager_setgunloadvalue, (msg) =>
        {
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            gunLoadValueText.text = data.ToString();
        });
        AddAction(MessageTitles.uimanager_setgunchargetimevalue, (msg) => 
        {
            FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
            gunChargeValueText.text = ((int)(data.value * 100.0f)).ToString();
            aimEnergyBar.SetFrontValue(data.value);
            if(data.value == 1.0f)
            {
                aimEnergyBarImage.color = Color.blue;
            }
            else
            {
                aimEnergyBarImage.color = Color.white;
            }

            centerGage.fillAmount = data.value;
        });

        AddAction(MessageTitles.uimanager_setgunenergyvalue, (msg) =>
        {
            FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
            aimEnergyBar.SetBackValue(data.value);
            aimEnergyBar2.fillAmount = data.value / 100.0f;

            int loadCount = (int)(data.value / _player.NoramlGunCost);

            if (prevLoadCount == loadCount)
                return;

            for(int i = 0; i<MAX_LOAD_COUNT; i++)
            {
                if(i < loadCount)
                {
                    loadCountUi[i].color = highlightColor;
                }
                else
                {
                    loadCountUi[i].color = Color.white;
                }
            }

            prevLoadCount = loadCount;
        });

        AddAction(MessageTitles.uimanager_activegunui, (msg) =>
        {
            BoolData data = MessageDataPooling.CastData<BoolData>(msg.data);
            gunUiCanvas.enabled = data.value;
        });

        AddAction(MessageTitles.uimanager_getUimanager, (msg) =>
         {
             SendMessageQuick((MessageReceiver)msg.sender, MessageTitles.set_setUimanager, this);
         });

        AddAction(MessageTitles.scene_beforeSceneChange, (msg) =>
         {
             ActiveLoadingUI(true);
         });

        AddAction(MessageTitles.scene_sceneChanged, (msg) =>
        {
            ActiveLoadingUI(false);
        });

        AddAction(MessageTitles.uimanager_activeInGameTutorial, (msg) =>
         {
             InGameTutorialTypeData data = MessageDataPooling.CastData<InGameTutorialTypeData>(msg.data);
             inGameTutorialCtrl.Active(data.type);
             _currentPauseState = PauseMenuState.InGameTutorial;
             Cursor.lockState = CursorLockMode.None;
             Cursor.visible = true;
             BoolData setTimeStop = MessageDataPooling.GetMessageData<BoolData>();
             setTimeStop.value = true;
             SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), setTimeStop);
         });

        AddAction(MessageTitles.uimanager_damageEffect, (msg) =>
         {
             damageEffect.Effect();
         });

        AddAction(MessageTitles.uimanager_activeGameOverUi, (msg) =>
        {
            ActiveGameOverUI();
        });

        AddAction(MessageTitles.uimanager_activeScanMaker, (msg) =>
         {
             var maker = scanMakerPool.Active();
             ScanMakerData data = MessageDataPooling.CastData<ScanMakerData>(msg.data);
             maker.Active(data.collider);//data.center, data.min, data.max);

            SoundPlayData soundPlay = MessageDataPooling.GetMessageData<SoundPlayData>();
            soundPlay.id = 1303; soundPlay.position = Camera.main.transform.position; soundPlay.returnValue = false; soundPlay.dontStop = false;
            SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundPlay);
         });

        //AddAction(MessageTitles.uimanager_visibleScanCoolTimeUi, (msg) =>
        // {
        //     BoolData data = MessageDataPooling.CastData<BoolData>(msg.data);
        //     droneUiCanvas.enabled = data.value;
        // });

        //AddAction(MessageTitles.uimanager_setScanCoolTimeValue, (msg) =>
        //{
        //    FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
        //    droneCoolTimeCircle.fillAmount = data.value;
        //});

        AddAction(MessageTitles.uimanager_setChargingTextColor, (msg) =>
        {
            //ColorData data = MessageDataPooling.CastData<ColorData>(msg.data);
            //gunChargeValueText.color = data.value;
        });

        //AddAction(MessageTitles.uimanager_setFactorQuickStandingCoolTime, (msg) =>
        // {
        //     FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
        //     if(quickStandingGage != null)
        //         quickStandingGage.SetFactor(data.value);
        // });

        //AddAction(MessageTitles.uimanager_setFactorDashCoolTime, (msg) =>
        //{
        //    FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
        //    if (dashGage != null)
        //        dashGage.SetFactor(data.value);
        //});

        AddAction(MessageTitles.uimanager_enableDroneStatusUi, (msg) =>
        {
            BoolData data = MessageDataPooling.CastData<BoolData>(msg.data);
            droneStatusUI.Enable(data.value);
        });

        AddAction(MessageTitles.uimanager_setDroneHpValue, (msg) =>
        {
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            droneStatusUI.SetHpCount(data.value);
        });

        AddAction(MessageTitles.uimanager_shakeStackCameraCanvas, (msg) =>
        {
            ShakeStackCameraData data = MessageDataPooling.CastData<ShakeStackCameraData>(msg.data);
            StartCoroutine(ShakeStackCameraCanvas(data.time, data.power));
        });

        AddAction(MessageTitles.uimanager_shakeAmountStackCameraCanvas, (msg) =>
        {
            ShakeStackCameraData data = MessageDataPooling.CastData<ShakeStackCameraData>(msg.data);
            StartCoroutine(ShakeAmountStackCameraCanvas(data.time, data.power));
        });


        AddAction(MessageTitles.set_setplayer, (msg) =>
        {
            _player = (PlayerUnit)msg.data;
        });

        AddAction(MessageTitles.uimanager_activeTargetMakerUiAndSetTarget, (msg) =>
         {
             targetMakerUi.gameObject.SetActive(true);
             targetMakerUi.Target = (Transform)msg.data;
         });

        AddAction(MessageTitles.uimanager_DisableTargetMakerUi, (msg) =>
        {
            targetMakerUi.gameObject.SetActive(false);
        });

        AddAction(MessageTitles.uimanager_ActiveLeveLineUIAndSetBossName, (msg) =>
        {
            var data = (string)msg.data;
            levelLineUi.SetBossName(data);
            levelLineUi.Appear();
        });

        AddAction(MessageTitles.uimanager_AppearMissionUiAndSetKey, (msg) =>
        {
            var data = (string)msg.data;
            missionUi.SetText(data);
            missionUi.Appear();
        });

        AddAction(MessageTitles.uimanager_DisappearMissionUi, (msg) =>
        {
            missionUi.Dissapear();
        });

        AddAction(MessageTitles.uimanager_SetLevelLineAlphabet, (msg) =>
        {
            var data = MessageDataPooling.CastData<LevelLineAlphabetData>(msg.data);
            levelLineUi.SetAlphabet(data.value);
        });

        AddAction(MessageTitles.uimanager_AppearInformationUi, (msg) =>
        {
            informationUi.Appear((string)msg.data);
        });

        AddAction(MessageTitles.uimanager_SetShowTimeInformationUi, (msg) =>
        {
            var data = MessageDataPooling.CastData<FloatData>(msg.data);
            informationUi.ShowTime = data.value;
        });

        AddAction(MessageTitles.uimanager_activePlayUi, (msg) =>
         {
             var data = MessageDataPooling.CastData<BoolData>(msg.data);
             if (data.value == false)
             {
                 crossHairCanvas.enabled = data.value;
                 statusUi.SetVisible(false,0.1f);
             }
         });
    }

    public override void Initialize()
    {
        base.Initialize();

        SendMessageEx(MessageTitles.videomanager_settargetimage, GetSavedNumber("VideoManager"), videoRawImage);
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        fadeCanvas.enabled = false;
    }

    public void OnPauseButton()
    {
        if (_currentPauseState == PauseMenuState.Loading || _currentPauseState == PauseMenuState.GameOver)
            return;

        if(_currentPauseState == PauseMenuState.Game)
        {
            if (PlayerUnit.GamepadMode == true)
                keyGuideImage.sprite = gamepadSprite;
            else
                keyGuideImage.sprite = keyboardSprite;

            BoolData setTimeStop = MessageDataPooling.GetMessageData<BoolData>();
            setTimeStop.value = true;
            SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), setTimeStop);
            ActivePage((int)PauseMenuState.Pause);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }


        if (_currentPauseState == PauseMenuState.Pause)
        {
            BoolData setTimeStop = MessageDataPooling.GetMessageData<BoolData>();
            setTimeStop.value = false;
            SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), setTimeStop);
            ActivePage((int)PauseMenuState.Game);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        if(_currentPauseState == PauseMenuState.InGameTutorial)
        {
            BoolData setTimeStop = MessageDataPooling.GetMessageData<BoolData>();
            setTimeStop.value = false;
            SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), setTimeStop);
            inGameTutorialCtrl.Disable();
            _currentPauseState = PauseMenuState.Game;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            return;
        }

        if(_currentPauseState == PauseMenuState.Tutorial || _currentPauseState == PauseMenuState.Option)
        {
            ActivePage((int)PauseMenuState.Pause);
            return;
        }
        else
        {
            ActivePage((int)PauseMenuState.Option);
            return;
        }


        //else
        //{
        //    ActivePage((int)PauseMenuState.Option);
        //}

        //if (_currentPauseState == PauseMenuState.Option)
        //{
        //    SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), false);
        //    ActivePage((int)PauseMenuState.Game);

        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        //else
        //{
        //    ActivePage((int)PauseMenuState.Option);
        //}
    }



    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            SendMessageEx(MessageTitles.uimanager_ActiveLeveLineUIAndSetBossName, GetSavedNumber("UIManager"), "이우민");
        }

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            SendMessageEx(MessageTitles.uimanager_AppearMissionUiAndSetKey, GetSavedNumber("UIManager"), "Test");
        }
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            SendMessageEx(MessageTitles.uimanager_DisappearMissionUi, GetSavedNumber("UIManager"), null);
        }
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            FloatData data = MessageDataPooling.GetMessageData<FloatData>();
            data.value = 6f;
            SendMessageEx(MessageTitles.uimanager_SetShowTimeInformationUi, GetSavedNumber("UIManager"), data);
            SendMessageEx(MessageTitles.uimanager_AppearInformationUi, GetSavedNumber("UIManager"), "Test");
        }
    }

    public void ActivePage(int pageNum)
    {
        if (_currentPage != null)
        {
            switch (_currentPauseState)
            {
                case PauseMenuState.Control:
                    {
                        CameraRotateSpeedData data = MessageDataPooling.GetMessageData<CameraRotateSpeedData>();
                        data.yaw = yawRotateSpeedSlider.value;
                        data.pitch = pitchRotateSpeedSlider.value;
                        SendMessageEx(MessageTitles.setting_savecamerarotatespeed, GetSavedNumber("SettingManager"), data);
                    }
                    break;
                case PauseMenuState.Sound:
                    {
                        VolumeData data = MessageDataPooling.GetMessageData<VolumeData>();
                        data.master = masterVolumeSlider.value;
                        data.sfx = sfxVolumeSlider.value;
                        data.ambient = ambientVolumeSlider.value;
                        data.bgm = bgmVolumeSlider.value;
                        SendMessageEx(MessageTitles.setting_saveVolume, GetSavedNumber("SettingManager"), data);

                        FloatData droneVolume = MessageDataPooling.GetMessageData<FloatData>();
                        droneVolume.value = masterVolumeSlider.value * sfxVolumeSlider.value;
                        SendMessageEx(MessageTitles.playermanager_setDroneVolume, GetSavedNumber("PlayerManager"), droneVolume);
                    }
                    break;
                case PauseMenuState.Tutorial:
                    backGroundCanvas.enabled = true;
                    SendMessageEx(MessageTitles.videomanager_stopvideo, GetSavedNumber("VideoManager"), null);
                    break;
            }
            _currentPage.Active(false);
        }

        _currentPauseState = (PauseMenuState)pageNum;
        switch (_currentPauseState)
        {
            case PauseMenuState.Game:
                _currentPage = null;
                backGroundCanvas.enabled = false;
                return;
            case PauseMenuState.Pause:
                _currentPage = pausePage;
                backGroundCanvas.enabled = true;
                break;
            case PauseMenuState.Option:
                _currentPage = optionPage;
                break;
            case PauseMenuState.Sound:
                _currentPage = soundPage;
                break;
            case PauseMenuState.Display:
                _currentPage = displayPage;
                break;
            case PauseMenuState.Control:
                _currentPage = controlPage;
                break;
            case PauseMenuState.KeyBinding:
                _currentPage = keybindingPage;
                break;
            case PauseMenuState.Tutorial:
                _currentPage = tutorialPage;
                backGroundCanvas.enabled = false;
                break;
        }
        _currentPage.Active(true);
    }

    #region CrossHair
    public void ActiveCrossHair(Message msg)
    {
        BoolData data = MessageDataPooling.CastData<BoolData>(msg.data);
        _crossHair.SetActive(data.value);
    }

    public void SetCrossHairPhase(Message msg)
    {
        IntData data = MessageDataPooling.CastData<IntData>(msg.data);
        switch(data.value)
        {
            case 1:
                _crossHair.First();
                break;
            case 2:
                _crossHair.Second();
                break;
            case 3:
                _crossHair.Third();
                break;
        }
    }

    public void SetChargeComplete()
    {
        
    }


    #endregion

    #region StateBar
    public void SetValueStateBar(Message msg)
    {
        StateBarSetValueType recv = MessageDataPooling.CastData<StateBarSetValueType>(msg.data);

        switch(recv.type)
        {
            case StateBarType.HP:
                _hpBar.SetValue(recv.value, false);
                break;
            case StateBarType.Stamina:
                _staminaBar.SetValue(recv.value,recv.visible);
                break;
            case StateBarType.Energy:
                _energyIcon.SetValue(recv.value, false);
                break;
        }
        statusUi.SetVisible(recv.visible);
    }

    public void SetValueHpPackUI(Message msg)
    {
        HpPackValueType recv = MessageDataPooling.CastData<HpPackValueType>(msg.data);

        _hpPackUI.SetValue(recv.value, recv.visible);
    }

    public void SetVisibleAllStateBar(Message msg)
    {
        BoolData data = MessageDataPooling.CastData<BoolData>(msg.data);

        statusUi.SetVisible(data.value);

        //_hpBar.SetVisible(data.value);
        //_staminaBar.SetVisible(data.value);
        //_energyBar.SetVisible(data.value);
        //_hpPackUI.SetVisible(data.value);
    }
    #endregion

    #region TutorialUI
    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }
    #endregion

    #region Fade
    public void FadeIn(Action action = null)
    {
        fadeCanvas.enabled = true;
        fadeImage.DOFade(1.0f, 0.3f).SetUpdate(true).OnComplete(()=>action?.Invoke());
    }

    public void FadeOut(Action action = null)
    {
        fadeImage.DOFade(0.0f, 0.3f).SetUpdate(true).OnComplete(() => { fadeCanvas.enabled = false; action?.Invoke(); });
    }


    IEnumerator DeferredCallFadeOutAction(float duration, Action fadeOutAction)
    {
        yield return new WaitForSecondsRealtime(duration);
        fadeOutAction?.Invoke();
    }
    #endregion

    #region LoadingUI

    public void ActiveLoadingUI(bool active)
    {
        if(active)
        {
            _currentPauseState = PauseMenuState.Loading;
            FadeIn(() => loadingUI.Active(true));
        }
        else
        {
            loadingUI.Active(false);
            FadeOut(()=> {
                _currentPauseState = PauseMenuState.Game;
            });
        }
    }

    public void FadeInOut(Action action)
    {
        fadeCanvas.enabled = true;
        fadeImage.DOFade(1.0f, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            StartCoroutine(DeferredCallFadeOutAction(0.5f*0.8f,action));
            fadeImage.DOFade(0.0f, 0.5f).SetUpdate(true).SetDelay(0.5f).OnComplete(()=> fadeCanvas.enabled = false);
        });
    }
    #endregion

    #region SettingSlider

    public void SetValueCameraRotateSpeedSlider(float yaw, float pitch)
    {
        yawRotateSpeedSlider.value = yaw;
        pitchRotateSpeedSlider.value = pitch;
    }

    public void SetValueVolumeSlider(float master, float vfx, float ambient, float bgm)
    {
        masterVolumeSlider.value = master;
        sfxVolumeSlider.value = vfx;
        ambientVolumeSlider.value = ambient;
        bgmVolumeSlider.value = bgm;
    }

    #endregion

    #region UISound

    public void PlayEnterSound()
    {
        SoundPlayData soundPlay = MessageDataPooling.GetMessageData<SoundPlayData>();
        soundPlay.id = 3000; soundPlay.position = Vector3.zero; soundPlay.returnValue = false; soundPlay.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundPlay);

        SetParameterData paramData = MessageDataPooling.GetMessageData<SetParameterData>();
        paramData.soundId = 3000; paramData.paramId = 30001; paramData.value = 0;
        SendMessageEx(MessageTitles.fmod_setParam, GetSavedNumber("FMODManager"), paramData);
    }

    public void PlayClickSound()
    {
        SoundPlayData soundPlay = MessageDataPooling.GetMessageData<SoundPlayData>();
        soundPlay.id = 3000; soundPlay.position = Vector3.zero; soundPlay.returnValue = false; soundPlay.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundPlay);

        SetParameterData paramData = MessageDataPooling.GetMessageData<SetParameterData>();
        paramData.soundId = 3000; paramData.paramId = 30001; paramData.value = 1;
        SendMessageEx(MessageTitles.fmod_setParam, GetSavedNumber("FMODManager"), paramData);
    }

    #endregion

    #region SettingDropDown

    public void RequestSetScreenMode()
    {
        IntData data = MessageDataPooling.GetMessageData<IntData>();
        data.value = screenModeDropdown.value;
        SendMessageEx(MessageTitles.setting_setScreenMode, GetSavedNumber("SettingManager"),data);
    }

    public void RequestSetResolution()
    {
        Debug.Log("RequestSetResolution");
        IntData data = MessageDataPooling.GetMessageData<IntData>();
        data.value = resolutionDropdown.value;
        SendMessageEx(MessageTitles.setting_setResolution, GetSavedNumber("SettingManager"), data);
    }

    public void RequestSetVsync()
    {
        IntData data = MessageDataPooling.GetMessageData<IntData>();
        data.value = vsyncDropdown.value;
        SendMessageEx(MessageTitles.setting_setVsync, GetSavedNumber("SettingManager"), data);
    }

    #endregion

    public void SetGuideTextDescription(string key)
    {
        guideText.SetDescription(key);
    }

    public void SetGuideTextSetSpace()
    {
        guideText.SetSpace();
    }

    public void ActiveGameOverUI()
    {
        gameoverPage.Active(true);
        _currentPauseState = PauseMenuState.GameOver;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnRestartButton()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameoverPage.Active(false);
        //PositionRotation data = MessageDataPooling.GetMessageData<PositionRotation>();
        //data.position = Vector3.zero;
        //data.rotation = Quaternion.identity;
        //SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);
        //SendMessageEx(MessageTitles.playermanager_initPlayerStatus, GetSavedNumber("PlayerManager"), null);
        SendMessageEx(MessageTitles.scene_loadRestartLevel, GetSavedNumber("SceneManager"), null);
    }

    public void OnTitleButton()
    {
        BoolData timeStop = MessageDataPooling.GetMessageData<BoolData>();
        timeStop.value = false;
        SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), timeStop);
        MD.StringData data = MessageDataPooling.GetMessageData<MD.StringData>();
        data.value = "MainTitle_NewStucture";
        SendMessageEx(MessageTitles.scene_loadSceneNotAsync, GetSavedNumber("SceneManager"), data);
    }

    private IEnumerator ShakeStackCameraCanvas(float time, float power)
    {
        float curTime = time;
        float curPower = power;

        while(curTime > 0.0f)
        {
            Vector3 shakeFactor = UnityEngine.Random.insideUnitCircle* power;
            stackCameraCanvasTransform.localPosition =  shakeFactor + _stackCameraCanvasInitPosition;

            curTime -= Time.deltaTime;
            yield return null;
        }

        stackCameraCanvasTransform.localPosition =  _stackCameraCanvasInitPosition;
    }

    private IEnumerator ShakeAmountStackCameraCanvas(float time, float power)
    {
        float curTime = time;
        float curPower = power;
        float amount = 1f;

        while (curTime > 0.0f)
        {
            Vector3 shakeFactor = UnityEngine.Random.insideUnitCircle * power * amount;
            stackCameraCanvasTransform.localPosition = shakeFactor + _stackCameraCanvasInitPosition;
            amount = Mathf.Lerp(amount, 0f, curTime / time);

            curTime -= Time.deltaTime;
            yield return null;
        }

        stackCameraCanvasTransform.localPosition = _stackCameraCanvasInitPosition;
    }

    public enum PauseMenuState
    {
        Game = 0, Pause,Option, Sound, Display, Control, KeyBinding, Loading, Tutorial, InGameTutorial, GameOver
    }

    public enum StateBarType
    {
        HP,Stamina,Energy
    }


    private void OnEnable()
    {
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }
}

namespace MD
{
    public class StateBarSetValueType : MessageData
    {
        public UIManager.StateBarType type;
        public float value;
        public bool visible;
    }

    public class HpPackValueType : MessageData
    {
        public int value;
        public bool visible;
    }

    public class ScanMakerData : MessageData
    {
        public Collider collider;
        // public Vector3 center;
        // public Vector3 min;
        // public Vector3 max;
    }

    public class ShakeStackCameraData : MessageData
    {
        public float time;
        public float power;
    }
}
//public class HpPackValueType : MessageData
//{
//    public int value;
//    public bool visible;
//}

