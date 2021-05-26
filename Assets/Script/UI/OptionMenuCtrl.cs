using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class OptionMenuCtrl : MonoBehaviour
{
    public enum MenuType
    {
        Sound = 0, Control, Display, Key, Option,None
    }

    public Image backGroundImage;
    public LeftOptionTitle titlePanel;
    public EscMenu optionItemPanel;
    [SerializeField]private MenuType currentMenu = MenuType.None;
    public MenuType CurrentMenuState => currentMenu;

    private EscMenu _currentPanel = null;
    public EscMenu optionPanel;
    public EscMenu soundPanel;
    public EscMenu controlPanel;
    public EscMenu displayPanel;
    public EscMenu keyBindingPanel;
    public EscMenu gameOverPanel;

    public SceneLoadUI sceneLoadUi;
    
    public TutorialVideoPlayer tutorialVideoPlayer;
    public RespawnFadeCtrl respawnFadeCtrl;

    public UnityEvent WhenCloseOption;

    private bool _currentTutorial = false;
    void Start()
    {
        Color color=backGroundImage.color;
        color.a = 0;
        backGroundImage.color = color;

        if (GameManager.Instance.player != null)
            GameManager.Instance.player.whenPlayerDead += () => { gameOverPanel.Active(true); };
    }

    void Update()
    {
        //if (InputManager.Instance.GetInput(KeybindingActions.Option))
        //{
        //    InputEsc();
        //}

        if (_currentTutorial != true)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            tutorialVideoPlayer.ThroughPage();
        }
        
        if (InputManager.Instance.GetInput(KeybindingActions.Cancel)) 
        { 
            tutorialVideoPlayer.Active(false);
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
                    currentMenu = MenuType.Option;
                    GameManager.Instance.PAUSE = true;
                    if(GameManager.Instance.cameraManager != null)
                    GameManager.Instance.cameraManager.ActiveAimCamera();
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    _currentPanel = optionPanel;

                    backGroundImage.DOFade(0.09f, 0.3f).OnComplete(() =>
                    {
                        titlePanel.Appear(0.25f);
                        optionItemPanel.Appear(0.25f);
                    });
                }
                break;
            case MenuType.Option:
                {
                    currentMenu = MenuType.None;

                    if(GameManager.Instance.player != null)
                      GameManager.Instance.player.Resume();
                    if(GameManager.Instance.followTarget)
                      GameManager.Instance.followTarget.Resume();

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    titlePanel.Disappear(0.25f);
                    optionItemPanel.Disappear(0.25f,()=>
                    {
                        backGroundImage.DOFade(0.0f, 0.3f).OnComplete(() =>
                        {
                            GameManager.Instance.PAUSE = false;
                            if (GameManager.Instance.cameraManager != null)
                                GameManager.Instance.cameraManager.ActivePlayerFollowCamera();
                        });
                    });
                    
                    WhenCloseOption?.Invoke();
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
        Debug.Log("OptionChange");
        currentMenu = (MenuType)menuType;
        EscMenu prevPanel = _currentPanel;
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
        titlePanel.ChangeOption((MenuType)menuType, () => {
            prevPanel.Active (false);
            _currentPanel.Active (true);
        }, 1f);
    }

    public bool TutorialEvent(string key)
    {
        if (tutorialVideoPlayer.SetPage(key) == false)
            return false;
        
        tutorialVideoPlayer.Active(true);
        _currentTutorial = true;
        return true;
    }

    public void DisableSceneLoadUI()
    {
        if (sceneLoadUi == null)
            return;
        sceneLoadUi.EndLoad();
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
