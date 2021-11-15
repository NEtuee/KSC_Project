using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionRelay : ObjectBase
{
    public string key;
    public float duration;

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public void ShowDesc()
    {
        var data = MessageDataPooling.GetMessageData<MD.DroneTextKeyAndDurationData>();
        data.key = key;
        data.duration = duration;
        SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey,GetSavedNumber("PlayerManager"),data);
    }

    public void ShowDescByKey(string key)
    {
        var data = MessageDataPooling.GetMessageData<MD.DroneTextKeyAndDurationData>();
        data.key = key;
        SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey,GetSavedNumber("PlayerManager"),data);
    }
}
