using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Set/Set Position : Transform")]
public class SetPositionNode : LinearConditionalNode
{
	[Input(name = "Transform")]
	public Transform transform;

    [Input(name = "Position")]
	public VectorClasses.Vector3C pos;

	public override string		name => "Set Position";

	protected override void Process()
	{
	    transform.position = (Vector3)pos;
	}
}
