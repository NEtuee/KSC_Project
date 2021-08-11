using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Get/Get Layer : GameObject")]
public class GetLayerFromGameObjectNode : BaseNode
{
	[Input(name = "GameObject")]
	public GameObject gameObject;


	[Output(name = "Layer")]
	public int layer;

	public override string		name => "Get Layer";

	protected override void Process()
	{
	    layer = 1 << gameObject.layer;
	}
}

[System.Serializable, NodeMenuItem("Get/Get Layer : GameObject")]
public class GetLayerFromTransformNode : BaseNode
{
	[Input(name = "Transform")]
	public Transform tp;


	[Output(name = "Layer")]
	public int layer;

	public override string		name => "Get Layer";

	protected override void Process()
	{
	    layer = 1 << tp.gameObject.layer;
	}
}

[System.Serializable, NodeMenuItem("Get/Get Layer : Collision")]
public class GetLayerFromCollisionNode : BaseNode
{
	[Input(name = "Collision")]
	public Collision coll;


	[Output(name = "Layer")]
	public int layer;

	public override string		name => "Get Layer";

	protected override void Process()
	{
	    layer = 1 << coll.gameObject.layer;
	}
}

[System.Serializable, NodeMenuItem("Get/Get Layer : Collider")]
public class GetLayerFromColliderNode : BaseNode
{
	[Input(name = "Collider")]
	public Collider coll;


	[Output(name = "Layer")]
	public int layer;

	public override string		name => "Get Layer";

	protected override void Process()
	{
	    layer = 1 << coll.gameObject.layer;
	}
}