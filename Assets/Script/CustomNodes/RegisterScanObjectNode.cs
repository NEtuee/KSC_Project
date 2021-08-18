using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Register Scan Object")]
public class RegisterScanObjectNode : LinearConditionalNode
{
    [Input(name = "Level Object")]
	public GraphObjectBase obj;

	public override string		name => "Register Scan Object";

	protected override void Process()
	{
        Debug.Log("Call");
        obj.SendMessageEx(MessageTitles.scan_registerScanObject,UniqueNumberBase.GetSavedNumberStatic("Drone"),obj);
	}

}