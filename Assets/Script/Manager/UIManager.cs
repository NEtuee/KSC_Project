using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.InputSystem;
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

    [Header("TutorialMenu")]
    [SerializeField] private RawImage videoRawImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Fade")]
    [SerializeField] private Canvas fadeCanvas;
    [SerializeField] private Image fadeImage;

    [Header("LoadingUI")]
    [SerializeField] private LoadingUI loadingUI;

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
        if(_currentPage != null)
            _currentPage.Active(false);
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
        fadeImage.DOFade(1.0f, 0.5f).OnComplete(()=>action?.Invoke());
    }

    public void FadeOut(Action action = null)
    {
        fadeImage.DOFade(0.0f, 0.5f).OnComplete(() => { fadeCanvas.enabled = false; action?.Invoke(); });
    }
    #endregion

    #region LoadingUI

    public void ActiveLoadingUI(bool active)
    {
        if(active)
        {
            FadeIn(() => loadingUI.Active(true));
        }
        else
        {
            FadeOut(() => loadingUI.Active(false));
        }
    }

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
