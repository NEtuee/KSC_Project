using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHelper_Empty : DroneHelper
{
    public float currentHelpTime;

    new void Start()
    {
        base.Start();
    }

    public override void HelperUpdate()
    {
        if (root.active == false)
            return;

        currentHelpTime = root.timer.GetCurrentTime("Help");

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
}