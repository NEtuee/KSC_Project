using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraDistanceCtrl : MonoBehaviour
{
    private PlayerCtrl_Ver2 player;

    private void Awake()
    {
        player = GetComponent<PlayerCtrl_Ver2>();
    }

    private void FixedUpdate()
    {
        
    }
}
