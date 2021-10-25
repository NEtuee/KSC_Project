using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHelper_Arachne : DroneHelper
{
    [SerializeField] private bool bombHint = false;
    [SerializeField] private bool arachneAppear = false;
    [SerializeField] private bool scanned = false;
    [SerializeField] private bool bombing = false;
    [SerializeField] private bool hitFlage = false;
    [SerializeField] private bool timeRunningHint = false;
    [SerializeField] private bool arachneDown = false;
    [SerializeField] private bool arachneDeadCheck = false;
    [SerializeField] private bool developerHint = false;

    [SerializeField] private float timeRunningHintTime = 120.0f;
    [SerializeField] private float timeRunningCoolTime = 60.0f;

    [SerializeField] private float developerHintTime = 180.0f;

    private void Start()
    {
        base.Start();

        root.timer.InitTimer("DownHintTimer");
    }


    public override void HelperUpdate()
    {
        if (root.active == false)
            return;

        CheckScan();
        CheckLevelTime();

        if (arachneDeadCheck == false)
        {

            if (developerHint == false)
            {
                if (Time.time - sceneStartTime > developerHintTime)
                {
                    developerHint = true;
                    root.HelpEvent("ArachneBigHint");
                }
            }

            if (arachneDown == false)
            {
                bool limit;
                if (timeRunningHint == false)
                {
                    root.timer.IncreaseTimer("DownHintTimer", timeRunningHintTime, out limit);
                    if (limit == true)
                    {
                        root.HelpEvent("ArachneDownHint");
                        timeRunningHint = true;
                        root.timer.InitTimer("DownHintTimer", 0.0f);
                    }
                }
                else
                {
                    root.timer.IncreaseTimer("DownHintTimer", timeRunningHintTime, out limit);
                    if (limit == true)
                    {
                        root.HelpEvent("ArachneDownHint");
                        root.timer.InitTimer("DownHintTimer", 0.0f);
                    }
                }
            }
        }

    }

    public void ScanFlag()
    {
        if (scanned == false)
        {
            scanned = true;
            root.HelpEvent("ArachneWeak");
        }
    }

    public void ArachneAppearFlag()
    {
        arachneAppear = true;
        root.HelpEvent("ArachneAppear");
    }

    public void BombHintFlag()
    {
        if (bombHint == false)
        {
            bombHint = true;
            root.HelpEvent("ArachneBombHint");
        }
    }

    public void HitFlag()
    {
        if (hitFlage == false)
        {
            hitFlage = true;
            root.HelpEvent("ArachneStun");
        }
    }

    public void BombingFlag()
    {
        if (bombing == false)
        {
            bombing = true;
            root.HelpEvent("ArachneBomb");
        }
    }

    public void ArachneDownFlag()
    {
        arachneDown = true;
    }

    public void ArachneDeadFlag()
    {
        arachneDeadCheck = true;
        StartCoroutine(LateEnd());
    }

    IEnumerator LateEnd()
    {
        yield return new WaitForSeconds(5f);
        GameManager.Instance.endBackGround.SetActive(true);
    }
}
