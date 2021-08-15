using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Operations/Is Null")]
public class IsUnityNullNode : BaseNode
{
	[Input(name = "UnityObject")]
	public UnityEngine.Object obj;

	[Output(name = "Result")]
	public bool result;

	public override string		name => "Is Null";

	protected override void Process()
	{
	    result = obj == null;
	}
}

[System.Serializable, NodeMenuItem("Operations/Equal : UnityObject")]
public class UnityObjectEqualNode : BaseNode
{
	[Input(name = "A")]
	public UnityEngine.Object a;

    [Input(name = "B")]
	public UnityEngine.Object b;

	[Output(name = "Result")]
	public bool result;

	public override string		name => "Equal";

	protected override void Process()
	{
	    result = a == b;
	}
}