using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class MainHubStageManager : ObjectBase
{
    public override void Assign()
    {
        base.Assign();

    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("StageManager"));

        var visibleData = MessageDataPooling.GetMessageData<BoolData>();
        visibleData.value = true;
        SendMessageEx(MessageTitles.player_visibledrone, GetSavedNumber("Player"), visibleData);

        var blockChargeShot = MessageDataPooling.GetMessageData<BoolData>();
        blockChargeShot.value = false;
        SendMessageEx(MessageTitles.player_blockChargeShot, GetSavedNumber("Player"), blockChargeShot);

        var blockDash = MessageDataPooling.GetMessageData<BoolData>();
        blockDash.value = false;
        SendMessageEx(MessageTitles.player_blockDash, GetSavedNumber("Player"), blockDash);

        var blockQuickStand = MessageDataPooling.GetMessageData<BoolData>();
        blockQuickStand.value = false;
        SendMessageEx(MessageTitles.player_blockQuickStand, GetSavedNumber("Player"), blockQuickStand);
    }
}
