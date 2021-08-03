using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using GraphProcessor;
using NodeGraphProcessor.Examples;

[System.Serializable]
public class FunctionStartNode : BaseNode, IConditionalNode
{
    [Output(name = "Executes")]
	public ConditionalLink		executes;
	public override string		name => "FunctionStart";
	public override bool deletable => false;

	List<ConditionalNode> excutedNodes = new List<ConditionalNode>();

    public Action onTitleChange;
    public string title = "";

    public void ChangeTitle(string title)
    {
        this.title = title;
        onTitleChange?.Invoke();
    }

	public List<ConditionalNode>	GetExecutedNodes()
	{
		excutedNodes.Clear();

		foreach(var output in outputPorts)
		{
			foreach(var edge in output.GetEdges())
			{
				if(edge.inputNode is ConditionalNode)
				{
					excutedNodes.Add((ConditionalNode)edge.inputNode);
				}
			}
		}

		return excutedNodes;
		// Return all the nodes connected to the executes port
		//return GetOutputNodes().Where(n => n is ConditionalNode).Select(n => n as ConditionalNode);
	}

	public override FieldInfo[] GetNodeFields()
	{
		var fields = base.GetNodeFields();
		Array.Sort(fields, (f1, f2) => f1.Name == nameof(executes) ? -1 : 1);
		return fields;
	}
}
