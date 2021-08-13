using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphProcessor;

namespace NodeGraphProcessor.Examples
{
	[System.Serializable, NodeMenuItem("Conditional/Start")]
	public class StartNode : BaseNode, IConditionalNode
	{
		[Output(name = "Executes")]
		public ConditionalLink		executes;

		List<ConditionalNode> excutedNodes = new List<ConditionalNode>();

		public override string		name => "Start";

		public List<ConditionalNode>	GetExecutedNodes()
		{
			excutedNodes.Clear();

			foreach(var output in outputPorts)
			{
				foreach(var edge in output.GetEdges())
				{
					if(edge.inputPort.portData.displayType == typeof(ConditionalLink))
					{
						excutedNodes.Add((ConditionalNode)edge.inputNode);
					}
				}
			}

			return excutedNodes;
			// Return all the nodes connected to the executes port
			//return GetOutputNodes().Where(n => n is ConditionalNode).Select(n => n as ConditionalNode);
		}

		public override FieldInfo[] GetNodeFields() => base.GetNodeFields();
	}
}
