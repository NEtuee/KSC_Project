using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable, NodeMenuItem("Primitives/Vector3C")]
public class Vector3CNode : BaseNode
{
    [Input(name = "X"), SerializeField]
	public float x;

    [Input(name = "Y"), SerializeField]
	public float y;

    [Input(name = "Z"), SerializeField]
	public float z;

	[Output(name = "Out")]
	public VectorClasses.Vector3C output = new VectorClasses.Vector3C();
	

	public override string		name => "Vector";

	protected override void Process()
	{
		output.Set(x,y,z);
	}
}