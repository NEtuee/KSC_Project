using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/FunctionNode")]
public class FunctionNode : BaseNode
{

	[Input(name = "list")]
	public List<int> inputs = new List<int>{1,2,3,};

	[Input(name = "In")]
    public float                input;

	[Output(name = "Out")]
	public float				output;


	public object inValue;

	public override string		name => "FunctionNode";

	protected override void Enable()
	{
		for(int i = 0; i < inputs.Count; ++i)
		{
			AddPort(true,"inValue",new PortData
			{
				identifier = "inValue" + i,
				displayName = "Value",
				displayType = typeof(int),
				//acceptMultipleEdges = true,
			});
		}
	}

	public override void ValueInputed(NodePort port)
	{
		Debug.Log(port.portData.identifier);
	}

	protected override void Process()
	{
	    output = input * 42;
	}
}
