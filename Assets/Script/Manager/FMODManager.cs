using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODManager : ManagerBase
{
    public override void Assign()
    {
        base.Assign();

        SaveMyNumber("FMODManager");

        AddAction(0x0200,(msg)=>{
            Debug.Log((string)msg.data);
        });
    }

}
