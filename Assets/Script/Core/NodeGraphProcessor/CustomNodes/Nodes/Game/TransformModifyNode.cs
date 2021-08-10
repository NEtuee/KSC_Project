using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Move Forward")]
public class MoveForwardNode : LinearConditionalNode
{
    [Input(name = "Transform")]
	public Transform transform;

	[Input(name = "speed"),SerializeField]
	public float speed;

	[Input(name = "deltaTime")]
	public float deltaTime;


	public override string		name => "Move Forward";

	protected override void Process()
	{
	    transform.position += transform.forward * speed * deltaTime;
	}
}

[System.Serializable, NodeMenuItem("Game/Ground Move")]
public class GroundMoveNode : ConditionalNode
{
	[Output(name = "True")]
	public ConditionalLink	@true;
	[Output(name = "False")]
	public ConditionalLink	@false;

    [Input(name = "Transform")]
	public Transform transform;
	[Input(name = "Ground Ray Point")]
	public Transform groundRayPoint;

	[Input(name = "Direction"),SerializeField]
	public Vector3C direction;

	
	[Input(name = "speed"),SerializeField]
	public float speed;

	[Input(name = "deltaTime")]
	public float deltaTime;

	[Input(name = "Ground Layer"),SerializeField]
	public LayerMask groundLayer;


	public override string		name => "Ground Move";
	private bool _condition = false;

	public bool GroundCheck(Vector3 pos, float dist)
	{
		Ray ray = new Ray();
        ray.origin = groundRayPoint.position + pos;
        ray.direction = -transform.up;

        return Physics.Raycast(ray,dist,groundLayer);
	}

	protected override void Process()
	{
		var normalSpeed = speed < 0 ? -speed : speed;
		_condition = GroundCheck((Vector3)direction * 2.5f * normalSpeed,10f);
		if(!_condition)
		{
			return;
		}

	    transform.position += transform.forward * speed * deltaTime;
	}

	public override List<ConditionalNode>	GetExecutedNodes()
	{
		string fieldName = _condition ? nameof(@true) : nameof(@false);
        _condition = false;

        Debug.Log(fieldName);

		executedNodes.Clear();

		var ports = outputPorts.Find((x)=>{return x.fieldName == fieldName;});
		var edges = ports?.GetEdges();
		foreach(var edge in edges)
		{
			executedNodes.Add((ConditionalNode)edge.inputNode);
		}

		return executedNodes;
		// Return all the nodes connected to either the true or false node
		//return outputPorts.FirstOrDefault(n => n.fieldName == fieldName)
//			.GetEdges().Select(e => e.inputNode as ConditionalNode);
	}
}

[System.Serializable, NodeMenuItem("Game/Turn")]
public class TurnNode : LinearConditionalNode
{
    [Input(name = "Transform")]
	public Transform transform;

	[Input(name = "speed"),SerializeField]
	public float speed;

	[Input(name = "deltaTime")]
	public float deltaTime;

	[Input(name = "leftTurn"),SerializeField]
	public bool leftTurn;


	public override string		name => "Turn";

	protected override void Process()
	{
		var factor = speed * deltaTime * (leftTurn ? 1f : -1f);
		transform.RotateAround(transform.position,transform.up,factor);
	}
}