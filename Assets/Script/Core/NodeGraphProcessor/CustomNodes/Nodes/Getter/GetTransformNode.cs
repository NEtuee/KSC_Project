using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Get/Get Transform : GameObject")]
public class GetTransformFromGameObjectNode : BaseNode
{
	[Input(name = "GameObject")]
	public GameObject gameObject;


	[Output(name = "Transform")]
	public Transform tp;

	public override string		name => "Get Transform";

	protected override void Process()
	{
	    tp = gameObject.transform;
	}
}

[System.Serializable, NodeMenuItem("Get/Get Transform : Collision")]
public class GetTransformFromCollisionNode : BaseNode
{
	[Input(name = "Collision")]
	public Collision coll;


	[Output(name = "Transform")]
	public Transform tp;

	public override string		name => "Get Transform";

	protected override void Process()
	{
	    tp = coll.transform;
	}
}

[System.Serializable, NodeMenuItem("Get/Get Transform : Collider")]
public class GetTransformFromColliderNode : BaseNode
{
	[Input(name = "Collider")]
	public Collider coll;


	[Output(name = "Transform")]
	public Transform tp;

	public override string		name => "Get Transform";

	protected override void Process()
	{
	    tp = coll.transform;
	}
}
