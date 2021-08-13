using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/Int : Equal")]
public class IntEqualNode : BaseNode
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
	    result = a == b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Int : Subtract")]
public class IntSubNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public int a;

    [Input(name = "B"),SerializeField]
	public int b;


	[Output(name = "Result")]
	public int result;

	public override string		name => "Subtract";

	protected override void Process()
	{
	    result = a - b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Int : Add")]
public class IntAddNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public int a;

    [Input(name = "B"),SerializeField]
	public int b;


	[Output(name = "Result")]
	public int result;

	public override string		name => "Add";

	protected override void Process()
	{
	    result = a + b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Int : Divide")]
public class IntDivideNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public int a;

    [Input(name = "B"),SerializeField]
	public int b;


	[Output(name = "Result")]
	public int result;

	public override string		name => "Divide";

	protected override void Process()
	{
	    result = a / b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Int : Multifly")]
public class IntMultiflyNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public int a;

    [Input(name = "B"),SerializeField]
	public int b;


	[Output(name = "Result")]
	public int result;

	public override string		name => "Multifly";

	protected override void Process()
	{
	    result = a * b;
	}
}