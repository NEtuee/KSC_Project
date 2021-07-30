using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        if(Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            PositionRotation data = new PositionRotation(spawnPoint.position, spawnPoint.rotation);
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);
        }

        if(Keyboard.current.digit2Key.wasPressedThisFrame && spawnPoint2 != null)
        {
            PositionRotation data = new PositionRotation(spawnPoint2.position, spawnPoint2.rotation);
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);
        }
    }
}
