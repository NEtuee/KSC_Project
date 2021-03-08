using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum MenuState { MenuOn,MenuOff}

    public BulletTimeManager timeManager;
    public float killEventFov = 66f;
    [SerializeField] public PlayerCtrl player;
    [SerializeField] public FollowTargetCtrl followTarget;
    [SerializeField] public UIManager uiManager;
    [SerializeField] public CameraManager cameraManger;
    [SerializeField] public LevelEdit_BehaviorControll bossControll;
    [SerializeField] private Transform coreTransfrom;
    [SerializeField] private Transform killEventTransform;
    [SerializeField] private HudTest hudTest;
    [SerializeField] private EscMainMenu escMainMenu;
    [SerializeField] private InputMenu inputMenu;
    [SerializeField] private EscMenu soundMenu;
    [SerializeField] public FMODSoundManager soundManager;
    [SerializeField] public StateManager stateManager;
    public Transform bossTransform;

    public List<LockOnTarget> lockOnTargets = new List<LockOnTarget>();

    private Vector3 mainCameraStartPosition;
    private Vector3 mainCameraStartLocalPosition;
    private Vector3 camRootStartPosition;

    private Quaternion camRootStartRot;

    private bool isCurrentCameraEvent;

    private bool isMenuBlending = false;

    private static GameManager instance;
    public static GameManager Instance { get { if (null == instance) { return null; } return instance; } }
    //카메라 이벤트를 위한 변수. 카메라 이벤트 전반 함수이전의 카메라 위치, 회전을 저장함
    Vector3 prevPos;
    Quaternion prevRot;
    Transform originalParent;

    private MenuState menuState = MenuState.MenuOff;

    //해상도 정보
    private float screenWidth;
    private float screenHeight;
    private Vector2 screenCenter;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("Boss") != null)
        {
            bossControll = GameObject.FindGameObjectWithTag("Boss").GetComponent<LevelEdit_BehaviorControll>();
        }

        screenWidth = Screen.width;
        screenHeight = Screen.height;
        screenCenter.Set(screenWidth * 0.5f, screenHeight * 0.5f);

        foreach(var lockTarget in GameObject.FindGameObjectsWithTag("LockOnTarget"))
        {
            lockOnTargets.Add(lockTarget.GetComponent<LockOnTarget>());
        }
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    mainCam.RequstChangePP(2f);
        //}

        //if (Input.GetKeyDown(KeyCode.Escape)&&hudTest != null && isMenuBlending == false)
        //{
        //    switch (menuState)
        //    {
        //        case MenuState.MenuOff:
        //            player.Pause();
        //            followTarget.Pause();
        //            menuState = MenuState.MenuOn;
        //            isMenuBlending = true;
        //            //cameraManger.ActiveAimCamera(() => hudTest.HUDActive());
        //            //cameraManger.ActiveAimCamera(() => escMainMenu.Appear(0.2f, () => SwitchMenuDone()));
        //            //cameraManger.ActiveAimCamera(() => inputMenu.Appear(0.08f, () => SwitchMenuDone()));
        //            cameraManger.ActiveAimCamera(() => soundMenu.Appear(0.5f, () => SwitchMenuDone()));
        //            Cursor.lockState = CursorLockMode.None;
        //            Cursor.visible = true;
        //            break;
        //        case MenuState.MenuOn:
        //            player.Resume();
        //            followTarget.Resume();
        //            menuState = MenuState.MenuOff;
        //            isMenuBlending = true;
        //            //hudTest.HUDDissable(() => cameraManger.ActivePlayerFollowCamera());
        //            //cameraManger.ActiveAimCamera(() => escMainMenu.Disappear(0.2f, () => { SwitchMenuDone();cameraManger.ActivePlayerFollowCamera(); }));
        //            //cameraManger.ActiveAimCamera(() => inputMenu.Disappear(0.08f, () => { SwitchMenuDone(); cameraManger.ActivePlayerFollowCamera(); }));
        //            cameraManger.ActiveAimCamera(() => soundMenu.Disappear(0.5f, () => { SwitchMenuDone(); cameraManger.ActivePlayerFollowCamera(); }));
        //            Cursor.lockState = CursorLockMode.Locked;
        //            Cursor.visible = false;
        //            break;
        //    }
        //}
    }

    public void SwitchMenuDone()
    {
        isMenuBlending = false;
    }

    public void PausePlayer()
    {
        player.Pause();
    }

    public void PauseControl(bool result)
    {
        player.PauseControl(result);
    }

   

    public void ResumePlayerControl()
    {
        player.Resume();
    }


 

    public LevelEdit_BehaviorControll.State GetBossState()
    {
        if (bossControll != null)
        {
            return bossControll.GetState();
        }
        else
        {
            return LevelEdit_BehaviorControll.State.Idle;
        }
    }

    public Transform GetCoreTransform() { return coreTransfrom; }
    public Transform GetKillEventTransform() {return killEventTransform;}
    public void LookingEvent_CameraCollision(Transform target)
    {
    }

    public void LookingEventEnd_CameraCollision()
    {
    }

    public void RequstCameraShakeByFactor(float factor, float time)
    {
    }

    public void RequstCameraShakeByPosition(Vector3 position)
    {
    }

    public void RequstCameraShakeDefault(float factor)
    {
    }

    public GameObject GetPlayerObject()
    {
        return player.gameObject;
    }

    public void SetPlayer(PlayerCtrl player)
    {
        this.player = player;
    }

    public void RequstGameResult()
    {
        if(uiManager != null)
        {
            uiManager.GameResult();
        }
    }

    //public void RequstChangePP(float time)
    //{
    //    mainCam.RequstChangePP(time);
    //}

    //public void RequstReturnPP(float time)
    //{
    //    mainCam.RequstReturnPP(time);
    //}    

    public bool IsCurrentCameraEvent()
    {
        return isCurrentCameraEvent;
    }

    //public float GetInputVertical()
    //{
    //    if (player == null)
    //        return 0.0f;

    //    return player.GetInputVertical();
    //}

    //public float GetInputHorizontal()
    //{
    //    if (player == null)
    //        return 0.0f;

    //    return player.GetInputHorizontal();
    //}

    public Vector2 GetScreenCenter()
    {
        return screenCenter;
    }

    public float GetScreenWidth()
    {
        return screenWidth;
    }

    public float GetScreenHeight()
    {
        return screenHeight;
    }
}
