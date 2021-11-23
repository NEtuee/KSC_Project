using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_GlobalTriggerEdit : MonoBehaviour
{
    public void Active(string target)
    {
        var data = MessageDataPooling.GetMessageData<MD.TriggerData>();
        data.name = target;
        data.trigger = true;

        var msg = MessagePool.GetMessage();
        msg.Set(MessageTitles.boolTrigger_setTrigger, UniqueNumberBase.GetSavedNumberStatic("GlobalTriggerManager"), data, null);

        MasterManager.instance.HandleMessage(msg);
    }

    public void Deactive(string target)
    {
        var data = MessageDataPooling.GetMessageData<MD.TriggerData>();
        data.name = target;
        data.trigger = false;

        var msg = MessagePool.GetMessage();
        msg.Set(MessageTitles.boolTrigger_setTrigger, UniqueNumberBase.GetSavedNumberStatic("GlobalTriggerManager"), data,null);

        MasterManager.instance.HandleMessage(msg);
    }
}
