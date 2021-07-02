using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCtrlManager : MonoBehaviour
{
    public enum MainMenuState
    {
        Main = 0, Option, Sound, Display, Control, KeyBinding
    }

    [SerializeField] private MenuPage mainPage;
    [SerializeField] private MenuPage optionPage;
    [SerializeField] private MenuPage soundPage;
    [SerializeField] private MenuPage displayPage;
    [SerializeField] private MenuPage controlPage;
    [SerializeField] private MenuPage keybindingPage;

    [SerializeField] private MainMenuState _current = MainMenuState.Main;
    private MenuPage _currentPage;

    private void Start()
    {
        _currentPage = mainPage;
        _currentPage.Active(true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Prev();
        }
    }

    public void Prev()
    {
        if (_current == MainMenuState.Main)
            return;

        if(_current == MainMenuState.Option)
        {
            ActivePage((int)MainMenuState.Main);
            return;
        }

        ActivePage((int)MainMenuState.Option);
    }

    public void ActivePage(int pageNum)
    {
        _currentPage.Active(false);
        _current = (MainMenuState)pageNum;
        switch (_current)
        {
            case MainMenuState.Main:
                _currentPage = mainPage;
                break;
            case MainMenuState.Option:
                _currentPage = optionPage;
                break;
            case MainMenuState.Sound:
                _currentPage = soundPage;
                break;
            case MainMenuState.Display:
                _currentPage = displayPage;
                break;
            case MainMenuState.Control:
                _currentPage = controlPage;
                break;
            case MainMenuState.KeyBinding:
                _currentPage = keybindingPage;
                break;
        }
        _currentPage.Active(true);
    }
}
