using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable, NodeMenuItem("Reference/Game Object")]
public class GraphObjectNode : BaseNode, ICreateNodeFrom<GraphObjectBase>
{
	[Output(name = "Out"), SerializeField]
	public GraphObjectBase			output;

	public override string		name => "Level Object";

	public bool InitializeNodeFromObject(GraphObjectBase value)
	{
		output = value;
		return true;
	}
}