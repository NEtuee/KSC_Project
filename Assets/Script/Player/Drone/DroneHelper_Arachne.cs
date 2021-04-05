using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHelper_Arachne : DroneHelper
{
    [SerializeField] private bool bombHint = false;
    [SerializeField] private bool arachneAppear = false;
    [SerializeField] private bool scanned = false;
    [SerializeField] private bool bombing = false;
    [SerializeField] private bool timeRunningHint = false;
    [SerializeField] private bool arachneDown = false;
    [SerializeField] private bool arachneDeadCheck = false;
    [SerializeField] private bool developerHint = false;

    [SerializeField] private float timeRunningHintTime = 120.0f;
    [SerializeField] private float timeRunningCoolTime = 60.0f;

    [SerializeField] private float developerHintTime = 180.0f;

    private float sceneStartTime;

    private void Start()
    {
        base.Start();
        sceneStartTime = Time.time;

        root.timer.InitTimer("DownHintTimer");
    }


    public override void HelperUpdate()
    {
        if (root.active == false)
            return;

        if(developerHint == false)
        {
            if(Time.time-sceneStartTime > developerHintTime)
            {
                developerHint = true;
                root.HelpEvent("Arachne_BigHint");
            }
        }

        if(arachneDown == false)
        {
            bool limit;
            if (timeRunningHint == false)
            {
                root.timer.IncreaseTimer("DownHintTimer", timeRunningHintTime, out limit);
                if (limit == true)
                {
                    root.HelpEvent("Arachne_DownHint");
                    timeRunningHint = true;
                    root.timer.InitTimer("DownHintTimer", 0.0f);
                }
            }
            else
            {
                root.timer.IncreaseTimer("DownHintTimer", timeRunningHintTime, out limit);
                if (limit == true)
                {
                    root.HelpEvent("Arachne_DownHint");
                    root.timer.InitTimer("DownHintTimer", 0.0f);
                }
            }
        }

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

    public void ScanFlag()
    {
        scanned = true;
        root.HelpEvent("Arachne_Weak");
    }

    public void ArachneAppearFlag()
    {
        arachneAppear = true;
        root.HelpEvent("Arachne_Appear");
    }

    public void BombHintFlag()
    {
        bombHint = true;
        root.HelpEvent("Arachne_BombHint");
    }

    public void BombingFlag()
    {
        bombing = true;
        root.HelpEvent("Arachne_Bomb");
    }

    public void ArachneDownFlag()
    {
        arachneDown = true;
    }

    public void ArachneDeadFlag()
    {
        arachneDeadCheck = true;
    }
}
