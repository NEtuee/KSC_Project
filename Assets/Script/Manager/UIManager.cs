using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public enum GameState
{
    Title,
    Game,
    Pause,
    Result,
    Sound,
    GameOver,
    SceneReload,
    LoadResult
}
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    private GameState prevState;
    [SerializeField] private Transform titleCameraPosition;
    [SerializeField] private GameObject titleMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject crossHairPanel;
    [SerializeField] private Text startText;
    [SerializeField] private Text soundText;
    [SerializeField] private Text exitText;

    [Header("Sound Setting")]
    [SerializeField] private GameObject soundSettingPanel;

    [Header("Stamina Bar")]
    [SerializeField] private GameObject staminaPanel;
    [SerializeField] private GageBarUI staminaBar;
    [SerializeField] private Text staminaValue;

    [Header("Hp Bar")]
    [SerializeField] private GageBarUI hpBar;
    [SerializeField] private Text hpValue;

    [Header("Spear")]
    [SerializeField] private Text spearCount;

    [Header("GameOver Panel")]
    [SerializeField] private PanelCtrl gameOverPanel;

    [Header("Black Curtain")]
    [SerializeField] private Image blackCurtain;

    [Header("FButton Guide")]
    [SerializeField] private GameObject fButtonGuide;

    [Header("SoundSlider")]
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;

    [Header("Spear Guide")]
    [SerializeField] private GameObject specialSpearText;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Image resultBackGround;
    [SerializeField] private Image catImage;
    [SerializeField] private Text clearText;
    [SerializeField] private Text pressAnyKeyText;
    [SerializeField] private Text teamAdText;


    private Vector3 mainCameraStartPosition;

    //private PlayerCtrl player;
    private PlayerCtrl_State player;

    private void Awake()
    {
        GameManager.Instance.uiManager = this;
    }
    void Start()
    {
        //InitGame();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl_State>();
        player.stamina.SubscribeToText(staminaValue);
        player.stamina.Subscribe(value => 
        {
            staminaBar.SetValue(value/100f);
        });

        player.hp.SubscribeToText(hpValue);
        player.hp.Subscribe(value =>
        {
            hpBar.SetValue(value / 100f);
        });
    }

    private void InitGame()
    {
        GameManager.Instance.timeManager.ResumeTime();
        GameManager.Instance.uiManager = this;
        //mainCameraStartPosition = GameManager.Instance.GetMainCameraPosition();
        //GameManager.Instance.MainCameraSetWorldPosition(titleCameraPosition.position);
        gameState = GameState.Title;

        pauseMenu.SetActive(false);
        staminaPanel.SetActive(false);
        soundSettingPanel.SetActive(false);

        GameManager.Instance.CameraRootSetWorldPosition(titleCameraPosition.position);
        GameManager.Instance.PausePlayer();

        //ActiveMouse();

        if(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>() != null)
        {
            //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl_State>();
            player.OnAim += ActiveCrossHair;
            player.OnAimOff += DisableCrossHair;
            player.OnAimOff += UpdateSpearNum;
            player.OnDead += GameOver;
            player.OnSpearDrop += UpdateSpearNum;
            player.OnAbsorbAllCore += DisplayCanEquipSpear;
            UpdateSpearNum();
        }

        blackCurtain.gameObject.SetActive(true);
        StartCoroutine(BlackFadeOut());
    }

    void Update()
    {
        if(gameState == GameState.GameOver || gameState == GameState.Result)
        {
            if(Input.anyKeyDown)
            {
                gameState = GameState.SceneReload;
                StartCoroutine(SceneReload());
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.IsCurrentCameraEvent() == false)
        {
            SwitchPause();
        }

        if(gameState == GameState.Game)
        {
            //UpdateStaminaValue();
        }
    }

    private void FixedUpdate()
    {
        if(gameState == GameState.Game)
        {
            if(player.CheckInterantion() == true)
            {
                fButtonGuide.SetActive(true);
            }
            else
            {
                fButtonGuide.SetActive(false);
            }
        }
    }

    public void OnSoundButton()
    {
        Debug.Log("OnSoundButton");
        if (gameState == GameState.Title)
        {
            prevState = gameState;
            titleMenu.SetActive(false);
            soundSettingPanel.SetActive(true);
            gameState = GameState.Sound;
        }
        else if (gameState == GameState.Pause)
        {
            prevState = gameState;
            pauseMenu.SetActive(false);
            soundSettingPanel.SetActive(true);
            gameState = GameState.Sound;
        }
    }

    public void OnBackSoundMenu()
    {
        soundSettingPanel.SetActive(false);
        if (prevState == GameState.Title)
        {
            titleMenu.SetActive(true);
        }
        else if(prevState == GameState.Pause)
        {
            pauseMenu.SetActive(true);
        }
        gameState = prevState;
    }

    public void OnStartButton()
    {
        //StartCoroutine(GameStartCameraMove());
        //DisableMouse();
        //titleMenu.SetActive(false);

        //GameManager.Instance.ResumePlayerControl();
        //titleMenu.SetActive(false);
        //GameManager.Instance.ResumePlayerControl();

        StartCoroutine(GameStart());

        //gameState = GameState.Game;
    }

    IEnumerator GameStart()
    {
        DisableMouse();

        yield return StartCoroutine(FadeOutTitleMenu());

        titleMenu.SetActive(false);
        GameManager.Instance.ResumePlayerControl();

        staminaPanel.SetActive(true);

        StartCoroutine(GameManager.Instance.GameStartCameraMove());

        gameState = GameState.Game;
    }

    IEnumerator GameStartCameraMove()
    {
        titleMenu.SetActive(false);
        GameManager.Instance.ResumePlayerControl();

        yield return StartCoroutine(GameManager.Instance.GameStartCameraMove());
    }

    private void ActiveMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DisableMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SwitchPause()
    {
        if(!(gameState == GameState.Game || gameState == GameState.Pause||gameState == GameState.Sound))
        {
            return;
        }

        if(gameState == GameState.Game)
        {
            gameState = GameState.Pause;
            pauseMenu.SetActive(true);
            ActiveMouse();
            GameManager.Instance.PausePlayer();
            GameManager.Instance.timeManager.PauseTime();
        }
        else if (gameState == GameState.Pause)
        {
            gameState = GameState.Game;
            pauseMenu.SetActive(false);
            GameManager.Instance.ResumePlayerControl();
            GameManager.Instance.timeManager.ResumeTime();
        }
        else if (gameState == GameState.Sound)
        {
            OnBackSoundMenu();
        }
    }

    IEnumerator FadeOutTitleMenu()
    {
        Color color = startText.color;

        while(color.a > 0f)
        {
            color.a -= 1f * Time.deltaTime;
            startText.color = color;
            soundText.color = color;
            exitText.color = color;

            yield return null;
        }
    }

    public IEnumerator FadeOut(float speed)
    {
        Color color = blackCurtain.color;
        color.a = 1;
        blackCurtain.color = color;


        while (color.a > 0f)
        {
            color.a -= speed * Time.unscaledDeltaTime;
            blackCurtain.color = color;

            yield return null;
        }
    }

    public IEnumerator FadeIn(float speed)
    {
        Color color = blackCurtain.color;
        color.a = 0;
        blackCurtain.color = color;

        while (color.a < 1f)
        {
            color.a += speed * Time.unscaledDeltaTime;
            blackCurtain.color = color;

            yield return null;
        }

    }

    public void ActiveCrossHair()
    {
        crossHairPanel.SetActive(true);
    }

    private void DisableCrossHair()
    {
        crossHairPanel.SetActive(false);
    }

    private void GameOver()
    {
        StartCoroutine(GameOverProgress());
    }

    IEnumerator GameOverProgress()
    {
        titleMenu.SetActive(false);
        crossHairPanel.SetActive(false);

        gameOverPanel.gameObject.SetActive(true);
        GameManager.Instance.PausePlayer();

        GameManager.Instance.timeManager.PauseTime();

        yield return StartCoroutine(gameOverPanel.FadeIn());

        gameState = GameState.GameOver;
    }

    //private void SceneReload()
    //{
    //    GameManager.Instance.timeManager.ResumeTime();
    //}

    IEnumerator BlackFadeOut()
    {
        Color color = blackCurtain.color;
        color.a = 1;
        blackCurtain.color = color;

        yield return new WaitForSeconds(1f);

        while (color.a > 0f)
        {
            color.a -= 1f * Time.unscaledDeltaTime;
            blackCurtain.color = color;

            yield return null;
        }
    }

    IEnumerator BlackFadeIn()
    {
        Color color = blackCurtain.color;
        color.a = 0;
        blackCurtain.color = color;

        while (color.a < 1f)
        {
            color.a += 1f * Time.unscaledDeltaTime;
            blackCurtain.color = color;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);
    }

    IEnumerator SceneReload()
    {
        yield return StartCoroutine(BlackFadeIn());
        SceneManager.LoadScene(0);
    }

    public void OnTitleExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();        
#endif
    }

    public void OnMenuExit()
    {
        gameState = GameState.SceneReload;
        StartCoroutine(SceneReload());
    }

    private void UpdateSpearNum()
    {
        spearCount.text = player.GetCurrentSpearNum().ToString();
    }

    public void OnChangeVolumeMainSlider()
    {
        if(AudioManager.instance != null)
        AudioManager.instance.GetMainMixer().SetFloat("masterVolume", Mathf.Log(Mathf.Lerp(0.001f, 1f, mainSlider.value)) * 20);
    }

    public void OnChangeVolumeSfxSlider()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.GetMainMixer().SetFloat("sfxVolume", Mathf.Log(Mathf.Lerp(0.001f, 1f, sfxSlider.value)) * 20);
    }

    public void OnChangeVolumeBgmSlider()
    {
        //AudioManager.instance.GetMainMixer().SetFloat("sfxVolume", Mathf.Log(Mathf.Lerp(0.001f, 1f, mainSlider.value)) * 20);
        if (AudioManager.instance != null)
            AudioManager.instance.GetMainMixer().SetFloat("bgmVolume", Mathf.Log(Mathf.Lerp(0.001f, 1f, bgmSlider.value)) * 20);
    }

    public void DisplayCanEquipSpear()
    {
        specialSpearText.SetActive(true);
    }

    public void GameResult()
    {
        StartCoroutine(LoadResult());
    }

    IEnumerator LoadResult()
    {
        gameState = GameState.LoadResult;
        resultPanel.SetActive(true);

        Color color = resultBackGround.color;
        color.a = 0;
        resultBackGround.color = color;

        while (color.a < 1f)
        {
            color.a += 0.5f * Time.unscaledDeltaTime;
            resultBackGround.color = color;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        color = clearText.color;
        color.a = 0;
        clearText.color = color;

        while (color.a < 1f)
        {
            color.a += 1f * Time.unscaledDeltaTime;
            clearText.color = color;

            yield return null;
        }

        color = catImage.color;
        color.a = 0;
        catImage.color = color;
        teamAdText.color = color;

        while(color.a < 1f)
        {
            color.a += 2f * Time.unscaledDeltaTime;
            catImage.color = color;
            teamAdText.color = color;

            yield return null;
        }

        color = pressAnyKeyText.color;
        color.a = 0;
        pressAnyKeyText.color = color;

        while (color.a < 1f)
        {
            color.a += 1f * Time.unscaledDeltaTime;
            pressAnyKeyText.color = color;

            yield return null;
        }

        gameState = GameState.Result;
    }
}
