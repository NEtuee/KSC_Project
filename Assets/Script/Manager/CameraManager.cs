using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain brain;
    private Transform brainCameraTransfrom;
    [SerializeField] private CinemachineVirtualCameraBase playerFollowCam;
    [SerializeField] private CinemachineVirtualCameraBase playerAimCam;
    [SerializeField] private List<CinemachineVirtualCameraBase> otherCameras = new List<CinemachineVirtualCameraBase>();
    [SerializeField] private bool isBlend;
    private bool isRunningCallBackCoroutine;
    private CinemachineVirtualCameraBase currentActiveCam = null;
    private CinemachineVirtualCameraBase prevActiveCam = null;

    private Dictionary<string, CinemachineVirtualCameraBase> cameraDictionary = new Dictionary<string, CinemachineVirtualCameraBase>();

    private void Start()
    {
        brainCameraTransfrom = brain.transform;

        GameManager.Instance.cameraManger = this;

        otherCameras.Add(playerFollowCam);
        otherCameras.Add(playerAimCam);

        InitializeCameraAtGameStart();
    }

    /// <summary>
    /// 게임 시작시 플레이어 캠 빼고 전부 비활성화 합니다.
    /// </summary>
    private void InitializeCameraAtGameStart()
    { 
        playerAimCam.gameObject.SetActive(false);
        foreach(var cam in otherCameras)
        {
            cam.gameObject.SetActive(false);
        }

        playerFollowCam.gameObject.SetActive(true);

        currentActiveCam = playerFollowCam;
    }

    /// <summary>
    /// 카메라를 플레이어 캠으로 바꿉니다.
    /// </summary>
    public bool ActivePlayerFollowCamera()
    {
        if (playerFollowCam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = playerFollowCam;
        currentActiveCam.gameObject.SetActive(true);
        return true;
    }

    public bool ActivePlayerFollowCamera(Action doneBlendCall)
    {
        if (playerFollowCam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = playerFollowCam;
        currentActiveCam.gameObject.SetActive(true);
        if (isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    /// <summary>
    /// 카메라를 조준 카메라로 바꿉니다.
    /// </summary>
    public bool ActiveAimCamera()
    {
        if (playerAimCam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = playerAimCam;
        currentActiveCam.gameObject.SetActive(true);
        return true;
    }

    public bool ActiveAimCamera(Action doneBlendCall)
    {
        if (playerAimCam == null)
            return false;

        prevActiveCam = currentActiveCam;
        prevActiveCam.gameObject.SetActive(false);
        currentActiveCam = playerAimCam;
        currentActiveCam.gameObject.SetActive(true);
        if (isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }


    /// <summary>
    /// 이전에 사용한 카메라로 전환합니다.
    /// </summary>
    public bool BackToPrevCamera()
    {
        //이전 카메라가 없으면 반환
        if(prevActiveCam == null)
        {
            return false;
        }

        CinemachineVirtualCameraBase temp;
        temp = currentActiveCam;
        currentActiveCam.gameObject.SetActive(false);
        currentActiveCam = prevActiveCam;
        prevActiveCam = temp;
        currentActiveCam.gameObject.SetActive(true);

        return true;
    }

    public bool BackToPrevCamera(Action doneBlendCall)
    {
        //이전 카메라가 없으면 반환
        if (prevActiveCam == null)
        {
            return false;
        }

        CinemachineVirtualCameraBase temp;
        temp = currentActiveCam;
        currentActiveCam.gameObject.SetActive(false);
        currentActiveCam = prevActiveCam;
        prevActiveCam = temp;
        currentActiveCam.gameObject.SetActive(true);
        if (isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    /// <summary>
    /// 넘겨준 버추얼 카메라를 현재 카메라로 설정합니다. 
    /// </summary>
    /// <param name="activeCamera"></param>
    public bool ActiveCamera(CinemachineVirtualCameraBase activeCamera)
    {
        if (activeCamera == null)
            return false;

        currentActiveCam.gameObject.SetActive(false);
        prevActiveCam = currentActiveCam;
        currentActiveCam = activeCamera;
        currentActiveCam.gameObject.SetActive(true);
        return true;
    }

    public bool ActiveCamera(CinemachineVirtualCameraBase activeCamera, Action doneBlendCall)
    {
        if (activeCamera == null)
            return false;

        currentActiveCam.gameObject.SetActive(false);
        prevActiveCam = currentActiveCam;
        currentActiveCam = activeCamera;
        currentActiveCam.gameObject.SetActive(true);
        if(isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    /// <summary>
    /// 해당하는 키의 카메라를 현재 카메라로 설정합니다.
    /// </summary>
    /// <param name="cameraKey"></param>
    public bool ActiveCamera(string cameraKey)
    {
        if(cameraDictionary.ContainsKey(cameraKey) == false)
        {
            Debug.Log(cameraKey + " does not exist.");
            return false;
        }

        currentActiveCam.gameObject.SetActive(false);
        prevActiveCam = currentActiveCam;
        currentActiveCam = cameraDictionary[cameraKey];
        currentActiveCam.gameObject.SetActive(true);
        return true;
    }

    public bool ActiveCamera(string cameraKey, Action doneBlendCall)
    {
        if (cameraDictionary.ContainsKey(cameraKey) == false)
        {
            Debug.Log(cameraKey + " does not exist.");
            return false;
        }

        currentActiveCam.gameObject.SetActive(false);
        prevActiveCam = currentActiveCam;
        currentActiveCam = cameraDictionary[cameraKey];
        currentActiveCam.gameObject.SetActive(true);
        if (isRunningCallBackCoroutine)
        {
            StopAllCoroutines();
        }
        StartCoroutine(BlendingCallBack(doneBlendCall));
        return true;
    }

    /// <summary>
    /// 카메라를 키로 등록합니다.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="camera"></param>
    public void RegisterCameraKey(string key, CinemachineVirtualCameraBase camera)
    {
        cameraDictionary.Add(key, camera);
    }

    public bool IsCameraBlending()
    {
        return brain.IsBlending;
    }

    public Vector3 GetCameraPosition() { return brainCameraTransfrom.position; }
    public Quaternion GetCameraRotation() { return brainCameraTransfrom.rotation; }

    IEnumerator BlendingCallBack(Action action)
    {
        isRunningCallBackCoroutine = true;
        yield return null;
        while(brain.IsBlending)
        {
            yield return null;
        }
        action();
        isRunningCallBackCoroutine = false;
    }
}
