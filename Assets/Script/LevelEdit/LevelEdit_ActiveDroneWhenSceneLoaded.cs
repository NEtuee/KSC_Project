using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_ActiveDroneWhenSceneLoaded : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.asynSceneManager.RegisterBeforeLoadOnStart(ActiveDrone);
    }

    public void ActiveDrone()
    {
        (GameManager.Instance.player as PlayerCtrl_Ver2).GetDrone().Visible = true;
        GameManager.Instance.asynSceneManager.CancelBeforeLoad(ActiveDrone);
    }
}
