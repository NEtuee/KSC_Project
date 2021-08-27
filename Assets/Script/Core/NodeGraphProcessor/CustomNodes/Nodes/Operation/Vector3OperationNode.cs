using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;

[System.Serializable, NodeMenuItem("Primitives/Vector3")]
public class Vector3Node : BaseNode
{
	[Input(name = "x"),SerializeField]
	public float x;

    [Input(name = "y"),SerializeField]
	public float y;

	[Input(name = "z"),SerializeField]
	public float z;


	[Output(name = "Vector3")]
	public Vector3C vector3 = new Vector3C();

	public override string		name => "Vector3";

	protected override void Process()
	{
	    vector3.Set(x,y,z);
	}
}

[System.Serializable, NodeMenuItem("Operations/Vector3 : Equal")]
public class Vector3EqualNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public Vector3C a;

    [Input(name = "B"),SerializeField]
	public Vector3C b;


	[Output(name = "Result")]
	public bool result;

	public override string		name => "Equal";

	protected override void Process()
	{
	    result = ((Vector3)a) == ((Vector3)b);
	}
}

[System.Serializable, NodeMenuItem("Operations/Vector3 : Subtract")]
public class Vector3SubNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public Vector3C a;

    [Input(name = "B"),SerializeField]
	public Vector3C b;


	[Output(name = "Result")]
	public Vector3C result = new Vector3C();

	public override string		name => "Subtract";

	protected override void Process()
	{
	    result.Set(((Vector3)a) - ((Vector3)b));
	}
}

[System.Serializable, NodeMenuItem("Operations/Vector3 : Add")]
public class Vector3AddNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public Vector3C a;

    [Input(name = "B"),SerializeField]
	public Vector3C b;


	[Output(name = "Result")]
	public Vector3C result = new Vector3C();

	public override string		name => "Add";

	protected override void Process()
	{
	    result.Set(((Vector3)a) + ((Vector3)b));
	}
}

[System.Serializable, NodeMenuItem("Operations/Vector3 : Multiply")]
public class Vector3MultiplyNode : BaseNode
{
	[Input(name = "A"),SerializeField]
	public Vector3C a;

    [Input(name = "B"),SerializeField]
	public float b;


	[Output(name = "Result")]
	public Vector3C result = new Vector3C();

	public override string		name => "Multiply";

	protected override void Process()
	{
	    result.Set(((Vector3)a) * b);
	}
}

[System.Serializable, NodeMenuItem("Operations/Vector3 : Split")]
public class Vector3SplitNode : BaseNode
{
	[Input(name = "A")]
	public Vector3C a;


	[Output(name = "x")]
	public float x;

    [Output(name = "y")]
	public float y;

    [Output(name = "z")]
	public float z;

	public override string		name => "Split";

	protected override void Process()
	{
	    x = a.x;
        y = a.y;
        z = a.z;
	}
}

[System.Serializable, NodeMenuItem("Operations/Get Direction : Vector3")]
public class GetDirectionVector3Node : BaseNode
{
	[Input(name = "A")]
	public Vector3C a;

	[Input(name = "B")]
	public Vector3C b;

	[Output(name = "B to A")]
	public Vector3C direction = new Vector3C();

	public override string		name => "Get Direction";

	protected override void Process()
	{
	    var dir = ((Vector3)a - (Vector3)b).normalized;
		direction.Set(dir);
	}
}

[System.Serializable, NodeMenuItem("Operations/Get Distance : Vector3")]
public class GetDistanceVector3Node : BaseNode
{
	[Input(name = "A")]
	public Vector3C a;

	[Input(name = "B")]
	public Vector3C b;

	[Output(name = "Distance")]
	public float distance;

	public override string		name => "Get Distance";

	protected override void Process()
	{
	    distance = Vector3.Distance((Vector3)a,(Vector3)b);
	}
}

[System.Serializable, NodeMenuItem("Operations/Get 2D Distance : Vector3")]
public class Get2DDistanceVector3Node : BaseNode
{
	[Input(name = "A")]
	public Vector3C a;

	[Input(name = "B")]
	public Vector3C b;

	[Output(name = "Distance")]
	public float distance;

	public override string		name => "Get 2D Distance";

	protected override void Process()
	{
		var ap = (Vector3)a;
		var bp = (Vector3)b;

		ap.y = 0f;
		bp.y = 0f;

	    distance = Vector3.Distance(ap,bp);
	}
}

[System.Serializable, NodeMenuItem("Operations/Get Direction : Transform")]
public class GetDirectionTransformNode : BaseNode
{
	[Input(name = "A")]
	public Transform a;

	[Input(name = "B")]
	public Transform b;

	[Output(name = "B to A")]
	public Vector3C direction = new Vector3C();

	public override string		name => "Get Direction";

	protected override void Process()
	{
	    var dir = (a.position - b.position).normalized;
		direction.Set(dir);
	}
}

[System.Serializable, NodeMenuItem("Operations/Get 2D Direction : Transform")]
public class Get2DDirectionTransformNode : BaseNode
{
	[Input(name = "A")]
	public Transform a;

	[Input(name = "B")]
	public Transform b;

	[Output(name = "B to A")]
	public Vector3C direction = new Vector3C();

	public override string		name => "Get 2D Direction";

	protected override void Process()
	{
		var ap = a.position;
		var bp = b.position;

		ap.y = 0f;
		bp.y = 0f;

	    var dir = (ap - bp).normalized;

		direction.Set(dir);
	}
}

[System.Serializable, NodeMenuItem("Operations/Get Distance : Transform")]
public class GetDistanceTransformNode : BaseNode
{
	[Input(name = "A")]
	public Transform a;

	[Input(name = "B")]
	public Transform b;

	[Output(name = "Distance")]
	public float distance;

	public override string		name => "Get Distance";

	protected override void Process()
	{
	    distance = Vector3.Distance(a.position,b.position);
	}
}