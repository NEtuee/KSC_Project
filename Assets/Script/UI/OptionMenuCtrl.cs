using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public enum TutorialType { Climbing, Move, Special, Scan, Emp}
public enum MenuType
{
    Sound = 0, Control, Display, Key, Option, Pause, Tutorial, None
}

public class OptionMenuCtrl : MonoBehaviour
{

    public Canvas escMenuCanvas;
    public Image backGroundImage;
    public LeftOptionTitle titlePanel;
    public EscMenu optionItemPanel;
    [SerializeField]private MenuType currentMenu = MenuType.None;
    public MenuType CurrentMenuState => currentMenu;

    private EscMenu _currentPanel = null;
    private EscMenu _prevPanel = null;
    public EscMenu pausePanel;
    public EscMenu optionPanel;
    public EscMenu soundPanel;
    public EscMenu controlPanel;
    public EscMenu displayPanel;
    public EscMenu keyBindingPanel;
    public EscMenu gameOverPanel;
    public EscMenu tutorialPanel;

    public SceneLoadUI sceneLoadUi;
    
    public TutorialVideoPlayer tutorialVideoPlayer;
    public RespawnFadeCtrl respawnFadeCtrl;

    public UnityEvent WhenCloseOption;

    public InGameTutorialPanel currnetInGameTutorial;
    public InGameTutorialPanel climbingTutorialPanel;
    public InGameTutorialPanel moveTutorialPanel;
    public InGameTutorialPanel specialTutorial;
    public InGameTutorialPanel scanTutorial;
    public InGameTutorialPanel empTutorial;

    private bool _currentTutorial = false;
    public bool CurrentTutorial { get => _currentTutorial; set { _currentTutorial = value; }}
    void Start()
    {
        if (backGroundImage != null)
        {
            Color color = backGroundImage.color;
            color.a = 0;
            backGroundImage.color = color;
        }

        if (GameManager.Instance.player != null)
            GameManager.Instance.player.whenPlayerDead += () => { gameOverPanel.Active(true); };

        escMenuCanvas.enabled = false;

        currentMenu = MenuType.None;

        pausePanel.Init();
        optionPanel.Init();
        soundPanel.Init();
        controlPanel.Init();
        displayPanel.Init();
        keyBindingPanel.Init();
    }

    void Update()
    {
        //if (InputManager.Instance.GetInput(KeybindingActions.Option))
        //{
        //    InputEsc();
        //}

        if (_currentTutorial != true)
            return;

        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    tutorialVideoPlayer.ThroughPage();
        //}

        if (InputManager.Instance.GetInput(KeybindingActions.Cancel))
        {
            currnetInGameTutorial.Active(false);
            _currentTutorial = false;
        }

    }

    public void InputEsc()
    {
        if (_currentTutorial == true)
            return;
        
        switch(currentMenu)
        {
            case MenuType.None:
                {
                    currentMenu = MenuType.Pause;
                    GameManager.Instance.PAUSE = true;
                    if(GameManager.Instance.cameraManager != null)
                    GameManager.Instance.cameraManager.ActiveAimCamera();
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    escMenuCanvas.enabled = true;
                    _currentPanel = pausePanel;

                    //backGroundImage.DOFade(0.09f, 0.3f).OnComplete(() =>
                    //{
                    //    titlePanel.Appear(0.25f);
                    //    optionItemPanel.Appear(0.25f);
                    //});
                    pausePanel.Active(true);
                }
                break;
            case MenuType.Pause:
                {
                    currentMenu = MenuType.None;

                    if(GameManager.Instance.player != null)
                      GameManager.Instance.player.Resume();
                    if(GameManager.Instance.followTarget)
                      GameManager.Instance.followTarget.Resume();

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    GameManager.Instance.PAUSE = false;
                    if (GameManager.Instance.cameraManager != null)
                        GameManager.Instance.cameraManager.ActivePlayerFollowCamera();
                    //titlePanel.Disappear(0.25f);
                    //optionItemPanel.Disappear(0.25f,()=>
                    //{
                    //    backGroundImage.DOFade(0.0f, 0.3f).OnComplete(() =>
                    //    {
                    //        GameManager.Instance.PAUSE = false;
                    //        if (GameManager.Instance.cameraManager != null)
                    //            GameManager.Instance.cameraManager.ActivePlayerFollowCamera();
                    //    });
                    //});
                    pausePanel.Active(false);
                    escMenuCanvas.enabled = false;

                    WhenCloseOption?.Invoke();
                }
                break;
            case MenuType.Option:
                {
                    currentMenu = MenuType.Pause;
                    optionPanel.Active(false);
                    pausePanel.Active(true);
                }
                break;
            case MenuType.Tutorial:
                {
                    currentMenu = MenuType.Pause;
                    tutorialPanel.Active(false);
                    pausePanel.Active(true);
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
            case MenuType.Pause:
                _currentPanel = pausePanel;
                break;
            case MenuType.Tutorial:
                _currentPanel = tutorialPanel;
                break;
        }
        _currentPanel.Active(true);
    }

    public bool TutorialEvent(string key)
    {
        //if (tutorialVideoPlayer.SetPage(key) == false)
        //    return false;
        
        //tutorialVideoPlayer.Active(true);
        //_currentTutorial = true;
        return true;
    }

    public void DisableSceneLoadUI()
    {
        if (sceneLoadUi == null)
            return;
        sceneLoadUi.EndLoad();
    }

    public void InGameTutorial(TutorialType type)
    {
        switch(type)
        {
            case TutorialType.Climbing:
                {
                    currnetInGameTutorial = climbingTutorialPanel;
                }
                break;
            case TutorialType.Move:
                {
                    currnetInGameTutorial = moveTutorialPanel;
                }
                break;
            case TutorialType.Special:
                {
                    currnetInGameTutorial = specialTutorial;
                }
                break;
            case TutorialType.Scan:
                {
                    currnetInGameTutorial = scanTutorial;
                }
                break;
            case TutorialType.Emp:
                {
                    currnetInGameTutorial = empTutorial;
                }
                break;
        }

        currnetInGameTutorial.Active(true);
    }


    public void GameQuit()
    {
        Debug.Log("Exit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }

    public void OnTitleButton()
    {
        sceneLoadUi.FadeScreen(1.0f, 1.0f, GameManager.Instance.LoadTitleScene);
    }
}
