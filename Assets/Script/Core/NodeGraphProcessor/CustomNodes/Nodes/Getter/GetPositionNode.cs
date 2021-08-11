using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Get/Get Position : GameObject")]
public class GetPositionFromGameObjectNode : BaseNode
{
	[Input(name = "GameObject")]
	public GameObject gameObject;


	[Output(name = "Position")]
	public Vector3C pos = new Vector3C();

	public override string		name => "Get Position";

	protected override void Process()
	{
	    pos.Set(gameObject.transform.position);
	}
}

[System.Serializable, NodeMenuItem("Get/Get Position : Transform")]
public class GetPositionFromTransformNode : BaseNode
{
	[Input(name = "Transform")]
	public Transform transform;


	[Output(name = "Position")]
	public Vector3C pos = new Vector3C();

	public override string		name => "Get Position";

	protected override void Process()
	{
	    pos.Set(transform.position);
	}
}

[System.Serializable, NodeMenuItem("Get/Get Position : Collision")]
public class GetPositionFromCollisionNode : BaseNode
{
	[Input(name = "Collision")]
	public Collision coll;


	[Output(name = "Position")]
	public Vector3C pos = new Vector3C();

	public override string		name => "Get Position";

	protected override void Process()
	{
	    pos.Set(coll.transform.position);
	}
}

[System.Serializable, NodeMenuItem("Get/Get Position : Collider")]
public class GetPositionFromColliderNode : BaseNode
{
	[Input(name = "Collider")]
	public Collider coll;


	[Output(name = "Position")]
	public Vector3C pos = new Vector3C();

	public override string		name => "Get Position";

	protected override void Process()
	{
	    pos.Set(coll.transform.position);
	}
}