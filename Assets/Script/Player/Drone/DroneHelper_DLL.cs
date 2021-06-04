using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DroneHelper_DLL : DroneHelper
{
    private int fallCount = 0;

    [SerializeField] private bool _start1 = false;
    [SerializeField] private bool _start2 = false;
    [SerializeField] private bool _respawn = false;
    [SerializeField] private bool _drone = false;

    private void Start()
    {
        base.Start();
        StartCoroutine(LateStart());

        root.drone.whenCompleteRespawn += FirstRespawn;
    }

    public override void HelperUpdate()
    {
        if (root.active == false)
            return;

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

                if(_start2 == false)
                {
                    root.HelpEvent("DaddyLongLeg_Start02");
                    _start2 = true;
                }
            }
        }
    }

    public void FirstRespawn()
    {
        if (_respawn)
            return;
        _respawn = true;
        root.HelpEvent("DaddyLongLeg_Respawn");
    }

    public void EnterRotatorPattern()
    {
        root.HelpEvent("DaddyLongLeg_RotatorTrap");
    }

    public void EnterDoorPattern()
    {
        if (_drone)
            return;
        _drone = true;
        root.HelpEvent("DaddyLongLeg_Drone");
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1f);
        root.HelpEvent("DaddyLongLeg_Start01");
        _start1 = true;
    }
}
