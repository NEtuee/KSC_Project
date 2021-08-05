using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/Layer : Equal")]
public class LayerEqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public int a;

    [Input(name = "B"),SerializeField]
	public int b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Equal";

	protected override void Process()
	{
	    result = ((1 << a) & (1 << b)) != 0;
	}
}