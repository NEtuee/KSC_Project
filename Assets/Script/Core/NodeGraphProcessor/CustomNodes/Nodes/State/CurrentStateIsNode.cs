using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("State/Current State Is")]
public class CurrentStateIsNode : LinearConditionalNode
{
	[Output(name = "Output")]
	public bool output;

	public override string		name => "Current State Is";

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
		output = stateMachineGraph.currentState == stateID;
	}
}
