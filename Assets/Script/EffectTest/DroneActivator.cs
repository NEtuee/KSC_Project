using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneActivator : MonoBehaviour
{
    public void Active()
    {
        var player = GameManager.Instance.player as PlayerCtrl_Ver2;
        player.GetDrone().Visible = true;
    }

    public void Deactive()
    {
        var player = GameManager.Instance.player as PlayerCtrl_Ver2;
        player.GetDrone().Visible = false;
    }
}
