using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/Vector3 : Equal")]
public class Vector3EqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public Vector3C a;

    [Input(name = "B"),SerializeField]
	public Vector3C b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Equal";

	protected override void Process()
	{
	    result = ((Vector3)a) == ((Vector3)b);
	}
}

[System.Serializable, NodeMenuItem("Operations/Vector3 : Subtract")]
public class Vector3SubNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public Vector3C a;

    [Input(name = "B"),SerializeField]
	public Vector3C b;


	[Output(name = "Result")]
	public Vector3C result = new Vector3C();

	public override string		name => "Subtract";

	protected override void Process()
	{
	    result.Set(((Vector3)a) - ((Vector3)b));
	}
}

[System.Serializable, NodeMenuItem("Operations/Vector3 : Add")]
public class Vector3AddNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public Vector3C a;

    [Input(name = "B"),SerializeField]
	public Vector3C b;


	[Output(name = "Result")]
	public Vector3C result = new Vector3C();

	public override string		name => "Add";

	protected override void Process()
	{
	    result.Set(((Vector3)a) + ((Vector3)b));
	}
}

[System.Serializable, NodeMenuItem("Operations/Vector3 : Split")]
public class Vector3SplitNode : BaseNode
{
	[Input(name = "A")]
	public Vector3C a;


	[Output(name = "x")]
	public float x;

    [Output(name = "x")]
	public float y;

    [Output(name = "x")]
	public float z;

	public override string		name => "Split";

	protected override void Process()
	{
	    x = a.x;
        y = a.y;
        z = a.z;
	}
}