using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessageManager : ManagerBase
{
    public override void Assign()
    {
        base.Assign();

        SaveMyNumber("TestManager");

        AddAction(0x0200,(msg)=>{
            Debug.Log((string)msg.data);
        });
    }
}
