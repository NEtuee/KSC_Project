using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("LevelEdit/Rotator")]
public class RotatorExNode : LinearConditionalNode
{
	[Input(name = "DeltaTime")]
    public float                deltaTime;

	[Input(name = "Axis")]
	public VectorClasses.Vector3C				axis;

	[Input(name = "GameObject")]
	public GameObject			gameObject;



	public override string		name => "Rotator";

	protected override void Process()
	{
	    gameObject.transform.rotation *= Quaternion.Euler((Vector3)axis * deltaTime);
	}
}
