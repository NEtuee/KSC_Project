using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Immortal : DroneHelper
{
    [SerializeField] private bool entranceFlag = false;
    [SerializeField] private bool scanFlag = false;
    [SerializeField] private bool scanning = false;
    [SerializeField] private bool scan2 = false;
    [SerializeField] private bool shieldDestroy = false;
    [SerializeField] private bool rolling = false;

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
        else
        {
            if(scanning && !scan2)
            {
                Scan2();
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

    public void Scanning()
    {
        if (scanning == true)
            return;
        scanning = true;
        root.HelpEvent("JirungE_Tip01");
    }

    private void Scan2()
    {
        if (scan2 == true)
            return;
        scan2 = true;
        root.HelpEvent("JirungE_Tip02");
    }

    public void ShieldDestroyFlag()
    {
        if(shieldDestroy == false)
        {
            shieldDestroy = true;
            root.HelpEvent("JirungE_ShieldCrack");
        }
    }

    public void RollingFlag()
    {
        if (rolling == false)
        {
            rolling = true;
            root.HelpEvent("JirungE_RollRush");
        }
    }

    public void ShiledDestroy()
    {
        root.HelpEvent("JirungE_Explosion");
    }

    public void Grogy()
    {
        root.HelpEvent("JirungE_Groggy");
    }

    public void Run()
    {
        root.HelpEvent("JirungE_Run");
    }

    public void RestoreShield()
    {
        root.HelpEvent("JirungE_Recharge");
    }

    public void ElectricShock()
    {
        root.HelpEvent("JirungE_ElectricShock");
    }
}
