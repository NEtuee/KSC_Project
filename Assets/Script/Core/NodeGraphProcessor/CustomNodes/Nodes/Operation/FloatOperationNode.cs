using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/Float : Equal")]
public class FloatEqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Equal";

	protected override void Process()
	{
	    result = a == b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Float : Subtract")]
public class FloatSubNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public float result;

	public override string		name => "Subtract";

	protected override void Process()
	{
	    result = a - b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Float : Add")]
public class FloatAddNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public float result;

	public override string		name => "Add";

	protected override void Process()
	{
	    result = a + b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Float : Divide")]
public class FloatDivideNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public float result;

	public override string		name => "Divide";

	protected override void Process()
	{
	    result = a / b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Float : Multifly")]
public class FloatMultiflyNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public float result;

	public override string		name => "Multifly";

	protected override void Process()
	{
	    result = a * b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Float : Greater")]
public class FloatGreaterNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Greater";

	protected override void Process()
	{
	    result = a > b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Float : Greater Equal")]
public class FloatGreaterEqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Greater Equal";

	protected override void Process()
	{
	    result = a >= b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Float : Smaller")]
public class FloatSmallerNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Smaller";

	protected override void Process()
	{
	    result = a < b;
	}
}

[System.Serializable, NodeMenuItem("Operations/Float : Smaller Equal")]
public class FloatSmallerEqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public float a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Smaller Equal";

	protected override void Process()
	{
	    result = a <= b;
	}
}