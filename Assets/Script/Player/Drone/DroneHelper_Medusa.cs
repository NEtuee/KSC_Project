using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DroneHelper_Medusa : DroneHelper
{
    [SerializeField] private bool scanned = false;
    [SerializeField] private bool tip1 = false;
    [SerializeField] private bool tip2 = false;
    [SerializeField] private bool checkingTip3 = false;
    [SerializeField] private bool destroyedShield = false;
    [SerializeField] private bool destoryedMedusa = false;

    [SerializeField] private bool hint1Complete = false;
    [SerializeField] private bool hint2Complete = false;
    
    public void ScanFlag()
    {
        scanned = true;
        root.HelpEvent("Medusa_Start");
        root.timer.InitTimer("Tip01Timer", 0.0f, 120.0f);
    }

    public void NoEscape()
    {
        root.HelpEvent("Medusa_NoEscape");
    }

    public void LockOff()
    {
        root.HelpEvent("Medusa_Lockoff");
        checkingTip3 = true;
        root.timer.InitTimer("Tip03Timer", 0.0f, 10.0f);
    }

    public void HitShild()
    {
        root.HelpEvent("Medusa_ShieldAttackFeedback01");
    }

    public void DestroyShiled()
    {
        root.HelpEvent("Medusa_ShieldAttackFeedback02");
        destroyedShield = true;
    }

    public void DestroyMedusa()
    {
        root.HelpEvent("Medusa_Death");
        destoryedMedusa = true;
    }

    public void Hint1Flag()
    {
        hint1Complete = true;
    }

    public void Hint2Flag()
    {
        hint2Complete = true;
    }

    public void CheckExitEvent()
    {
        if(root != null)
        root.HelpEvent("CheckExit");
    }

    public void ReleaseLockOnFlag()
    {
        root.HelpEvent("MD_ReleaseLockOn");
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
            if (destoryedMedusa == true)
                return;

            if(tip1 == false)
            {
                root.timer.IncreaseTimer("Tip01Timer",out bool limit);
                if(limit == true)
                {
                    tip1 = true;
                    root.HelpEvent("Medusa_Tip01");
                    root.timer.InitTimer("Tip02Timer");
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
