using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OptionMenuCtrl : MonoBehaviour
{
    public enum MenuType
    {
        Sound = 0, Control, Display, Key, Option,None
    }

    public Image backGroundImage;
    public LeftOptionTitle titlePanel;
    public EscMenu optionItemPanel;
    [SerializeField]private MenuType _currentMenu = MenuType.None;

    private EscMenu _currnetPanel = null;
    public EscMenu optionPanel;
    public EscMenu soundPanel;
    public EscMenu controlPanel;
    public EscMenu displayPanel;
    public EscMenu keyBindingPanel;
    public EscMenu gameOverPanel;

    public TutorialVideoPlayer tutorialVideoPlayer;

    private bool _currentTutorial = false;
    void Start()
    {
        Color color=backGroundImage.color;
        color.a = 0;
        backGroundImage.color = color;

        if( GameManager.Instance.player != null)
        GameManager.Instance.player.whenPlayerDead += () => { gameOverPanel.Active(true);};
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
        
        switch(_currentMenu)
        {
            case MenuType.None:
                {
                    _currentMenu = MenuType.Option;
                    GameManager.Instance.PAUSE = true;
                    if(GameManager.Instance.cameraManager != null)
                    GameManager.Instance.cameraManager.ActiveAimCamera();
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    _currnetPanel = optionPanel;

                    backGroundImage.DOFade(0.09f, 0.3f).OnComplete(() =>
                    {
                        titlePanel.Appear(0.25f);
                        optionItemPanel.Appear(0.25f);
                    });
                }
                break;
            case MenuType.Option:
                {
                    _currentMenu = MenuType.None;

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
        _currentMenu = (MenuType)menuType;
        EscMenu prevPanel = _currnetPanel;
        switch ((MenuType)menuType)
        {
            case MenuType.Sound:
                _currnetPanel = soundPanel;
                break;
            case MenuType.Control:
                _currnetPanel = controlPanel;
                break;
            case MenuType.Display:
                _currnetPanel = displayPanel;
                break;
            case MenuType.Key:
                _currnetPanel = keyBindingPanel;
                break;
            case MenuType.Option:
                _currnetPanel = optionPanel;
                break;
        }
        titlePanel.ChangeOption((MenuType)menuType, () => {
            prevPanel.Active (false);
            _currnetPanel.Active (true);
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
    
    public void GameQuit()
    {
        Debug.Log("Exit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}
