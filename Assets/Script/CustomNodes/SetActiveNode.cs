using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Set Active : GameObject")]
public class SetActiveGameObjectNode : LinearConditionalNode
{
    [Input(name = "Target")]
	public GameObject target;

    [Input(name = "Active"),SerializeField]
	public bool active;



	public override string		name => "Set Active";

	protected override void Process()
	{
        target.SetActive(active);
	}

}

[System.Serializable, NodeMenuItem("Game/Set Active : MessageReceiver")]
public class SetActiveMessageReceiverNode : LinearConditionalNode
{
    [Input(name = "Target")]
	public MessageReceiver target;

    [Input(name = "Active"),SerializeField]
	public bool active;



	public override string		name => "Set Active";

	protected override void Process()
	{
        target.gameObject.SetActive(active);
	}

}
