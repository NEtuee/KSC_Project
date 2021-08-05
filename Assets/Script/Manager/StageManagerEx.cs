using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class StageManagerEx : ManagerBase
{
    public BooleanTrigger stageTriggerAsset;
    
    public Elevator entranceElevator;
    public Elevator exitElevator;
    public Transform loadedPlayerPosition;

    protected override void Awake()
    {
        base.Awake();
        SaveMyNumber("StageManager", true);
        RegisterRequest();

        AddAction(MessageTitles.scene_sceneChanged, (msg) =>
        {
            PositionRotation data = MessageDataPooling.GetMessageData<PositionRotation>();
            data.position = loadedPlayerPosition.position;
            data.rotation = loadedPlayerPosition.rotation;
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);
        });
    }

    public override void Assign()
    {
        base.Assign();
    }
}
