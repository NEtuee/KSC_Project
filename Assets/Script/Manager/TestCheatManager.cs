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

        if(Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            PositionRotation data = MessageDataPooling.GetMessageData<PositionRotation>();
            data.position = spawnPoint.position;
            data.rotation = spawnPoint.rotation;
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);
        }

        if(Keyboard.current.digit2Key.wasPressedThisFrame && spawnPoint2 != null)
        {
            PositionRotation data = MessageDataPooling.GetMessageData<PositionRotation>();
            data.position = spawnPoint2.position;
            data.rotation = spawnPoint2.rotation;
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);
        }
    }
}
