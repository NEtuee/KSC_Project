using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BulletTimeManager timeManager;
    public float killEventFov = 66f;
    public CameraCtrl camCtrl;
    [SerializeField] private CameraCollision mainCam;
    //[SerializeField] private PlayerCtrl player;
    [SerializeField] private PlayerCtrl_State player;
    [SerializeField] public UIManager uiManager;
    [SerializeField] public CameraManager cameraManger;
    [SerializeField] public LevelEdit_BehaviorControll bossControll;
    [SerializeField] private Transform coreTransfrom;
    [SerializeField] private Transform killEventTransform;

    private Vector3 mainCameraStartPosition;
    private Vector3 mainCameraStartLocalPosition;
    private Vector3 camRootStartPosition;

    private Quaternion camRootStartRot;

    private bool isCurrentCameraEvent;

    private static GameManager instance;
    public static GameManager Instance { get { if (null == instance) { return null; } return instance; } }

    //카메라 이벤트를 위한 변수. 카메라 이벤트 전반 함수이전의 카메라 위치, 회전을 저장함
    Vector3 prevPos;
    Quaternion prevRot;
    Transform originalParent;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if(mainCam== null)
        {
            mainCam = Camera.main.GetComponent<CameraCollision>();
            camCtrl = mainCam.transform.parent.GetComponent<CameraCtrl>();
        }

        mainCameraStartPosition = mainCam.transform.position;
        mainCameraStartLocalPosition = mainCam.transform.localPosition;
        camRootStartPosition = camCtrl.transform.position;
        camRootStartRot = camCtrl.transform.rotation;

        if(GameObject.FindGameObjectWithTag("Boss") != null)
        {
            bossControll = GameObject.FindGameObjectWithTag("Boss").GetComponent<LevelEdit_BehaviorControll>();
        }
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    mainCam.RequstChangePP(2f);
        //}
    }

    public void PausePlayer()
    {
        if (camCtrl == null || mainCam == null || player == null)
        {
            Debug.LogWarning("Not Set PlayerControl Elements");
            return; 
        }

        player.Pause();
        camCtrl.Pause();
        mainCam.Pause();
    }

    public void PauseControl(bool result)
    {
        player.PauseControl(result);
    }

   

    public void ResumePlayerControl()
    {
        if (camCtrl == null || mainCam == null || player == null)
        {
            Debug.LogWarning("Not Set PlayerControl Elements");
            return;
        }

        player.Resume();
        camCtrl.Resume();
        mainCam.Resume();
    }

    public Vector3 GetMainCameraPosition()
    {
        return mainCam.transform.position;
    }

    public void MainCameraSetWorldPosition(Vector3 pos)
    {
        mainCam.transform.position = pos;
    }

    public void CameraRootSetWorldPosition(Vector3 pos)
    {
        camCtrl.transform.position = pos;
    }

    public IEnumerator GameStartCameraMove()
    {
        Vector3 targetPosition = mainCameraStartPosition;

        camCtrl.transform.rotation = camRootStartRot;
        camCtrl.SetFollowSmooth(0.8f);

        while ((camCtrl.transform.position - player.transform.position).magnitude > 5f)
        {
            //mainCam.transform.position = Vector3.MoveTowards(mainCam.transform.position, targetPosition, 25f * Time.deltaTime);
            //camRoot.transform.rotation = camRootStartRot;
            yield return null;
        }

        camCtrl.ResetFollowSmooth();
        //mainCam.transform.position = mainCameraStartPosition;
    }

    public void CameraEvent(Transform eventTransform)
    {
        if (uiManager == null)
            return;

        player.ReleaseAim();
        StartCoroutine(CameraMoveEvent(eventTransform));
    }


    IEnumerator CameraMoveEvent(Transform eventTransform)
    {
        player.Pause();
        camCtrl.Pause();
        mainCam.Pause();

        isCurrentCameraEvent = true;

        Vector3 prevPos = mainCam.transform.position;
        Quaternion prevRot = mainCam.transform.rotation;
        Transform originalParent = mainCam.transform.parent;

        yield return StartCoroutine(uiManager.FadeIn(1f));
        StartCoroutine(SetCameraTransfromAsync(eventTransform.position, eventTransform.rotation, eventTransform));
        yield return StartCoroutine(uiManager.FadeOut(1f));

        yield return new WaitForSeconds(4f);

        yield return StartCoroutine(uiManager.FadeIn(1f));
        mainCam.transform.SetPositionAndRotation(prevPos, prevRot);
        mainCam.transform.parent = originalParent;
        yield return StartCoroutine(uiManager.FadeOut(1f));

        player.Resume();
        camCtrl.Resume();
        mainCam.Resume();
        isCurrentCameraEvent = false;
    }

    public void SetCameraFov()
    {
        camCtrl.GetCurrentCamera().fieldOfView = killEventFov;
    }

    public void CameraEventIntroduction_Immediate(Transform eventTransform)
    {
        player.Pause();
        //camRoot.Pause();
        //mainCam.Pause();

        prevPos = mainCam.transform.position;
        prevRot = mainCam.transform.rotation;
        originalParent = mainCam.transform.parent;

        mainCam.ZoomLock();

        StartCoroutine(SetCameraTransfromAsync(eventTransform.position, eventTransform.rotation, eventTransform));
    }

    public void CameraEventIntroductionEnd_Immediate()
    {
        mainCam.transform.SetPositionAndRotation(prevPos, prevRot);
        mainCam.transform.parent = originalParent;

        player.Resume();
        // camRoot.Resume();
        // mainCam.Resume();

        mainCam.ZoomUnLock();
    }

    public void CameraEventIntroduction(Transform eventTransform)
    {
        StartCoroutine(CameraMoveEvent_Introdution(eventTransform));
    }

    IEnumerator CameraMoveEvent_Introdution(Transform eventTransform)
    {
        player.Pause();
        camCtrl.Pause();
        mainCam.Pause();

        prevPos = mainCam.transform.position;
        prevRot = mainCam.transform.rotation;
        originalParent = mainCam.transform.parent;

        mainCam.ZoomLock();
        yield return StartCoroutine(uiManager.FadeIn(1f));
        StartCoroutine(SetCameraTransfromAsync(eventTransform.position, eventTransform.rotation, eventTransform));
        yield return StartCoroutine(uiManager.FadeOut(1f));
        mainCam.ZoomUnLock();
    }

    public void CameraEventEnd()
    {
        StartCoroutine(CameraMoveEvent_End());
    }

    IEnumerator CameraMoveEvent_End()
    {
        yield return StartCoroutine(uiManager.FadeIn(1f));
        mainCam.transform.SetPositionAndRotation(prevPos, prevRot);
        mainCam.transform.parent = originalParent;
        yield return StartCoroutine(uiManager.FadeOut(1f));

        player.Resume();
        camCtrl.Resume();
        mainCam.Resume();
    }

    IEnumerator SetCameraTransfromAsync(Vector3 position , Quaternion rotation, Transform parnet)
    {
        yield return null;
        mainCam.transform.parent = parnet;
        //mainCam.transform.SetPositionAndRotation(position, rotation);
        mainCam.transform.localPosition = Vector3.zero;
        mainCam.transform.rotation = rotation;
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
        mainCam.LookingEvent(target);
        mainCam.ZoomLock();
    }

    public void LookingEventEnd_CameraCollision()
    {
        mainCam.LookingEventEnd();
        mainCam.ZoomUnLock();
    }

    public void RequstCameraShakeByFactor(float factor, float time)
    {
        mainCam.OnShake(factor, time);
    }

    public void RequstCameraShakeByPosition(Vector3 position)
    {
        mainCam.OnShake(position);
    }

    public void RequstCameraShakeDefault(float factor)
    {
        mainCam.OnShake(factor, 1f);
    }

    public GameObject GetPlayerObject()
    {
        return player.gameObject;
    }

    public void SetPlayer(PlayerCtrl_State player)
    {
        this.player = player;
    }

    public void ClearAllCore()
    {
        player.ClearAllCore();
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

    
}
