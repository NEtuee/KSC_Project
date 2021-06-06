using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneActivator : MonoBehaviour
{
    public Transform droneStartPosition;
    public void Active()
    {
        var player = GameManager.Instance.player as PlayerCtrl_Ver2;
        player.GetDrone().Visible = true;
        player.GetDrone().SetPosition(droneStartPosition.position);
    }

    public void Deactive()
    {
        var player = GameManager.Instance.player as PlayerCtrl_Ver2;
        player.GetDrone().Visible = false;
    }
}
