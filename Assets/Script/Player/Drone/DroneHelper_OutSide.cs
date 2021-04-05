using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHelper_OutSide : DroneHelper
{
    private bool guide = true;
    private void Start()
    {
        base.Start();
        StartCoroutine(GuideLog());
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

    IEnumerator GuideLog()
    {
        int count = 1;
        string outSideKey = "Out_";

        yield return new WaitForSeconds(2.0f);

        while (guide == true)
        {
            root.HelpEvent(outSideKey + count.ToString());
            yield return new WaitForSeconds(5.5f);
            count++;
            if (count == 9)
                guide = false;
        }
    }


    public void ArriveAelevator()
    {
        guide = false;
        root.HelpEvent("Out_Elevator");
    }
}
