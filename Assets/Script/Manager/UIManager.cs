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
    [SerializeField] private CrossHair _crossHair;

    [Header("StateUI")]
    [SerializeField] private FadeUI _hpBar;
    [SerializeField] private FadeUI _staminaBar;
    [SerializeField] private FadeUI _energyBar;
    [SerializeField] private HpPackUI _hpPackUI;
    [SerializeField] private DamageEffect damageEffect;

    [Header("GunUI")]
    [SerializeField] private Canvas gunUiCanvas;
    [SerializeField] private TextMeshProUGUI gunLoadValueText;
    [SerializeField] private TextMeshProUGUI gunChargeValueText;
    [SerializeField] private GunGageUi aimEnergyBar;

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

    private EventSystem _eventSystem;

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
    }

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("UIManager",true);

        MessageDataPooling.RegisterMessageData<StateBarSetValueType>();
        MessageDataPooling.RegisterMessageData<HpPackValueType>();

        AddAction(MessageTitles.uimanager_activecrosshair, ActiveCrossHair);
        AddAction(MessageTitles.uimanager_setcrosshairphase, SetCrossHairPhase);

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
        });
        AddAction(MessageTitles.uimanager_setgunenergyvalue, (msg) =>
        {
            FloatData data = MessageDataPooling.CastData<FloatData>(msg.data);
            aimEnergyBar.SetBackValue(data.value);
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
            soundPlay.id = 1303; soundPlay.position = Vector3.zero; soundPlay.returnValue = false; soundPlay.dontStop = false;
            SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundPlay);
         });
    }

    public override void Initialize()
    {
        base.Initialize();

        SendMessageEx(MessageTitles.videomanager_settargetimage, GetSavedNumber("VideoManager"), videoRawImage);

        fadeCanvas.enabled = false;
    }

    public void OnPauseButton()
    {
        if (_currentPauseState == PauseMenuState.Loading || _currentPauseState == PauseMenuState.GameOver)
            return;

        if(_currentPauseState == PauseMenuState.Game)
        {
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

        //if(Keyboard.current.nKey.wasPressedThisFrame)
        //{
        //    SendMessageEx(MessageTitles.uimanager_activeInGameTutorial, GetSavedNumber("UIManager"), InGameTutorialCtrl.InGameTutorialType.Climbing);
        //}
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
    #endregion

    #region StateBar
    public void SetValueStateBar(Message msg)
    {
        StateBarSetValueType recv = MessageDataPooling.CastData<StateBarSetValueType>(msg.data);

        switch(recv.type)
        {
            case StateBarType.HP:
                _hpBar.SetValue(recv.value,recv.visible);
                break;
            case StateBarType.Stamina:
                _staminaBar.SetValue(recv.value,recv.visible);
                break;
            case StateBarType.Energy:
                _energyBar.SetValue(recv.value, recv.visible);
                break;
        }
    }

    public void SetValueHpPackUI(Message msg)
    {
        HpPackValueType recv = MessageDataPooling.CastData<HpPackValueType>(msg.data);

        _hpPackUI.SetValue(recv.value, recv.visible);
    }

    public void SetVisibleAllStateBar(Message msg)
    {
        BoolData data = MessageDataPooling.CastData<BoolData>(msg.data);
        _hpBar.SetVisible(data.value);
        _staminaBar.SetVisible(data.value);
        _energyBar.SetVisible(data.value);
        _hpPackUI.SetVisible(data.value);
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
        fadeImage.DOFade(1.0f, 0.5f).SetUpdate(true).OnComplete(()=>action?.Invoke());
    }

    public void FadeOut(Action action = null)
    {
        fadeImage.DOFade(0.0f, 0.5f).SetUpdate(true).OnComplete(() => { fadeCanvas.enabled = false; action?.Invoke(); });
    }


    IEnumerator DeferredCallFadeOutAction(float duration, Action fadeOutAction)
    {
        yield return new WaitForSeconds(duration);
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
        fadeImage.DOFade(1.0f, 1.0f).SetUpdate(true).OnComplete(() =>
        {
            StartCoroutine(DeferredCallFadeOutAction(1f*0.8f,action));
            fadeImage.DOFade(0.0f, 1.0f).SetUpdate(true).SetDelay(1f).OnComplete(()=> fadeCanvas.enabled = false);
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
}
//public class HpPackValueType : MessageData
//{
//    public int value;
//    public bool visible;
//}

