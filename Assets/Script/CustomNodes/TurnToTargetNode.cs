using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Turn To Target")]
public class TurnToTargetNode : LinearConditionalNode
{
    [Input(name = "PathFollow Graph Object")]
	public PathFollowGraphObjectBase graphObj;

    [Input(name = "Target")]
	public Transform target;

    [Input(name = "DeltaTime")]
	public float deltaTime;


    [Input(name = "Rotation Speed"),SerializeField]
	public float rotationSpeed;

    private float _accel = 0f;

	public override string		name => "Turn To Target";

	protected override void Process()
	{
        var targetDirection = (target.position - graphObj.transform.position).normalized;
        var angle = Vector3.SignedAngle(graphObj.transform.forward,targetDirection,graphObj.transform.up);

        if(Mathf.Abs(angle) > graphObj.turnAccuracy)
        {
            _accel += 60f * deltaTime;
            _accel = _accel >= rotationSpeed ? rotationSpeed : _accel;

            if(angle > 0)
                graphObj.Turn(true,graphObj.transform,_accel,deltaTime);
            else
                graphObj.Turn(false,graphObj.transform,_accel,deltaTime);
        }
        else
        {
            _accel -= 60f * deltaTime;
            _accel = _accel <= 0f ? 0f : _accel;

        }

	}

}
