using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraChanger : MonoBehaviour
{
    public CinemachineVirtualCameraBase firstCam;
    public CinemachineVirtualCameraBase secondCam;
    public CinemachineVirtualCameraBase thirdCam;
    public CinemachineVirtualCameraBase fourthCam;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            GameManager.Instance.cameraManger.ActiveCamera(firstCam);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameManager.Instance.cameraManger.ActiveCamera(secondCam);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameManager.Instance.cameraManger.ActiveCamera(thirdCam);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            GameManager.Instance.cameraManger.ActiveCamera(fourthCam);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameManager.Instance.cameraManger.BackToPrevCamera();
        }
    }

}
