using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;
using UnityEngine.Rendering;

[System.Serializable, NodeMenuItem("Trigger/Oneshot Trigger")]
public class OneshotTriggerNode : ConditionalNode
{
	[Output(name = "Triggered")]
	public ConditionalLink	@true;
	[Output(name = "False")]
	public ConditionalLink	@false;

    private bool condition = true;

	public override string		name => "Oneshot Trigger";

	public override List<ConditionalNode>	GetExecutedNodes()
	{
		string fieldName = condition ? nameof(@true) : nameof(@false);
        condition = false;

        Debug.Log(fieldName);

		executedNodes.Clear();

		var ports = outputPorts.Find((x)=>{return x.fieldName == fieldName;});
		var edges = ports?.GetEdges();
		foreach(var edge in edges)
		{
			executedNodes.Add((ConditionalNode)edge.inputNode);
		}

		return executedNodes;
		// Return all the nodes connected to either the true or false node
		//return outputPorts.FirstOrDefault(n => n.fieldName == fieldName)
//			.GetEdges().Select(e => e.inputNode as ConditionalNode);
	}
}
