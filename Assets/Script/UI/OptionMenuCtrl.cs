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

        if (Input.GetKeyDown(KeyCode.J))
        {
            tutorialVideoPlayer.SetVideo(1);
            tutorialVideoPlayer.Active(true);
            tutorialVideoPlayer.SetAndPrepareVideo(1);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            tutorialVideoPlayer.SetVideo(0);
            tutorialVideoPlayer.Active(true);
            tutorialVideoPlayer.SetAndPrepareVideo(2);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            tutorialVideoPlayer.Active(false);
        }
    }

    public void InputEsc()
    {
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
