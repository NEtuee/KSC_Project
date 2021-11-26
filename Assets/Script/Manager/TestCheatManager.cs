using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MD;

public class TestCheatManager : ManagerBase
{
    public Transform spawnPoint;
    public Transform spawnPoint2;

    protected override void Awake()
    {
        base.Awake();
        SaveMyNumber("CheatManager", true);
        RegisterRequest();
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

       
    }
}
