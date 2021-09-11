using UnityEngine;
using GraphProcessor;
using UnityEngine.Events;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Custom/Unity Event Node")]
public class UnityEventNode : LinearConditionalNode
{
	public UnityEvent			evt;

	public override string		name => "Unity Event Node";

	protected override void Process()
	{
	    evt?.Invoke();
	}
}
