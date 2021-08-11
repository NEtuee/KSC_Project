using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Layer")]
public class LayerNode : BaseNode
{
	public LayerMask layer;


	[Output(name = "Layer")]
	public int output;

	public override string		name => "Layer";

	protected override void Process()
	{
	    output = layer.value;
	}
}