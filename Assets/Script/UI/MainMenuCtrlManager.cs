using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MD;
using UnityEngine.EventSystems;

public class MainMenuCtrlManager : ManagerBase
{
    public InputAction pauseButton;

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
    [SerializeField] private GameObject gameStartButton;
    [SerializeField] private GameObject soundMenuButton;
    [SerializeField] private GameObject displayMenuButton;
    [SerializeField] private GameObject controlMenuButton;

    [SerializeField] private MainMenuState _current = MainMenuState.Main;
    private MenuPage _currentPage;

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

    [Header("FadeCanvas")]
    [SerializeField] private Canvas fadeCanvas;
    [SerializeField] private Image fadeImage;

    [Header("GameScene")]
    [SerializeField] private string playerScene;

    private EventSystem _eventSystem;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("UIManager");

        AddAction(MessageTitles.uimanager_setvaluecamerarotatespeedslider, (msg) =>
         {
             CameraRotateSpeedData data = MessageDataPooling.CastData<CameraRotateSpeedData>(msg.data);
             yawRotateSpeedSlider.value = data.yaw;
             pitchRotateSpeedSlider.value = data.pitch;
         });
        AddAction(MessageTitles.uimanager_setresolutiondropdown, (msg) =>
         {
             ResolutionData data = MessageDataPooling.CastData<ResolutionData>(msg.data);
             resolutionDropdown.AddOptions(data.resolutionStrings);
         });

        AddAction(MessageTitles.uimanager_setvaluescreenmodedropdown, (msg) =>
         {
             IntData data = MessageDataPooling.CastData<IntData>(msg.data);
             screenModeDropdown.value = data.value;
         });
        AddAction(MessageTitles.uimanager_setvaluevsyncdropdown, (msg) =>
        {
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            vsyncDropdown.value = data.value;
            Debug.Log("Set Vsync");
        });

        AddAction(MessageTitles.uimanager_setvaluevolumeslider, (msg) =>
         {
             VolumeData data = MessageDataPooling.CastData<VolumeData>(msg.data);
             masterVolumeSlider.value = data.master;
             sfxVolumeSlider.value = data.sfx;
             ambientVolumeSlider.value = data.ambient;
             bgmVolumeSlider.value = data.bgm;
         });

        AddAction(MessageTitles.uimanager_setvalueresolutiondropdown, (msg) =>
        {
            Debug.Log("set resolution");
            IntData data = MessageDataPooling.CastData<IntData>(msg.data);
            resolutionDropdown.value = data.value;
        });


        if (GameObject.Find("EventSystem").TryGetComponent<EventSystem>(out _eventSystem) == false)
        {
            Debug.LogError("Not Exist EventSystem");
        }
    }

    private void Start()
    {
        pauseButton.performed += _ => Prev();
        _currentPage = mainPage;

        fadeCanvas.enabled = true;
        fadeImage.color = Color.black;
        fadeImage.DOFade(0f, 1f).OnComplete(() =>
        {
            _currentPage.Active(true);
            fadeCanvas.enabled = false;
        });
        //_currentPage = mainPage;
        //_currentPage.Active(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Prev()
    {
        if (_current == MainMenuState.Main)
            return;

        if(_current == MainMenuState.Option)
        {
            ActivePage((int)MainMenuState.Main);
            _eventSystem.SetSelectedGameObject(gameStartButton);
            return;
        }

        ActivePage((int)MainMenuState.Option);
    }

    public void ActivePage(int pageNum)
    {
        MainMenuState prevState = _current;

        switch (_current)
            {
                case MainMenuState.Option:
                    {
                        CameraRotateSpeedData data = MessageDataPooling.GetMessageData<CameraRotateSpeedData>();
                        data.yaw = yawRotateSpeedSlider.value;
                        data.pitch = pitchRotateSpeedSlider.value;
                        SendMessageEx(MessageTitles.setting_savecamerarotatespeed, GetSavedNumber("SettingManager"), data);

                        VolumeData volumedata = MessageDataPooling.GetMessageData<VolumeData>();
                        volumedata.master = masterVolumeSlider.value;
                        volumedata.sfx = sfxVolumeSlider.value;
                        volumedata.ambient = ambientVolumeSlider.value;
                        volumedata.bgm = bgmVolumeSlider.value;
                        SendMessageEx(MessageTitles.setting_saveVolume, GetSavedNumber("SettingManager"), volumedata);

                        if((MainMenuState)pageNum == MainMenuState.Main)
                            _currentPage.Active(false);

                }
                break;
            case MainMenuState.Sound:
                {
                    if ((MainMenuState)pageNum == MainMenuState.Option)
                        _eventSystem.SetSelectedGameObject(soundMenuButton);
                }
                break;
            case MainMenuState.Display:
                {
                    if ((MainMenuState)pageNum == MainMenuState.Option)
                        _eventSystem.SetSelectedGameObject(displayMenuButton);
                }
                break;
            case MainMenuState.Control:
                {
                    if ((MainMenuState)pageNum == MainMenuState.Option)
                        _eventSystem.SetSelectedGameObject(controlMenuButton);
                }
                break;
        }


        _current = (MainMenuState)pageNum;
        switch (_current)
        {
            case MainMenuState.Main:
                _currentPage = mainPage;
                _currentPage.Active(true);
                break;
            case MainMenuState.Option:
                _currentPage = optionPage;
                if (prevState == MainMenuState.Main)
                    _eventSystem.SetSelectedGameObject(soundMenuButton);
                _currentPage.Active(true);
                break;
            case MainMenuState.Sound:
                _currentPage = null;
                break;
            case MainMenuState.Display:
                _currentPage = null;
                break;
            case MainMenuState.Control:
                _currentPage = null;
                break;
            case MainMenuState.KeyBinding:
                _currentPage = keybindingPage;
                break;
        }
    }

    private void OnEnable()
    {
        pauseButton.Enable();
    }

    private void OnDisable()
    {
        pauseButton.Disable();
    }

    public void LogCheck()
    {
        Debug.Log("call");
    }

    public void PlayEnterSound()
    {
        SoundPlayData soundPlay = MessageDataPooling.GetMessageData<SoundPlayData>();
        soundPlay.id = 3000; soundPlay.position = Vector3.zero; soundPlay.returnValue = false; soundPlay.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundPlay);

        SetParameterData paramData = MessageDataPooling.GetMessageData<SetParameterData>();
        paramData.soundId = 3000; paramData.paramId = 30001; paramData.value = 0;
        SendMessageEx(MessageTitles.fmod_setParam, GetSavedNumber("FMODManager"), paramData);
    }

    public void PlayClickSound()
    {
        SoundPlayData soundPlay = MessageDataPooling.GetMessageData<SoundPlayData>();
        soundPlay.id = 3000; soundPlay.position = Vector3.zero; soundPlay.returnValue = false; soundPlay.dontStop = false;
        SendMessageEx(MessageTitles.fmod_play, GetSavedNumber("FMODManager"), soundPlay);

        SetParameterData paramData = MessageDataPooling.GetMessageData<SetParameterData>();
        paramData.soundId = 3000; paramData.paramId = 30001; paramData.value = 1;
        SendMessageEx(MessageTitles.fmod_setParam, GetSavedNumber("FMODManager"), paramData);
    }

    public void OnStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fadeCanvas.enabled = true;
        fadeImage.DOFade(1f, 1f).OnComplete(()=>
        {
            SceneManager.LoadScene(playerScene);
        });
    }

    public void RequestSetScreenMode()
    {
        IntData data = MessageDataPooling.GetMessageData<IntData>();
        data.value = screenModeDropdown.value;
        SendMessageEx(MessageTitles.setting_setScreenMode, GetSavedNumber("SettingManager"), data);
    }

    public void RequestSetResolution()
    {
        Debug.Log("RequestSetResolution");
        IntData data = MessageDataPooling.GetMessageData<IntData>();
        data.value = resolutionDropdown.value;
        SendMessageEx(MessageTitles.setting_setResolution, GetSavedNumber("SettingManager"), data);
    }

    public void RequestSetVsync()
    {
        IntData data = MessageDataPooling.GetMessageData<IntData>();
        data.value = vsyncDropdown.value;
        SendMessageEx(MessageTitles.setting_setVsync, GetSavedNumber("SettingManager"), data);
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_STANDALONE
        Application.Quit();
#endif

    }
}
