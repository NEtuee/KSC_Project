using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;
using VectorClasses;

[System.Serializable]
public class RaycastHitC
{
	public RaycastHit hit;
}

[System.Serializable, NodeMenuItem("Physics/Raycast")]
public class RaycastNode : LinearConditionalNode
{
	[Input(name = "Start"),SerializeField]
	public Vector3C start;

    [Input(name = "Direction"),SerializeField]
	public Vector3C direction;

	[Input(name = "MaxDistance"),SerializeField]
	public float dist;

	[Input(name = "Layer"),SerializeField]
	public LayerMask layer;


	[Output(name = "IsHit")]
	public bool isHit;

	[Output(name = "Data")]
	public RaycastHitC data = new RaycastHitC();

	public override string		name => "Raycast";

	protected override void Process()
	{
	    isHit = Physics.Raycast((Vector3)start,(Vector3)direction,out data.hit,dist,layer);
	}
}

[System.Serializable, NodeMenuItem("Operations/Split : RaycastHit")]
public class RaycastHitSplitNode : BaseNode
{
	[Input(name = "RaycastHit"),SerializeField]
	public RaycastHitC data = new RaycastHitC();


	[Output(name = "Distance")]
	public float dist;

	[Output(name = "Transform")]
	public Transform transform;

	[Output(name = "HitPoint")]
	public Vector3C hitPoint = new Vector3C();

	[Output(name = "Normal")]
	public Vector3C normal = new Vector3C();

	public override string		name => "Split";

	protected override void Process()
	{
	    dist = data.hit.distance;
		transform = data.hit.transform;
		hitPoint.Set(data.hit.point);
		normal.Set(data.hit.normal);
	}
}