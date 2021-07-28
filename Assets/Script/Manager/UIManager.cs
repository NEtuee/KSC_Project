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

    [Header("CrossHair")]
    [SerializeField] private CrossHair _crossHair;

    [Header("StateUI")]
    [SerializeField] private FadeUI _hpBar;
    [SerializeField] private FadeUI _staminaBar;
    [SerializeField] private FadeUI _energyBar;
    [SerializeField] private HpPackUI _hpPackUI;

    [Header("GunUI")]
    [SerializeField] private Canvas gunUiCanvas;
    [SerializeField] private TextMeshProUGUI gunLoadValueText;
    [SerializeField] private TextMeshProUGUI gunChargeValueText;
    [SerializeField] private GunGageUi aimEnergyBar;

    [Header("TutorialMenu")]
    [SerializeField] private RawImage videoRawImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

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
        
    }

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("UIManager");

        AddAction(MessageTitles.uimanager_activecrosshair, ActiveCrossHair);
        AddAction(MessageTitles.uimanager_setcrosshairphase, SetCrossHairPhase);

        AddAction(MessageTitles.uimanager_setvaluestatebar, SetValueStateBar);
        AddAction(MessageTitles.uimanager_setvisibleallstatebar, SetVisibleAllStateBar);
        AddAction(MessageTitles.uimanager_setvaluehppackui, SetValueHpPackUI);

        AddAction(MessageTitles.uimanager_settutorialdescription, (msg) => SetDescription((string)msg.data));

        AddAction(MessageTitles.uimanager_fadein, (msg) => FadeIn());
        AddAction(MessageTitles.uimanager_fadeout, (msg) => FadeOut());

        AddAction(MessageTitles.uimanager_activeloadingui, (msg) => 
        {
            bool active = (bool)msg.data;
            ActiveLoadingUI(active);
        });
        AddAction(MessageTitles.uimanager_setloadinggagevalue, (msg) => 
        {
            float value = (float)msg.data;
            loadingUI.SetLoadingGageValue(value);
        });
        AddAction(MessageTitles.uimanager_setloadingtiptext, (msg) =>
        {
            string text = (string)msg.data;
            loadingUI.SetLoadingTipText(text);
        });

        AddAction(MessageTitles.uimanager_setvaluecamerarotatespeedslider, (msg) =>
         {
             CameraRotateSpeedData data = (CameraRotateSpeedData)msg.data;
             SetValueCameraRotateSpeedSlider(data.yaw, data.pitch);
         });
        AddAction(MessageTitles.uimanager_setvaluevolumeslider, (msg) =>
        {
            VolumeData data = (VolumeData)msg.data;
            SetValueVolumeSlider(data.master, data.sfx,data.ambient, data.bgm);
        });

        AddAction(MessageTitles.uimanager_setresolutiondropdown, (msg) => 
        {
            ResolutionData data = (ResolutionData)msg.data;
            resolutionDropdown.AddOptions(data.resolutionStrings);
        });

        AddAction(MessageTitles.uimanager_setvalueresolutiondropdown,(msg)=>
        {
            int value = (int)msg.data;
            resolutionDropdown.value = value;
        });
        AddAction(MessageTitles.uimanager_setvaluescreenmodedropdown, (msg) =>
        {
            int value = (int)msg.data;
            screenModeDropdown.value = value;
        });
        AddAction(MessageTitles.uimanager_setvaluevsyncdropdown, (msg) =>
        {
            int value = (int)msg.data;
            vsyncDropdown.value = value;
        });

        AddAction(MessageTitles.uimanager_fadeinout, (msg) =>
        {
            Action action = (Action)msg.data;
            FadeInOut(action);
        });

        AddAction(MessageTitles.uimanager_setgunloadvalue, (msg) =>
        {
            int value = (int)msg.data;
            gunLoadValueText.text = value.ToString();
        });
        AddAction(MessageTitles.uimanager_setgunchargetimevalue, (msg) => 
        {
            float value = (float)msg.data;
            gunChargeValueText.text = ((int)(value * 100.0f)).ToString();
            aimEnergyBar.SetFrontValue(value);
        });
        AddAction(MessageTitles.uimanager_setgunenergyvalue, (msg) =>
        {
            float value = (float)msg.data;
            aimEnergyBar.SetBackValue(value);
        });
        AddAction(MessageTitles.uimanager_activegunui, (msg) =>
        {
            bool active = (bool)msg.data;
            gunUiCanvas.enabled = active;
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
    }

    public override void Initialize()
    {
        base.Initialize();

        SendMessageEx(MessageTitles.videomanager_settargetimage, GetSavedNumber("VideoManager"), videoRawImage);

        fadeCanvas.enabled = false;
    }

    public void OnPauseButton()
    {
        if (_currentPauseState == PauseMenuState.Loading)
            return;

        if(_currentPauseState == PauseMenuState.Game)
        {
            SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), true);
            ActivePage((int)PauseMenuState.Pause);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }


        if (_currentPauseState == PauseMenuState.Pause)
        {
            SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), false);
            ActivePage((int)PauseMenuState.Game);

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

    public void ActivePage(int pageNum)
    {
        if (_currentPage != null)
        {
            switch (_currentPauseState)
            {
                case PauseMenuState.Control:
                    {
                        CameraRotateSpeedData data;
                        data.yaw = yawRotateSpeedSlider.value;
                        data.pitch = pitchRotateSpeedSlider.value;
                        SendMessageEx(MessageTitles.setting_savecamerarotatespeed, GetSavedNumber("SettingManager"), data);
                    }
                    break;
                case PauseMenuState.Sound:
                    {
                        VolumeData data;
                        data.master = masterVolumeSlider.value;
                        data.sfx = sfxVolumeSlider.value;
                        data.ambient = ambientVolumeSlider.value;
                        data.bgm = bgmVolumeSlider.value;
                        SendMessageEx(MessageTitles.setting_saveVolume, GetSavedNumber("SettingManager"), data);
                    }
                    break;
            }
            _currentPage.Active(false);
        }

        _currentPauseState = (PauseMenuState)pageNum;
        switch (_currentPauseState)
        {
            case PauseMenuState.Game:
                _currentPage = null;
                return;
            case PauseMenuState.Pause:
                _currentPage = pausePage;
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
                break;
        }
        _currentPage.Active(true);
    }

    #region CrossHair
    public void ActiveCrossHair(Message msg)
    {
        bool active = (bool)msg.data;
        _crossHair.SetActive(active);
    }

    public void SetCrossHairPhase(Message msg)
    {
        int phase = (int)msg.data;
        switch(phase)
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
        StateBarSetValueType recv = (StateBarSetValueType)msg.data;

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
        HpPackValueType recv = (HpPackValueType)msg.data;

        _hpPackUI.SetValue(recv.value, recv.visible);
    }

    public void SetVisibleAllStateBar(Message msg)
    {
        bool visibe = (bool)msg.data;
        _hpBar.SetVisible(visibe);
        _staminaBar.SetVisible(visibe);
        _energyBar.SetVisible(visibe);
        _hpPackUI.SetVisible(visibe);
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
        SoundPlayData soundPlay;
        soundPlay.id = 3000; soundPlay.position = Vector3.zero; soundPlay.returnValue = false; soundPlay.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundPlay);

        SetParameterData paramData;
        paramData.soundId = 3000; paramData.paramId = 30001; paramData.value = 0;
        SendMessageEx(MessageTitles.fmod_setParam, GetSavedNumber("FMODManager"), paramData);
    }

    public void PlayClickSound()
    {
        SoundPlayData soundPlay;
        soundPlay.id = 3000; soundPlay.position = Vector3.zero; soundPlay.returnValue = false; soundPlay.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundPlay);

        SetParameterData paramData;
        paramData.soundId = 3000; paramData.paramId = 30001; paramData.value = 1;
        SendMessageEx(MessageTitles.fmod_setParam, GetSavedNumber("FMODManager"), paramData);
    }

    #endregion

    #region SettingDropDown



    #endregion

    public enum PauseMenuState
    {
        Game = 0, Pause,Option, Sound, Display, Control, KeyBinding, Loading, Tutorial
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

struct StateBarSetValueType
{
    public UIManager.StateBarType type;
    public float value;
    public bool visible;
}

struct HpPackValueType
{
    public int value;
    public bool visible;
}
