using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Get/Get Tag : GameObject")]
public class GetTagFromGameObjectNode : BaseNode
{
	[Input(name = "GameObject")]
	public GameObject gameObject;


	[Output(name = "Tag")]
	public string tag;

	public override string		name => "Get Tag";

	protected override void Process()
	{
	    tag = gameObject.tag;
	}
}

[System.Serializable, NodeMenuItem("Get/Get Tag : Transform")]
public class GetTagFromTransformNode : BaseNode
{
	[Input(name = "Transform")]
	public Transform tp;


	[Output(name = "Tag")]
	public string tag;

	public override string		name => "Get Tag";

	protected override void Process()
	{
	    tag = tp.tag;
	}
}

[System.Serializable, NodeMenuItem("Get/Get Tag : Collision")]
public class GetTagFromCollisionNode : BaseNode
{
	[Input(name = "Collision")]
	public Collision coll;


	[Output(name = "Tag")]
	public string tag;

	public override string		name => "Get Tag";

	protected override void Process()
	{
	    tag = coll.gameObject.tag;
	}
}

[System.Serializable, NodeMenuItem("Get/Get Tag : Collider")]
public class GetTagFromColliderNode : BaseNode
{
	[Input(name = "Collider")]
	public Collider coll;


	[Output(name = "Tag")]
	public string tag;

	public override string		name => "Get Tag";

	protected override void Process()
	{
	    tag = coll.tag;
	}
}