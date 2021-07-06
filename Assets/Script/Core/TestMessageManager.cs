using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessageManager : ManagerBase<ObjectBase>
{
    public override void Assign()
    {
        _unknownMessageProcess = (msg)=>{
            MessagePool.ReturnMessage(msg);
        };
    }
}
