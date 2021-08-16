using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class DroneHelper : MonoBehaviour
{
    [SerializeField] protected DroneHelperRoot root;
    protected bool tryScan = false;

    protected float sceneStartTime;

    protected void Start()
    {
        root = GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneHelperRoot>();
        root.SetHelper(this);

        root.timer.InitTimer("ScanCheckTimer");
        root.timer.InitTimer("LevelTimer");
        sceneStartTime = Time.time;

        TimeCounterEx._isUpdate = true;

        root.drone.whenCompleteRespawn = null;
    }

    public void InitHelper(DroneHelperRoot root)
    {
        this.root = root;
    }

    public abstract void HelperUpdate();

    protected void CheckScan()
    {
        if (tryScan == true)
            return;

        bool limit;
        root.timer.IncreaseTimer("ScanCheckTimer", 30.0f, out limit);
        if (limit == true)
        {
            tryScan = true;
            root.HelpEvent("Support_Scan");
        }
    }

    protected void CheckLevelTime()
    {
        bool limit;
        root.timer.IncreaseTimer("LevelTimer", 300.0f, out limit);
        if (limit == true)
        {
            root.HelpEvent("Support_Level");
            root.timer.InitTimer("LevelTimer",0.0f);
        }
    }
}
