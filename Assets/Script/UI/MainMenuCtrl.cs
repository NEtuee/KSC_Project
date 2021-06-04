using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class MainMenuCtrl : MonoBehaviour
{
    public Canvas mainTitleCanvas;
    public Canvas fadePanel;
    public Image fadeImage;

    public TitleLogoCtrl title;
    public MainTitleButton startButton;
    public MainTitleButton optionButton;
    public MainTitleButton exitButton;

    public MenuType currentMenu;

    public Canvas optionCanvas;
    private EscMenu _currentPanel = null;
    private EscMenu _prevPanel = null;
    public EscMenu optionPanel;
    public EscMenu soundPanel;
    public EscMenu controlPanel;
    public EscMenu displayPanel;
    public EscMenu keyBindingPanel;

    private void Start()
    {
        title.Init();
        fadeImage.color = Color.black;
        FadeIn(()=> { title.Appear(); });

        optionCanvas.enabled = false;

        currentMenu = MenuType.None;

        optionPanel.Init();
        soundPanel.Init();
        controlPanel.Init();
        displayPanel.Init();
        keyBindingPanel.Init();
    }

    private void Update()
    {
        if (InputManager.Instance.GetInput(KeybindingActions.Option) && currentMenu != MenuType.None)
        {
            InputEsc();
        }
    }

    public void InputEsc()
    {  
        switch (currentMenu)
        {
            case MenuType.None:
                {
                    optionCanvas.enabled = true;
                    currentMenu = MenuType.Pause;                
                    _currentPanel = optionPanel;
                    optionPanel.Active(true);
                }
                break;
            case MenuType.Option:
                {
                    currentMenu = MenuType.None;

                    optionPanel.Active(false);
                    optionCanvas.enabled = false;

                    SetButtonInteractable(true);
                }
                break;
            default:
                {
                    Change(4);
                }
                break;
        }
    }

    public void Change(int menuType)
    {
        //Debug.Log("OptionChange");
        currentMenu = (MenuType)menuType;
        _prevPanel = _currentPanel;
        _prevPanel.Active(false);

        switch ((MenuType)menuType)
        {
            case MenuType.Sound:
                _currentPanel = soundPanel;
                break;
            case MenuType.Control:
                _currentPanel = controlPanel;
                break;
            case MenuType.Display:
                _currentPanel = displayPanel;
                break;
            case MenuType.Key:
                _currentPanel = keyBindingPanel;
                break;
            case MenuType.Option:
                _currentPanel = optionPanel;
                break;
        }
        _currentPanel.Active(true);
    }

    public void LoadPlayerScene()
    {
        FadeOut(() =>
        {
            SceneManager.LoadScene("PlayerScene_Character0426");
        });
    }

    private void FadeIn(TweenCallback complete)
    {
        fadeImage.DOFade(0f, 1f).OnComplete(()=> { complete?.Invoke(); fadePanel.enabled = false; });
    }

    private void FadeOut(TweenCallback complete)
    {
        fadePanel.enabled = true;
        fadeImage.DOFade(1f, 1f).OnComplete(complete);
    }

    public void OnOptionButton()
    {
        mainTitleCanvas.sortingOrder = 2;
    }

    public void OffOption()
    {
        mainTitleCanvas.sortingOrder = 4;
    }

    public void SetButtonInteractable(bool active)
    {
        startButton.interactable = active;
        optionButton.interactable = active;
        exitButton.interactable = active;
    }
}
