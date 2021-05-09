using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_VirtualCamTrigger : LevelEdit_Trigger
{
    public Cinemachine.CinemachineVirtualCameraBase virtualCameraBase;
    public bool setFollowTargetToPlayer;

    private Cinemachine.Cinemachine3rdPersonFollow _thirdPersonFollow;
    private Cinemachine.CinemachineVirtualCamera _virtualCam;
    public new void Start()
    {
        base.Start();
        _virtualCam = virtualCameraBase.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        _thirdPersonFollow = _virtualCam.GetCinemachineComponent<Cinemachine.Cinemachine3rdPersonFollow>();

        if(setFollowTargetToPlayer)
        {
            _virtualCam.m_LookAt = GameManager.Instance.player.transform;
        }
    }

    public void SetVirtualCamera()
    {
        GameManager.Instance.cameraManager.ActiveVirtualCamera(virtualCameraBase,_thirdPersonFollow);
    }

    public void SetFollowCamera()
    {
        GameManager.Instance.cameraManager.ActivePlayerFollowCamera();
    }
}
