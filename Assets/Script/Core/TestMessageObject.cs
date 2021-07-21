using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessageObject : ObjectBase
{
    string message = "";
    public override void Assign()
    {
        base.Assign();

        AddAction(0x0200,(msg)=>{
            message = (string)msg.data;
            //Debug.Log((string)msg.data);
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("TestManager"));
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        message = "";
        SendMessageQuick(0x0200,_currentManagerNumber,"log");

        Debug.Log(message);
    }
}
