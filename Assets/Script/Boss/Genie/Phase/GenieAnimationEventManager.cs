using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenieAnimationEventManager : MonoBehaviour
{
    public Genie_Phase_AI target;

    public void CreatLeft()
    {
        target.CreateLeftHit();
    }

    public void CreatRight()
    {
        target.CreateRightHit();
    }
}
