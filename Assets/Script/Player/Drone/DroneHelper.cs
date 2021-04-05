using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if(Input.GetKeyDown(KeyCode.V))
        {
            tryScan = true;
        }

        bool limit;
        root.timer.IncreaseTimer("ScanCheckTimer", 30.0f, out limit);
        if (limit == true)
        {
            tryScan = true;
            root.HelpEvent("Support_Scan");
        }
    }
}
