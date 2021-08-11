using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;
using UnityEngine.Rendering;

[System.Serializable, NodeMenuItem("Trigger/Ground Check")]
public class GroundCheckNode : ConditionalNode
{
	[Output(name = "True")]
	public ConditionalLink	@true;
	[Output(name = "False")]
	public ConditionalLink	@false;


    [Input(name = "Transform")]
	public Transform transform;

    [Input(name = "Ground Ray Point")]
	public Transform groundRayPoint;
    [Input(name = "Ray Distance"),SerializeField]
	public float rayDistance;

    [Input(name = "GroundLayer"),SerializeField]
	public LayerMask groundLayer;


    [Output(name = "RaycastHit")]
	public RaycastHitC	rayHit = new RaycastHitC();




    private bool condition = true;

	public override string		name => "Ground Check";


    protected override void Process()
	{
		Ray ray = new Ray();
        ray.origin = groundRayPoint.position;
        ray.direction = -transform.up;

        condition = Physics.Raycast(ray,out rayHit.hit, rayDistance, groundLayer);
	}


	public override List<ConditionalNode>	GetExecutedNodes()
	{
		string fieldName = condition ? nameof(@true) : nameof(@false);
        condition = false;

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
