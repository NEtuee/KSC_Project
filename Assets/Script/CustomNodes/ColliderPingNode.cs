using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Collider Ping")]
public class ColliderPingNode : LinearConditionalNode
{
    [Input(name = "Collider"),SerializeField]
	public Collider collider;

	public override string		name => "Collider Ping";

	protected override void Process()
	{
        MD.ScanMakerData data = MessageDataPooling.GetMessageData<MD.ScanMakerData>();
        data.collider = collider;
        // data.center = collider.bounds.center;
        // data.min = collider.bounds.min;
        // data.max = collider.bounds.max;
        var msg = MessagePool.GetMessage();
        msg.Set(MessageTitles.uimanager_activeScanMaker, UniqueNumberBase.GetSavedNumberStatic("UIManager"),data,null);
        
        MasterManager.instance.HandleMessage(msg); 
	}

}