using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Custom/ProgressState")]
public class StateProgressNode : LinearConditionalNode
{
	public override string		name => "ProgressState";

	[SerializeField]
	public StateMachineGraph stateMachineGraph;
	public StateMachineGraph.StateInfo stateInfo;

	protected override void Process()
	{
        stateMachineGraph = graph as StateMachineGraph;
        stateInfo = stateMachineGraph.FindState(stateMachineGraph.currentState);
	}

	public override IEnumerable< ConditionalNode >	GetExecutedNodes()
	{
        var node = outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
		 	.GetEdges().Select(e => e.inputNode as ConditionalNode);

        if(stateInfo == null)
            return node;


		stateInfo.stateProgress.endNode.nextNode = node;

		return stateInfo.stateProgress.entryNode.GetExecutedNodes();
	}
}
