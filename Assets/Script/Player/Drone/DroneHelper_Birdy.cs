using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHelper_Birdy : DroneHelper
{
    [SerializeField] private bool meet = false;
    [SerializeField] private bool scan1 = false;
    [SerializeField] private bool scan2 = false;
    [SerializeField] private bool lever1 = false;
    [SerializeField] private bool lever2 = false;
    [SerializeField] private bool lever3 = false;
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

        CheckLevelTime();

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

    public void Meet()
    {
        if (meet)
            return;
        meet = true;
        root.HelpEvent("Birdy_Meet");
    }

    public void Scan1()
    {
        if (scan1)
            return;
        scan1 = true;
        root.HelpEvent("Birdy_Scan01");
    }

    public void Scan2()
    {
        if (scan2)
            return;
        scan2 = true;
        root.HelpEvent("Birdy_Scan02");
    }

    public void Lever1()
    {
        if (lever1)
            return;
        lever1 = true;
        root.HelpEvent("Birdy_lever01");
    }

    public void Lever2()
    {
        if (lever2)
            return;
        lever2 = true;
        root.HelpEvent("Birdy_lever02");
    }

    public void Lever3()
    {
        if (lever3)
            return;
        lever3 = true;
        root.HelpEvent("Birdy_lever03");
    }
}
