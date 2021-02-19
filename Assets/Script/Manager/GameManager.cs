﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum MenuState { MenuOn,MenuOff}

    public BulletTimeManager timeManager;
    public float killEventFov = 66f;
    [SerializeField] private PlayerCtrl_State player;
    [SerializeField] private FollowTargetCtrl followTarget;
    [SerializeField] public UIManager uiManager;
    [SerializeField] public CameraManager cameraManger;
    [SerializeField] public LevelEdit_BehaviorControll bossControll;
    [SerializeField] private Transform coreTransfrom;
    [SerializeField] private Transform killEventTransform;
    [SerializeField] private HudTest hudTest;
    public Transform bossTransform;

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
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    mainCam.RequstChangePP(2f);
        //}

        if (Input.GetKeyDown(KeyCode.Escape)&&hudTest != null && isMenuBlending == false)
        {
            switch (menuState)
            {
                case MenuState.MenuOff:
                    player.Pause();
                    followTarget.Pause();
                    menuState = MenuState.MenuOn;
                    isMenuBlending = true;
                    cameraManger.ActiveAimCamera(() => hudTest.HUDActive());
                    break;
                case MenuState.MenuOn:
                    player.Resume();
                    followTarget.Resume();
                    menuState = MenuState.MenuOff;
                    isMenuBlending = true;
                    hudTest.HUDDissable(() => cameraManger.ActivePlayerFollowCamera());
                    break;
            }
        }
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

    public float GetInputVertical()
    {
        if (player == null)
            return 0.0f;

        return player.GetInputVertical();
    }

    public float GetInputHorizontal()
    {
        if (player == null)
            return 0.0f;

        return player.GetInputHorizontal();
    }
}
