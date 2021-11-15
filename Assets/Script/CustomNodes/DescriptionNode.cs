using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Description")]
public class DescriptionNode : LinearConditionalNode
{
    [Input(name = "LevelObject")]
	public GraphObjectBase obj;

    [Input(name = "Key")]
	public string key;

	public override string		name => "Description";

	protected override void Process()
	{
        var data = MessageDataPooling.GetMessageData<MD.DroneTextKeyAndDurationData>();
            data.key = key;
        obj.SendMessageEx(MessageTitles.playermanager_droneTextAndDurationByKey, MessageReceiver.GetSavedNumberStatic("PlayerManager"), data);
	}
}
