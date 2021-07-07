using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessageObject : ObjectBase
{
    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("TestManager"));
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        Debug.Log("Check");
    }
}
