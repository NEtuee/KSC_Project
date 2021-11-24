using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Move : PathFollow")]
public class PathFollowMoveNode : LinearConditionalNode
{
    [Input(name = "Path Follow Object")]
	public PathFollowGraphObjectBase obj;

    [Input(name = "Direction"),SerializeField]
	public Vector3C direction;

    [Input(name = "speed"),SerializeField]
	public float speed;

	[Input(name = "Rotation Speed"),SerializeField]
	public float rotationSpeed;

	[Input(name = "deltaTime")]
	public float deltaTime;


	public override string		name => "Move";

	protected override void Process()
	{
	    obj.Move((Vector3)direction,speed,rotationSpeed,deltaTime);
	}
}

[System.Serializable, NodeMenuItem("Game/Follow Path Translate")]
public class FollowPathTranslateNode : LinearConditionalNode
{
    [Input(name = "Path Follow Object")]
	public PathFollowGraphObjectBase obj;

    [Input(name = "speed"),SerializeField]
	public float speed;

	[Input(name = "Rotation Speed"),SerializeField]
	public float rotationSpeed;

	[Input(name = "Point Rotate"),SerializeField]
	public bool pointRotate;

	[Input(name = "deltaTime")]
	public float deltaTime;

	[Output(name = "Arrived")]
	public bool isArrived;


	public override string		name => "Follow Path Translate";

	protected override void Process()
	{
        var angle = Vector3.SignedAngle(obj.transform.forward, obj.targetDirection, obj.transform.up);

        if (Mathf.Abs(angle) > obj.turnAccuracy)
        {
            if (angle > 0)
                obj.Turn(true, obj.transform, rotationSpeed, deltaTime);
            else
                obj.Turn(false, obj.transform, rotationSpeed, deltaTime);
        }
        isArrived = obj.FollowPathTranslate(speed,rotationSpeed,pointRotate,deltaTime);
	}
}

[System.Serializable, NodeMenuItem("Game/Follow Path")]
public class FollowPathNode : LinearConditionalNode
{
    [Input(name = "Path Follow Object")]
	public PathFollowGraphObjectBase obj;

    [Input(name = "speed"),SerializeField]
	public float speed;

	[Input(name = "Rotation Speed"),SerializeField]
	public float rotationSpeed;

	[Input(name = "deltaTime")]
	public float deltaTime;

	[Output(name = "Arrived")]
	public bool isArrived;


	public override string		name => "Follow Path";

	protected override void Process()
	{
	    isArrived = obj.FollowPath(speed,rotationSpeed,deltaTime);
	}
}

[System.Serializable, NodeMenuItem("Game/Set Path")]
public class SetPathNode : LinearConditionalNode
{
    [Input(name = "Path Follow Object")]
	public PathFollowGraphObjectBase obj;

    [Input(name = "Path Name"),SerializeField]
	public string path;

	[Input(name = "Loop"),SerializeField]
	public bool loop;

	[Input(name = "Start Zero"),SerializeField]
	public bool startZero;


	public override string		name => "Set Path";

	protected override void Process()
	{
		var stringData = MessageDataPooling.GetMessageData<MD.StringData>();
		stringData.value = path;
	    obj.SendMessageQuick(MessageTitles.stage_getPath,UniqueNumberBase.GetSavedNumberStatic("StageManager"),stringData);

		var message = obj.DequeueGraphMessage();
		obj.SetPath(message,loop);

		if(startZero)
		{
			obj.SetPathTargetZero(); 
		}
		else
		{
			obj.SetPathTargetNear(false);
		}

		MessagePool.ReturnMessage(message);
	}
}