using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/Boolean : Equal")]
public class BooleanEqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public bool a;

    [Input(name = "B"),SerializeField]
	public bool b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Equal";

	protected override void Process()
	{
	    result = a == b;
	}
}