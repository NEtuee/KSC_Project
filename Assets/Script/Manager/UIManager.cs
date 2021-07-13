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

    private void Start()
    {
        pauseAction.performed += _ => OnPauseButton();
    }

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("UIManager");

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

    public enum PauseMenuState
    {
        Game = 0, Option, Sound, Display, Control, KeyBinding, Loading
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
