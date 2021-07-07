using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessageManager : ManagerBase
{
    public static TestMessageManager instance;

    public override void Assign()
    {
        _unknownMessageProcess = (msg)=>{
            MessagePool.ReturnMessage(msg);
        };

        SaveMyNumber("TestManager");

        instance = this;
    }
}
