using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessageManager : ManagerBase
{
    public static TestMessageManager instance;

    public override void Assign()
    {
        base.Assign();

        SaveMyNumber("TestManager");

        instance = this;
    }
}
