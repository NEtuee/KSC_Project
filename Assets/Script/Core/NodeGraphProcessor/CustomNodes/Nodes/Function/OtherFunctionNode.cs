using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;


[System.Serializable, NodeMenuItem("Functon/Function : other")]
public class OtherFunctionNode : LinearConditionalNode
{
    //[Input(name = "GraphObject"), SerializeField]
    public GraphObjectBase graphObject;

    
    public override string		name => "FunctionNode";

    public int functionID;
	public FunctionGraph.FunctionInfo functionInfo;

	protected override void Enable()
	{
		GetFunctionInfo();
	}

	public void GetFunctionInfo()
	{
        if(graphObject == null)
            return;

		var fg = graphObject.GetCopyedGraph();
		if(fg == null)
		{
			//graph.RemoveNode(this);
			return;
		}

		functionInfo = fg.FindFunction(functionID);

		if (functionInfo == null)
		{
			// Debug.Log("Property \"" + functionID + "\" Can't be found !");

			// // Delete this node as the property can't be found
			// graph.RemoveNode(this);
			return;
		}
	}

	protected override void Process()
	{
		Debug.Log("Function Call");
	    //output = input * 42;
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
			
		functionInfo.endNode.nextNode = executedNodes;

		// Return all the nodes connected to the executes port
		return functionInfo.entryNode.GetExecutedNodes();
		// return outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
		// 	.GetEdges().Select(e => e.inputNode as ConditionalNode);
	}
}
