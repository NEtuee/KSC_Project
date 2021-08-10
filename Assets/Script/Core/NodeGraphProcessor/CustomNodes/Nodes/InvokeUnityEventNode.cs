using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Event/Invoke Unity Event")]
public class InvokeUnityEventNode : LinearConditionalNode
{
	[Input(name = "Unity Event")]
	public UnityEngine.Events.UnityEvent unityEvent;

	public override string		name => "Invoke Unity Event";

	protected override void Process()
	{
	    unityEvent?.Invoke();
	}
}