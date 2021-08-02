using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Custom/ChangeState")]
public class StateChangeNode : LinearConditionalNode
{
	public override string		name => "ChangeState";

	[SerializeField]
	public int stateID = -1;
	public StateMachineGraph stateMachineGraph;
	public StateMachineGraph.StateInfo stateInfo;

	protected override void Enable()
	{
		stateMachineGraph = (graph as StateMachineGraph);

		if(stateMachineGraph.states.Count != 0)
		{
			if(stateID == -1)
				stateID = stateMachineGraph.states[0].uniqueID;
			GetStateInfo();
		}
		
	}

	public void GetStateInfo()
	{
		if(stateMachineGraph == null)
		{
			graph.RemoveNode(this);
			return;
		}

		stateInfo = stateMachineGraph.FindState(stateID);

		if (stateInfo == null)
		{
			Debug.Log("Property \"" + stateID + "\" Can't be found !");

			// Delete this node as the property can't be found
			graph.RemoveNode(this);
			return;
		}
	}

	protected override void Process()
	{
		stateMachineGraph.ChangeState(stateID);
	}

	public override List<ConditionalNode>	GetExecutedNodes()
	{
		executedNodes.Clear();

		var ports = outputPorts.Find((x)=>{return x.fieldName == nameof(executes);});
		var edges = ports?.GetEdges();
		foreach(var edge in edges)
		{
			executedNodes.Add((ConditionalNode)edge.inputNode);
		}
		// var node = outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
		//  	.GetEdges().Select(e => e.inputNode as ConditionalNode);

		stateInfo.stateInitialize.endNode.nextNode = executedNodes;

		return stateInfo.stateInitialize.entryNode.GetExecutedNodes();
	}
}
