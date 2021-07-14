using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.InputSystem;

public class UIManager : ManagerBase
{
    public InputAction pauseAction;

    [Header("PauseUI")]
    [SerializeField] private PauseMenuState _currentPauseState;
    private MenuPage _currentPage;
    [SerializeField] private MenuPage optionPage;
    [SerializeField] private MenuPage soundPage;
    [SerializeField] private MenuPage displayPage;
    [SerializeField] private MenuPage controlPage;
    [SerializeField] private MenuPage keybindingPage;

    [Header("CrossHair")]
    [SerializeField] private CrossHair _crossHair;

    [Header("StateUI")]
    [SerializeField] private FadeUI _hpBar;
    [SerializeField] private FadeUI _staminaBar;
    [SerializeField] private FadeUI _energyBar;

    private void Start()
    {
        pauseAction.performed += _ => OnPauseButton();

        if(_crossHair == null)
        {
            Debug.Log("Not Set CrossHair");
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
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public void OnPauseButton()
    {
        if (_currentPauseState == PauseMenuState.Loading)
            return;

        if(_currentPauseState == PauseMenuState.Game)
        {
            SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), true);
            ActivePage((int)PauseMenuState.Option);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if(_currentPauseState == PauseMenuState.Option)
        {
            SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), false);
            ActivePage((int)PauseMenuState.Game);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            ActivePage((int)PauseMenuState.Option);
        }
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

    public void SetVisibleAllStateBar(Message msg)
    {
        bool visibe = (bool)msg.data;
        _hpBar.SetVisible(visibe);
        _staminaBar.SetVisible(visibe);
        _energyBar.SetVisible(visibe);
    }
    #endregion

    public enum PauseMenuState
    {
        Game = 0, Option, Sound, Display, Control, KeyBinding, Loading
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
