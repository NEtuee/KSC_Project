using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/String : Equal")]
public class StringEqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public string a;

    [Input(name = "B"),SerializeField]
	public string b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Equal";

	protected override void Process()
	{
	    result = a == b;
	}
}

[System.Serializable, NodeMenuItem("Operations/String : Add")]
public class StringAddNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public string a;

    [Input(name = "B"),SerializeField]
	public string b;


	[Output(name = "Result")]
	public string result;

	public override string		name => "Add";

	protected override void Process()
	{
	    result = a + b;
	}
}