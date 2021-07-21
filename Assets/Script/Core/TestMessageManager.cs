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
            var target = (MessageReceiver)msg.sender;
            SendMessageQuick(target,0x0200,msg.data);
            // Debug.Log((string)msg.data);
        });
    }
}
