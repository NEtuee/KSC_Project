using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraTargetSetting : MonoBehaviour
{
    [SerializeField]private CinemachineVirtualCameraBase vcam;

    void Start()
    {
        vcam.Follow = GameObject.Find("CM vcam_FollowCam").transform;
    }

    void Update()
    {
        
    }
}
