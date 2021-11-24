using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;

public class MainHubStageManager : ObjectBase
{
    private string aClearTrigger = "AClear";
    private string bClearTrigger = "BClear";

    private bool clear;

    public GameObject firstHubTrigger;
    public GameObject aComeBackTrigger;
    public GameObject bComeBackTrigger;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.boolTrigger_getTrigger, (x) =>
        {
            var sendData = MessageDataPooling.CastData<MD.TriggerData>(x.data);
            if (sendData != null)
            {
                clear = sendData.trigger;
            }
        });
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

        var data = MessageDataPooling.GetMessageData<MD.StringData>();
        data.value = aClearTrigger;
        SendMessageEx(MessageTitles.boolTrigger_getTrigger, GetSavedNumber("GlobalTriggerManager"), data);

        if(clear == true)
        {
            firstHubTrigger.SetActive(false);
        }

        var data2 = MessageDataPooling.GetMessageData<MD.StringData>();
        data2.value = bClearTrigger;
        SendMessageEx(MessageTitles.boolTrigger_getTrigger, GetSavedNumber("GlobalTriggerManager"), data2);

        if (clear == true)
        {
            aComeBackTrigger.SetActive(false);
        }
    }
}
