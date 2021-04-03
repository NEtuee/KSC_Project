using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DroneHelper_Medusa : DroneHelper
{
    [SerializeField] private bool scanned = false;
    [SerializeField] private bool hint1Complete = false;
    [SerializeField] private bool hint2Complete = false;
    
    public void ScanFlag()
    {
        scanned = true;
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
            bool limit;
            root.timer.IncreaseTimer("HintTime", 10.0f, out limit);
            if (limit)
            {
                if (hint1Complete == false)
                {
                    root.HelpEvent("Hint1");
                    Hint1Flag();
                }
                else
                {
                    root.HelpEvent("Hint2");
                    Hint2Flag();
                }
            }
        }
    }
}
