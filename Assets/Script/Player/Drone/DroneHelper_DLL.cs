using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHelper_DLL : DroneHelper
{
    private int fallCount = 0;

    private void Start()
    {
        base.Start();
        StartCoroutine(LateStart());
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
            }
        }
    }

    public void NearPlatformFlag()
    {
        root.HelpEvent("DLL_NearPlatform");
    }

    public void UpPlatformFlag()
    {
        root.HelpEvent("DLL_UpPlatform");
    }

    public void DeadCountThreeFlag()
    {
        root.HelpEvent("DLL_3Dead");
    }

    public void FallFlag()
    {
        fallCount++;

        if(fallCount == 3)
        {
            DeadCountThreeFlag();
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1f);
        root.HelpEvent("DLL_Start");
    }
}
