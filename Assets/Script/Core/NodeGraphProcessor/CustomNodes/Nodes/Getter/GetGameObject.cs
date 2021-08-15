using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Get/Get GameObject : Transform")]
public class GetGameObjectFromTransformNode : BaseNode
{
	[Input(name = "Transform")]
	public Transform tp;


	[Output(name = "GameObject")]
	public GameObject gameObject;

	public override string		name => "Get GameObject";

	protected override void Process()
	{
	    gameObject = tp.gameObject;
	}
}

[System.Serializable, NodeMenuItem("Get/Get GameObject : Collision")]
public class GetGameObjectFromCollisionNode : BaseNode
{
	[Input(name = "Collision")]
	public Collision coll;


	[Output(name = "GameObject")]
	public GameObject gameObject;

	public override string		name => "Get GameObject";

	protected override void Process()
	{
	    gameObject = coll.gameObject;
	}
}

[System.Serializable, NodeMenuItem("Get/Get GameObject : Collider")]
public class GetGameObjectFromColliderNode : BaseNode
{
	[Input(name = "Collider")]
	public Collider coll;


	[Output(name = "GameObject")]
	public GameObject gameObject;

	public override string		name => "Get GameObject";

	protected override void Process()
	{
	    gameObject = coll.gameObject;
	}
}

[System.Serializable, NodeMenuItem("Get/Get GameObject : Component")]
public class GetGameObjectFromComponentNode : BaseNode
{
	[Input(name = "Component")]
	public Component component;


	[Output(name = "GameObject")]
	public GameObject gameObject;

	public override string		name => "Get GameObject";

	protected override void Process()
	{
	    gameObject = component.gameObject;
	}
}