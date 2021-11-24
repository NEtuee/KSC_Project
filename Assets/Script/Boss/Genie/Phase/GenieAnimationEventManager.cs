using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieAnimationEventManager : MonoBehaviour
{
    public Genie_Phase_AI target;

    public void ChestOpen()
    {
        target.ChestOpenSound();
    }

    public void LeftSound()
    {
        target.LeftHitSound();
    }

    public void RightSound()
    {
        target.RightHitSound();
    }

    public void CreatLeft()
    {
        target.CreateLeftHit();
    }

    public void CreatRight()
    {
        target.CreateRightHit();
    }
}
