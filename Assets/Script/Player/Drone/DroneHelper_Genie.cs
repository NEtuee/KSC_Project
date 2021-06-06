using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHelper_Genie : DroneHelper
{
    [SerializeField] private bool scan1 = false;
    [SerializeField] private bool scan2 = false;
    [SerializeField] private bool destroyedEscapeDrone = false;
    [SerializeField] private bool hitCore = false;
    [SerializeField] private bool createDronePattern = false;

    private void Start()
    {
        root.timer.InitTimer("Tip01Timer", 0.0f, 60.0f);
    }

    public override void HelperUpdate()
    {
        if (root.active == false)
            return;

        if (root.helping == true)
        {
            bool limit;
            root.timer.IncreaseTimer("Help", root.hintTime, out limit);
            if (limit == true)
            {
                root.helping = false;
                root.ActiveDescriptCanvas(false);
                root.drone.OrderDefault();
                root.timer.InitTimer("HintTime");
            }
        }
        else
        {
            if(hitCore == false)
            {
                root.timer.IncreaseTimer("Tip01Timer", out bool limit);
                if (limit == true)
                {
                    root.timer.InitTimer("Genie_Tip01");
                }
            }

            if(scan1 == true && scan2 == false)
            {
                Scan2();
            }
        }
    }

    public void Scan1()
    {
        if (scan1 == true)
            return;
        scan1 = true;
        root.HelpEvent("Genie_Scan01");
    }

    private void Scan2()
    {
        if (scan2 == true)
            return;
        scan2 = true;
        root.HelpEvent("Genie_Scan02");
    }

    public void Groggy()
    {
        root.HelpEvent("Genie_Groggy");
    }

    public void PatternCancel()
    {
        root.HelpEvent("Genie_Cancel");
    }

    public void HitCore()
    {
        root.HelpEvent("Genie_Attack");
    }

    public void ShieldBroke()
    {
        hitCore = true;
    }

    public void SpawnDrone()
    {
        root.HelpEvent("Genie_Drone");
    }
}
