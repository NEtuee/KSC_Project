using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageState : ObjectBase
{
    public StateProcessor stateProcessor;

    public override void Assign()
    {
        base.Assign();
        stateProcessor.InitializeProcessor(this);
        stateProcessor.StateChange("Drones");
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        stateProcessor.StateProcess(deltaTime);
    }
}
