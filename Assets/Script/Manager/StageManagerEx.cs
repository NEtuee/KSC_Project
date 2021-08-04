using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            PositionRotation data = new PositionRotation(loadedPlayerPosition.position, loadedPlayerPosition.rotation);
            SendMessageEx(MessageTitles.playermanager_setPlayerTransform, GetSavedNumber("PlayerManager"), data);
        });
    }

    public override void Assign()
    {
        base.Assign();
    }
}
