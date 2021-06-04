using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class DroneHelper_Medusa : DroneHelper
{
    [SerializeField] private bool scanned = false;
    [SerializeField] private bool scan1 = false;
    [SerializeField] private bool scan2 = false;
    [SerializeField] private bool tip1 = false;
    [SerializeField] private bool tip2 = false;
    [SerializeField] private bool checkingTip3 = false;
    [SerializeField] private bool destroyedShield = false;
    [SerializeField] private bool destroyedMedusa = false;
    
    public void ScanFlag()
    {
        scanned = true;
        root.HelpEvent("Medusa_Start");
        root.timer.InitTimer("Tip01Timer", 0.0f, 120.0f);
    }

    public void Scan1()
    {
        if (scan1)
            return;
        scan1 = true;
        root.HelpEvent("Medusa_Scan01");
    }

    private void Scan2()
    {
        if (scan2)
            return;
        scan2 = true;
        root.HelpEvent("Medusa_Scan02");
    }

    public void NoEscape()
    {
        if (destroyedMedusa == true)
            return;

        root.HelpEvent("Medusa_NoEscape");
    }

    public void LockOff()
    {
        root.HelpEvent("Medusa_Lockoff");
        checkingTip3 = true;
        root.timer.InitTimer("Tip03Timer", 0.0f, 10.0f);
    }

    public void HitShield()
    {
        root.HelpEvent("Medusa_ShieldAttackFeedback01");
    }

    public void DestroyShield()
    {
        root.HelpEvent("Medusa_ShieldAttackFeedback02");
        destroyedShield = true;
    }

    public void DestroyMedusa()
    {
        root.HelpEvent("Medusa_Death");
        destroyedMedusa = true;
    }
    
    
    public override void HelperUpdate()
    {
        if (root.active == false)
            return;

        //CheckScan();
        //CheckLevelTime();

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
            if (scanned == false || destroyedMedusa == true)
                return;

            if(scan1 == true && scan2 == false)
            {
                Scan2();
            }

            if(tip1 == false)
            {
                root.timer.IncreaseTimer("Tip01Timer",out bool limit);
                if(limit == true)
                {
                    tip1 = true;
                    root.HelpEvent("Medusa_Tip01");
                    root.timer.InitTimer("Tip02Timer",0.0f,60.0f);
                }
            }
            else
            {
                if(tip2 == false)
                {
                    root.timer.IncreaseTimer("Tip02Timer", out bool limit);
                    if (limit == true)
                    {
                        tip2 = true;
                        root.HelpEvent("Medusa_Tip02");
                    }
                }

            }

            if(checkingTip3 == true)
            {
                root.timer.IncreaseTimer("Tip03Timer", out bool limit);
                if(limit == true)
                {
                    checkingTip3 = false;
                    root.HelpEvent("Medusa_Tip03");
                }
            }
        }
    }
}
