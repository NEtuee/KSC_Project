using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Get/Get Global Trigger")]
public class GetGlobalTrigger : BaseNode
{
    [Input(name = "Trigger Name"),SerializeField]
	public string triggerName;

	[Input(name = "Level Object")]
	public GraphObjectBase obj;


	[Output(name = "Trigger")]
	public bool trigger;

    [Output(name = "Find")]
	public bool find;

	public override string		name => "Get Global Trigger";

	protected override void Process()
	{
        var data = MessageDataPooling.GetMessageData<MD.StringData>();
        data.value = triggerName;
	    obj.SendMessageQuick(MessageTitles.boolTrigger_getTrigger,UniqueNumberBase.GetSavedNumberStatic("GlobalTriggerManager"),data);

        var msg = obj.DequeueGraphMessage();
        if(msg?.data == null)
        {
            find = false;

            return;
        }

        find = true;
        trigger = MessageDataPooling.CastData<MD.TriggerData>(msg.data).trigger;

        MessagePool.ReturnMessage(msg);
	}
}

[System.Serializable, NodeMenuItem("Set/Set Global Trigger")]
public class SetGlobalTrigger : LinearConditionalNode
{
    [Input(name = "Trigger Name"),SerializeField]
	public string triggerName;

    [Input(name = "Trigger"),SerializeField]
	public bool trigger;


	[Input(name = "Level Object")]
	public GraphObjectBase obj;

	public override string		name => "Set Global Trigger";

	protected override void Process()
	{
        var data = MessageDataPooling.GetMessageData<MD.TriggerData>();
        data.name = triggerName;
        data.trigger = trigger;

	    obj.SendMessageQuick(MessageTitles.boolTrigger_setTrigger,UniqueNumberBase.GetSavedNumberStatic("GlobalTriggerManager"),data);
	}
}