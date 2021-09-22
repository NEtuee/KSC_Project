using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTriggerManager : ManagerBase
{
    public BooleanTrigger globalTrigger;

    public override void Assign()
    {
        base.Assign();

        SaveMyNumber("GlobalTriggerManager");

        AddAction(MessageTitles.boolTrigger_getTriggerAsset,GetGlobalTriggerAsset);
        AddAction(MessageTitles.boolTrigger_getTrigger,GetGlobalTrigger);
        AddAction(MessageTitles.boolTrigger_setTrigger,SetGlobalTrigger);

        var save = ScriptableObject.Instantiate(globalTrigger);

        if(!globalTrigger.LoadDataFromFile("SaveData",true))
        {
            globalTrigger.CopyTarget(save);
            globalTrigger.SaveDataToFile("SaveData");
        }

        ScriptableObject.DestroyImmediate(save,true);
    }

    public void GetGlobalTriggerAsset(Message msg)
    {
        var receiver = (MessageReceiver)msg.sender;
        var send = MessagePack(MessageTitles.boolTrigger_getTriggerAsset,receiver.uniqueNumber,globalTrigger);
        SendMessageQuick(receiver,send);
    }

    public void GetGlobalTrigger(Message msg)
    {
        var receivedData = MessageDataPooling.CastData<MD.StringData>(msg.data);
        var trigger = globalTrigger.FindTrigger(receivedData.value);

        if(trigger == null)
        {
            Debug.Log("Trigger does not exists");
            return;
        }

        var sendData = MessageDataPooling.GetMessageData<MD.TriggerData>();
        sendData.name = trigger.name;
        sendData.trigger = trigger.trigger;

        var receiver = (MessageReceiver)msg.sender;
        var send = MessagePack(MessageTitles.boolTrigger_getTrigger,receiver.uniqueNumber,sendData);
        SendMessageQuick(receiver,send);
    }

    public void SetGlobalTrigger(Message msg)
    {
        var receivedData = MessageDataPooling.CastData<MD.TriggerData>(msg.data);
        var trigger = globalTrigger.FindTrigger(receivedData.name);

        if(trigger == null)
        {
            Debug.Log("Trigger does not exists");
            return;
        }

        trigger.trigger = receivedData.trigger;
    }
}
