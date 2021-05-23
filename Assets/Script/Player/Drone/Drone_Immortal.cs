using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Immortal : DroneHelper
{
    [SerializeField] private bool entranceFlag = false;
    [SerializeField] private bool scanFlag = false;
    [SerializeField] private bool shieldDestroy = false;

    public override void HelperUpdate()
    {
        if (root.active == false)
            return;

        CheckScan();

        if (root.helping == true)
        {
            bool limit;
            root.timer.IncreaseTimer("Help", root.hintTime, out limit);
            if (limit == true)
            {
                root.helping = false;
                root.ActiveDescriptCanvas(false);
                root.drone.OrderDefault();
            }
        }
    }

    public void EntranceFlag()
    {
        if (entranceFlag == false)
        {
            entranceFlag = true;
            root.HelpEvent("JirungE_Start");
        }
    }

    public void ScanStart()
    {
        if (scanFlag == true)
            return;
        scanFlag = true;
        root.HelpEvent("JirungE_Scan");
    }

    public void ShieldDestroyFlag()
    {
        if(shieldDestroy == false)
        {
            shieldDestroy = true;
            root.HelpEvent("IJ_ShieldDestroy");
        }
    }

    public void AllShieldDestroyFlag()
    {
        Debug.Log("AllDestroy");
        root.HelpEvent("IJ_AllShieldDestroy");
    }

    public void RecoveryFlag()
    {
        root.HelpEvent("IJ_Recovery");
    }

    public void RestoreShieldFlag()
    {
        root.HelpEvent("IJ_RestroeShield");
    }
}
